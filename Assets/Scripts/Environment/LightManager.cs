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
    [SerializeField] BoolVariable hasStartedBossFight;

    void Awake()
    {
        GlobalLight.Init();
    }

    void OnEnable()
    {
        GlobalEvent.OnGameInit += OnGameInit;
        isSecurityDisabled.OnChanged += OnSecurityDisabledChange;
        hasStartedBossFight.OnChanged += OnHasEnteredBossArenaChanged;
        isBossDefeated.OnChanged += OnBossDefeatedChange;
    }

    void OnDisable()
    {
        GlobalEvent.OnGameInit -= OnGameInit;
        isSecurityDisabled.OnChanged -= OnSecurityDisabledChange;
        hasStartedBossFight.OnChanged += OnHasEnteredBossArenaChanged;
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

    void OnHasEnteredBossArenaChanged(bool didEnter)
    {
        if (didEnter)
        {
            GlobalLight.FadeTo(1f, fadeDuration, this);
        }
        else
        {
            OnSecurityDisabledChange(isSecurityDisabled.value);
        }
    }

    void OnBossDefeatedChange(bool didDefeat)
    {
        if (didDefeat)
        {
            // GlobalLight.PulseLights(min: 0f, max: 0.7f, pulseInterval, this);
            GlobalLight.FadeTo(1f, fadeDuration, this);
        }
        else
        {
            OnSecurityDisabledChange(isSecurityDisabled.value);
        }
    }
}
