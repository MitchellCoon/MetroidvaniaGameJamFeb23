using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTelegraph : MonoBehaviour
{
    [SerializeField] float duration;
    float timeElapsed = 0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
        }
    }
}
