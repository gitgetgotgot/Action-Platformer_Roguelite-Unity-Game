using System.IO;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;
    public static string savesFolderPath;
    public static string settingsSavePath;
    public static string settingsSaveFilePath;
    private void Awake()
    {
        savesFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
        settingsSavePath = Path.Combine(Application.persistentDataPath, "Options");
        settingsSaveFilePath = Path.Combine(settingsSavePath, "options.json");
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
    private void Start()
    {
        LoadGameSettings();
    }
    public void LoadSaves()
    {
        GameContext.savesDataList.Clear();

        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);

        string[] json_files = Directory.GetFiles(savesFolderPath, "*.json");
        foreach (string file in json_files)
        {
            string save_json = File.ReadAllText(file);
            SaveData save_data = JsonUtility.FromJson<SaveData>(save_json);
            GameContext.savesDataList.Add(save_data);
        }
    }
    public void AddNewSave(string playerName)
    {
        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);

        SaveData newData = new SaveData(playerName);
        string json_data = JsonUtility.ToJson(newData, true);
        string path = Path.Combine(savesFolderPath, playerName + ".json");
        File.WriteAllText(path, json_data);
        GameContext.activeSave = newData;
    }

    public void ChangeSave()
    {
        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);

        SaveData data = GameContext.activeSave;
        PlayerStats playerStats = GameContext.playerStats;
        data.money = playerStats.money;
        data.xp = playerStats.current_xp;
        data.level = playerStats.level;
        GameContext.inventory.SaveArtifactsToSaveData();

        string json_data = JsonUtility.ToJson(data, true);
        string path = Path.Combine(savesFolderPath, data.playerName + ".json");
        File.WriteAllText(path, json_data);
    }

    public void DeleteSave(string playerName)
    {
        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);

        string path = Path.Combine(savesFolderPath, playerName + ".json");
        File.Delete(path);
    }

    public void LoadGameSettings()
    {
        if(GameSettings.firstInitIsDone) return;

        GameSettings.firstInitIsDone = true;

        if (!Directory.Exists(settingsSavePath))
            Directory.CreateDirectory(settingsSavePath);

        if (!File.Exists(settingsSaveFilePath))
        {
            int active_res_index = 0;
            for (int i = 0; i < GameSettings.resolutions.Length; i++)
            {
                Resolution resolution = GameSettings.resolutions[i];
                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height)
                {
                    active_res_index = i;
                    break;
                }
            }
            GameSettingsDTO gameSettings = new()
            {
                isFullscreen = GameSettings.isFullscreen,
                VsyncOn = GameSettings.VsyncOn,
                active_resolution_index = active_res_index,
                MusicVolume = GameSettings.MusicVolume,
                SoundsVolume = GameSettings.SoundsVolume
            };
            string json = JsonUtility.ToJson(gameSettings);
            File.WriteAllText(settingsSaveFilePath, json);
        }

        string json_data = File.ReadAllText(settingsSaveFilePath);
        GameSettingsDTO gameSettingsDTO = JsonUtility.FromJson<GameSettingsDTO>(json_data);
        GameSettings.isFullscreen = gameSettingsDTO.isFullscreen;
        GameSettings.VsyncOn = gameSettingsDTO.VsyncOn;
        GameSettings.active_resolution_index = gameSettingsDTO.active_resolution_index;
        GameSettings.MusicVolume = gameSettingsDTO.MusicVolume;
        GameSettings.SoundsVolume = gameSettingsDTO.SoundsVolume;

        //apply settings
        Screen.fullScreen = GameSettings.isFullscreen;
        if(GameSettings.VsyncOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        AudioMixerManager.Instance.SetMusicVolume(GameSettings.MusicVolume);
        AudioMixerManager.Instance.SetSoundsVolume(GameSettings.SoundsVolume);
        Resolution res = GameSettings.resolutions[GameSettings.active_resolution_index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        SettingsScript.instance.LoadSettings();
        SettingsScript.instance.UpdateResolutionInDropdown();
    }
    public void SaveGameSettings()
    {
        if (!Directory.Exists(settingsSavePath))
            Directory.CreateDirectory(settingsSavePath);

        GameSettingsDTO gameSettings = new()
        {
            isFullscreen = GameSettings.isFullscreen,
            VsyncOn = GameSettings.VsyncOn,
            active_resolution_index = GameSettings.active_resolution_index,
            MusicVolume = GameSettings.MusicVolume,
            SoundsVolume = GameSettings.SoundsVolume
        };
        string json = JsonUtility.ToJson(gameSettings);
        File.WriteAllText(settingsSaveFilePath, json);
    }
}
