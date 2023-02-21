using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using MapGen;

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
        [SerializeField][Range(0f, 5f)] float timeRespawnDelay;
        [Space]
        [Space]
        [Tooltip("Do not edit this directly - it gets automatically assigned by MapGenerator")]
        [SerializeField] MapRoomData mapRoomData;

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
        Coroutine spawning;
        Coroutine respawning;

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

        public void SetMapRoomData(MapRoomData incoming)
        {
            mapRoomData = incoming;
        }

        public void SetRespawnPoint(Transform respawnPoint)
        {
            currentRespawnPoint = respawnPoint;
        }

        void OnEnable()
        {
            GlobalEvent.OnPlayerDeath += OnPlayerDeath;
        }

        void OnDisable()
        {
            GlobalEvent.OnPlayerDeath -= OnPlayerDeath;
        }

        void Awake()
        {
            _guidComponent = GetComponent<GuidComponent>();
        }

        void Start()
        {
            SetPlayerSpawnPoint();
            spawning = StartCoroutine(OnLevelStart());
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
            if (mapRoomData != null) mapRoomData.FlagRoomVisited();
            GlobalEvent.Invoke.OnRoomLoaded(transform.position);
            yield return null;
        }

        IEnumerator SpawnPlayer()
        {
            // we can add SFX and VFX here as needed, hence the Coroutine
            Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
            yield return null;
            spawning = null;
        }

        IEnumerator RespawnPlayer()
        {
            yield return new WaitForSeconds(timeRespawnDelay);
            yield return SpawnPlayer();
            respawning = null;
        }

        void OnPlayerDeath()
        {
            if (spawning != null) StopCoroutine(spawning);
            if (respawning != null) StopCoroutine(respawning);
            respawning = StartCoroutine(RespawnPlayer());
        }
    }
}
