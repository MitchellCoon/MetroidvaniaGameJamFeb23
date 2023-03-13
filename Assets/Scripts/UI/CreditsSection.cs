using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class CreditsSection : MonoBehaviour
{
    enum SectionType
    {
        Scroll,
        Fade,
    }

    [SerializeField] SectionType sectionType;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] UnityEvent OnContentCenterReached;
    [Space]
    [Space]
    [Header("Fade Settings")]
    [SerializeField][Range(0.1f, 3f)] float fadeDuration = 1f;
    [SerializeField][Range(0.1f, 5f)] float showDuration = 3f;
    [Tooltip("-1 => screen bottom, 0 => screen center, 1 => screen top")]
    [SerializeField][Range(-1, 1)] float enterTriggerThreshold = 0f;
    [Tooltip("-1 => screen bottom, 0 => screen center, 1 => screen top")]
    [SerializeField][Range(-1, 1)] float exitTriggerThreshold = -1f;

    // Order: BL, TL, TR, BR
    Vector3[] worldCorners = new Vector3[4];

    // fade-specific
    bool hasTriggeredEnter;
    bool hasTriggeredExit;
    bool hasTriggeredCenterReached;
    new Camera camera;
    Coroutine fading;
    RectTransform triggerRect;
    Credits credits;

    void Awake()
    {
        SetDependencies();
        Assert.IsNotNull(rectTransform);
        Assert.IsNotNull(canvasGroup);
        Assert.IsNotNull(credits);
    }

    void SetDependencies()
    {
        if (camera == null) camera = Camera.main;
        if (credits == null) credits = GetComponentInParent<Credits>();
    }

    void Start()
    {
        if (sectionType == SectionType.Fade) InitFadeItem();
    }

    void InitFadeItem()
    {
        Assert.IsNotNull(transform.parent, "CreditsItem needs to be a child of CreditsRoll GameObject");

        // Clone thie item & set it to be invisible. This will be used for triggering.
        CreditsSection clone = Instantiate(this, transform.position, Quaternion.identity, transform.parent);
        clone.enabled = false;
        clone.GetComponent<CanvasGroup>().alpha = 0f;
        triggerRect = clone.transform.GetChild(0).GetComponent<RectTransform>();

        // Canvas must be manually calculated as RectTransforms are not calculated until after the first Update frame.
        // see: https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
        Canvas.ForceUpdateCanvases();

        // Re-parent this GameObject outside of the CreditsRoll & center it on the screen
        CalcContentPosition();
        transform.position = Vector3.zero - (GetContentCenter(includeOffset: false) - transform.position);
        transform.SetParent(null);
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        CalcContentPosition();
        HandleFadeBehavior();
        HandleTriggerBehavior();
    }

    void HandleTriggerBehavior()
    {
        if (ShouldTriggerCenterReached())
        {
            hasTriggeredCenterReached = true;
            OnContentCenterReached.Invoke();
        }
    }

    void HandleFadeBehavior()
    {
        if (sectionType != SectionType.Fade) return;
        if (ShouldTriggerEnter())
        {
            FadeIn();
        }
        if (hasTriggeredEnter && !hasTriggeredExit && GetBottomBound() > exitTriggerThreshold * camera.orthographicSize)
        {
            FadeOut();
        }
    }

    bool ShouldTriggerEnter()
    {
        return !hasTriggeredEnter && GetTopBound() > enterTriggerThreshold * camera.orthographicSize;
    }

    bool ShouldTriggerExit()
    {
        return hasTriggeredEnter && !hasTriggeredExit && GetBottomBound() > exitTriggerThreshold * camera.orthographicSize;
    }

    bool ShouldTriggerCenterReached()
    {
        return !hasTriggeredCenterReached && GetContentCenter().y >= 0;
    }

    void FadeIn()
    {
        hasTriggeredEnter = true;
        fading = StartCoroutine(CFade(toAlpha: 1));
    }

    void FadeOut()
    {
        if (fading != null) StopCoroutine(fading);
        hasTriggeredExit = true;
        fading = StartCoroutine(CFade(toAlpha: 0));
    }

    IEnumerator CFade(float toAlpha)
    {
        float direction = toAlpha > canvasGroup.alpha ? 1 : -1;
        while (canvasGroup.alpha != toAlpha)
        {
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha + (Time.deltaTime * direction) / fadeDuration);
            yield return null;
        }
        fading = null;
    }

    void CalcContentPosition()
    {
        // see: https://docs.unity3d.com/ScriptReference/RectTransform.GetWorldCorners.html
        (triggerRect != null ? triggerRect : rectTransform).GetWorldCorners(worldCorners);
    }

    Vector3 GetContentCenter(bool includeOffset = true)
    {
        if (worldCorners == null || worldCorners.Length < 4) return transform.position;
        Vector3 center = (worldCorners[0] + worldCorners[1] + worldCorners[2] + worldCorners[3]) * 0.25f;
        if (!includeOffset || sectionType == SectionType.Scroll) return center;
        return new Vector3(center.x, center.y - GetFadeTransitionBoundOffset(), center.z);
    }

    void Validate()
    {
        Assert.IsNotNull(worldCorners);
        Assert.IsFalse(worldCorners.Length < 4);
    }

    float GetShowTransitionBoundOffset()
    {
        SetDependencies();
        return credits.ScrollSpeed * showDuration * 0.5f;
    }

    float GetFadeTransitionBoundOffset()
    {
        SetDependencies();
        return credits.ScrollSpeed * fadeDuration;
    }

    float GetTopBound()
    {
        Validate();
        if (sectionType == SectionType.Scroll) return worldCorners[1].y;
        return GetContentCenter().y + GetShowTransitionBoundOffset();
    }

    float GetBottomBound()
    {
        Validate();
        if (sectionType == SectionType.Scroll) return worldCorners[0].y;
        return GetContentCenter().y - GetShowTransitionBoundOffset();
    }

    float GetContentHeight()
    {
        Validate();
        if (sectionType == SectionType.Scroll) return worldCorners[1].y - worldCorners[0].y;
        return GetShowTransitionBoundOffset() * 2 + GetFadeTransitionBoundOffset();
    }

    float GetContentWidth()
    {
        Validate();
        return worldCorners[2].x - worldCorners[1].x;
    }

    void OnDrawGizmos()
    {
        SetDependencies();
        if (!credits.DebugLayout) return;
        CalcContentPosition();
        // draw bounds including screen padding
        Gizmos.color = Color.yellow.toAlpha(0.3f);
        Gizmos.DrawWireCube(GetContentCenter(), new Vector3(GetContentWidth() + 1f, GetContentHeight() + camera.orthographicSize * 2, 10));
        // draw bounds
        Gizmos.color = Color.yellow.toAlpha(0.3f);
        if (hasTriggeredEnter) Gizmos.color = Color.green.toAlpha(0.3f);
        if (hasTriggeredExit) Gizmos.color = Color.blue.toAlpha(0.3f);
        Gizmos.DrawCube(GetContentCenter(), new Vector3(GetContentWidth(), GetContentHeight(), 10));
    }
}
