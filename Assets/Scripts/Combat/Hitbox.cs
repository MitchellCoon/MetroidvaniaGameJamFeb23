using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  DisableSpriteRender causes the sprite to be disabled in runtime builds, but not the editor.
// This is useful for hitboxes, or other objects that you don't want to see in the game, but you do want to see in the editor.
public class Hitbox : DisableSpriteRender
{
    [SerializeField] AttackData attackData;
    public bool isEnemyHitbox = false;
    public bool hitOnce = false;

    [SerializeField] PossessionManager sourcePossessionManager;
    PossessionManager targetPossessionManager;

    public void UpdateAttackData(AttackData newAttackData)
    {
        attackData = newAttackData;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.ENEMY_TAG) && (!isEnemyHitbox || (isEnemyHitbox && sourcePossessionManager != null && sourcePossessionManager.IsPossessed())))
        {
            if (attackData.willPossessTarget && other.TryGetComponent<PossessionManager>(out var targetPossessionManager))
            {
                targetPossessionManager.GetPossessed(transform.parent.gameObject);
                return;
            }
            if (other.TryGetComponent<BaseEnemyAI>(out var enemyAI))
            {
                enemyAI.TakeDamage(attackData, transform.position);
            }
            else if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(attackData.damage, transform.position, true);
            }
            else if (other.TryGetComponent<MechBossAI>(out var boss))
            {
                boss.TakeDamage(attackData, transform.position);
            }
            if (hitOnce)
            {
                gameObject.SetActive(false);
            }
        }
        targetPossessionManager = other.GetComponent<PossessionManager>();
        if (isEnemyHitbox && (other.CompareTag(Constants.PLAYER_TAG) && targetPossessionManager == null) ||
            (targetPossessionManager != null && targetPossessionManager.IsPossessed() && (sourcePossessionManager == null || !sourcePossessionManager.IsPossessed())))
        {
            other.GetComponent<PlayerCombat>().TakeDamage(attackData, transform.position);
            if (hitOnce)
            {
                gameObject.SetActive(false);
            }
        }
        if (other.CompareTag("Interactable"))
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Use();
            }
            else
            {
                Debug.LogError($"{gameObject.name} has \"Interactable\" tag but needs a component that implements the Interactable interface");
            }
        }
    }

    public void SetPossessionManager(PossessionManager possessionManager)
    {
        sourcePossessionManager = possessionManager;
    }
}
