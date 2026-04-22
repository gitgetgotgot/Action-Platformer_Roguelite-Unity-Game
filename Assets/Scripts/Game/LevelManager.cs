using System.Collections;
using System.Collections.Generic;
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
    int artifact1_id = 1;
    int artifact2_id = 2;
    int artifact3_id = 3;
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
            if (artifact1_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(artifact1_id), 30);
        }
        else if (shopMarkName == "ShopMark2")
        {
            if (artifact2_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(artifact2_id), 30);
        }
        else if (shopMarkName == "ShopMark3")
        {
            if (artifact3_id == 0) return;
            GameContext.hudManager.UpdateShopArtifactTooltip(ArtifactsManager.Instance.GetArtifact(artifact3_id), 30);
        }
        GameContext.hudManager.ShowShopArtifactTooltip();
    }

    public void BuyShopArtifact(string shopMarkName)
    {
        if (shopMarkName == "ShopMark1")
        {
            GameContext.inventory.AddArtifact(artifact1_id);
            artifact1_id = 0;
        }
        else if (shopMarkName == "ShopMark2")
        {
            GameContext.inventory.AddArtifact(artifact2_id);
            artifact2_id = 0;
        }
        else if (shopMarkName == "ShopMark3")
        {
            GameContext.inventory.AddArtifact(artifact3_id);
            artifact3_id = 0;
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
    public void UpdateAvailableBuffs()
    {
        int id1, id2, id3;
        if (GameContext.activeSave.active_room == 1)
        {
            id1 = 16; id2 = 19; id3 = 12;
        }
        else if (GameContext.activeSave.active_room == 2)
        {
            id1 = 31; id2 = 28; id3 = 6;
        }
        else if(GameContext.activeSave.active_room == 3)
        {
            id1 = 3; id2 = 9; id3 = 19;
        }
        else
        {
            id1 = 16; id2 = 19; id3 = 12;
        }
        //choose appropriate buffs
        
        //put them on the page
        RunBasedBuffsPage.Instance.UpdateAvailableRunBuffs(id1, id2, id3);
    }
    public void SpawnEnemy(int id)
    {
        Instantiate(enemyPrefabs[id]).transform.position = GameContext.playerPos + new Vector2(2f, 2f);
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
    }
    public void OnLevelFinished()
    {
        StartCoroutine(FinishLevel());
    }

    public IEnumerator FinishLevel()
    {
        //wait for 1 second when player finishes level
        yield return new WaitForSeconds(1f);
        //show black screen with "saving" label
        backgroundCanvas.enabled = true;
        yield return null;
        UpdateLevel();
        //apply different buffs for player after level is finished (f.e. HP_Regen)
        GameContext.playerStats.UpdateStatsOnLevelFinished();
        GameContext.activeSave.active_room++;
        SavesManager.Instance.ChangeSave();
        yield return new WaitForSeconds(1f);
        backgroundCanvas.enabled = false;
        playerCanActivateRunBasedBuff = true;
    }
}
