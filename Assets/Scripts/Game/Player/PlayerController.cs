using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle, Jump, Run, Dash, SwordAttack1, SwordAttack2, SwordAttack3
    }
    private PlayerState currentState = PlayerState.Idle;
    private PlayerState newState = PlayerState.Idle;
    private readonly string IDLE_STATE = "Idle";
    private readonly string JUMP_STATE = "Jump";
    private readonly string RUN_STATE = "Run";
    private readonly string DASH_STATE = "Run";
    private readonly string SWORD_ATTACK_1_STATE = "SwordAttack1";
    private readonly string SWORD_ATTACK_2_STATE = "SwordAttack2";
    private readonly string SWORD_ATTACK_3_STATE = "SwordAttack3";

    [Header("Basic Parameters")]
    public float walk_V = 1f;
    public float jump_V = 5f;
    public float dash_force = 3f;
    public float dash_time = 0.2f;
    public float dmg_cd = 0.5f;
    public float stamina_usage = 50f;
    public float mana_bow_usage = 50f;
    [Header("Collision Options")]
    public LayerMask groundMask;
    public float groundDetectDistance = 0.1f;
    [Header("Additional objects")]
    public HUD_Manager hud_Manager;
    public GameMenuScript gameMenuScript;
    public Inventory inventory;
    public PlayerStats stats;
    public GameObject arrowPrefab;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;
    private bool isGrounded = false;
    private bool shouldJump = false;
    private bool shouldDash = false;
    private bool shouldAttack = false;
    private bool isDashing = false;
    private float timeDashing = 0f;
    private float dashDir = 0f;
    private float horizontalValue = 0f;
    private float lookDirection = 1f;

    private float dmg_cd_time_point;
    private bool isAttacking = false;
    private int active_weapon_id = 0;
    private string activeShopName = "";

    //knockback
    private float currentKnockbackForce;
    private float knockbackForceDelta;
    private float timeKnockbackStarted;
    private bool hasKnockback;
    //collided platform
    BoxCollider2D platformCollider = null;

    private void Awake()
    {
        GameContext.inventory = inventory;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;
        stats = new PlayerStats();
        stats.Init_Stats(hud_Manager);
        hud_Manager.ChangeWeapon(active_weapon_id);
        GameContext.playerStats = stats;
        GameContext.hudManager = hud_Manager;
        dmg_cd_time_point = Time.time;

        animator.Play(IDLE_STATE);
    }
    public void ShootArrow() {
        GameObject arrow = Instantiate(arrowPrefab);
        PlayerArrow arrowData = arrow.GetComponent<PlayerArrow>();
        arrowData.dmg = 50f;
        arrowData.speedX = 15f;
        Vector2 startPos = transform.position;
        startPos.y += 0.6f;
        arrow.transform.position = startPos;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        arrowData.SetStartDirection((mouseWorldPos - startPos).normalized);
        AudioMixerManager.Instance.PlaySound(5);
    }

    void Update() {
        if (stats.hp == 0) return;
        stats.Update_Stats();

        //change weapon
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (active_weapon_id == 0) active_weapon_id = 1;
            else active_weapon_id = 0;
            hud_Manager.ChangeWeapon(active_weapon_id);
        }
        //const buffs page
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameMenuScript.OpenConstBuffsPage();
        }
        //horizontal = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.A)) horizontalValue = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontalValue = 1f;
        else horizontalValue = 0f;
        //attack
        shouldAttack = false;
        if (Input.GetMouseButtonDown(0) && GameContext.gameState == GameState.inGame && !isDashing)
            if(GameContext.attackOutsideAreaIsAllowed ||
                !GameContext.attackOutsideAreaIsAllowed && GameContext.playerIsInDesignatedArea) {
                shouldAttack = true;
                horizontalValue = 0f;
            }

        isGrounded = IsGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            shouldJump = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) && stats.stamina >= stamina_usage)
            shouldDash = true;
        if (Input.GetKeyDown(KeyCode.S) && platformCollider != null)
            StartCoroutine(DropFromPlatform());

        if (isAttacking || shouldAttack) horizontalValue = 0f;

        ChangePlayerSpriteDirection();

        ////temporary controls
        if (Input.GetKeyDown(KeyCode.Alpha1)) LevelManager.Instance.SpawnEnemy(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) LevelManager.Instance.SpawnEnemy(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) LevelManager.Instance.SpawnEnemy(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) LevelManager.Instance.SpawnEnemy(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) LevelManager.Instance.SpawnEnemy(4);
        if (Input.GetKeyDown(KeyCode.H)) Increase_HP(500);

        if (Input.GetKeyDown(KeyCode.T))
        {
            LevelManager.Instance.UpdateAvailableBuffs();
            gameMenuScript.OpenBuffsPage();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.AddArtifact(1); //give "wizard flashlight"
            inventory.AddArtifact(2); //give "soul in a bottle"
        }
        ////

        //buy artifact from shop if it's available
        if (Input.GetKeyDown(KeyCode.B) && activeShopName != "" && stats.money >= 30)
        {
            LevelManager.Instance.BuyShopArtifact(activeShopName);
            activeShopName = "";
            hud_Manager.HideShopArtifactTooltip();
            stats.SpendMoney(30);
        }

        //attack
        if (shouldAttack)
        {
            //shouldAttack = false;
            //isAttacking = true;
            //animator.Play(SWORD_ATTACK_1_STATE);
        }
        //knockback
        if (hasKnockback)
        {
            if (Time.time - timeKnockbackStarted > 0.25f)
            {
                hasKnockback = false;
                currentKnockbackForce = 0;
                return;
            }
            currentKnockbackForce -= knockbackForceDelta * Time.deltaTime;

        }

        UpdateAnimState();
        ChangeAnimState(newState);
    }

    private void FixedUpdate()
    {
        if(!isDashing)
            rb.linearVelocityX = walk_V * horizontalValue + currentKnockbackForce;
        if (shouldJump)
        {
            rb.linearVelocityY = jump_V;
            shouldJump = false;
        }
        if (shouldDash && horizontalValue != 0f && !isDashing)
        {
            Use_Stamina(stamina_usage);
            shouldDash = false;
            isDashing = true;
            timeDashing = 0f;
            dashDir = horizontalValue;
        }
        if(isDashing)
        {
            float dashV = timeDashing / dash_time;
            if (dashV > 0.75f) dashV = 0.75f;
            rb.linearVelocityX = dash_force * dashDir * (1f - dashV);
            timeDashing += Time.deltaTime;
            if(timeDashing >= dash_time)
            {
                isDashing = false;
            }
        }
        GameContext.playerPos = rb.position;
    }
    private void UpdateAnimState()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName(SWORD_ATTACK_1_STATE) && state.normalizedTime >= 1.0f) {
            newState = PlayerState.SwordAttack2;
            //return;

            //stop attack state
            isAttacking = false;
            newState = PlayerState.Idle;
        }

        if (state.IsName(SWORD_ATTACK_2_STATE) && state.normalizedTime >= 1.0f)
        {
            newState = PlayerState.SwordAttack3;
            //return;

            //stop attack state
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        if (state.IsName(SWORD_ATTACK_3_STATE) && state.normalizedTime >= 1.0f)
        {
            newState = PlayerState.Idle;
            isAttacking = false;
            //return;

            //stop attack state
            isAttacking = false;
            newState = PlayerState.Idle;
        }

        if (horizontalValue != 0f) newState = PlayerState.Run;
        else newState = PlayerState.Idle;

        if (!isGrounded) newState = PlayerState.Jump;

        if (isAttacking) {
            if (state.IsName(SWORD_ATTACK_1_STATE))
                newState = PlayerState.SwordAttack1;
            else if (state.IsName(SWORD_ATTACK_2_STATE))
                newState = PlayerState.SwordAttack2;
            else
                newState = PlayerState.SwordAttack3;
        }
        if (shouldAttack)
        {
            shouldAttack = false;
            isAttacking = true;
            if (state.IsName(SWORD_ATTACK_1_STATE) && state.normalizedTime >= 0.8f)
                newState = PlayerState.SwordAttack2;
            else if (state.IsName(SWORD_ATTACK_2_STATE) && state.normalizedTime >= 0.8f)
                newState = PlayerState.SwordAttack3;
            else if (!state.IsName(SWORD_ATTACK_3_STATE))
                newState = PlayerState.SwordAttack1;
        }

        if (isDashing) newState = PlayerState.Dash;
    }
    private void ChangeAnimState(PlayerState state)
    {
        if (currentState == state) return;
        currentState = state;
        switch(state) {
            case PlayerState.Idle: {
                    animator.Play(IDLE_STATE);
                    break;
                }
            case PlayerState.Dash: {
                    animator.Play(DASH_STATE);
                    break;
                }
            case PlayerState.Jump: {
                    animator.Play(JUMP_STATE);
                    break;
                }
            case PlayerState.Run: {
                    animator.Play(RUN_STATE);
                    break;
                }
            case PlayerState.SwordAttack1: {
                    animator.Play(SWORD_ATTACK_1_STATE);
                    break;
                }
            case PlayerState.SwordAttack2: {
                    animator.Play(SWORD_ATTACK_2_STATE);
                    break;
                }
            case PlayerState.SwordAttack3: {
                    animator.Play(SWORD_ATTACK_3_STATE);
                    break;
                }
        }
    }
    public void ApplyKnockback(float force, bool rightSide)
    {
        //apply knockback force to right side
        if (rightSide)
        {
            currentKnockbackForce = force;
        }
        else
        {
            currentKnockbackForce = -force;
        }
        knockbackForceDelta = 0.25f / currentKnockbackForce;
        timeKnockbackStarted = Time.time;
        hasKnockback = true;
    }
    public void Decrease_HP(float value)
    {
        stats.hp -= value;
        if (stats.hp <= 0)
        {
            stats.hp = 0;
            horizontalValue = 0f;
            OnPlayerDeath();
        }
        hud_Manager.Set_Hp_Bar(stats.hp / stats.maxHP);
    }
    public void Decrease_DEF(float value)
    {
        stats.def -= value;
    }
    public bool Use_MANA(float value)
    {
        if (stats.mana - value < 0) return false;
        stats.mana -= value;
        hud_Manager.Set_Mana_Bar(stats.mana / stats.maxMANA);
        return true;
    }
    public bool Use_Stamina(float value)
    {
        if (stats.stamina - value < 0) return false;
        stats.stamina -= value;
        hud_Manager.Set_Stamina_Bar(stats.stamina / stats.maxStamina);
        return true;
    }

    public void Increase_HP(float value)
    {
        stats.hp += value;
        if(stats.hp + value > stats.maxHP) stats.hp = stats.maxHP;
        hud_Manager.Set_Hp_Bar(stats.hp / stats.maxHP);
    }
    public void Increase_MANA(float value)
    {
        stats.mana += value;
        if (stats.mana + value > stats.maxMANA) stats.mana = stats.maxMANA;
        hud_Manager.Set_Mana_Bar(stats.mana / stats.maxMANA);
    }
    public void Increase_Stamina(float value)
    {
        stats.stamina += value;
        if (stats.stamina + value > stats.maxStamina) stats.stamina = stats.maxStamina;
        hud_Manager.Set_Stamina_Bar(stats.stamina / stats.maxStamina);
    }
    public void Take_Damage(float dmg) {
        if (CanTakeDamage() && !isDashing)
        {
            dmg_cd_time_point = Time.time;
            Decrease_HP(dmg);
            AudioMixerManager.Instance.PlaySound(0);
        }
    }
    public bool CanTakeDamage() {
        if (Time.time - dmg_cd_time_point >= dmg_cd) return true;
        else return false;
    }

    public void OnPlayerDeath() {
        GameContext.activeSave.active_room = 1;
        GameContext.activeSave.level = stats.level;
        GameContext.activeSave.money = 0;
        stats.ClearMoney();
        //clear temporary buffs
        ClearRunBasedSkills();
        //open death page
        gameMenuScript.OpenDeathPage();
    }

    public void ClearRunBasedSkills() {
        GameContext.activeSave.runBuffs.Clear();
    }

    public void SetActiveShopName(string name) {
        this.activeShopName = name;
    }

    private bool IsGrounded() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDetectDistance, groundMask);
        if (hit)
        {
            if (hit.collider.CompareTag("Platform"))
            {
                platformCollider = hit.collider.GetComponent<BoxCollider2D>();
            }
            return true;
        }
        else return false;
    }
    private IEnumerator DropFromPlatform() {
        Physics2D.IgnoreCollision(boxCollider, platformCollider, true);
        rb.linearVelocityY = -4f;
        yield return new WaitForSeconds(0.4f);
        Physics2D.IgnoreCollision(boxCollider, platformCollider, false);
        platformCollider = null;
    }
    private void ChangePlayerSpriteDirection() {
        if (horizontalValue > 0f)
        {
            lookDirection = 1f;
            Vector3 localScale = transform.localScale;
            localScale.x = 2.25f;
            transform.localScale = localScale;
        }
        else if (horizontalValue < 0f)
        {
            lookDirection = -1f;
            Vector3 localScale = transform.localScale;
            localScale.x = -2.25f;
            transform.localScale = localScale;
        }
    }
}
