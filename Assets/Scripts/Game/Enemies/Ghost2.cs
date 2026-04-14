using Codice.CM.Common;
using UnityEngine;

public class Ghost2 : Enemy
{
    protected Animator animator;
    public float speedY = 2f;
    public float offset_y_target = 1f;
    public float attack_radius = 5f;
    public float attack_interval_sec = 1f;
    public float dash_speed = 5f;
    protected float last_attack_time = 0f;

    private bool death_anim_triggered = false;
    private float current_x_speed = 0f;
    private float current_y_speed = 0f;
    private float current_dash_speed;
    private float timeDashStarted;
    private float dashDelta;
    private bool isDashing = false;
    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        last_attack_time = Time.time;
    }
    public override void Update()
    {
        base.Update();
        if (current_hp == 0)
        {
            if (!death_anim_triggered)
            {
                /*animator.SetTrigger("isDead");
                x_direction = 0;
                death_anim_triggered = true;*/
                OnDeath();
                AudioMixerManager.Instance.PlaySound(9);
            }
            return;
        }
        //change direction based on player X pos
        x_direction = GameContext.playerPos.x - rb.position.x;
        float y_direction = GameContext.playerPos.y + offset_y_target - rb.position.y;
        if (x_direction > 0)
        {
            current_x_speed += speed * Time.deltaTime;
            if (current_x_speed > speed) current_x_speed = speed;
            sr.flipX = false;
        }
        else
        {
            current_x_speed -= speed * Time.deltaTime;
            if (current_x_speed < -speed) current_x_speed = -speed;
            sr.flipX = true;
        }

        if (y_direction > 0)
        {
            current_y_speed += speedY * Time.deltaTime;
            if (current_y_speed > speedY) current_y_speed = speedY;
        }
        else
        {
            current_y_speed -= speedY * Time.deltaTime;
            if (current_y_speed < -speedY) current_y_speed = -speedY;
        }

        if (isDashing)
        {
            if(Time.time - timeDashStarted > 0.5f)
            {
                isDashing = false;
                current_dash_speed = 0;
            }
            current_dash_speed -= dashDelta * Time.deltaTime;
        }

        if(Time.time - timeDashStarted > attack_interval_sec)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        if(GameContext.playerPos.x - rb.position.x > 0)
        {
            current_dash_speed = dash_speed;
        }
        else
        {
            current_dash_speed = -dash_speed;
        }
        timeDashStarted = Time.time;
        dashDelta = 0.5f / current_dash_speed;
        isDashing = true;
        animator.SetTrigger("attack");
        AudioMixerManager.Instance.PlaySound(10 + Random.Range(0, 3));
    }
    public override void FixedUpdate()
    {
        rb.linearVelocityX = current_x_speed + currentKnockbackForce + current_dash_speed;
        rb.linearVelocityY = current_y_speed;
    }
    /*public override void Take_damage(float dmg)
    {
        if (current_hp == 0) return;
        current_hp -= dmg;
        if (current_hp <= 0)
        {
            current_hp = 0;
            hp_bar_holder.SetActive(false);
        }
        else
        {
            hp_bar_holder.SetActive(true);
            hp_bar.localScale = new Vector3(hp_bar_x_max_scale * current_hp / hp, 1, 1);
        }
        AudioMixerManager.Instance.PlaySound(8);
    }*/
    public override void Attack()
    {

    }
}
