using UnityEngine;

namespace MapGen
{

    public class WorldMapPlayerMarker : MonoBehaviour
    {
        void LateUpdate()
        {
            if (GlobalMapState.isPlayerActive) transform.position = GlobalMapState.playerPosition * 0.5f;
        }
    }
}
