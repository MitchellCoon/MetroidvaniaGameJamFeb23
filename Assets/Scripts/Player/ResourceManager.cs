using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    
    public List<Resource> resources;
    
    void Start()
    {
        foreach(Resource resource in resources)
        {
            resource.Init();
        }
    }

}
