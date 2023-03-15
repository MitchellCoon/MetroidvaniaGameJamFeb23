using UnityEngine;
using UnityEngine.SceneManagement;
using DevLocker.Utils;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : Menu
{
    [SerializeField] SceneReference mainMenuScene;
    [SerializeField] GameObject panel;
    [SerializeField] Sound pauseSound;

    Canvas canvas;

    bool isPausedEnabled;
    bool isPaused;

    public void Unpause()
    {
        HandleUnpause();
    }

    public void GotoMainMenu()
    {
        HandleUnpause();
        SceneManager.LoadScene(mainMenuScene.SceneName);
    }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        Init();
    }

    void OnDestroy()
    {
        HandleUnpause();
    }

    void OnEnable()
    {
        GlobalEvent.OnGameInit += OnGameInit;
        GlobalEvent.OnPlayerSpawn += OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath += OnPlayerDeath;
        GlobalEvent.OnEnemyPossessed += OnEnemyPossessed;
        GlobalEvent.OnWinGame += OnWinGame;
    }

    void OnDisable()
    {
        GlobalEvent.OnGameInit -= OnGameInit;
        GlobalEvent.OnPlayerSpawn -= OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath -= OnPlayerDeath;
        GlobalEvent.OnEnemyPossessed -= OnEnemyPossessed;
        GlobalEvent.OnWinGame -= OnWinGame;
    }

    void OnGameInit()
    {
        isPausedEnabled = false;
        HandleUnpause();
    }

    void OnPlayerSpawn(PlayerMain obj)
    {
        isPausedEnabled = true;
    }

    void OnPlayerDeath()
    {
        isPausedEnabled = true;
    }

    void OnEnemyPossessed(PlayerMain obj)
    {
        isPausedEnabled = true;
    }

    void OnWinGame()
    {
        isPausedEnabled = false;
        HandleUnpause();
    }

    void Start()
    {
        canvas.enabled = false;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (!isPausedEnabled) return;
        bool hasPauseInput = MInput.GetKeyDown(KeyCode.Escape) || MInput.GetPadDown(GamepadCode.Start);
        if (!hasPauseInput) return;
        if (isPaused)
        {
            HandleUnpause();
        }
        else
        {
            HandlePause();
        }
    }

    void HandlePause()
    {
        if (isPaused) return;
        SetActiveElements(true);
        Time.timeScale = 0f;
        GlobalEvent.Invoke.OnPause();
        if (pauseSound != null) pauseSound.Play();
        FocusOnFirstButton();
    }

    void HandleUnpause()
    {
        if (!isPaused) return;
        SetActiveElements(false);
        Time.timeScale = 1f;
        GlobalEvent.Invoke.OnUnpause();
    }

    void SetActiveElements(bool value)
    {
        isPaused = value;
        panel.SetActive(value);
        canvas.enabled = value;
    }
}
