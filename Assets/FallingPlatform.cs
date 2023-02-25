using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject spriteObject;
    public float elapsed = 0.0f;
    public float duration = 1.5f;
    public float magnitudeX = 0.3f;
    public float magnitudeY = 0.3f;
    public float RestoreTime = 5f;
    Vector3 spriteOriginalPos;
    Vector3 objectOriginalPos;
    Rigidbody2D localRB ; 

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
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Shake());
            Invoke("Fall", duration);
        }
    }

    void Fall()
    {
        localRB.isKinematic = false;
        localRB.constraints = RigidbodyConstraints2D.FreezeRotation ; 
        Invoke("Restore", RestoreTime);
    }

    IEnumerator Shake()
    {

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitudeX;
            float y = Random.Range(-1f, 1f) * magnitudeY;
            spriteObject.transform.position = new Vector3(spriteOriginalPos.x + x, spriteOriginalPos.y + y, spriteOriginalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = spriteOriginalPos;
    }

}
