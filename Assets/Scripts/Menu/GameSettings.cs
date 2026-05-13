using UnityEngine;
public static class GameSettings
{
    public static bool firstInitIsDone = false;
    //screen
    public static Resolution[] resolutions = Screen.resolutions;
    public static bool isFullscreen = Screen.fullScreen;
    public static bool VsyncOn = QualitySettings.vSyncCount > 0;
    public static int active_resolution_index = 0;
    //audio
    public static int MusicVolume = 0;
    public static int SoundsVolume = 0;
}

public class GameSettingsDTO
{
    public bool isFullscreen;
    public bool VsyncOn;
    public int active_resolution_index;
    public int MusicVolume;
    public int SoundsVolume;
}
