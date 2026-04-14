using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Enemy enemyController;
    private void Start()
    {
        enemyController = GetComponentInParent<Enemy>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            enemyController.Take_damage(25f, PlayerAttackType.isSword);
            if(GameContext.playerPos.x > transform.position.x)
            {
                //apply knockback to left side
                enemyController.ApplyKnockback(4f, false);
            }
            else
            {
                enemyController.ApplyKnockback(2f, true);
            }
        }
        if (collision.CompareTag("Mace_Trap") && !enemyController.isGhost)
        {
            MaceTrap maceTrap = collision.gameObject.GetComponentInParent<MaceTrap>();
            enemyController.Take_damage(maceTrap.DMG, PlayerAttackType.isSword);
            
            if(collision.gameObject.transform.position.x > transform.position.x)
                enemyController.ApplyKnockback(5f, false);
            else
                enemyController.ApplyKnockback(5f, false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
