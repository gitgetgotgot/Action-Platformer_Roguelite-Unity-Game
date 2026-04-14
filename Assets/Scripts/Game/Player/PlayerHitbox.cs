using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private PlayerController playerController = null;
    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BuffMark"))
        {
            LevelManager.Instance.GiveNewRunBasedBuff();
        }
        else if (collision.CompareTag("BattleMark"))
        {
            CastleArena arena = collision.GetComponentInParent<CastleArena>();
            arena.StartArenaBattle();
        }
        else if (collision.CompareTag("ShopMark"))
        {
            playerController.SetActiveShopName(collision.gameObject.name);
            LevelManager.Instance.ShowShopArtifact(collision.gameObject.name);
        }
        else if (collision.CompareTag("Finish"))
        {
            LevelManager.Instance.OnLevelFinished();
        }
        else if (collision.CompareTag("EnemyAttack"))
        {
            //get Enemy script in parent of attackCollider
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
            playerController.Take_Damage(enemy.dmg);
        }
        else if (collision.CompareTag("Mace_Trap"))
        {
            MaceTrap maceTrap = collision.gameObject.GetComponentInParent<MaceTrap>();
            playerController.Take_Damage(maceTrap.DMG);
            if (collision.gameObject.transform.position.x > transform.position.x)
                playerController.ApplyKnockback(5f, false);
            else
                playerController.ApplyKnockback(5f, true);
        }
        else if (collision.CompareTag("Flame_Trap"))
        {
            Flamethrower flamethrower = collision.gameObject.GetComponentInParent<Flamethrower>();
            playerController.Take_Damage(flamethrower.DMG);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ShopMark"))
        {
            //LevelManager.Instance.LeaveShopArtifact();
            playerController.SetActiveShopName("");
            playerController.hud_Manager.HideShopArtifactTooltip();
        }
        else if (collision.CompareTag("AttackArea"))
        {
            GameContext.playerIsInDesignatedArea = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyHitbox"))
        {
            playerController.Take_Damage(collision.gameObject.GetComponentInParent<Enemy>().dmg);
        }
        else if (collision.CompareTag("AttackArea"))
        {
            GameContext.playerIsInDesignatedArea = true;
        }
    }
}
