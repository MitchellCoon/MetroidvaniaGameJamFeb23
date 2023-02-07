using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public float spawnRate = 1f ; 
    public GameObject prefab;
    void Start()
    {   
        InvokeRepeating("SpawnObject", 0f, spawnRate);
    }

    void SpawnObject(){
        Instantiate(prefab, this.transform.position, this.transform.rotation);
    }

}
