using UnityEngine;

using DevLocker.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class Credits : MonoBehaviour
{
    [SerializeField] bool debugLayout;
    [SerializeField] float scrollSpeed = 1f;
    [Space]
    [Space]
    [SerializeField] SceneReference mainMenuRef;
    [SerializeField] GameObject showItemsOnStopped;

    public float ScrollSpeed => scrollSpeed;
    public bool DebugLayout => debugLayout;

    public void StopScrolling()
    {
        scrollSpeed = 0;
        if (showItemsOnStopped != null) showItemsOnStopped.SetActive(true);
    }

    void Awake()
    {
        Assert.IsNotNull(mainMenuRef);
    }

    void Start()
    {
        if (showItemsOnStopped != null) showItemsOnStopped.SetActive(false);
        GlobalEvent.Invoke.OnWinGame();
    }

    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * scrollSpeed);
        if (IsScrollingStopped() && MInput.GetAnyKeyDown())
        {
            SceneManager.LoadScene(mainMenuRef.SceneName);
        }
    }

    void OnValidate()
    {
        if (scrollSpeed <= 0) scrollSpeed = 0.1f;
    }

    bool IsScrollingStopped()
    {
        return scrollSpeed == 0;
    }
}
