using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElRaccoone.Tweens;
// https://github.com/jeffreylanters/unity-tweens
public class FloorMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeToTween = 5;
    [Tooltip("The object to move to")]
    public Transform targetObject;

    private Vector3 startPosition; 

    void OnEnable() {
      this.startPosition = transform.position;
      if (targetObject != null) this.gameObject.TweenPosition (targetObject.position, timeToTween).SetFrom (transform.position).SetPingPong ().SetInfinite();//.SetLoopCount(-1);//.SetOnComplete (SomeMethod);
    }

    void OnDisable() {
      transform.position = this.startPosition; 
    }
}
