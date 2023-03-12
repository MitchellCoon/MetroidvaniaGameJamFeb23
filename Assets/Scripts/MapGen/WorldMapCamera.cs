using System;
using UnityEngine;

namespace MapGen
{

    [RequireComponent(typeof(Camera))]
    public class WorldMapCamera : MonoBehaviour
    {
        [SerializeField] Canvas renderOutputCanvas;

        new Camera camera;

        void OnEnable()
        {
            GlobalEvent.OnRoomLoaded += OnRoomLoaded;
        }

        void OnDisable()
        {
            GlobalEvent.OnRoomLoaded -= OnRoomLoaded;
        }

        void Awake()
        {
            camera = GetComponent<Camera>();
            camera.enabled = false;
            SetMapActive(false);
        }

        void Update()
        {
            if (Time.timeScale > 0 && MInput.GetKeyDown(KeyCode.Tab) || MInput.GetPadDown(GamepadCode.Select))
            {
                SetMapActive(!camera.enabled);
            }
        }

        void SetCameraPosition(Vector3 incoming)
        {
            // preserve z value
            incoming.z = camera.transform.position.z;
            camera.transform.position = incoming;
        }

        void OnRoomLoaded(Vector2 roomPosition)
        {
            SetCameraPosition(roomPosition * 0.5f);
        }

        void SetMapActive(bool isActive)
        {
            camera.enabled = isActive;
            if (renderOutputCanvas != null)
            {
                renderOutputCanvas.enabled = isActive;
                renderOutputCanvas.gameObject.SetActive(isActive);
            }
        }
    }
}

