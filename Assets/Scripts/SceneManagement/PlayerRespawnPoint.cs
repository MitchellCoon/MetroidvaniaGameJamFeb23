using UnityEngine;

namespace DTDEV.SceneManagement
{

    public class PlayerRespawnPoint : MonoBehaviour
    {
        Room room;

        void Awake()
        {
            room = FindObjectOfType<Room>();
            if (room == null)
            {
                Debug.LogWarning("PlayerRespawnPoint could not find Room component in Scene");
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
