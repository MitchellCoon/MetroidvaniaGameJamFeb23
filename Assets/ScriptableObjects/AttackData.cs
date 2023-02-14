using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackType {Melee, Projectile};

[CreateAssetMenu(fileName = "New Attack", menuName = "AttackData")]

public class AttackData : ScriptableObject
{
    public new string name;
    public string description;
    public string animationName;
    public int damage;
    public float duration;
    public AudioClip attackSoundEffect;
    public AttackType attackType;


    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileRange;
    public bool destroyProjectileOnHit;
    public bool knockback; 

}
