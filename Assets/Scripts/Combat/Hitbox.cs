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

    public void UpdateAttackData(AttackData newAttackData)
    {
        attackData = newAttackData;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemyHitbox && other.CompareTag(Constants.ENEMY_TAG))
        {
            other.GetComponent<BaseEnemyAI>().TakeDamage(attackData, transform.position);
            if (attackData.willPossessTarget)
            {
                other.GetComponent<PossessionManager>().GetPossessed(transform.parent.gameObject);
            }
        }

        if (isEnemyHitbox && other.CompareTag(Constants.PLAYER_TAG))
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
