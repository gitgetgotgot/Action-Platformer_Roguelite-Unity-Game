using UnityEngine;

public class SkeletonArrow : MonoBehaviour
{
    public float dmg = 10f;
    public float speedX = 3f;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.Take_Damage(dmg);
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
