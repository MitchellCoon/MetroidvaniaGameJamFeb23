using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HiddenArea : MonoBehaviour
{
    [SerializeField][Range(0, 2)] float revealDuration = 0.5f;
    [SerializeField][Range(0, 2)] float hideDuration = 0.3f;
    [SerializeField][Range(0, 1)] float minAlpha = 0f;

    // we will assume that all sibling and child sprites are overlay sprites
    SpriteRenderer[] sprites;
    Tilemap[] tilemaps;
    CompositeCollider2D compositeCollider;

    Coroutine revealing;
    Coroutine hiding;

    float currentAlpha = 1;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        tilemaps = GetComponentsInChildren<Tilemap>();
        InitCompositeColliderIfExists();
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
        hiding = null;
        revealing = StartCoroutine(Revealing());
    }

    void Hide()
    {
        if (hiding != null) return;
        if (revealing != null) StopCoroutine(revealing);
        revealing = null;
        hiding = StartCoroutine(Hiding());
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
        if (sprites != null)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] == null) continue;
                sprites[i].color = sprites[i].color.toAlpha(incoming);
            }
        }
        if (tilemaps != null)
        {
            for (int i = 0; i < tilemaps.Length; i++)
            {
                if (tilemaps[i] == null) continue;
                tilemaps[i].color = tilemaps[i].color.toAlpha(incoming);
            }
        }
    }

    void InitCompositeColliderIfExists()
    {
        compositeCollider = GetComponent<CompositeCollider2D>();
        if (compositeCollider == null) return;
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }
}
