using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float dmg = 10f;
    public float speedX = 3f;
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
            enemy.Take_damage(dmg, PlayerAttackType.isMagicArrow);
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
