using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    protected readonly string INITIAL_POSE_STATE = "InitialPose";
    protected readonly string IDLE_STATE = "Idle";

    [Header("----Basic Stats----")]
    public int enemy_id;
    public string bossName;
    public float hp;
    public float healing;
    public float dmg;
    public float speed;
    public float jump_speed;
    public int xp_amount = 20;
    public int money_amount = 5;
    public int damage_sound_id = 0;
    public float startTriggerDistance = 1f;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected float base_x_scale = 1.0f, base_y_scale = 1.0f;
    protected CapsuleCollider2D capsuleCollider;
    protected Animator animator;
    protected float x_direction;
    protected float current_hp;
    protected float knockbackForceDelta;
    protected float currentKnockbackForce;
    protected float timeKnockbackStarted;
    protected bool hasKnockback = false;
    protected bool canWalk = false;
    protected bool canBeDamaged = false;
    protected bool deathAnimIsTriggered = false;
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        animator.Play(INITIAL_POSE_STATE, 0, 0.0f);
        rb.freezeRotation = true;
        current_hp = hp;
        base_x_scale = transform.localScale.x;
        base_y_scale = transform.localScale.y;
    }
    public virtual void Update()
    {
        if (!canWalk)
        {
            if ((GameContext.playerPos - rb.position).magnitude <= startTriggerDistance)
            {
                canWalk = true;
                canBeDamaged = true;
                animator.Play(IDLE_STATE, 0, 0.0f);
                GameContext.hudManager.ShowBossHPBar(bossName);
                GameContext.bossArena.CloseArena();
            }
        }
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
    }
    public virtual void FixedUpdate() { }
    public virtual void OnDeath()
    {
        GameContext.hudManager.HideBossHPBar();
        GameContext.playerStats.GetMoney(money_amount);
        GameContext.playerStats.GetXP(xp_amount);
        GameContext.bossArena.OpenArena();
        Destroy(gameObject);
    }
    public virtual bool Take_damage(
        float dmg,
        PlayerAttackType attackType,
        bool applyKnockback = false,
        float knockbackForce = 0
    )
    {
        //return TRUE only if boss loses HP
        if (current_hp == 0 || !canBeDamaged)
            return false;
        
        current_hp -= dmg;
        if (current_hp <= 0)
        {
            current_hp = 0;
        }
        GameContext.hudManager.UpdateBossHPBar(current_hp / hp);
        GameContext.playerStats.Get_Ultimate_Stack();
        if (applyKnockback)
        {
            if (GameContext.playerPos.x > transform.position.x)
                ApplyKnockback(knockbackForce, false); //apply knockback to left side
            else
                ApplyKnockback(knockbackForce, true);
        }
        AudioMixerManager.Instance.PlaySound(damage_sound_id);
        return true;
    }
    public virtual void ApplyKnockback(float force, bool rightSide)
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
    public virtual void Attack() { }
    public float Get_Damage() { return dmg; }
    public float Get_hp() { return hp; }
    public void Heal_hp() { hp += healing; }
}
