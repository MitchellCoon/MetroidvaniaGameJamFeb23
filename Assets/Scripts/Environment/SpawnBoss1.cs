using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SpawnBoss1 : MonoBehaviour
{
    [SerializeField][Range(0, 10)] float bossEnterDelay = 5.0f; 
    AudioSource bgm; 
    AudioSource bossTheme; 

    void Start()
    {
        bossTheme = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        StartCoroutine(SpawnBoss()); 
    }

    IEnumerator SpawnBoss(){ 
        Debug.Log($"GOING TO PLAY {bossTheme.name}"); 
        yield return new WaitForSeconds(bossEnterDelay);
        bossTheme.Play(); 
    }
}
