using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LightingElement : MonoBehaviour
{
    SpriteRenderer sprite;

    float initialAlpha = 1f;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        initialAlpha = sprite.color.a;
        if (enabled) sprite.color = sprite.color.toAlpha(0);
    }

    void Start()
    {
        sprite.enabled = true;
        if (enabled) RenderLight();
    }

    void Update()
    {
        RenderLight();
    }

    void RenderLight()
    {
        sprite.color = sprite.color.toAlpha(Mathf.Lerp(0, initialAlpha, 1 - GlobalLight.Value));
    }
}
