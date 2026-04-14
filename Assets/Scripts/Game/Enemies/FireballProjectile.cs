using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float dmg = 10f;
    public float speed = 3f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 direction)
    {
        //rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //velocity
        velocity = direction * speed;
    }
    private void Update()
    {
        rb.linearVelocityX = velocity.x;
        rb.linearVelocityY = velocity.y;
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
