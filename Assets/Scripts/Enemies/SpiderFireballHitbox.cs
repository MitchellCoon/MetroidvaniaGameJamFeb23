using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderFireballHitbox : Hitbox
{
    [SerializeField] GameObject firePrefab;
    [SerializeField] Vector3 spawnAngle;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!Layer.LayerMaskContainsLayer(targetLayers, other.gameObject.layer)) return;
        
        if (other.CompareTag(Constants.ENEMY_TAG) && (!isEnemyHitbox || (isEnemyHitbox && sourcePossessionManager != null && sourcePossessionManager.IsPossessed())))
        {
            PlayHitSound();
            Instantiate(firePrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        targetPossessionManager = other.GetComponent<PossessionManager>();
        if (isEnemyHitbox && (other.CompareTag(Constants.PLAYER_TAG) && targetPossessionManager == null) ||
            (targetPossessionManager != null && targetPossessionManager.IsPossessed() && (sourcePossessionManager == null || !sourcePossessionManager.IsPossessed())))
        {
            other.GetComponent<PlayerCombat>().TakeDamage(attackData, transform.position);
            PlayHitSound();
            Instantiate(firePrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
