using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("----Basic Stats----")]
    public int enemy_id;
    public float hp;
    public float healing;
    public float dmg;
    public float speed;
    public float jump_speed;
    public int xp_amount = 20;
    public int money_amount = 5;
    public int damage_sound_id = 0;
    public bool isGhost = false;
    [Header("----HP Bar----")]
    public GameObject hp_bar_holder;
    public Transform hp_bar;

    protected float hp_bar_x_max_scale;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected float x_direction;
    protected float current_hp;
    protected float knockbackForceDelta;
    protected float currentKnockbackForce;
    protected float timeKnockbackStarted;
    protected bool hasKnockback = false;
    protected int magicShieldStrength = 0;
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        current_hp = hp;
        if (hp_bar_holder != null)
        {
            hp_bar_x_max_scale = hp_bar.localScale.x;
            hp_bar_holder.SetActive(false);
        }
    }
    public virtual void Update()
    {
        if(hasKnockback)
        {
            if(Time.time - timeKnockbackStarted > 0.25f)
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
        GameContext.playerStats.GetMoney(money_amount);
        GameContext.playerStats.GetXP(xp_amount);
        Destroy(gameObject);
        GameContext.enemies_destroyed++;
    }
    public virtual void Take_damage(float dmg, PlayerAttackType attackType)
    {
        if (current_hp == 0) return;
        if(magicShieldStrength > 0)
        {
            if (attackType != PlayerAttackType.isMagicArrow) return;
            else if(attackType == PlayerAttackType.isMagicArrow)
            {
                magicShieldStrength--;
                if(magicShieldStrength == 0)
                {
                    sr.color = Color.white;
                }
            }
            return;
        }
        current_hp -= dmg;
        if(current_hp <= 0)
        {
            current_hp = 0;
            hp_bar_holder.SetActive(false);
        }
        else
        {
            hp_bar_holder.SetActive(true);
            hp_bar.localScale = new Vector3(hp_bar_x_max_scale * current_hp / hp, 1, 1);
        }
        AudioMixerManager.Instance.PlaySound(3);
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
    public virtual void ProtectByMagicShield(int shieldStrength)
    {
        //1 point of shield strength is one hit from magic weapon(f.e. magic bow)
        if (shieldStrength <= 0) return;
        //apply shield
        magicShieldStrength = shieldStrength;
        sr.color = Color.blue;
    }
    public float Get_Damage() { return dmg; }
    public float Get_hp() { return hp; }
    public void Heal_hp() { hp += healing; } 
}
