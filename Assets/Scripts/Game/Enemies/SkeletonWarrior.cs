using TMPro;
using UnityEngine;

public class SkeletonWarrior : Enemy
{
    static string ATTACK_STATE = "Attack";

    protected Animator animator;
    public float stop_radius = 5f;
    public float attack_interval_sec = 1f;
    public GameObject attackArea;
    protected float last_attack_time = 0f;
    protected CircleCollider2D hitCollider;

    private bool death_anim_triggered = false;
    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        hitCollider = attackArea.GetComponent<CircleCollider2D>();
        hitCollider.enabled = false;
        last_attack_time = Time.time;
    }
    public override void Update()
    {
        base.Update();
        if (current_hp == 0)
        {
            if (!death_anim_triggered)
            {
                animator.SetTrigger("isDead");
                x_direction = 0;
                death_anim_triggered = true;
                AudioMixerManager.Instance.PlaySound(2);
            }
            return;
        }

        if (canWalk)
        {
            //change direction based on player X pos
            x_direction = GameContext.playerPos.x - rb.position.x;
            if (x_direction > 0)
            {
                x_direction = 1f;
                sr.flipX = false;
            }
            else
            {
                x_direction = -1f;
                sr.flipX = true;
            }
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            x_direction = 0f;
        }

        //check if near enough the player to stop walking
        bool isStanding = false;
        if ((GameContext.playerPos - rb.position).magnitude <= stop_radius)
        {
            x_direction = 0f;
            animator.SetBool("isWalking", false);
            isStanding = true;
        }
        //attack if can
        if (Time.time - last_attack_time >= attack_interval_sec && isStanding)
        {
            last_attack_time = Time.time;
            animator.SetTrigger("attack");
            AudioMixerManager.Instance.PlaySound(4);
        }
        //update attack collider
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(ATTACK_STATE))
        {
            UpdateAttackCollider(stateInfo);
        }
    }
    public void UpdateAttackCollider(AnimatorStateInfo stateInfo)
    {
        float t = stateInfo.normalizedTime;
        if(t >= 0.25f && t <= 0.95f) hitCollider.enabled = true;
        else hitCollider.enabled = false;
    }
    public override void FixedUpdate()
    {
        rb.linearVelocityX = x_direction * speed + currentKnockbackForce;
    }
    public override void Attack()
    {
        
    }
}