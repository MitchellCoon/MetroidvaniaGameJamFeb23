using UnityEngine;

using CyberneticStudios.SOFramework;
using System;

public class LightManager : MonoBehaviour
{
    [SerializeField][Range(0f, 3f)] float fadeDuration = 1f;
    [SerializeField][Range(0f, 3f)] float pulseInterval = 1f;
    [Space]
    [Space]
    [SerializeField] BoolVariable isSecurityDisabled;
    [SerializeField] BoolVariable isBossDefeated;

    void Awake()
    {
        GlobalLight.Init();
    }

    void OnEnable()
    {
        GlobalEvent.OnGameInit += OnGameInit;
        isSecurityDisabled.OnChanged += OnSecurityDisabledChange;
        isBossDefeated.OnChanged += OnBossDefeatedChange;
    }

    void OnDisable()
    {
        GlobalEvent.OnGameInit -= OnGameInit;
        isSecurityDisabled.OnChanged -= OnSecurityDisabledChange;
        isBossDefeated.OnChanged -= OnBossDefeatedChange;
    }

    void OnGameInit()
    {
        GlobalLight.Init();
    }

    void OnSecurityDisabledChange(bool didDisable)
    {
        if (didDisable)
        {
            GlobalLight.FadeTo(0f, fadeDuration, this);
        }
        else
        {
            GlobalLight.FadeTo(1f, fadeDuration, this);
        }
    }

    void OnBossDefeatedChange(bool didDefeat)
    {
        if (didDefeat)
        {
            GlobalLight.PulseLights(min: 0f, max: 0.7f, pulseInterval, this);
        }
        else
        {
            OnSecurityDisabledChange(isSecurityDisabled.value);
        }
    }
}
