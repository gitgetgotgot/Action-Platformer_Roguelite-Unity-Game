using UnityEngine;

public class DarkFireTrap : MonoBehaviour
{
    public float attack_cd = 2.0f;
    public float attack_max_cd_spread = 2.0f; //can attack each 2-4 seconds in default state
    public float projectile_dmg = 20.0f;
    public float projectile_speed = 10.0f;
    public float offset_y_target = 2.0f;
    public GameObject projectile_prefab = null;

    private float lastTimeShoot;
    private float current_cd;

    private void Start()
    {
        lastTimeShoot = Time.time;
        current_cd = attack_cd;
    }
    void Update()
    {
        if(Time.time - lastTimeShoot > current_cd)
        {
            lastTimeShoot = Time.time;
            current_cd = attack_cd + Random.Range(0.0f, attack_max_cd_spread);
            GameObject obj = Instantiate(projectile_prefab);
            obj.transform.position = transform.position;
            if( obj.TryGetComponent<FireballProjectile>(out var fireball))
            {
                fireball.dmg = projectile_dmg;
                fireball.speed = projectile_speed;
                Vector2 fireballPos = transform.position;
                Vector2 targetPos = GameContext.playerPos;
                targetPos.y += offset_y_target;
                fireball.SetVelocity((targetPos - fireballPos).normalized);
            }
        }
    }
}
