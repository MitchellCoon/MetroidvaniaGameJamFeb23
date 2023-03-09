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

    [SerializeField] PossessionManager possessionManager;
    PossessionManager targetPossessionManager;

    public void UpdateAttackData(AttackData newAttackData)
    {
        attackData = newAttackData;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((!isEnemyHitbox && other.CompareTag(Constants.ENEMY_TAG)) || (isEnemyHitbox && possessionManager != null && possessionManager.IsPossessed()))
        {
            if (attackData.willPossessTarget && other.TryGetComponent<PossessionManager>(out var possessionManager))
            {
                possessionManager.GetPossessed(transform.parent.gameObject);
                return;
            }
            if (other.TryGetComponent<BaseEnemyAI>(out var enemyAI))
            {
                enemyAI.TakeDamage(attackData, transform.position);
            }
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(attackData.damage, transform.position, true);
            }
        }
        targetPossessionManager = other.GetComponent<PossessionManager>();
        if (isEnemyHitbox && other.CompareTag(Constants.PLAYER_TAG) && targetPossessionManager == null)
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
}
