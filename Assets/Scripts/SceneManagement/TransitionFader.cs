using System.Collections;
using UnityEngine;

namespace DTDEV.SceneManagement
{

    [RequireComponent(typeof(CanvasGroup))]
    public class TransitionFader : MonoBehaviour
    {
        [SerializeField] bool isFadedOutAtStart = false;
        [SerializeField][Range(0f, 5f)] float fadeInDuration = 1f;
        [SerializeField][Range(0f, 1f)] float fadeInDelay = .1f;
        [SerializeField][Range(0f, 5f)] float fadeOutDuration = 1f;

        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            if (isFadedOutAtStart)
            {
                SetOpaque();
            }
            else
            {
                SetTransparent();
            }
        }

        public void SetOpaque()
        {
            canvasGroup.alpha = 1f;
        }

        public void SetTransparent()
        {
            canvasGroup.alpha = 0f;
        }

        public IEnumerator FadeIn(System.Action<float> OnFadeTick = null)
        {
            StopCoroutine("FadeOut");
            yield return new WaitForSeconds(fadeInDelay);
            if (fadeInDuration <= 0)
            {
                canvasGroup.alpha = 0f;
                yield break;
            }
            float t = 0;
            while (canvasGroup.alpha > 0 && t < fadeInDuration * 2f)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha -= Time.unscaledDeltaTime / fadeInDuration;
                ExecFadeTick(OnFadeTick);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            ExecFadeTick(OnFadeTick);
        }

        public IEnumerator FadeOut(System.Action<float> OnFadeTick = null)
        {
            StopCoroutine("FadeIn");
            if (fadeOutDuration <= 0)
            {
                canvasGroup.alpha = 1f;
                yield break;
            }
            float t = 0;
            while (canvasGroup.alpha < 1 && t < fadeOutDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha += Time.unscaledDeltaTime / fadeOutDuration;
                ExecFadeTick(OnFadeTick);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            ExecFadeTick(OnFadeTick);
        }

        void ExecFadeTick(System.Action<float> OnFadeTick = null)
        {
            if (OnFadeTick == null) return;
            if (OnFadeTick != null) OnFadeTick(canvasGroup.alpha);
        }
    }
}

