using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class HiddenArea : MonoBehaviour
{
    [SerializeField][Range(0, 2)] float revealDuration = 0.5f;
    [SerializeField][Range(0, 2)] float hideDuration = 0.3f;
    [SerializeField][Range(0, 1)] float minAlpha = 0f;

    // we will assume that all sibling and child sprites are overlay sprites
    SpriteRenderer[] sprites;

    Coroutine revealing;
    Coroutine hiding;

    float currentAlpha = 1;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        Reveal();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        Hide();
    }

    void Reveal()
    {
        if (revealing != null) return;
        if (hiding != null) StopCoroutine(hiding);
        revealing = StartCoroutine(Revealing());
    }

    void Hide()
    {
        if (hiding != null) return;
        if (revealing != null) StopCoroutine(revealing);
        StartCoroutine(Hiding());
    }

    IEnumerator Revealing()
    {
        if (revealDuration <= 0)
        {
            SetSpritesAlpha(minAlpha);
            revealing = null;
            yield break;
        }
        while (currentAlpha > minAlpha)
        {
            currentAlpha = Mathf.Clamp(currentAlpha - (Time.deltaTime / revealDuration), minAlpha, 1);
            SetSpritesAlpha(currentAlpha);
            yield return new WaitForEndOfFrame();
        }
        revealing = null;
    }

    IEnumerator Hiding()
    {
        if (hideDuration <= 0)
        {
            currentAlpha = 1;
            SetSpritesAlpha(1);
            hiding = null;
            yield break;
        }
        while (currentAlpha < 1)
        {
            currentAlpha = Mathf.Clamp(currentAlpha + (Time.deltaTime / revealDuration), minAlpha, 1);
            SetSpritesAlpha(currentAlpha);
            yield return new WaitForEndOfFrame();
        }
        hiding = null;
    }

    void SetSpritesAlpha(float incoming)
    {
        if (sprites == null) return;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null) continue;
            sprites[i].color = sprites[i].color.toAlpha(incoming);
        }
    }
}
