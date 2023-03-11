using System.Collections;
using UnityEngine;

public static class GlobalLight
{
    static float value = 1f;

    public static float Value => Mathf.Clamp01(value);

    static MonoBehaviour currentScript;
    static Coroutine currentAction;

    public static void Init()
    {
        StopCurrentAction();
        value = 1;
    }

    public static void FadeTo(float targetValue, float duration, MonoBehaviour script)
    {
        StopCurrentAction();
        currentScript = script;
        currentAction = script.StartCoroutine(FadingTo(targetValue, duration));
    }

    public static void PulseLights(float min, float max, float interval, MonoBehaviour script)
    {
        StopCurrentAction();
        currentScript = script;
        currentAction = script.StartCoroutine(PulsingLights(min, max, interval));
    }

    public static void StopCurrentAction()
    {
        if (currentAction != null && currentScript != null) currentScript.StopCoroutine(currentAction);
        currentAction = null;
        currentScript = null;
    }

    static IEnumerator FadingTo(float targetValue, float duration)
    {
        float t = 0f;
        float initialValue = value;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            value = Mathf.Lerp(initialValue, targetValue, t);
            yield return null;
        }
    }

    static IEnumerator PulsingLights(float minValue, float maxValue, float interval)
    {
        while (true)
        {
            yield return FadingTo(maxValue, interval);
            yield return FadingTo(minValue, interval);
        }
    }
}
