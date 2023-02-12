using UnityEngine;

namespace DTDev.Core
{

    /// <summary>
    /// DESCRIPTION
    /// The Persistent Object Spawner pattern is a good alternative to the Singleton
    /// pattern to ensure that only ONE of something is ever instantiated and persists
    /// across multiple scenes.
    /// SETUP
    /// - create a new GameObject -> save as a prefab "PersistentObjects"
    /// - in the prefab, place child GameObjects with high-level systems you want to
    ///   persist from scene to scene, like AudioManager, AudioListener, SaveSystem, etc.
    /// - add a PersistentObjectSpawner to each scene, referencing the "PersistentObjects" prefab
    /// </summary>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjects;

        static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;
            hasSpawned = true;
            SpawnPersistentObjects();
        }

        void SpawnPersistentObjects()
        {
            GameObject spawned = Instantiate(persistentObjects);
            DontDestroyOnLoad(spawned);
        }
    }
}
