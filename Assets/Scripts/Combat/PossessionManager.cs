using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

// NOTE - this component goes on the __enemy__, not the player.
public class PossessionManager : MonoBehaviour
{
    const string LAYER_WHILE_POSSESSED = Constants.PLAYER_LAYER;
    const string PREFIX_WHILE_POSSESSED = "POSSESSED_";
    const string ERR_MSG_COMPONENT_MISSING = "component missing";
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform unpossessionSpawnPoint;
    [SerializeField] BoolVariable isPlayerPossessing;

    Rigidbody2D body;

    // enemy components
    Enemy enemy;
    EnemyAttack enemyAttack;
    AIMovement aIMovement;
    BaseEnemyAI enemyAI;

    // player components - enabled while the enemy is being possessed
    PlayerMain playerMain;
    PlayerMovementController playerMovementController;
    PlayerCombat playerCombat;
    InputManager inputManager;
    Animator animator;
    GroundCheck groundCheck;
    Move move;
    Jump jump;
    Attack attack;

    GameObject possessionTarget;
    bool isPossessed = false;
    int initialLayer;
    string initialName;

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
        body = GetComponent<Rigidbody2D>();
        // enemy components
        enemy = GetComponent<Enemy>();
        enemyAttack = GetComponent<EnemyAttack>();
        aIMovement = GetComponent<AIMovement>();
        enemyAI = GetComponent<BaseEnemyAI>();
        // player components
        playerMain = GetComponent<PlayerMain>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerCombat = GetComponent<PlayerCombat>();
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();
        move = GetComponent<Move>();
        jump = GetComponent<Jump>();
        attack = GetComponent<Attack>();
        // validations
        Assert.IsNotNull(body, ERR_MSG_COMPONENT_MISSING);
        // Assert.IsNotNull(enemy, ERR_MSG_COMPONENT_MISSING);
        // Assert.IsNotNull(enemyAttack, ERR_MSG_COMPONENT_MISSING);
        // Assert.IsNotNull(aIMovement, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(enemyAI, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(playerMain, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(playerMovementController, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(playerCombat, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(inputManager, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(animator, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(groundCheck, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(move, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(jump, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(attack, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(isPlayerPossessing, "Please assign ref to `isPlayerPossessing` in `PossessionManager`");
        // initial state
        initialLayer = gameObject.layer;
        initialName = gameObject.name;
    }

    void Update()
    {
        bool isUnpossessButtonPressed = MInput.GetKeyDown(KeyCode.F) || MInput.GetPadDown(GamepadCode.ButtonNorth);
        if (isPossessed && isUnpossessButtonPressed)
        {
            Debug.Log(isUnpossessButtonPressed);
            RevertPossession();
        }
    }

    void OnPlayerSpawn(PlayerMain player)
    {
        isPlayerPossessing.value = false;
        if (isPossessed) FailBadlyAndNoticeably("An enemy was already possessed when a new player spawned - either the player got incorrectly spawned in or an enemy was not un-possessed correctly, or some other bug.");
    }

    void OnPlayerDeath()
    {
        isPlayerPossessing.value = false;
    }

    public void GetPossessed(GameObject playerObj)
    {
        SetEnemyComponentsEnabled(false);
        SetPlayerComponentsEnabled(true);
        Destroy(playerObj);
        isPossessed = true;
        isPlayerPossessing.value = true;
        gameObject.name = PREFIX_WHILE_POSSESSED + initialName;
        gameObject.tag = Constants.PLAYER_TAG;
        gameObject.layer = Layer.Parse(LAYER_WHILE_POSSESSED);
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        GlobalEvent.Invoke.OnEnemyPossessed(playerMain);
    }

    public void RevertPossession()
    {
        // prevent duplicate player spawns
        if (!isPossessed) return;
        // enable AI scripts, disable player control scripts on enemy
        SetEnemyComponentsEnabled(true);
        SetPlayerComponentsEnabled(false);
        StartCoroutine(SpawnPlayerCoroutine());
        isPossessed = false;
        isPlayerPossessing.value = false;
        gameObject.name = initialName;
        gameObject.tag = Constants.ENEMY_TAG;
        body.interpolation = RigidbodyInterpolation2D.None;
        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        gameObject.layer = initialLayer;
    }

    void SetEnemyComponentsEnabled(bool value)
    {
        // enemy.enabled = value;
        // enemyAttack.enabled = value;
        // aIMovement.enabled = value;
        enemyAI.enabled = value;
    }

    void SetPlayerComponentsEnabled(bool value)
    {
        playerMain.enabled = value;
        playerMovementController.enabled = value;
        playerCombat.enabled = value;
        inputManager.enabled = value;
        //animator.enabled = value;
        groundCheck.enabled = value;
        move.enabled = value;
        jump.enabled = value;
        attack.enabled = value;
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
