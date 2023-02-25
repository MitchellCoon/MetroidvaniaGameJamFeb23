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
    Vector3 spriteOriginalPos;
    Vector3 objectOriginalPos;
    Rigidbody2D localRB;

    void Start()
    {

        spriteOriginalPos = spriteObject.transform.position;
        objectOriginalPos = transform.position;
        localRB = GetComponent<Rigidbody2D>();

    }
    void Restore()
    {
        localRB.isKinematic = true;
        localRB.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.position = objectOriginalPos;
        spriteObject.transform.position = spriteOriginalPos;
        elapsed = 0.0f;
    }
    IEnumerator OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Shake());
            yield return new WaitForSeconds(duration);
            Fall();
            yield return new WaitForSeconds(RestoreTime);
            Restore();
        }
    }

    void Fall()
    {
        localRB.isKinematic = false;
        localRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator Shake()
    {

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitudeX;
            float y = Random.Range(-1f, 1f) * shakeMagnitudeY;
            spriteObject.transform.position = new Vector3(spriteOriginalPos.x + x, spriteOriginalPos.y + y, spriteOriginalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = spriteOriginalPos;
    }

}
