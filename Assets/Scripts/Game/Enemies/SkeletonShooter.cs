using UnityEngine;

public class SkeletonShooter : Enemy
{
    protected Animator animator;
    public float stop_radius = 5f;
    public float attack_interval_sec = 1f;
    public float offset_y_target = 2f;
    public GameObject arrowPrefab;
    public Transform arrowStartPosRight;
    public Transform arrowStartPosLeft;
    public float arrow_speed = 15f;
    public bool canWalk = true;
    protected float last_attack_time = 0f;
    private bool looks_right = true;
    private bool death_anim_triggered = false;
    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        last_attack_time = Time.time;
    }
    public override void Update()
    {
        base.Update();
        if (current_hp == 0) {
            if (!death_anim_triggered) {
                animator.SetTrigger("isDead");
                x_direction = 0;
                death_anim_triggered = true;
                AudioMixerManager.Instance.PlaySound(2);
            }
            return;
        }
       
            //change direction based on player X pos
            x_direction = GameContext.playerPos.x - rb.position.x;
            if (x_direction > 0)
            {
                x_direction = 1f;
                sr.flipX = false;
                looks_right = true;
            }
            else
            {
                x_direction = -1f;
                sr.flipX = true;
                looks_right = false;
            }
            animator.SetBool("isWalking", true);

        if (!canWalk)
        {
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
        }
    }
    public override void FixedUpdate()
    {
        rb.linearVelocityX = x_direction * speed + currentKnockbackForce;
    }
    public override void Attack()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        SkeletonArrow arrowData = arrow.GetComponent<SkeletonArrow>();
        arrowData.dmg = this.dmg;
        arrowData.speedX = arrow_speed;
        Vector2 direction;
        if (looks_right)
        {
            direction = new Vector2(1f, 0.1f);
            arrow.transform.position = arrowStartPosRight.position;
        }
        else
        {
            direction = new Vector2(-1f, 0.1f);
            arrow.transform.position = arrowStartPosLeft.position;
        }
        //Vector2 playerPos = GameContext.playerPos;
        //playerPos.y += offset_y_target;
        //Vector2 direction = (playerPos - (Vector2)arrow.transform.position).normalized;
        arrowData.SetStartDirection(direction);
        AudioMixerManager.Instance.PlaySound(1);
    }
}
