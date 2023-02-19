using UnityEngine;

namespace MapGen
{

    public class WorldMapRoom : MonoBehaviour
    {
        [SerializeField] string roomGuid;

        public string RoomGuid => roomGuid;

        public void SetRoomGuid(string incoming)
        {
            roomGuid = incoming;
        }
    }
}
