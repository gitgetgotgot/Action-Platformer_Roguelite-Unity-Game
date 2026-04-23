using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GameMenuScript gameMenu;
    public GameObject LevelObjectsHolder;
    public GameObject player;
    public Canvas backgroundCanvas;
    public List<GameObject> enemyPrefabs;
    
    public EnemyManager enemyManager;
    public LevelGenerator levelGenerator;

    private bool playerCanActivateRunBasedBuff = false;

    //shop
    int shop_artifact1_id = 1;
    int shop_artifact2_id = 2;
    int shop_artifact3_id = 3;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            UpdateLevel();
            playerCanActivateRunBasedBuff = true;
            backgroundCanvas.enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
        GameContext.gameState = GameState.inGame;
    }

    private void Start()
    {
        AudioMixerManager.Instance.PlayMusic();
        //UpdateLevel();
    }

    public void ShowShopArtifact(string shopMarkName)
    {
        if (shopMarkName == "ShopMark1")
        {
            if (shop_artifact1_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(shop_artifact1_id), 30);
        }
        else if (shopMarkName == "ShopMark2")
        {
            if (shop_artifact2_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(shop_artifact2_id), 30);
        }
        else if (shopMarkName == "ShopMark3")
        {
            if (shop_artifact3_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(shop_artifact3_id), 30);
        }
        GameContext.hudManager.ShowShopArtifactTooltip();
    }

    public void BuyShopArtifact(string shopMarkName)
    {
        if (shopMarkName == "ShopMark1")
        {
            GameContext.inventory.AddArtifact(shop_artifact1_id);
            shop_artifact1_id = 0;
        }
        else if (shopMarkName == "ShopMark2")
        {
            GameContext.inventory.AddArtifact(shop_artifact2_id);
            shop_artifact2_id = 0;
        }
        else if (shopMarkName == "ShopMark3")
        {
            GameContext.inventory.AddArtifact(shop_artifact3_id);
            shop_artifact3_id = 0;
        }
    }
    public void GiveNewRunBasedBuff()
    {
        //give player new run-based buff when player collides with buff mark
        if (playerCanActivateRunBasedBuff) //only if player hasn't collected the buff on this level yet
        {
            playerCanActivateRunBasedBuff = false;
            UpdateAvailableBuffs();
            gameMenu.OpenBuffsPage();
            GameContext.gameState = GameState.inRunBasedBuffs;
        }
    }
    public void UpdateAvailableShopArtifacts()
    {
        int[] ids = { 1, 2, 4, 5, 7, 8, 10, 11}; //any except 3, 6, 9 and 12
        // Shuffle
        for (int i = 7; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }
        shop_artifact1_id = ids[0];
        shop_artifact2_id = ids[1];
        shop_artifact3_id = ids[2];
    }
    public void UpdateAvailableBuffs()
    {
        //get 3 available buffs
        int max_id = 30;
        int id1, id2, id3;
        List<uint> playerBuffs = GameContext.activeSave.runBuffs;
        HashSet<int> available_ids = new HashSet<int>();

        for (int i = 0; i <= max_id; i++)
            available_ids.Add(i);

        foreach (uint id in playerBuffs)
            available_ids.Remove((int)id);

        List<int> list = available_ids.ToList();

        // Shuffle
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        id1 = list[0];
        id2 = list[1];
        id3 = list[2];


        //put them on the page
        RunBasedBuffsPage.Instance.UpdateAvailableRunBuffs(id1, id2, id3);
    }
    public void SpawnEnemy(int id)
    {
        Vector3 startPos = GameContext.playerPos;
        Instantiate(enemyPrefabs[id]).transform.position = startPos + new Vector3(2f, 2f, -0.1f);
    }
    public GameObject SpawnEnemyById(int enemy_id)
    {
        return Instantiate(enemyPrefabs[enemy_id]);
    }
    void UpdateLevel()
    {
        //clear all segments
        foreach (Transform segment in LevelObjectsHolder.transform)
            Destroy(segment.gameObject);
        //generate new level with new segments and get start pos for player
        Vector3 playerPos = levelGenerator.GenerateLevel(LevelObjectsHolder.transform);
        playerPos.z = -0.1f;
        player.transform.position = playerPos;
        //update shop artifacts (even if there is no shop :) )
        UpdateAvailableShopArtifacts();
        Debug.Log("Current room = " + GameContext.activeSave.active_room.ToString());
    }
    public void OnLevelFinished()
    {
        StartCoroutine(FinishLevel());
    }

    public IEnumerator FinishLevel()
    {
        GameContext.activeSave.active_room++;
        //wait for 1 second when player finishes level
        yield return new WaitForSeconds(1f);
        //show black screen with "saving" label
        backgroundCanvas.enabled = true;
        yield return null;
        UpdateLevel();
        //apply different buffs for player after level is finished (f.e. HP_Regen)
        GameContext.playerStats.UpdateStatsOnLevelFinished();
        SavesManager.Instance.ChangeSave();
        yield return new WaitForSeconds(1f);
        backgroundCanvas.enabled = false;
        playerCanActivateRunBasedBuff = true;
    }
}
