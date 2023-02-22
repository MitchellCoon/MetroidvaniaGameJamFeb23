using UnityEngine;
using UnityEngine.Assertions;

public class Laser : MonoBehaviour
{
    const float MAX_LASER_DISTANCE = 50f;

    [SerializeField] bool debug;
    [SerializeField] LayerMask impactLayer;
    [SerializeField][Range(0, 1)] float raycastFrequency = 0.1f;
    [Space]
    [Space]
    [SerializeField] AttackData laserAttackData;
    [SerializeField][Range(0, 1)] float damageFrequency = 0.1f;
    [Space]
    [Space]
    [SerializeField] SpriteRenderer beamSprite;
    [SerializeField] SpriteRenderer originSprite;
    [SerializeField] ParticleSystem originFX;
    [SerializeField] ParticleSystem impactFX;

    Enemy enemy;
    PlayerCombat player;

    RaycastHit2D[] targetRaycastHits = new RaycastHit2D[1];
    Vector2 impactPoint;
    int numHits;
    float timeLastRaycasted = float.MaxValue;
    float timeLastDamaged = float.MaxValue;

    void Awake()
    {
        Assert.IsNotNull(beamSprite);
        Assert.IsNotNull(originSprite);
        Assert.IsNotNull(originFX);
        Assert.IsNotNull(impactFX);
    }

    void Start()
    {
        RepositionElements();
    }

    void Update()
    {
        PerformRaycast();
        RepositionElements();
        ApplyDamage();
        timeLastRaycasted += Time.deltaTime;
        timeLastDamaged += Time.deltaTime;
    }

    void PerformRaycast()
    {
        if (timeLastRaycasted < raycastFrequency) return;
        numHits = Physics2D.RaycastNonAlloc(transform.position + transform.right, transform.right, targetRaycastHits, MAX_LASER_DISTANCE, impactLayer);
        if (numHits > 0)
        {
            impactPoint = targetRaycastHits[0].point;
            timeLastRaycasted = 0;
            AcquireDamageTarget(targetRaycastHits[0].rigidbody);
        }
    }

    void RepositionElements()
    {
        if (numHits > 0)
        {
            impactFX.transform.position = impactPoint;
            impactFX.Play();
        }
        else
        {
            impactPoint = transform.position + transform.right * MAX_LASER_DISTANCE;
            impactFX.Stop();
        }
        beamSprite.transform.position = (Vector2)transform.position * 0.5f + impactPoint * 0.5f;
        beamSprite.transform.localScale = new Vector3(1, Vector2.Distance(transform.position, impactPoint), 1);
    }

    void AcquireDamageTarget(Rigidbody2D other)
    {
        if (other == null)
        {
            player = null;
            enemy = null;
            timeLastDamaged = float.MaxValue;
        }
        else
        {
            player = other.GetComponent<PlayerCombat>();
            enemy = other.GetComponent<Enemy>();
        }
    }

    void ApplyDamage()
    {
        if (timeLastDamaged < damageFrequency) return;
        if (player != null)
        {
            player.TakeDamage(laserAttackData, transform.position);
            timeLastDamaged = 0f;
        }
        else if (enemy != null)
        {
            enemy.TakeDamage(laserAttackData.damage, transform.position, false);
            timeLastDamaged = 0f;
        }
    }

    void OnDrawGizmos()
    {
        if (!debug) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.DrawSphere(impactPoint, 0.5f);
    }
}
