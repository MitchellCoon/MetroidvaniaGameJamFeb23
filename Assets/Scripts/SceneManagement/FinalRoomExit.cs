using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using DevLocker.Utils;
using DTDEV.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class FinalRoomExit : MonoBehaviour
{
    [SerializeField] SceneReference winSceneRef;
    [SerializeField] Sound winGameSound;
    [SerializeField] float delayBeforeWinScreen = 1f;

    Coroutine winning;

    void Start()
    {
        GlobalEvent.Invoke.OnStopMusic();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        if (winning != null) return;
        winning = StartCoroutine(CWinGame());
    }

    void GotoWinScreen()
    {
        SceneManager.LoadScene(winSceneRef.SceneName);
    }

    IEnumerator CWinGame()
    {
        yield return null;
        TransitionFader fader = FindObjectOfType<TransitionFader>();
        Time.timeScale = 0f;
        if (winGameSound != null) winGameSound.Play();
        if (fader != null) yield return fader.FadeOut();
        if (winGameSound != null) while (winGameSound.isPlaying) yield return null;
        yield return new WaitForSecondsRealtime(delayBeforeWinScreen);
        Time.timeScale = 1f;
        GotoWinScreen();
    }
}
