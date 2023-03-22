using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderFireHitboxSpawner : MonoBehaviour
{
    [SerializeField] AttackData attackData;
    [SerializeField] Hitbox hitboxPrefab;
    
    [SerializeField] PossessionManager sourcePossessionManager;
    float hitboxSpawnTimer = 0f;

    void Update()
    {
        hitboxSpawnTimer += Time.deltaTime;

        if (hitboxSpawnTimer >= attackData.duration)
        {
            Instantiate(hitboxPrefab, transform.position, transform.rotation).SetPossessionManager(sourcePossessionManager);
            hitboxSpawnTimer = 0f;
        }
    }

    public void SetPossessionManager(PossessionManager possessionManager)
    {
        sourcePossessionManager = possessionManager;
    }
}
