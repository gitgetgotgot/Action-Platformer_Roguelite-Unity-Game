using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public SaveData() {}
    public SaveData(string playerName) {
        this.playerName = playerName;
    }
    public string playerName;
    public int level = 0, money = 0, xp = 0;
    public int active_room = 1;
    //lists contain identificators
    public List<uint> constBuffs = new();
    public List<uint> runBuffs = new();
    public List<uint> inventory_unactive_items = new() { 0, 0, 0, 0, 0, 0, 0, 0 }; //8 default empty slots
    public List<uint> inventory_active_items = new() { 0, 0, 0, 0 }; //4 default empty slots
}
