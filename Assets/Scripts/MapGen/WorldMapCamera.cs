using UnityEngine;

namespace MapGen
{

    [RequireComponent(typeof(Camera))]
    public class WorldMapCamera : MonoBehaviour
    {

        new Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        void Update()
        {
            // TODO: REMOVE THIS IN FAVOR OF PLAYER CONTROLLER + GLOBAL EVENT
            if (Input.GetButtonDown("Submit"))
            {
                camera.depth *= -1;
            }
        }
    }
}

