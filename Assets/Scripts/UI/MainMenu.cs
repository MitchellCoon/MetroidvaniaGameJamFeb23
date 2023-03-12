using UnityEngine;
using UnityEngine.SceneManagement;

using DevLocker.Utils;

public class MainMenu : Menu
{
    [SerializeField] SceneReference firstLevel;

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevel.SceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Awake()
    {
        Init();
    }

    void Start()
    {
        FocusOnFirstButton();
        GlobalEvent.Invoke.OnGameInit();
    }
}
