using System.Collections.Generic;
using UnityEngine;

namespace MapGen
{

    // Note - WorldMap is generated dynamically by MapGen
    public class WorldMap : MonoBehaviour
    {
        [SerializeField] List<MapRoomData> mapRoomDataList = new List<MapRoomData>();

        WorldMapRoom[] mapRooms = new WorldMapRoom[0];
        Dictionary<string, bool> roomVisitedMap = new Dictionary<string, bool>();

        public void OnRoomVisited(string roomGuid)
        {
            roomVisitedMap[roomGuid] = true;
            RenderVisibleRooms();
        }

        public void SetMapRoomDataList(List<MapRoomData> incoming)
        {
            mapRoomDataList.Clear();
            foreach (var item in incoming) mapRoomDataList.Add(item);
        }

        void Awake()
        {
            mapRooms = GetComponentsInChildren<WorldMapRoom>();
            foreach (var item in mapRoomDataList)
            {
                roomVisitedMap[item.RoomGuid] = item.IsVisited;
            }
        }

        void Start()
        {
            RenderVisibleRooms();
        }

        bool IsRoomVisited(string roomGuid)
        {
            if (roomVisitedMap.ContainsKey(roomGuid) && roomVisitedMap[roomGuid]) return true;
            return false;
        }

        void RenderVisibleRooms()
        {
            for (int i = 0; i < mapRooms.Length; i++)
            {
                if (IsRoomVisited(mapRooms[i].RoomGuid))
                {
                    mapRooms[i].gameObject.SetActive(true);
                }
                else
                {
                    mapRooms[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
