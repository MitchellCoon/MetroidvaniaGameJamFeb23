using UnityEngine;
using UnityEngine.SceneManagement;

using DevLocker.Utils;

public class MainMenu : MonoBehaviour
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

    void Start()
    {
        GlobalEvent.Invoke.OnGameInit();
    }
}
