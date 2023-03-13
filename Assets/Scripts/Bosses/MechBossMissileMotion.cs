using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBossMissileMotion : MonoBehaviour
{
    [SerializeField] AttackData attackData;

    [SerializeField] float riseTime;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float riseVelocity;
    [SerializeField] float fallVelocity;
    [SerializeField] float fallHeight;
    [Space]
    [Space]
    [SerializeField] Sound missileFallSound;
    [Space]
    [Space]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] LayerMask detonationLayerMask;

    GameObject player;
    float spawnTime;
    Vector3 playerLocationAtSpawn;
    bool isFalling = false;
    float horizontalOffset;

    void Start()
    {
        rb.velocity = new Vector3(0, riseVelocity , 0);
        spawnTime = Time.time;
        player = GameObject.FindWithTag(Constants.PLAYER_TAG);
        playerLocationAtSpawn = player.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        HandleCollision(other);
    }

    void OnCollisionEnter2D(Collision2D other) {
        HandleCollision(other.collider);
    }

    void HandleCollision(Collider2D other) {
        if (other == null) return;
        if (Time.time - spawnTime < 1f) return;
        if (!Layer.LayerMaskContainsLayer(detonationLayerMask, other.gameObject.layer)) return;
        Explode();
    }

    void Update()
    {
        if (!isFalling && Time.time - spawnTime >= riseTime)
        {
            transform.position = new Vector3(playerLocationAtSpawn.x + horizontalOffset, fallHeight, transform.position.z);
            Vector3 localScale = transform.localScale;
		    localScale.y *= -1;
		    transform.localScale = localScale;
            rb.velocity = new Vector3(0, -fallVelocity , 0);
            isFalling = true;
            if (missileFallSound != null && !missileFallSound.isPlaying) missileFallSound.Play();
        }
    }

    void Explode() {
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (missileFallSound != null) missileFallSound.Stop();
        Destroy(gameObject);
    }

    public void SetHorizontalOffset(float offset)
    {
        horizontalOffset = offset;
    }

}
