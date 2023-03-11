using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class TriggerVolume : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] SpriteRenderer debugSprite;
    [SerializeField] Color debugColorActive = Color.red.toAlpha(0.6f);
    [SerializeField] Color debugColorInactive = Color.yellow.toAlpha(0.6f);
    [Space]
    [Space]
    [SerializeField] BoolVariable boolVariable;
    [SerializeField] bool setToFalse = false; 
    [SerializeField] bool canDeactivate = false;

    BoxCollider2D box;

    public bool HasBoolRef => boolVariable != null;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(boolVariable);
    }

    void Start()
    {
        if (debugSprite != null) debugSprite.color = debugColorInactive;
        if (debugSprite != null && !debug) debugSprite.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        Activate();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        if (canDeactivate) Deactivate();
    }

    void Activate()
    {
        boolVariable.value = !setToFalse;
        if (debugSprite != null) debugSprite.color = debugColorActive;
    }

    void Deactivate()
    {
        boolVariable.value = setToFalse;
        if (debugSprite != null) debugSprite.color = debugColorInactive;
    }

    public void ResizeSpriteToColliderBounds()
    {
        if (box == null) box = GetComponent<BoxCollider2D>();
        if (debugSprite == null) debugSprite = GetComponent<SpriteRenderer>();
        if (debugSprite == null) { Debug.LogWarning("No sprite found to resize"); return; }
        debugSprite.transform.position = box.bounds.center;
        debugSprite.transform.localScale = new Vector3(box.bounds.size.x, box.bounds.size.y, 1);
    }
}
