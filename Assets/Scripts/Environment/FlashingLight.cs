using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] bool activeAtStart;
    [SerializeField] SpriteRenderer lightSprite;

    float luminosity = 0f;
    float maxLumen = 0f;

    public void AnimSetLuminosity(float incoming)
    {
        luminosity = incoming;
        RenderLight();
    }

    void Awake()
    {
        maxLumen = lightSprite.color.a;
        if (!activeAtStart) animator.enabled = false;
        RenderLight();
    }

    void RenderLight()
    {
        lightSprite.color = lightSprite.color.toAlpha(luminosity * maxLumen);
    }
}
