using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncyPlatform : MonoBehaviour
{
    [SerializeField][Range(0f, 100f)] float bounceForce = 20f;

    Animator animator;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.attachedRigidbody == null) return;
        if (!IsOnTopOfPlatform(other.transform)) return;
        other.collider.attachedRigidbody.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        if (animator != null) animator.SetTrigger(Constants.BOUNCE_ANIMATION);
    }

    bool IsOnTopOfPlatform(Transform other)
    {
        Vector2 headingToPlayer = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        return Vector2.Dot(Vector2.up, headingToPlayer) > 0.3f;
    }
}
