using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using DevLocker.Utils;
using System.Collections;

namespace DTDEV.SceneManagement
{

    /// <summary>
    /// SETUP
    /// - add door script to a GameObject
    /// - add a colider2d (trigger)
    /// - add a rigidbody2d (kinematic)
    /// - add a child GameObject - assign as the spawnPoint
    /// USAGE
    /// - doorChannel determines which door in a scene maps to a door in another scene
    /// - targetSceneRef utilizes a slick SceneReference library so we don't have to reference scenes by name
    /// - spawnPoint is where the player will appear after they enter a door collider and the next scene loads
    /// </summary>
    public class Door : MonoBehaviour
    {
        enum DoorChannel
        {
            A, B, C, D, E, F, G,
        }

        [SerializeField] DoorChannel doorChannel;
        [SerializeField] SceneReference targetSceneRef;
        [SerializeField] Transform spawnPoint;

        string targetSceneName;
        string outgoingSceneName;
        int outgoingSceneIndex;
        bool isTriggered = false;

        void Awake()
        {
            outgoingSceneName = SceneManager.GetActiveScene().name;
            outgoingSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Validate();
            targetSceneName = targetSceneRef.IsEmpty ? "" : targetSceneRef.SceneName;
        }

        void Validate()
        {
            Assert.IsNotNull(spawnPoint);
            Assert.IsNotNull(targetSceneRef);
            Assert.IsFalse(targetSceneRef.IsEmpty, $"sceneRef missing on Door \"${gameObject.name}\" in scene ${SceneManager.GetActiveScene().name}");
            Assert.IsTrue(outgoingSceneIndex > 0, $"Scene ${SceneManager.GetActiveScene().name} needs to be added to Builds");
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (isTriggered) return;
            if (!other.CompareTag("Player")) return;
            Validate();
            StartCoroutine(TransitionToNewScene());
            isTriggered = true;
        }

        IEnumerator TransitionToNewScene()
        {
            DontDestroyOnLoad(gameObject);
            // TODO: ADD ROOM SLIDE TRANSITION
            yield return SceneManager.LoadSceneAsync(targetSceneName);
            Door otherDoor = GetOtherDoor();
            MovePlayerToSpawnPoint(otherDoor);
            Destroy(gameObject);
        }

        Door GetOtherDoor()
        {
            Door[] doors = FindObjectsOfType<Door>();
            foreach (var door in doors)
            {
                if (door == this) continue;
                if (door.doorChannel != doorChannel) continue;
                if (door.targetSceneName != outgoingSceneName) continue;
                return door;
            }
            return null;
        }

        void MovePlayerToSpawnPoint(Door otherDoor)
        {
            if (otherDoor == null) return;
            if (otherDoor.spawnPoint == null) return;
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            player.transform.position = otherDoor.spawnPoint.position;
            player.transform.rotation = otherDoor.spawnPoint.rotation;
        }
    }
}
