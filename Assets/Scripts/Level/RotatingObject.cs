using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    const float ROTATION_MOD = 50f;

    [SerializeField][Range(0, 20f)] float rotationSpeed = 5f;
    [SerializeField] bool isClockwise = true;

    Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (body != null) body.isKinematic = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * ROTATION_MOD * rotationSpeed * (isClockwise ? -1 : 1));
    }
}
