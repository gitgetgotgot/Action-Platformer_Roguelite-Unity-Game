using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float dmg = 10f;
    public float speedX = 3f;
    public bool destroyOnEnemyHit = true;
    public bool hasKnockback = false;
    public float knockbackForce = 0f;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = speedX;
        rb.linearVelocityY = 1f;
    }
    public void SetStartDirection(Vector2 direction)
    {
        rb.linearVelocityX = direction.x * speedX;
        rb.linearVelocityY = direction.y * speedX;
    }
    private void Update()
    {
        Vector2 velocity = rb.linearVelocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            bool isCrit = GameContext.playerStats.IsCritHit();
            //if arrow is not destroyed on enemy hit, then it's an ultimate magic arrow (not good code logic, but ok)
            float damage = GameContext.playerStats.GetMagicDamage(!destroyOnEnemyHit, isCrit);
            enemy.Take_damage(damage, PlayerAttackType.isMagicArrow);
            DamageTextPoolManager.instance.ActivateDamageText(damage, isCrit, enemy.gameObject.transform.position);
            if (hasKnockback)
            {
                if (GameContext.playerPos.x > transform.position.x)
                    enemy.ApplyKnockback(knockbackForce, false); //apply knockback to left side
                else
                    enemy.ApplyKnockback(knockbackForce, true);
            }
            if(destroyOnEnemyHit)
                Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
