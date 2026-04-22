using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle, Jump, Run, Dash,
        SwordAttack1, SwordAttack2, SwordAttack3, SwordUltimate,
        MagicBowAttack, MagicBowUltimate
    }

    private PlayerState currentState = PlayerState.Idle;
    private PlayerState newState = PlayerState.Idle;
    private readonly string IDLE_STATE = "Idle";
    private readonly string JUMP_STATE = "Jump";
    private readonly string RUN_STATE = "Run";
    private readonly string DASH_STATE = "Dash";
    private readonly string SWORD_ATTACK_1_STATE = "SwordAttack1";
    private readonly string SWORD_ATTACK_2_STATE = "SwordAttack2";
    private readonly string SWORD_ATTACK_3_STATE = "SwordAttack3";
    private readonly string SWORD_ULTIMATE_STATE = "SwordUltimate";
    private readonly string MAGIC_BOW_ATTACK_STATE = "MagicBowAttack";
    private readonly string MAGIC_BOW_ULTIMATE_STATE = "MagicBowUltimate";

    [Header("Basic Parameters")]
    public float walk_V = 1f;
    public float jump_V = 5f;
    public float dash_force = 3f;
    public float dash_time = 0.2f;
    public float dmg_cd = 0.5f;
    public float stamina_usage = 50f;
    public float mana_bow_usage = 50f;
    public float SwordBasicDmg = 25f;
    public float MagicBowBasicDmg = 50f;
    [Header("Collision Options")]
    public LayerMask groundMask;
    public float groundDetectDistance = 0.1f;
    [Header("Additional objects")]
    public HUD_Manager hud_Manager;
    public GameMenuScript gameMenuScript;
    public Inventory inventory;
    public PlayerStats stats;
    public GameObject arrowPrefab;
    public GameObject arrowUltimatePrefab;
    public Transform arrowStartPos;
    [Header("SwordColliders")]
    public BoxCollider2D swordStabCollider;
    public BoxCollider2D swordSwingCollider;
    public Transform swordSwingColliderPivot;
    public BoxCollider2D swordUltimateCollider;
    public LayerMask enemyHitboxMask;
    public SwordUltimateEffect ultimateEffectLeft;
    public SwordUltimateEffect ultimateEffectRight;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;
    private WeaponType activeWeapon = WeaponType.isSword;
    private bool isGrounded = false;
    private bool shouldJump = false;
    private bool shouldDash = false;
    private bool shouldAttack = false;
    private bool shouldActivateUltimateAttack = false;
    private bool isDashing = false;
    private float timeDashing = 0f;
    private float dashDir = 0f;
    private float horizontalValue = 0f;
    private float lookDirection = 1f;

    private float dmg_cd_time_point;
    private bool isAttacking = false;
    private string activeShopName = "";

    //knockback
    private float currentKnockbackForce;
    private float knockbackForceDelta;
    private float timeKnockbackStarted;
    private bool hasKnockback;
    
    //collided platform
    BoxCollider2D platformCollider = null;

    //enemies
    HashSet<Enemy> damagedEnemies = new();
    //array of 20 colliders to check hits on enemies
    Collider2D[] results = new Collider2D[20];

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
        hud_Manager.ChangeWeapon(activeWeapon);
        GameContext.playerStats = stats;
        GameContext.hudManager = hud_Manager;
        dmg_cd_time_point = Time.time;

        animator.Play(IDLE_STATE);

        DeactivateSwordStabCollider();
    }
    public void ShootArrow(int isUltimate) {
        if (isUltimate == 1)
        {
            GameObject arrow = Instantiate(arrowUltimatePrefab);
            PlayerArrow arrowData = arrow.GetComponent<PlayerArrow>();
            arrowData.dmg = stats.magic_ultimate_basic_dmg * stats.magic_dmg_mlpr;
            arrowData.speedX = 20f;
            arrow.transform.position = arrowStartPos.position;
            Vector2 direction;
            if (lookDirection > 0) direction = new Vector2(1f, 0f);
            else direction = new Vector2(-1f, 0f);
            arrowData.SetStartDirection(direction);
            AudioMixerManager.Instance.PlaySound(5);
        }
        else
        {
            Use_MANA(mana_bow_usage);
            GameObject arrow = Instantiate(arrowPrefab);
            PlayerArrow arrowData = arrow.GetComponent<PlayerArrow>();
            arrowData.dmg = stats.magic_basic_dmg * stats.magic_dmg_mlpr;
            arrowData.speedX = 15f;
            Vector3 startPos = arrowStartPos.position;
            startPos.y += 0.2f;
            if (lookDirection > 0) startPos.x += 2f;
            else startPos.x -= 2f;
            arrow.transform.position = arrowStartPos.position;
            arrowData.SetStartDirection((startPos - arrowData.transform.position).normalized);
            //Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //arrowData.SetStartDirection((mouseWorldPos - startPos).normalized);
            AudioMixerManager.Instance.PlaySound(5);
        }
    }
    public void ActivateSwordUltimateEffect()
    {
        ultimateEffectLeft.ActivateEffect();
        ultimateEffectRight.ActivateEffect();
    }
    public void DeactivateSwordUltimateEffect()
    {
        ultimateEffectLeft.DeactivateEffect();
        ultimateEffectRight.DeactivateEffect();
    }
    public void CheckUltimateHits()
    {
        damagedEnemies.Clear();
        CheckSwordUltimateHit(swordUltimateCollider);
    }
    private void Attack()
    {
        if (GameContext.attackOutsideAreaIsAllowed ||
                !GameContext.attackOutsideAreaIsAllowed && GameContext.playerIsInDesignatedArea)
        {
            if (activeWeapon == WeaponType.isMagicBow && stats.mana < mana_bow_usage) return;
                shouldAttack = true;
            horizontalValue = 0f;
        }
    }
    public void ActivateSwordStabCollider()
    {
        damagedEnemies.Clear();
        swordStabCollider.enabled = true;
    }
    public void DeactivateSwordStabCollider()
    {
        swordStabCollider.enabled = false;
    }
    public void ActivateSwordSwingCollider(float start_pivot_angle)
    {
        damagedEnemies.Clear();
        swordSwingCollider.enabled = true;
        swordSwingColliderPivot.localRotation = Quaternion.Euler(0, 0, start_pivot_angle);
    }
    public void DeactivateSwordSwingCollider()
    {
        swordSwingCollider.enabled = false;
    }
    public void CheckSwordSwingColliderHit(float pivot_angle)
    {
        swordSwingColliderPivot.localRotation = Quaternion.Euler(0, 0, pivot_angle);
        CheckSwordBasicHit(swordSwingCollider);
    }
    public void CheckSwordStabColliderHit()
    {
        CheckSwordBasicHit(swordStabCollider);
    }
    public void CheckSwordBasicHit(BoxCollider2D boxCollider)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.useTriggers = true;
        filter.SetLayerMask(enemyHitboxMask);

        int count = boxCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            Enemy e = results[i].GetComponent<Enemy>();
            if (e != null && !damagedEnemies.Contains(e))
            {
                damagedEnemies.Add(e);
                bool isCrit = stats.IsCritHit();
                float damage = stats.GetSwordDamage(false, isCrit);
                e.Take_damage(damage, PlayerAttackType.isSword, true, 2f);
                DamageTextPoolManager.instance.ActivateDamageText(damage, isCrit, e.gameObject.transform.position);
            }
        }
    }
    public void CheckSwordUltimateHit(BoxCollider2D boxCollider)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.useTriggers = true;
        filter.SetLayerMask(enemyHitboxMask);

        int count = boxCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            Enemy e = results[i].GetComponent<Enemy>();
            if (e != null && !damagedEnemies.Contains(e))
            {
                damagedEnemies.Add(e);
                bool isCrit = stats.IsCritHit();
                float damage = stats.GetSwordDamage(true, isCrit);
                e.Take_damage(damage, PlayerAttackType.isSword, true, 5f);
                DamageTextPoolManager.instance.ActivateDamageText(damage, isCrit, e.gameObject.transform.position);
            }
        }
    }
    void Update() {
        if (stats.hp == 0) return;
        stats.Update_Stats();

        isGrounded = IsGrounded();

        //change weapon
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (activeWeapon == WeaponType.isSword) activeWeapon = WeaponType.isMagicBow;
            else activeWeapon = WeaponType.isSword;
            hud_Manager.ChangeWeapon(activeWeapon);
        }
        //horizontal = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.A)) horizontalValue = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontalValue = 1f;
        else horizontalValue = 0f;
        //attack
        if (Input.GetMouseButtonDown(0) && GameContext.gameState == GameState.inGame && !isDashing)
            Attack();
        //ultimate
        if (Input.GetKeyDown(KeyCode.Q) && GameContext.gameState == GameState.inGame &&
            !isDashing && !isAttacking && isGrounded && stats.Activate_Ultimate())
        {
            shouldActivateUltimateAttack = true;
            horizontalValue = 0f;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            shouldJump = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) && stats.stamina >= stamina_usage)
            shouldDash = true;
        if (Input.GetKeyDown(KeyCode.S) && platformCollider != null)
            StartCoroutine(DropFromPlatform());

        if (isAttacking || shouldAttack || shouldActivateUltimateAttack) horizontalValue = 0f;

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
        if (Input.GetKeyDown(KeyCode.U))
            stats.Get_Ultimate_Stack();
        ////

        //buy artifact from shop if it's available
        if (Input.GetKeyDown(KeyCode.B) && activeShopName != "" && stats.money >= 30)
        {
            LevelManager.Instance.BuyShopArtifact(activeShopName);
            activeShopName = "";
            hud_Manager.HideShopArtifactTooltip();
            stats.SpendMoney(30);
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

        //STOP ATTACK STATES
        //sword
        if (state.IsName(SWORD_ATTACK_1_STATE) && state.normalizedTime >= 1.0f) {
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        else if (state.IsName(SWORD_ATTACK_2_STATE) && state.normalizedTime >= 1.0f) {
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        else if (state.IsName(SWORD_ATTACK_3_STATE) && state.normalizedTime >= 1.0f) {
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        else if (state.IsName(SWORD_ULTIMATE_STATE) && state.normalizedTime >= 1.0f)
        {
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        //magic bow
        else if (state.IsName(MAGIC_BOW_ATTACK_STATE) && state.normalizedTime >= 1.0f)
        {
            isAttacking = false;
            newState = PlayerState.Idle;
        }
        else if (state.IsName(MAGIC_BOW_ULTIMATE_STATE) && state.normalizedTime >= 1.0f)
        {
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
            else if (state.IsName(SWORD_ATTACK_3_STATE))
                newState = PlayerState.SwordAttack3;
            else if (state.IsName(SWORD_ULTIMATE_STATE))
                newState = PlayerState.SwordUltimate;
            else if (state.IsName(MAGIC_BOW_ATTACK_STATE))
                newState = PlayerState.MagicBowAttack;
            else if (state.IsName(MAGIC_BOW_ULTIMATE_STATE))
                newState = PlayerState.MagicBowUltimate;
        }
        if (shouldAttack)
        {
            shouldAttack = false;
            isAttacking = true;
            if(activeWeapon == WeaponType.isSword)
            {
                if (state.IsName(SWORD_ATTACK_1_STATE) && state.normalizedTime >= 0.8f)
                    //go from sword first attack to second
                    newState = PlayerState.SwordAttack2;
                else if (state.IsName(SWORD_ATTACK_2_STATE) && state.normalizedTime >= 0.8f)
                    //go from second swored attack to third
                    newState = PlayerState.SwordAttack3;
                else if (!state.IsName(SWORD_ATTACK_3_STATE))
                    //otherwise use first attack
                    newState = PlayerState.SwordAttack1;
            }
            else if (activeWeapon == WeaponType.isMagicBow)
            {
                newState = PlayerState.MagicBowAttack;
            }

        }
        if (shouldActivateUltimateAttack)
        {
            isAttacking = true;
            shouldActivateUltimateAttack = false;
            if (activeWeapon == WeaponType.isSword)
            {
                newState = PlayerState.SwordUltimate;
            }
            else if (activeWeapon == WeaponType.isMagicBow)
            {
                newState = PlayerState.MagicBowUltimate;
            }
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
            case PlayerState.SwordUltimate:
                {
                    animator.Play(SWORD_ULTIMATE_STATE);
                    break;
                }
            case PlayerState.MagicBowAttack:
                {
                    animator.Play(MAGIC_BOW_ATTACK_STATE);
                    break;
                }
            case PlayerState.MagicBowUltimate:
                {
                    animator.Play(MAGIC_BOW_ULTIMATE_STATE);
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
    public void Decrease_HP(float damage)
    {
        stats.hp -= damage * (100.0f / (100.0f + stats.def)) * (1 - stats.dmg_reduction);
        if (stats.hp <= 0)
        {
            stats.hp = 0;
            horizontalValue = 0f;
            OnPlayerDeath();
        }
        hud_Manager.Set_Hp_Bar(stats.hp / stats.maxHP);
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
    public bool Take_Damage(float dmg) {
        if (CanTakeDamage() && !isDashing)
        {
            dmg_cd_time_point = Time.time;
            Decrease_HP(dmg);
            AudioMixerManager.Instance.PlaySound(0);
            return true;
        }
        return false;
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
