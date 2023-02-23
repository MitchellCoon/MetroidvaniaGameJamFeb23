using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

// NOTE - this component goes on the __enemy__, not the player.
public class PossessionManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform unpossessionSpawnPoint;
    [SerializeField] BoolVariable isPlayerPossessing;

    GameObject possessionTarget;
    bool isPossessed = false;

    void OnEnable()
    {
        GlobalEvent.OnPlayerSpawn += OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath += OnPlayerDeath;
    }

    void OnDisable()
    {
        GlobalEvent.OnPlayerSpawn -= OnPlayerSpawn;
        GlobalEvent.OnPlayerDeath -= OnPlayerDeath;
    }

    void Awake()
    {
        Assert.IsNotNull(isPlayerPossessing, "Please assign ref to `isPlayerPossessing` in `PossessionManager`");
    }

    void Update()
    {
        if (isPossessed && Input.GetKeyDown(KeyCode.F))
        {
            RevertPossession();
        }
    }

    void OnPlayerSpawn()
    {
        isPlayerPossessing.value = false;
        if (isPossessed) FailBadlyAndNoticeably("An enemy was already possessed when a new player spawned - either the player got incorrectly spawned in or an enemy was not un-possessed correctly, or some other bug.");
    }

    void OnPlayerDeath()
    {
        isPlayerPossessing.value = false;
    }

    public void GetPossessed(GameObject player)
    {
        GetComponent<Enemy>().enabled = false;
        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<AIMovement>().enabled = false;
        GetComponent<PlayerMain>().enabled = true;
        GetComponent<PlayerMovementController>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;
        GetComponent<InputManager>().enabled = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<GroundCheck>().enabled = true;
        GetComponent<Move>().enabled = true;
        GetComponent<Jump>().enabled = true;
        GetComponent<Attack>().enabled = true;
        Destroy(player);
        isPossessed = true;
        isPlayerPossessing.value = true;
        gameObject.tag = "Player";
    }

    public void RevertPossession()
    {
        // enable AI scripts, disable player control scripts on enemy
        GetComponent<Enemy>().enabled = true;
        GetComponent<EnemyAttack>().enabled = true;
        GetComponent<AIMovement>().enabled = true;
        GetComponent<PlayerMain>().enabled = false;
        GetComponent<PlayerMovementController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<InputManager>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<GroundCheck>().enabled = false;
        GetComponent<Move>().enabled = false;
        GetComponent<Jump>().enabled = false;
        GetComponent<Attack>().enabled = false;
        StartCoroutine(SpawnPlayerCoroutine());
        isPossessed = false;
        isPlayerPossessing.value = false;
        gameObject.tag = "Enemy";
    }

    // This method will be used to update the prefab created when respawning the player

    public void UpdatePlayerPrefab(GameObject newPlayerPrefab)
    {
        playerPrefab = newPlayerPrefab;
    }

    IEnumerator SpawnPlayerCoroutine()
    {
        yield return SpawnPlayer();
    }

    IEnumerator SpawnPlayer()
    {
        Instantiate(playerPrefab, unpossessionSpawnPoint.position, Quaternion.identity);
        yield return null;
    }

    void FailBadlyAndNoticeably(string reason)
    {
        Destroy(gameObject);
        throw new UnityException(reason);
    }
}
