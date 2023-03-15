using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject spriteObject;

    [SerializeField]
    float elapsed = 0.0f;

    [SerializeField]
    float duration = 1.5f;

    [SerializeField]
    float shakeMagnitudeX = 0.0f;

    [SerializeField]
    float shakeMagnitudeY = 0.1f;

    [SerializeField]
    float RestoreTime = 5f;

    [SerializeField]
    bool isShaking = false;

    [SerializeField]
    bool disableCollidersWhileFalling = false;

    [SerializeField] Sound rumbleSound;
    [SerializeField] Sound fallSound;

    Vector3 spriteOriginalPos;
    Vector3 objectOriginalPos;
    RigidbodyConstraints2D originalConstraints;
    Rigidbody2D localRB;
    new Collider2D collider;

    void Start()
    {
        spriteOriginalPos = spriteObject.transform.position;
        objectOriginalPos = transform.position;
        localRB = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        originalConstraints = localRB.constraints;
    }
    void Restore()
    {
        localRB.isKinematic = true;
        localRB.velocity = Vector2.zero;
        localRB.constraints = originalConstraints;
        transform.position = objectOriginalPos;
        spriteObject.transform.position = spriteOriginalPos;
        elapsed = 0.0f;
        isShaking = false;
        if (collider != null) collider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isShaking) return;
        if (!IsOnTopOfPlatform(other.transform)) return;
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG))
        {
            isShaking = true;
            StartCoroutine(CFallSequence());
        }
    }

    IEnumerator CFallSequence()
    {
        yield return Shake();
        Fall();
        yield return new WaitForSeconds(RestoreTime);
        Restore();
    }

    void Fall()
    {
        if (fallSound != null) fallSound.Play();
        localRB.isKinematic = false;
        localRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (disableCollidersWhileFalling && collider != null) collider.enabled = false;
    }

    IEnumerator Shake()
    {
        if (rumbleSound != null) rumbleSound.Play();
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitudeX;
            float y = Random.Range(-1f, 1f) * shakeMagnitudeY;
            spriteObject.transform.position = new Vector3(spriteOriginalPos.x + x, spriteOriginalPos.y + y, spriteOriginalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = spriteOriginalPos;
        if (rumbleSound != null) rumbleSound.Stop();
    }

    bool IsOnTopOfPlatform(Transform other)
    {
        Vector2 headingToPlayer = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        return Vector2.Dot(Vector2.up, headingToPlayer) > 0.3f;
    }
}
