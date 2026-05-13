using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMageBoss : BossEnemy
{
    enum BossAttackState
    {
        isNone, isCLOSE_ATTACK, IS_FORWARD_ATTACK, IS_UPPER_ATTACK, IS_DARK_ATTACK
    }

    private readonly string WALK_STATE = "Walk";
    private readonly string DEFEAT_STATE = "Defeat";
    private readonly string CLOSE_ATTACK_STATE = "CloseAttack";
    private readonly string FORWARD_ATTACK_STATE = "ForwardAttack";
    private readonly string UPPER_ATTACK_STATE = "UpperAttack";
    private readonly string DARK_ATTACK_STATE = "DarkAttack";

    [Header("--Main Options--")]
    [Header("CloseAttack")]
    public Vector3 pentagram_close_pos;
    public LayerMask playerMask;
    public float closeDamage = 25.0f;
    public float closeAttackDistance = 3.0f;
    public float closeAttackTimeToActivate = 3.0f; //how long player should be close to boss to activate close attack
    public float fireCircleMaxScale = 5.0f;
    public float fireCircleExplosionTime = 0.5f;
    public float firePentagramMinScale = 0.2f;
    public float firePentagramLifetime = 2.0f;
    public float closeAttackDuration = 5.0f;
    [Header("UpperAttack")]
    public Vector3 pentagram_upper_pos;
    public List<Transform> fire_pos_list = new();
    public List<Transform> fire_list = new();
    public float upperAttackDuration = 5.5f; //total state time
    public float upperAttackShootCD = 1.0f; //time between each fire attack
    public float upperPentagramRotateSpeed = 30.0f; //speed of pentagram rotation in 1 sec
    public float upperFireballsDamage = 20.0f;
    public float upperFireballsSpeed = 12.0f;
    [Header("ForwardAttack")]
    public Vector3 pentagram_forward_pos;
    public float forwardAttackCD = 10.0f; //how often this attack is used
    public float forwardAttackDuration = 1.5f;
    public float forwardAttackShootTimeOffset = 0.75f; //release fireballs in X.X seconds after this state is activated
    public float fireballsDamage = 25.0f;
    public float fireballsSpeed = 12.0f;
    [Header("DarkFireAttack")]
    public GameObject darkLivingFire;
    public GameObject darkFireballPrefab;
    public float darkAttackDuration = 2.0f;
    public float darkFireballsSpeed = 12.0f;
    public float darkFireballsDamage = 20.0f;
    public float darkFireballsShootCD = 0.5f;

    [Header("--Additional--")]
    public Transform livingFirePentagram;
    public Transform fireCircle;
    public GameObject fireballPrefab;
    public float maxTimeWalking = 1.0f;

    private SpriteRenderer livingFirePentagramSR;
    private SpriteRenderer fireCircleSR;
    private List<SpriteRenderer> fire_SR_list = new();

    private float timeCloseToPlayer = 0.0f;
    private BossAttackState currentAttackState = BossAttackState.isNone;
    private float timeUsingCurrentAttack = 0.0f;
    private float timeNotAttacking = 0.0f;

    private float base_pentagram_scale;
    private float base_fire_circle_scale;
    private float pentagram_scale_delta;
    private float fire_circle_scale_delta;
    private bool shouldChangePentagram = false;
    private float base_fire_circle_radius = 1.735f;

    private bool shouldShootForwardFireballs = false;

    private WaitForSeconds upperAttackCDTime;
    private WaitForSeconds darkAttackCDTime;
    public override void Awake()
    {
        base.Awake();

        livingFirePentagramSR = livingFirePentagram.GetComponent<SpriteRenderer>();
        livingFirePentagramSR.enabled = false;

        for (int i = 0; i < fire_list.Count; i++)
        {
            fire_SR_list.Add(fire_list[i].GetComponent<SpriteRenderer>());
            fire_SR_list[i].enabled = false;
        }
        
        darkLivingFire.SetActive(false);

        fireCircleSR = fireCircle.GetComponent<SpriteRenderer>();
        fireCircleSR.enabled = false;

        base_pentagram_scale = livingFirePentagram.localScale.x;
        base_fire_circle_scale = fireCircle.localScale.x;

        pentagram_scale_delta = (base_pentagram_scale - firePentagramMinScale) / firePentagramLifetime;
        fire_circle_scale_delta = (fireCircleMaxScale - base_fire_circle_scale) / fireCircleExplosionTime;

        upperAttackCDTime = new WaitForSeconds(upperAttackShootCD);
        darkAttackCDTime = new WaitForSeconds(darkFireballsShootCD);
        timeCloseToPlayer = closeAttackTimeToActivate; //not waiting after battle started
    }
    public override void Update()
    {
        base.Update();
        if (current_hp == 0)
        {
            if (!deathAnimIsTriggered)
            {
                deathAnimIsTriggered = true;
                x_direction = 0;
                
                //deactivate any additional objects
                livingFirePentagramSR.enabled = false;
                fireCircleSR.enabled = false;
                darkLivingFire.SetActive(false);
                for (int i = 0; i < fire_SR_list.Count; i++)
                {
                    fire_SR_list[i].enabled = false;
                }

                animator.Play(DEFEAT_STATE, 0, 0.0f);
            }
            return;
        }

        float distance = GameContext.playerPos.x - rb.position.x;
        if (canWalk)
        {
            //change direction based on player X pos
            if (Mathf.Abs(distance) <= closeAttackDistance)
                x_direction = 0f;
            else if (distance > 0)
            {
                x_direction = 1f;
                transform.localScale = new Vector3(base_x_scale, base_y_scale, 1.0f);
            }
            else
            {
                x_direction = -1f;
                transform.localScale = new Vector3(-base_x_scale, base_y_scale, 1.0f);
            }
        }
        else
        {
            x_direction = 0f;
            return;
        }

        if (currentAttackState != BossAttackState.isNone)
        {
            x_direction = 0f;
            if (currentAttackState == BossAttackState.isCLOSE_ATTACK)
                UpdateCloseAttack();
            else if (currentAttackState == BossAttackState.IS_UPPER_ATTACK)
                UpdateUpperAttack();
            else if (currentAttackState == BossAttackState.IS_FORWARD_ATTACK)
                UpdateForwardAttack();
            else if (currentAttackState == BossAttackState.IS_DARK_ATTACK)
                UpdateDarkAttack();
        }
        else
        {
            if (x_direction == 0f)
                animator.Play(IDLE_STATE);
            else
                animator.Play(WALK_STATE);
            timeNotAttacking += Time.deltaTime;
        }

        if (Mathf.Abs(distance) <= closeAttackDistance)
        {
            timeCloseToPlayer += Time.deltaTime;
            if(timeCloseToPlayer >= closeAttackTimeToActivate &&
                currentAttackState == BossAttackState.isNone
                )
            {
                //if player is close to boss and boss is not attacking, then close attack can be used
                //but only if player is more time close to boss than "closeAttackTimeToActivate" to prevent spamming close attack
                timeCloseToPlayer = 0f;
                timeNotAttacking = 0f;
                ActivateCloseAttack();
            }
        }
        else if(timeNotAttacking >= maxTimeWalking &&
            currentAttackState == BossAttackState.isNone)
        {
            ActivateDarkAttack();
            //use any other kind of attack if boss was not attacking long enough
            /*
            if(Random.Range(0, 2) == 0)
                ActivateUpperAttack();
            else 
                ActivateForwardAttack();
            */
            timeNotAttacking = 0f;
        }
    }
    public override void FixedUpdate()
    {
        rb.linearVelocityX = x_direction * speed + currentKnockbackForce;
    }

    private void ActivateUpperAttack()
    {
        currentAttackState = BossAttackState.IS_UPPER_ATTACK;
        timeUsingCurrentAttack = 0.0f;
        animator.Play(UPPER_ATTACK_STATE, 0, 0.0f);
        
        livingFirePentagram.localRotation = Quaternion.identity;
        livingFirePentagram.localPosition = pentagram_upper_pos;
        livingFirePentagramSR.enabled = true;
        for(int i = 0;i < fire_SR_list.Count;i++)
        {
            fire_SR_list[i].enabled = true;
        }
        StartCoroutine(UpperAttackShooter());
    }
    IEnumerator UpperAttackShooter()
    {
        int count = 0;
        while(count < 10 && current_hp != 0.0f)
        {
            yield return upperAttackCDTime;
            ShootFireball();
            count++;
        }
    }
    void ShootFireball()
    {
        if (current_hp == 0.0f) return;
        GameObject fireball = Instantiate(fireballPrefab);
        fireball.transform.position = livingFirePentagram.position;
        FireballProjectile fireballData = fireball.GetComponent<FireballProjectile>();
        fireballData.dmg = upperFireballsDamage;
        fireballData.speed = upperFireballsSpeed;
        Vector2 fireball_vec2_pos = fireball.transform.position;
        fireballData.SetVelocity((GameContext.playerPos - fireball_vec2_pos).normalized);
        AudioMixerManager.Instance.PlaySound(6);
    }
    void ShootDarkFireball()
    {
        if (current_hp == 0.0f) return;
        GameObject fireball = Instantiate(darkFireballPrefab);
        fireball.transform.position = livingFirePentagram.position;
        FireballProjectile fireballData = fireball.GetComponent<FireballProjectile>();
        fireballData.dmg = darkFireballsDamage;
        fireballData.speed = darkFireballsSpeed;
        Vector2 fireball_vec2_pos = fireball.transform.position;
        fireballData.SetVelocity((GameContext.playerPos - fireball_vec2_pos).normalized);
        AudioMixerManager.Instance.PlaySound(6);
    }
    private void UpdateUpperAttack()
    {
        timeUsingCurrentAttack += Time.deltaTime;
        if (timeUsingCurrentAttack >= upperAttackDuration)
        {
            DeactivateUpperAttack();
            return;
        }

        livingFirePentagram.Rotate(0, 0, Time.deltaTime * upperPentagramRotateSpeed, Space.Self);
        for (int i = 0; i < fire_pos_list.Count; i++)
        {
            fire_list[i].position = fire_pos_list[i].position;
        }
    }
    private void DeactivateUpperAttack()
    {
        currentAttackState = BossAttackState.isNone;
        timeUsingCurrentAttack = 0.0f;
        animator.Play(IDLE_STATE, 0, 0.0f);
        
        livingFirePentagramSR.enabled = false;
        for (int i = 0; i < fire_SR_list.Count; i++)
        {
            fire_SR_list[i].enabled = false;
        }
    }

    private void ActivateCloseAttack()
    {
        currentAttackState = BossAttackState.isCLOSE_ATTACK;
        timeUsingCurrentAttack = 0.0f;
        animator.Play(CLOSE_ATTACK_STATE, 0, 0.0f);
        
        livingFirePentagram.localPosition = pentagram_close_pos;
        livingFirePentagram.localRotation = Quaternion.identity;
        livingFirePentagramSR.enabled = true;
        shouldChangePentagram = true;
    }
    private void UpdateCloseAttack()
    {
        timeUsingCurrentAttack += Time.deltaTime;

        if (shouldChangePentagram)
        {
            //rotate and scale pentagram for a charge effect
            livingFirePentagram.Rotate(0, 0, Time.deltaTime * 180.0f, Space.Self);
            Vector3 pentagramLocalScale = livingFirePentagram.localScale;
            pentagramLocalScale.x -= pentagram_scale_delta * Time.deltaTime;
            pentagramLocalScale.y -= pentagram_scale_delta * Time.deltaTime;
            livingFirePentagram.localScale = pentagramLocalScale;

            if(pentagramLocalScale.x <= firePentagramMinScale)
            {
                livingFirePentagramSR.enabled = false;
                fireCircleSR.enabled = true;
                shouldChangePentagram = false;
            }
        }
        else
        {
            //scale fire circle for a fire burst effect
            Vector3 fireCircleLocalScale = fireCircle.localScale;
            fireCircleLocalScale.x += fire_circle_scale_delta * Time.deltaTime;
            fireCircleLocalScale.y += fire_circle_scale_delta * Time.deltaTime;
            fireCircle.localScale = fireCircleLocalScale;

            if (fireCircleLocalScale.x >= fireCircleMaxScale)
            {
                //do overlap and do dmg to player
                Collider2D hit = Physics2D.OverlapCircle(
                    fireCircle.position, 5.34f, playerMask);
                if (hit != null)
                {
                    hit.GetComponent<PlayerController>().Take_Damage(closeDamage);
                }

                fireCircleSR.enabled = false;
                DeactivateCloseAttack();
            }
        }
    }
    private void DeactivateCloseAttack()
    {
        timeUsingCurrentAttack = 0.0f;
        currentAttackState = BossAttackState.isNone;
        animator.Play(IDLE_STATE, 0, 0.0f);
        
        Vector3 pentagramLocalScale = livingFirePentagram.localScale;
        pentagramLocalScale.x = base_pentagram_scale;
        pentagramLocalScale.y = base_pentagram_scale;
        livingFirePentagram.localScale = pentagramLocalScale;

        Vector3 fireCircleLocalScale = fireCircle.localScale;
        fireCircleLocalScale.x = base_fire_circle_scale;
        fireCircleLocalScale.y = base_fire_circle_scale;
        fireCircle.localScale = fireCircleLocalScale;
    }

    private void ActivateForwardAttack()
    {
        timeUsingCurrentAttack = 0.0f;
        currentAttackState = BossAttackState.IS_FORWARD_ATTACK;
        shouldShootForwardFireballs = true;
        animator.Play(FORWARD_ATTACK_STATE, 0, 0.0f);
        
        livingFirePentagram.localPosition = pentagram_forward_pos;
        livingFirePentagram.localRotation = Quaternion.identity;
        livingFirePentagramSR.enabled = true;
    }
    private void UpdateForwardAttack()
    {
        timeUsingCurrentAttack += Time.deltaTime;
        livingFirePentagram.Rotate(0, 0, Time.deltaTime * 180.0f, Space.Self);

        if (shouldShootForwardFireballs && timeUsingCurrentAttack >= forwardAttackShootTimeOffset)
        {
            shouldShootForwardFireballs = false;
            //shoot fireballs based on player's distance to both borders of a boss arena
            Vector2 arenaLeftBorderPos = GameContext.bossArena.arenaLeftSide.position;
            Vector2 arenaRightBorderPos = GameContext.bossArena.arenaRightSide.position;
            float playerDistToLeftArenaSide = (GameContext.playerPos - arenaLeftBorderPos).magnitude;
            float playerDistToRightArenaSide = (GameContext.playerPos - arenaRightBorderPos).magnitude;
            Vector3 fireballsPos = Vector3.zero;
            Vector2 fireballsDirection = Vector2.zero;
            if (playerDistToLeftArenaSide > playerDistToRightArenaSide)
            {
                //shoot fireballs for left border to right side
                fireballsPos = GameContext.bossArena.arenaLeftSide.position;
                fireballsDirection = Vector2.right;
            }
            else
            {
                //shoot fireballs from right border to left side
                fireballsPos = GameContext.bossArena.arenaRightSide.position;
                fireballsDirection = Vector2.left;
            }
            for (int i = 0; i < 7; i++)
            {
                GameObject fireball = Instantiate(fireballPrefab);
                fireball.transform.position = fireballsPos;
                FireballProjectile fireballData = fireball.GetComponent<FireballProjectile>();
                fireballData.dmg = fireballsDamage;
                fireballData.speed = fireballsSpeed;
                fireballData.SetVelocity(fireballsDirection);
                fireballsPos.y += 1.0f;
            }
            //play fireballs sound
            AudioMixerManager.Instance.PlaySound(6);
        }

        if (timeUsingCurrentAttack >= forwardAttackDuration)
        {
            DeactivateForwardAttack();
        }
    }
    private void DeactivateForwardAttack()
    {
        timeUsingCurrentAttack = 0.0f;
        currentAttackState = BossAttackState.isNone;
        livingFirePentagramSR.enabled = false;
        animator.Play(IDLE_STATE, 0, 0.0f);
    }

    private void ActivateDarkAttack()
    {
        timeUsingCurrentAttack = 0.0f;
        currentAttackState = BossAttackState.IS_DARK_ATTACK;
        animator.Play(DARK_ATTACK_STATE, 0, 0.0f);

        darkLivingFire.SetActive(true);
        StartCoroutine(DarkAttackShooter());
    }
    IEnumerator DarkAttackShooter()
    {
        int count = 0;
        while (count < 10 && current_hp != 0.0f)
        {
            yield return darkAttackCDTime;
            ShootDarkFireball();
            count++;
        }
    }
    private void UpdateDarkAttack()
    {
        timeUsingCurrentAttack += Time.deltaTime;

        if (timeUsingCurrentAttack >= darkAttackDuration)
        {
            DeactivateDarkAttack();
            return;
        }
    }
    private void DeactivateDarkAttack()
    {
        timeUsingCurrentAttack = 0.0f;
        currentAttackState = BossAttackState.isNone;
        darkLivingFire.SetActive(false);
    }
}
