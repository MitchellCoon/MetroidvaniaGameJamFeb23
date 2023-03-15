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
    [SerializeField] GameObject enableOnPossessionContainer;
    [SerializeField] Transform unpossessionSpawnPoint;
    [SerializeField] BoolVariable isPlayerPossessing;
    [SerializeField] SpriteRenderer slimePossessionSpriteRenderer;
    [SerializeField] Animator slimePossessionAnimator;
    [SerializeField] SlimePossessMotion slimePossessHealthyPrefab;
    [SerializeField] SlimePossessMotion slimePossessHurtPrefab;
    [SerializeField] SlimePossessMotion slimePossessPerilPrefab;
    [SerializeField] Transform slimeAttachPoint;
    [SerializeField] bool canGetPossessed;
    [Space]
    [Space]
    [SerializeField] Sound possessSound;
    [SerializeField] Sound unpossessSound;
    private float possessionTimer = 0.5f;
	private float possessionCooldown = 0.5f;


    Rigidbody2D body;

    // enemy components
    Enemy enemy;
    EnemyAttack enemyAttack;
    AIMovement aIMovement;
    BaseEnemyAI enemyAI;
    [SerializeField] ReflexJump reflexJump;

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
        Assert.IsNotNull(slimePossessionSpriteRenderer, ERR_MSG_COMPONENT_MISSING);
        Assert.IsNotNull(isPlayerPossessing, "Please assign ref to `isPlayerPossessing` in `PossessionManager`");
        // initial state
        initialLayer = gameObject.layer;
        initialName = gameObject.name;
        if (enableOnPossessionContainer != null) enableOnPossessionContainer.SetActive(false);
    }

    void Update()
    {
        possessionTimer += Time.deltaTime;
        bool isUnpossessButtonPressed = MInput.GetKeyDown(KeyCode.Mouse1) || MInput.GetKeyDown(KeyCode.O) || MInput.GetPadDown(GamepadCode.ButtonEast) || MInput.GetPadDown(GamepadCode.BumperRight);
        //bool isUnpossessButtonPressed = MInput.GetKeyDown(KeyCode.F) || MInput.GetPadDown(GamepadCode.ButtonNorth);
        if (isPossessed && isUnpossessButtonPressed && possessionTimer >= possessionCooldown)
        {
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
        if (isPlayerPossessing.value || !canGetPossessed) return;
        if (possessSound != null) possessSound.Play();
        if (enableOnPossessionContainer != null) enableOnPossessionContainer.SetActive(true);
        SetEnemyComponentsEnabled(false);
        if(playerObj.GetComponent<PlayerMovementController>().IsFacingRight() != enemyAI.IsFacingRight())
        {
            playerMovementController.Flip();
        }
        possessionTimer = 0f;
        move.ResetPossessionTimer();
        SetPlayerComponentsEnabled(true);
        SpawnPlayerPossessObject(playerObj);
        Destroy(playerObj);
        isPossessed = true;
        enemyAI.GetComponent<Animator>().SetBool("isPossessed", true);
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
        if (unpossessSound != null) unpossessSound.Play();
        if (enableOnPossessionContainer != null) enableOnPossessionContainer.SetActive(false);
        // enable AI scripts, disable player control scripts on enemy
        SetEnemyComponentsEnabled(true);
        SetPlayerComponentsEnabled(false);
        StartCoroutine(SpawnPlayerCoroutine());
        isPossessed = false;
        enemyAI.GetComponent<Animator>().SetBool("isPossessed", false);
        isPlayerPossessing.value = false;
        slimePossessionSpriteRenderer.enabled = false;
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
        if(value)
        {
            enemyAI.ResetDirection();
        }
        if (reflexJump != null)
        {
            reflexJump.enabled = value;
        }
    }

    void SetPlayerComponentsEnabled(bool value)
    {
        playerMain.enabled = value;
        playerMovementController.enabled = value;
        if(value)
        {
            playerMovementController.ResetDirection();
        }
        playerCombat.enabled = value;
        inputManager.enabled = value;
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
        int currentHealth = playerCombat.GetHealthValue();
        Instantiate(playerPrefab, unpossessionSpawnPoint.position, Quaternion.identity).GetComponent<PlayerCombat>().SetHealthValue(currentHealth);
        yield return null;
    }

    void SpawnPlayerPossessObject(GameObject playerObj)
    {
        PlayerCombat playerCombat = playerObj.GetComponent<PlayerCombat>();
        if(playerCombat.GetHealthStatus() == HealthStatus.Healthy)
        {
            Instantiate(slimePossessHealthyPrefab, playerObj.transform.position, playerObj.transform.rotation).SetTarget(slimePossessionSpriteRenderer, slimePossessionAnimator, slimeAttachPoint.position, playerMovementController.IsFacingRight());
        }
        else if(playerCombat.GetHealthStatus() == HealthStatus.Hurt)
        {
            Instantiate(slimePossessHurtPrefab, playerObj.transform.position, playerObj.transform.rotation).SetTarget(slimePossessionSpriteRenderer, slimePossessionAnimator, slimeAttachPoint.position, playerMovementController.IsFacingRight());
        }
        else
        {
            Instantiate(slimePossessPerilPrefab, playerObj.transform.position, playerObj.transform.rotation).SetTarget(slimePossessionSpriteRenderer, slimePossessionAnimator, slimeAttachPoint.position, playerMovementController.IsFacingRight());
        }

    }

    void FailBadlyAndNoticeably(string reason)
    {
        Destroy(gameObject);
        throw new UnityException(reason);
    }

    public bool IsPossessed()
    {
        return isPossessed;
    }
}
