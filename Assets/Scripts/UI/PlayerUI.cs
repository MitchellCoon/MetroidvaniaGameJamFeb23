using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PlayerUI : MonoBehaviour
{
    [SerializeField] Resource health;
    [SerializeField] Image healthBar;

    Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        // register events
        GlobalEvent.OnPlayerSpawn += OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath += OnPlayerDeath;
        GlobalEvent.OnWinGame += OnWinGame;
        GlobalEvent.OnHidePlayerUI += OnHidePlayerUI;
        health.OnResourceUpdated += OnHealthUpdated;
        // init
        Hide();
    }

    void OnDestroy()
    {
        GlobalEvent.OnPlayerSpawn -= OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath -= OnPlayerDeath;
        GlobalEvent.OnWinGame -= OnWinGame;
        GlobalEvent.OnHidePlayerUI -= OnHidePlayerUI;
        health.OnResourceUpdated -= OnHealthUpdated;
    }

    void OnPlayerSpawn(PlayerMain obj)
    {
        Show();
    }

    void OnPlayerDeath()
    {
        Hide();
    }

    void OnWinGame()
    {
        Hide();
    }

    void OnHidePlayerUI()
    {
        Hide();
    }

    void Show()
    {
        canvas.enabled = true;
        RenderUI(health.GetCurrentPercentage());
    }

    void Hide()
    {
        canvas.enabled = false;
    }

    void OnHealthUpdated()
    {
        if (!canvas.enabled) return;
        RenderUI(health.GetCurrentPercentage());
    }

    void RenderUI(float healthPercentage)
    {
        healthBar.fillAmount = healthPercentage;
    }
}
