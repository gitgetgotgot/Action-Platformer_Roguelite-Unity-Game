using System.IO;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;
    public static string folderPath;
    private void Awake()
    {
        folderPath = Path.Combine(Application.persistentDataPath, "Saves");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSaves()
    {
        GameContext.savesDataList.Clear();

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string[] json_files = Directory.GetFiles(folderPath, "*.json");
        foreach (string file in json_files)
        {
            string save_json = File.ReadAllText(file);
            SaveData save_data = JsonUtility.FromJson<SaveData>(save_json);
            GameContext.savesDataList.Add(save_data);
        }
    }
    public void AddNewSave(string playerName)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        SaveData newData = new SaveData(playerName);
        string json_data = JsonUtility.ToJson(newData, true);
        string path = Path.Combine(folderPath, playerName + ".json");
        File.WriteAllText(path, json_data);
        GameContext.activeSave = newData;
    }

    public void ChangeSave()
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        SaveData data = GameContext.activeSave;
        PlayerStats playerStats = GameContext.playerStats;
        data.money = playerStats.money;
        data.xp = playerStats.current_xp;
        data.level = playerStats.level;
        GameContext.inventory.SaveArtifactsToSaveData();

        string json_data = JsonUtility.ToJson(data, true);
        string path = Path.Combine(folderPath, data.playerName + ".json");
        File.WriteAllText(path, json_data);
    }
}
