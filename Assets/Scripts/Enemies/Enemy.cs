using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] int maxHealth = 1;
    public int currentHealth;
    public Color damagedColor; 
    public Vector2 knockbackForce;
    Rigidbody2D rb ; 
    Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        originalColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
        

    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        currentHealth -= damage;
        Vector2 getX = knockbackForce* (hitPosition - transform.position).normalized; 

        rb.AddForce( new Vector2( getX.x,knockbackForce.y), ForceMode2D.Impulse); 
        changeColor();
        if(currentHealth <= 0)
        {
            Die();
        }
    }
    public void changeColor()
    {
        GetComponent<SpriteRenderer>().color = damagedColor;
        Invoke("resetColor", 0.1f);
    }
    public void resetColor()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
