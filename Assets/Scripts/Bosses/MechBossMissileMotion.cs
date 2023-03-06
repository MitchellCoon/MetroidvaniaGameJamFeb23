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
   
    GameObject player;
    float spawnTime;
    Vector3 playerLocationAtSpawn;
    bool isFalling = false;

    void Start()
    {
        rb.velocity = new Vector3(0, riseVelocity , 0);
        spawnTime = Time.time;
        player = GameObject.Find("Player");
        playerLocationAtSpawn = player.transform.position;

    }

    void Update()
    {
        if (!isFalling && Time.time - spawnTime >= riseTime)
        {
            transform.position = new Vector3(playerLocationAtSpawn.x, fallHeight, transform.position.z);
            Vector3 localScale = transform.localScale;
		    localScale.y *= -1;
		    transform.localScale = localScale;
            rb.velocity = new Vector3(0, -fallVelocity , 0);
            isFalling = true;
        }
    }

}