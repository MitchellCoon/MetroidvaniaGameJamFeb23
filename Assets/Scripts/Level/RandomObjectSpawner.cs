using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs;

    public void Spawn()
    {
        int index = UnityEngine.Random.Range(0, prefabs.Count);
        Instantiate(prefabs[index], transform.position, Quaternion.identity);
    }
}
