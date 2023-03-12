using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using CyberneticStudios.SOFramework;

// Source: Sebastian Lague - Field of view visualisation
// Converted to 2D Physics
// https://www.youtube.com/watch?v=rQG9aUWarwE
// https://github.com/SebLague/Field-of-View/tree/master/Episode%2002/Scripts

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    // Right now, we just want to target the player. If in the future we want to target more than
    // the player, we need a higher max targets count.
    const int MAX_NUM_TARGETS = 1;

    [SerializeField] bool debug = false;

    [Space]
    [Space]

    [SerializeField] bool canUntrigger = true;
    [SerializeField][Range(0, 50)] float viewRadius = 10f;
    [SerializeField][Range(0, 360)] float viewAngle = 30f;
    [SerializeField] Color activeColor = Color.yellow;
    [SerializeField] Color triggeredColor = Color.red;
    [SerializeField][Range(0, 1)] float alpha = 0.5f;
    [SerializeField] bool eclipsePlayer = true;

    [Space]
    [Space]

    [SerializeField] BoolVariable didMotionSensorTrigger;

    [Space]
    [Space]

    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;

    [Space]
    [Space]

    [SerializeField][Range(0, 1)] float delayBetweenRaycasts = 0.1f;
    [SerializeField][Range(0, 2)] float meshResolution = 1f;
    [SerializeField][Range(1, 5)] int edgeResolveIterations = 1;
    [SerializeField] float edgeDstThreshold = 0.1f;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;
    MaterialPropertyBlock propBlock;

    Collider2D[] targetsInViewRadius = new Collider2D[MAX_NUM_TARGETS];
    RaycastHit2D[] targetRaycastHits = new RaycastHit2D[MAX_NUM_TARGETS];

    public float ViewRadius => viewRadius;
    public float ViewAngle => viewAngle;

    bool isTriggered = false;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "View Mesh";
        meshFilter.mesh = mesh;
        StartCoroutine(FindTargets());
    }

    IEnumerator FindTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenRaycasts);
            FindVisibleTargets();
            if (canUntrigger)
            {
                didMotionSensorTrigger.value = isTriggered;
            }
            else
            {
                if (isTriggered) didMotionSensorTrigger.value = true;
            }
        }
    }

    void Update()
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", isTriggered ? triggeredColor.toAlpha(alpha) : activeColor.toAlpha(alpha));
        meshRenderer.SetPropertyBlock(propBlock);
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        int numTargets = Physics2D.OverlapCircleNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);
        if (numTargets == 0) isTriggered = false;

        for (int i = 0; i < numTargets; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                int numHits = Physics2D.RaycastNonAlloc(transform.position, dirToTarget, targetRaycastHits, dstToTarget, targetMask);
                isTriggered = numHits > 0;
                if (numHits > 0)
                {
                    visibleTargets.Add(target);
                }
            }
            else
            {
                isTriggered = false;
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            if (debug) Debug.DrawLine(transform.position, (Vector2)transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.didHit != newViewCast.didHit || (oldViewCast.didHit && newViewCast.didHit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector2.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector2.zero) viewPoints.Add(edge.pointB);
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.didHit == minViewCast.didHit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        Vector3 origin = transform.position + dir.normalized * 0.5f;
        LayerMask viewMask = eclipsePlayer ? (obstacleMask | targetMask) : obstacleMask;
        int numHits = Physics2D.RaycastNonAlloc(origin, dir, targetRaycastHits, viewRadius, viewMask);
        if (numHits > 0)
        {
            return new ViewCastInfo(true, targetRaycastHits[0].point, targetRaycastHits[0].distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool didHit;
        public Vector2 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool didHit, Vector2 point, float dst, float angle)
        {
            this.didHit = didHit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector2 pointA;
        public Vector2 pointB;

        public EdgeInfo(Vector2 pointA, Vector2 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }
}