using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempObject : MonoBehaviour
{
    // Start is called before the first frame update
   //public float disableTimer = 1f ;
   public float fadeOutTime = 0.5f ;

    public bool fadeOnCollide = true; 
    public bool fadeStarted; 
    public float fadeDelay = 2.5f;
    void Start()
    {   if(!fadeOnCollide){
        StartCoroutine(fadeOut());
    }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (fadeOnCollide)
        {
            StartCoroutine(fadeOut());
        }
      
    }
    IEnumerator fadeOut(){
          if (fadeStarted)
        {
            yield break;
        }
        fadeStarted = true;
        yield return new WaitForSeconds(fadeDelay);
        var localRender = this.gameObject.GetComponent<SpriteRenderer>();   
        for (float f = fadeOutTime; f >= 0; f -= 0.1f)
        {
            Color c = localRender.material.color;
            c.a = f;
            localRender.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
   
    }

}
