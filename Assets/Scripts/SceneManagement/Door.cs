using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using DevLocker.Utils;
using System.Collections;

namespace DTDEV.SceneManagement
{

    /// <summary>
    /// DESCRIPTION
    /// A Door serves as a connection between two Rooms, and also includes the logic of
    /// how to transition from room to room.
    /// See <see cref="DTDEV.SceneManagement.Room"/> for more details of how Rooms work.
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
        enum TransitionType
        {
            Fade,
            Slide,
        }

        enum DoorChannel
        {
            A, B, C, D, E, F, G,
        }

        [SerializeField] DoorChannel doorChannel;
        [SerializeField] SceneReference targetSceneRef;
        [SerializeField] Transform spawnPoint;

        [Space]
        [Space]

        [SerializeField] TransitionType transitionType;

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
            StartCoroutine(PlayFadeTransition());
            isTriggered = true;
        }

        IEnumerator PlayFadeTransition()
        {
            // move to top of hierarchy
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            TransitionFader fader = FindObjectOfType<TransitionFader>();
            System.Action<float> OnFadeTick;
            OnFadeTick = (float t) => Time.timeScale = Easing.InQuad(0.9f * (1 - t) + 0.1f);
            if (fader != null) yield return fader.FadeOut(OnFadeTick);
            Time.timeScale = 0.1f;
            yield return SceneManager.LoadSceneAsync(targetSceneName);
            Door otherDoor = GetOtherDoor();
            MovePlayerToSpawnPoint(otherDoor);
            SetIncomingRoomSpawnPoint(otherDoor);
            OnFadeTick = (float t) => Time.timeScale = Easing.InOutQuad(0.9f * t + 0.1f);
            if (fader != null) yield return fader.FadeIn(OnFadeTick);
            Time.timeScale = 1f;
            Destroy(gameObject);
        }

        IEnumerator PlaySlideTransition()
        {
            // at this point, the camera should be clamped against a boundary
            // Time.timeScale = 0
            // turn off boundary
            // load in new scene additively, syncronously
            // WONT_DO: (JUST MOVE AUDIO LISTENER TO PERSISTENT OBJ) turn off incoming Room systems (camera, audiolistener)
            // move player to incoming Door spawnPoint position
            // lerp camera to the incoming camera position
            // once finished, unload outgoing scene

            yield return null;
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

        void SetIncomingRoomSpawnPoint(Door otherDoor)
        {
            if (otherDoor == null) return;
            if (otherDoor.spawnPoint == null) return;
            Room room = Room.FindActive();
            if (room == null) return;
            room.SetRespawnPoint(otherDoor.spawnPoint);
        }
    }
}
