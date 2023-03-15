using UnityEngine;
using UnityEngine.Serialization;

public class DisableSpriteRender : MonoBehaviour
{
    [FormerlySerializedAs("disableInEditor")][SerializeField] bool disableInProdBuild = false;
    [SerializeField] bool debugInPlayMode = false;

    void Start()
    {
        if (Application.isEditor && debugInPlayMode)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (disableInProdBuild)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
