using System.Collections.Generic;
using UnityEngine;

public class CastleArena : MonoBehaviour
{
    [Header("Fences")]
    public GameObject arenaLeftFence;
    public GameObject arenaRightFence;
    [Header("Traps")]
    public GameObject mace1_left;
    public GameObject mace1_right;
    public GameObject mace2;
    public GameObject darkFlame1;
    public GameObject darkFlame2;
    public GameObject darkFlame3;
    public GameObject attackArea;
    [Header("ArenaOptions")]
    public GameObject battleMark;
    public bool isArenaWithRandomConditions;
    public List<Transform> enemySpawnPositions = new();

    private bool isBattle = false;
    private int condition_id = 0;
    private const int ENEMIES_MAGIC_SHIELD_STREGTH = 2;
    private int waves_total = 0;
    private int wave_current = 0;
    private int wave_enemies_count = 0;
    //list of enemies for each wave
    private List<List<int>> enemy_id_list = new();
    void Start()
    {
        //hide arena blockades
        arenaLeftFence.SetActive(false);
        arenaRightFence.SetActive(false);
        //deactivate all conditions
        mace1_left.SetActive(false);
        mace1_right.SetActive(false);
        mace2.SetActive(false);
        darkFlame1.SetActive(false);
        darkFlame2.SetActive(false);
        darkFlame3.SetActive(false);
        attackArea.SetActive(false);
        //get condition
        if(isArenaWithRandomConditions)
        {
            //-enemies with magic shield(attack with magic bow to break shields)
            //-arena with dark flame traps
            //-arena with maces or flamethrower or both
            //-can deal dmg to enemies only in designated dynamic area(this area is marked)
            condition_id = Random.Range(1, 5); // [1, 4]
        }
    }

    public void StartArenaBattle()
    {
        //activate arena blockades and prepare waves
        isBattle = true;
        arenaLeftFence.SetActive(true);
        arenaRightFence.SetActive(true);
        battleMark.SetActive(false);
        GameContext.enemies_destroyed = 0;
        
        if(GameContext.activeSave.active_room == 1)
        {
            waves_total = 3;
            wave_current = 0;
            enemy_id_list.Add(new List<int>());
            enemy_id_list.Add(new List<int>());
            enemy_id_list.Add(new List<int>());
            List<int> list = enemy_id_list[0];
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list = enemy_id_list[1];
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list = enemy_id_list[2];
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(1, 3));
            list.Add(Random.Range(1, 3));

        }
        else if (GameContext.activeSave.active_room == 2)
        {
            waves_total = 3;
            wave_current = 0;
            enemy_id_list.Add(new List<int>());
            enemy_id_list.Add(new List<int>());
            enemy_id_list.Add(new List<int>());
            List<int> list = enemy_id_list[0];
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(0, 2));
            list = enemy_id_list[1];
            list.Add(Random.Range(0, 3));
            list.Add(Random.Range(2, 4));
            list.Add(Random.Range(2, 4));
            list = enemy_id_list[2];
            list.Add(Random.Range(0, 2));
            list.Add(Random.Range(3, 5));
            list.Add(Random.Range(3, 5));
        }
        else if(GameContext.activeSave.active_room > 2)
        {
            waves_total = 1;
            wave_current = 0;
            enemy_id_list.Add(new List<int>());
            List<int> list = enemy_id_list[0];
            list.Add(4);
            list.Add(4);
        }

        SpawnWaveEnemies();
        if(condition_id > 0)
            ActivateCondition();
    }
    public void StopArenaBattle()
    {
        isBattle = false;
        arenaLeftFence.SetActive(false);
        arenaRightFence.SetActive(false);
        if (condition_id > 0)
            DeactivateCondition();

    }
    private void SpawnWaveEnemies()
    {
        List<int> waveList = enemy_id_list[wave_current];
        wave_enemies_count = waveList.Count;
        for (int i = 0; i < waveList.Count; i++)
        {
            GameObject enemy = LevelManager.Instance.SpawnEnemyById(waveList[i]);
            enemy.transform.position = enemySpawnPositions[i].position;
            if (condition_id == 1)
                enemy.GetComponent<Enemy>().ProtectByMagicShield(ENEMIES_MAGIC_SHIELD_STREGTH);
        }
    }
    void Update()
    {
        if (isBattle)
        {   
            
            //if all enemies of current wave are destroyed then try next wave
            if(GameContext.enemies_destroyed == wave_enemies_count)
            {
                wave_current++;
                //stop battle if all waves are complete
                if (wave_current == waves_total)
                {
                    StopArenaBattle();
                    isBattle = false;
                    return;
                }
                SpawnWaveEnemies();
                GameContext.enemies_destroyed = 0;
            }
        }
    }

    private void ActivateCondition()
    {
        if (condition_id == 1)
        {
            
        }
        if (condition_id == 2)
        {
            darkFlame1.SetActive(true);
            darkFlame2.SetActive(true);
            darkFlame3.SetActive(true);
        }
        if (condition_id == 3)
        {
            mace1_left.SetActive(true);
            mace1_right.SetActive(true);
            mace2.SetActive(true);
        }
        if (condition_id == 4)
        {
            attackArea.SetActive(true);
            GameContext.attackOutsideAreaIsAllowed = false;
        }
    }
    private void DeactivateCondition()
    {
        if (condition_id == 1)
        {

        }
        if (condition_id == 2)
        {
            darkFlame1.SetActive(false);
            darkFlame2.SetActive(false);
            darkFlame3.SetActive(false);
        }
        if (condition_id == 3)
        {
            mace1_left.SetActive(false);
            mace1_right.SetActive(false);
            mace2.SetActive(false);
        }
        if (condition_id == 4)
        {
            attackArea.SetActive(false);
            GameContext.attackOutsideAreaIsAllowed = true;
        }
    }
}
