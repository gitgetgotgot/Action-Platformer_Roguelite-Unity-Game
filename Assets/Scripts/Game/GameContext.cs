using System.Collections.Generic;
using UnityEngine;

public enum GameState { inGame, inPauseMenu, inSettings, inConstBuffs, inRunBasedBuffs, inInventory };
public static class GameContext
{
    public static GameState gameState = GameState.inGame;
    public static List<SaveData> savesDataList = new();
    public static SaveData activeSave = new();

    public static PlayerStats playerStats;
    public static HUD_Manager hudManager;
    public static ConstBuff selectedConstBuff;
    public static RunBasedBuff selectedRunBasedBuff;

    public static Inventory inventory;

    public static Vector2 playerPos = new();
    public static bool playerIsInDesignatedArea = false;
    public static bool attackOutsideAreaIsAllowed = true;

    public static int enemies_destroyed = 0;
    public static void Update_active_save(int saveDataIndex)
    {
        activeSave = savesDataList[saveDataIndex];
    }
}
