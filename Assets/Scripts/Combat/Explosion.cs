using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class Explosion : MonoBehaviour
{
    [SerializeField] ParticleSystem smokeFx;
    [SerializeField] Sound explosionSound;

    new Collider2D collider;
    Animator animator;
    Hitbox hitbox;

    public void AnimStopDamage()
    {
        hitbox.enabled = false;
        collider.enabled = false;
        Destroy(hitbox);
        Destroy(collider);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        hitbox = GetComponent<Hitbox>();
        collider = GetComponent<Collider2D>();
        Assert.IsNotNull(smokeFx);
        Assert.IsNotNull(hitbox);
        Assert.IsNotNull(collider);
    }

    void Start()
    {
        if (explosionSound != null) explosionSound.Play();
    }

    void Update()
    {
        if (!smokeFx.isPlaying) Destroy(gameObject);
    }
}
