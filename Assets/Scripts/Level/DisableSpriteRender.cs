using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSpriteRender : MonoBehaviour
{
    public bool disableInEditor = false;
    public bool doNotDisableSpriteRenderer = false;
    void Start()
    {
        if (doNotDisableSpriteRenderer) return;
        if (disableInEditor)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            if (Application.isEditor)
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

}
