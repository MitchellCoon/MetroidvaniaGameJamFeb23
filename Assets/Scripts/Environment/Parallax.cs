using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Tooltip("-1 => closest in FG, 1 => furtherst in BG")]
    [SerializeField][Range(-1f, 1f)] float depth = 0.25f;

    Vector2 initialPosition;
    Vector3 position;

    new Camera camera;

    void Start()
    {
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        if (camera == null) camera = Camera.main;
        if (camera == null) return;
        position = initialPosition + (Vector2)camera.transform.position * depth;
        position.z = transform.position.z;
        transform.position = position;
    }
}
