using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using DevLocker.Utils;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class WinPortal : MonoBehaviour
{
    [Tooltip("This is the scene to load for the Win Screen")]
    [SerializeField] SceneReference targetSceneRef;

    bool isTriggered = false;

    void Awake()
    {
        Assert.IsFalse(targetSceneRef.IsEmpty, $"targetSceneRef missing on WinPortal");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        isTriggered = true;
        GlobalEvent.Invoke.OnWinGame();
        StartCoroutine(CLoadWinScreen());
    }

    IEnumerator CLoadWinScreen()
    {
        yield return SceneManager.LoadSceneAsync(targetSceneRef.SceneName);
    }
}
