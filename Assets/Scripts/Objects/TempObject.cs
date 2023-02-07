using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempObject : MonoBehaviour
{
    // Start is called before the first frame update
   public float disableTimer = 1f ;
   public float delay = 1f ;


    void Start()
    {

        StartCoroutine(fadeOut());
    }
    IEnumerator fadeOut(){
        var localRender = this.gameObject.GetComponent<SpriteRenderer>();   
        for (float f = delay; f >= 0; f -= 0.1f)
        {
            Color c = localRender.material.color;
            c.a = f;
            localRender.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
   
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
