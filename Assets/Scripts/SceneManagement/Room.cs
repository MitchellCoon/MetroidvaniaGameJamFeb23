using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace DTDEV.SceneManagement
{

    /// <summary>
    /// A Room functions as a separate space in the overall game world. Each Room
    /// is a separate scene that can either be loaded additively (via a Slide transition)
    /// or immediately (via a Fade transition).
    /// Each Room initially tries to load one or more common Scenes in order to load
    /// in common systems, like a Camera, etc. It also spawns in the player.
    /// </summary>

    [RequireComponent(typeof(GuidComponent))]
    public class Room : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;

        GuidComponent _guidComponent;
        GuidComponent guidComponent
        {
            get
            {
                if (_guidComponent == null) _guidComponent = GetComponent<GuidComponent>();
                return _guidComponent;
            }
        }

        GameObject playerSpawnPointObj;
        Transform playerSpawnPoint;
        Transform currentRespawnPoint;

        bool initialSpawnEnabled = true;

        public string guid => guidComponent.GetUniqueIdentifier();

        public void Validate()
        {
            Assert.IsNotNull(guidComponent, $"Please add a GuidComponent to Room \"{gameObject.name}\" in scene {SceneManager.GetActiveScene().name}");
        }

        public void DisableInitialSpawn()
        {
            initialSpawnEnabled = false;
        }

        public void SetRespawnPoint(Transform respawnPoint)
        {
            currentRespawnPoint = respawnPoint;
        }

        void Awake()
        {
            _guidComponent = GetComponent<GuidComponent>();
        }

        void Start()
        {
            SetPlayerSpawnPoint();
            StartCoroutine(OnLevelStart());
            Assert.IsNotNull(guidComponent, $"Please add a GuidComponent to Room \"{gameObject.name}\" in scene {SceneManager.GetActiveScene().name}");
        }

        void SetPlayerSpawnPoint()
        {
            playerSpawnPointObj = GameObject.FindWithTag("PlayerSpawnPoint");
            Assert.IsNotNull(playerSpawnPointObj, "Unable to find PlayerSpawnPoint in current scene");
            playerSpawnPoint = playerSpawnPointObj.transform;
        }

        IEnumerator OnLevelStart()
        {
            if (initialSpawnEnabled)
            {
                if (currentRespawnPoint == null) SetRespawnPoint(playerSpawnPoint);
                yield return SpawnPlayer();
            }
            yield return null;
        }

        IEnumerator SpawnPlayer()
        {
            // we can add SFX and VFX here as needed, hence the Coroutine
            Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
            yield return null;
        }

        // TODO: handle event OnPlayerDeath -> respawn player
    }
}
