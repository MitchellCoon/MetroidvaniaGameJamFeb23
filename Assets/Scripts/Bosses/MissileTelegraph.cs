using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTelegraph : MonoBehaviour
{
    [SerializeField] float duration;
    float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime > duration)
        {
            Destroy(gameObject);
        }
    }
}
