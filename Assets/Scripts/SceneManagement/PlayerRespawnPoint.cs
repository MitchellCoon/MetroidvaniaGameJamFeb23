using UnityEngine;

namespace DTDEV.SceneManagement
{

    public class PlayerRespawnPoint : MonoBehaviour
    {
        Room room;

        void Awake()
        {
            room = GetComponentInParent<Room>();
            if (room == null)
            {
                Debug.LogWarning("PlayerRespawnPoint could not find Room parent");
                gameObject.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            room.SetRespawnPoint(gameObject.transform);
        }
    }
}
