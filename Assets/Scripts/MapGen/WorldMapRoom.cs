using UnityEngine;

namespace MapGen
{

    public class WorldMapRoom : MonoBehaviour
    {
        [SerializeField] string roomGuid;

        public string RoomGuid => roomGuid;
    }
}
