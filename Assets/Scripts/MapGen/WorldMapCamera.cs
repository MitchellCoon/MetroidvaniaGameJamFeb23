using System;
using UnityEngine;

namespace MapGen
{

    [RequireComponent(typeof(Camera))]
    public class WorldMapCamera : MonoBehaviour
    {

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
        }

        void Update()
        {
            // TODO: REMOVE THIS IN FAVOR OF PLAYER CONTROLLER + GLOBAL EVENT
            if (Input.GetButtonDown("Submit"))
            {
                camera.enabled = !camera.enabled;
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
    }
}

