using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Cinemachine;

using DevLocker.Utils;
using System.Collections;
using CyberneticStudios.SOFramework;

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
        const string UNTAGGED = "Untagged";
        struct Translation
        {
            public Vector2 from;
            public Vector2 to;
        }

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
        [SerializeField] FloatVariable slideTransitionDuration;

        Room outgoingRoom;

        string targetSceneName;
        string outgoingSceneName;
        int outgoingSceneIndex;
        bool isTriggered = false;

        void Awake()
        {
            targetSceneName = targetSceneRef.IsEmpty ? "" : targetSceneRef.SceneName;
            slideTransitionDuration.ResetVariable();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (isTriggered) return;
            if (!other.CompareTag("Player")) return;
            if (transitionType == TransitionType.Fade)
            {
                StartCoroutine(PlayFadeTransition());
            }
            else if (transitionType == TransitionType.Slide)
            {
                StartCoroutine(PlaySlideTransition());
            }
            isTriggered = true;
        }

        void Validate()
        {
            Assert.IsNotNull(spawnPoint);
            Assert.IsFalse(targetSceneRef.IsEmpty, $"sceneRef missing on Door \"${gameObject.name}\" in scene ${SceneManager.GetActiveScene().name}");
            Assert.IsTrue(outgoingSceneIndex > -1, $"Scene {SceneManager.GetActiveScene().name} needs to be added to Builds - found buildIndex {outgoingSceneIndex}");
        }

        void PrepareRoomTransition()
        {
            outgoingRoom = FindObjectOfType<Room>();
            outgoingSceneName = SceneManager.GetActiveScene().name;
            outgoingSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Validate();
        }

        IEnumerator PlayFadeTransition()
        {
            PrepareRoomTransition();
            // move to top of hierarchy so DontDestroyOnLoad works
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            TransitionFader fader = FindObjectOfType<TransitionFader>();
            System.Action<float> OnFadeTick;
            OnFadeTick = (float t) => Time.timeScale = Easing.InQuad(0.9f * (1 - t) + 0.1f);
            if (fader != null) yield return fader.FadeOut(OnFadeTick);
            Time.timeScale = 0.1f;
            yield return SceneManager.LoadSceneAsync(targetSceneName);
            Door otherDoor = GetOtherDoor();
            GameObject player = GameObject.FindWithTag("Player");
            MovePlayerToSpawnPoint(player, otherDoor);
            SetIncomingRoomSpawnPoint(otherDoor);
            OnFadeTick = (float t) => Time.timeScale = Easing.InOutQuad(0.9f * t + 0.1f);
            if (fader != null) yield return fader.FadeIn(OnFadeTick);
            Time.timeScale = 1f;
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

        void MovePlayerToSpawnPoint(GameObject player, Door otherDoor)
        {
            if (otherDoor == null) return;
            if (otherDoor.spawnPoint == null) return;
            if (player == null) return;
            player.transform.position = otherDoor.spawnPoint.position;
            player.transform.rotation = otherDoor.spawnPoint.rotation;
        }

        void SetIncomingRoomSpawnPoint(Door otherDoor)
        {
            if (otherDoor == null) return;
            if (otherDoor.spawnPoint == null) return;
            Room room = FindObjectOfType<Room>();
            if (room == null) return;
            room.SetRespawnPoint(otherDoor.spawnPoint);
        }

        // NOTES
        // --------------------------------------------------------------------
        // This method got very unwieldy, but it works. Highly recommend not touching it
        // unless you want things to break terribly 😅
        // In a nutshell, here's what the method is doing:
        // - Pausing time during the transition
        // - Disabling all camera bounds during transition so that outgoing camera can move into place
        // - Destroying GameObjects / components that would be redundant or cause problems if more than one existed
        // - Loading the new scene additively
        // - Prevents the incoming Room from spawning a new player
        // - Positions the incoming camera to where the player will ultimately end up taking camera bounds into account
        // - Animates the player and the camera to their new respective positions in a sliding effect
        // - Performs cleanup
        IEnumerator PlaySlideTransition()
        {
            PrepareRoomTransition();
            CinemachineVirtualCamera vCam = FindObjectOfType<CinemachineVirtualCamera>();
            GameObject[] cameraBounds = GameObject.FindGameObjectsWithTag("CameraBounds");
            // move to top of hierarchy so DontDestroyOnLoad works
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Time.timeScale = 0;
            AudioListener.pause = true;
            // Turn off the virtual camera as we will be controlling the camera manually
            // Also make current camera takes precedence over incoming camera
            Destroy(vCam.gameObject);
            Camera outgoingCamera = Camera.main;
            outgoingCamera.tag = UNTAGGED;
            outgoingCamera.gameObject.name = "OutgoingCamera";
            outgoingCamera.depth = 100;
            if (outgoingCamera.transform.parent) outgoingCamera.transform.parent.gameObject.name = "OutgoingCamera";
            // disable all camera bounds so that a smooth transition can occur
            foreach (var bound in cameraBounds) if (bound != null) bound.SetActive(false);
            // Destroy current scene objects
            Destroy(outgoingRoom.gameObject);
            Destroy(outgoingCamera.GetComponent<AudioListener>());
            Destroy(outgoingCamera.GetComponent<CinemachineBrain>());
            // Set outgoing variables
            Scene outgoingScene = SceneManager.GetActiveScene();
            GameObject[] outgoingSceneRootObjects = outgoingScene.GetRootGameObjects();
            PlayerMain player = GameObject.FindWithTag("Player").GetComponent<PlayerMain>();
            player.SetKinematic();
            // Load incoming scene
            yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            DisableIncomingRoomSpawn();
            Scene incomingScene = SceneManager.GetSceneByName(targetSceneName);
            SceneManager.SetActiveScene(incomingScene);
            yield return null;
            // set incoming variables
            Door otherDoor = GetOtherDoor();
            Camera incomingCamera = Camera.main;
            // Set incomingCamera position to the player position, but clamped inside its camerBounds.
            // Run physics for a single frame in order for CinemachineConfiner2D to do its job
            MoveOutgoingGameObjectsToScene(outgoingSceneRootObjects, incomingScene);
            player.SetCameraTargetAsPlayer();
            Time.timeScale = 1;
            yield return new WaitForEndOfFrame();
            Time.timeScale = 0;
            // Animate player and camera to new position
            Translation playerTransition = new Translation
            {
                from = player.transform.position,
                to = otherDoor.spawnPoint.transform.position,
            };
            Translation cameraTransition = new Translation
            {
                from = outgoingCamera.transform.position,
                to = incomingCamera.transform.position,
            };
            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime / slideTransitionDuration.value;
                SetPlayerPositionPreserveY(player, Vector3.Lerp(playerTransition.from, playerTransition.to, t));
                SetCameraPositionPreserveZ(outgoingCamera, Vector3.Lerp(cameraTransition.from, cameraTransition.to, t));
                yield return null;
            }
            SetPlayerPositionPreserveY(player, playerTransition.to);
            SetCameraPositionPreserveZ(incomingCamera, cameraTransition.to);
            // unload outgoing scene
            foreach (var bound in cameraBounds) if (bound != null) bound.SetActive(true);
            DestroyOutgoingGameObjects(outgoingSceneRootObjects);
            yield return SceneManager.UnloadSceneAsync(outgoingScene);
            AudioListener.pause = false;
            Time.timeScale = 1;
            player.SetDynamic();
            Destroy(gameObject);
        }

        void DisableIncomingRoomSpawn()
        {
            Room room = FindObjectOfType<Room>();
            room.DisableInitialSpawn();
        }

        void SetCameraPositionPreserveZ(Camera camera, Door door)
        {
            if (camera == null) return;
            if (door == null) return;
            if (door.spawnPoint == null) return;
            SetCameraPositionPreserveZ(camera, door.spawnPoint.transform.position);
        }

        void SetCameraPositionPreserveZ(Camera camera, Vector3 position)
        {
            if (camera == null) return;
            Vector3 cameraPosition = position;
            cameraPosition.z = camera.transform.position.z;
            camera.transform.position = cameraPosition;
        }

        void SetPlayerPositionPreserveY(PlayerMain player, Vector3 position)
        {
            if (player == null) return;
            Vector3 playerPosition = position;
            playerPosition.y = player.transform.position.y;
            player.transform.position = playerPosition;
        }

        void MoveOutgoingGameObjectsToScene(GameObject[] outgoingSceneRootObjects, Scene scene)
        {
            for (int i = 0; i < outgoingSceneRootObjects.Length; i++)
            {
                if (outgoingSceneRootObjects[i] == null) continue;
                if (!outgoingSceneRootObjects[i].activeSelf) continue;
                SceneManager.MoveGameObjectToScene(outgoingSceneRootObjects[i], scene);
            }
        }

        void DestroyOutgoingGameObjects(GameObject[] outgoingSceneRootObjects)
        {
            for (int i = 0; i < outgoingSceneRootObjects.Length; i++)
            {
                if (outgoingSceneRootObjects[i] == null) continue;
                if (outgoingSceneRootObjects[i].CompareTag("Player")) continue;
                if (!outgoingSceneRootObjects[i].activeSelf) continue;
                Destroy(outgoingSceneRootObjects[i]);
            }
        }
    }
}
