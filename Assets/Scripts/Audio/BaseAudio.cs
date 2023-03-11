using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using FMODUnity;
using System;

[RequireComponent(typeof(StudioEventEmitter))]
public abstract class BaseAudio : MonoBehaviour
{
    [SerializeField] bool playWhilePaused = false;

    StudioEventEmitter _emitter;
    StudioEventEmitter emitter { get { if (_emitter == null) _emitter = GetComponent<StudioEventEmitter>(); return _emitter; } }

    // NOTE - the instance gets dynamically attached to the StudioEventEmitter
    // component some time after Play() is invoked.
    FMOD.Studio.EventInstance eventInstance;

    Coroutine fadingIn;
    Coroutine fadingOut;
    Coroutine settingInstance;

    bool _isPaused;

    public bool isPaused => _isPaused;
    public bool isPlaying => emitter.IsPlaying();
    public float curentVolume => GetCurrentVolume();

    public void Play()
    {
        Validate();
        if (settingInstance != null) StopCoroutine(settingInstance);
        if (fadingIn != null) StopCoroutine(fadingIn);
        if (fadingOut != null) StopCoroutine(fadingOut);
        fadingIn = null;
        fadingOut = null;
        emitter.Play();
        settingInstance = StartCoroutine(CSetEventInstance(initialVolume: 1, isPaused));
    }

    public void Stop()
    {
        Validate();
        if (settingInstance != null) StopCoroutine(settingInstance);
        if (fadingIn != null) StopCoroutine(fadingIn);
        if (fadingOut != null) StopCoroutine(fadingOut);
        settingInstance = null;
        fadingIn = null;
        fadingOut = null;
        emitter.Stop();
    }

    public void FadeIn(float fadeDuration)
    {
        if (fadingIn != null) return;
        Validate();
        if (settingInstance != null) StopCoroutine(settingInstance);
        if (fadingOut != null) StopCoroutine(fadingOut);
        settingInstance = null;
        fadingOut = null;
        fadingIn = StartCoroutine(CFadeIn(fadeDuration));
    }

    public void FadeOut(float fadeDuration)
    {
        if (fadingOut != null) return;
        Validate();
        if (settingInstance != null) StopCoroutine(settingInstance);
        if (fadingIn != null) StopCoroutine(fadingIn);
        settingInstance = null;
        fadingIn = null;
        fadingOut = StartCoroutine(CFadeOut(fadeDuration));
    }

    public void Init()
    {
        _emitter = _emitter == null ? GetComponent<FMODUnity.StudioEventEmitter>() : _emitter;
        eventInstance = emitter.EventInstance;
        // emitter.Preload = true;
    }

    protected void RegisterListeners()
    {
        GlobalEvent.OnPause += OnPause;
        GlobalEvent.OnUnpause += OnUnpause;
    }

    protected void UnregisterListeners()
    {
        GlobalEvent.OnPause -= OnPause;
        GlobalEvent.OnUnpause -= OnUnpause;
    }

    void Validate()
    {
        Assert.IsNotNull(emitter, "FMOD StudioEventEmitter is null - make sure BaseAudio.Init() is being called in Awake()");
    }

    void OnPause()
    {
        if (playWhilePaused) return;
        _isPaused = true;
        eventInstance.setPaused(true);
    }

    void OnUnpause()
    {
        _isPaused = false;
        eventInstance.setPaused(false);
    }

    IEnumerator CFadeIn(float fadeDuration)
    {
        yield return CPlayIfNotAlreadyPlaying(initialVolume: 0);
        float durationQuotient = 1f / fadeDuration;
        float volume = 0f;
        while ((volume = GetCurrentVolume()) < 1f)
        {
            while (GetIsPaused()) yield return null;
            volume += Time.deltaTime * durationQuotient;
            eventInstance.setVolume(Mathf.Clamp01(volume));
            yield return null;
        }
        fadingIn = null;
    }

    IEnumerator CFadeOut(float fadeDuration)
    {
        yield return CPlayIfNotAlreadyPlaying(initialVolume: 1);
        float durationQuotient = 1f / fadeDuration;
        float volume = 1f;
        while ((volume = GetCurrentVolume()) > 0f)
        {
            while (GetIsPaused()) yield return null;
            volume -= Time.deltaTime * durationQuotient;
            eventInstance.setVolume(Mathf.Clamp01(volume));
            yield return null;
        }
        emitter.Stop();
        fadingOut = null;
    }

    IEnumerator CPlayIfNotAlreadyPlaying(float initialVolume)
    {
        if (emitter.IsPlaying() && eventInstance.isValid()) yield break;
        emitter.Play();
        yield return CSetEventInstance(initialVolume, isPaused);
        while (!GetIsStatePlaying()) yield return null;
    }

    IEnumerator CSetEventInstance(float initialVolume, bool paused)
    {
        while (!eventInstance.isValid())
        {
            eventInstance = emitter.EventInstance;
            yield return null;
        }
        eventInstance.setVolume(initialVolume);
        eventInstance.setPaused(paused);
    }

    float GetCurrentVolume()
    {
        if (emitter == null && !Application.isPlaying) Init();
        float volume = 0f;
        FMOD.RESULT result = eventInstance.getVolume(out volume);
        if (result < FMOD.RESULT.OK)
        {
            throw new UnityException(System.Enum.GetName(typeof(FMOD.RESULT), result));
        }
        return volume;
    }

    bool GetIsPaused()
    {
        if (emitter == null && !Application.isPlaying) Init();
        bool paused;
        FMOD.RESULT result = eventInstance.getPaused(out paused);
        if (result < FMOD.RESULT.OK)
        {
            throw new UnityException(System.Enum.GetName(typeof(FMOD.RESULT), result));
        }
        return paused;
    }

    bool GetIsStatePlaying()
    {
        if (emitter == null) Init();
        if (!eventInstance.isValid()) return false;
        FMOD.Studio.PLAYBACK_STATE state;
        FMOD.RESULT result = eventInstance.getPlaybackState(out state);
        if (result < FMOD.RESULT.OK)
        {
            throw new UnityException(System.Enum.GetName(typeof(FMOD.RESULT), result));
        }
        switch (state)
        {
            case FMOD.Studio.PLAYBACK_STATE.PLAYING:
                return true;
            case FMOD.Studio.PLAYBACK_STATE.STARTING:
            case FMOD.Studio.PLAYBACK_STATE.SUSTAINING:
            case FMOD.Studio.PLAYBACK_STATE.STOPPING:
            case FMOD.Studio.PLAYBACK_STATE.STOPPED:
            default:
                return false;
        }
    }
}
