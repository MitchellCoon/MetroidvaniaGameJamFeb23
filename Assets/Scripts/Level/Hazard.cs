using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Hazard : MonoBehaviour
{
    [SerializeField] AttackData attackData;
    [SerializeField] bool shouldDamageContinuously = false;

    ContactPoint2D[] contacts = new ContactPoint2D[4];

    CompositeCollider2D compositeCollider;

    void Awake()
    {
        Assert.IsNotNull(attackData);
        InitCompositeColliderIfExists();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 collisionPoint = other.attachedRigidbody != null
            ? -other.attachedRigidbody.velocity.normalized
            : Vector2.zero;
        HandleCollision(other, collisionPoint);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!shouldDamageContinuously) return;
        HandleCollision(other, -other.attachedRigidbody.velocity.normalized);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.collider, GetCollisionCenterPoint(other));
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (!shouldDamageContinuously) return;
        HandleCollision(other.collider, GetCollisionCenterPoint(other));
    }

    void HandleCollision(Collider2D other, Vector2 collisionPoint)
    {
        if (other.CompareTag(Constants.PLAYER_TAG) && other.TryGetComponent<PlayerCombat>(out PlayerCombat playerCombat))
        {
            playerCombat.TakeDamage(attackData, collisionPoint);
        }
        if (other.CompareTag(Constants.ENEMY_TAG) && other.TryGetComponent<BaseEnemyAI>(out BaseEnemyAI enemyAI))
        {
            enemyAI.TakeDamage(attackData, transform.position);
        }
    }

    Vector2 GetCollisionCenterPoint(Collision2D other)
    {
        int numContacts = other.GetContacts(contacts);
        if (numContacts == 0) return transform.position;
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < numContacts; i++)
        {
            sum += contacts[i].point;
        }
        return sum / numContacts;
    }

    void InitCompositeColliderIfExists()
    {
        if (!shouldDamageContinuously) return;
        compositeCollider = GetComponent<CompositeCollider2D>();
        if (compositeCollider == null) return;
        // If a composite collider is being used, we need to set its
        // geometry type to be "Polygons" so that the collider registers
        // the player as being inside the collider. Otherwise, OnTriggerExit
        // will fire when the player collider is no longer touching one
        // of the CompositeCollider edges (i.e. inside but not overlapping).
        // see: https://docs.unity3d.com/ScriptReference/CompositeCollider2D.GeometryType.Polygons.html
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }
}
