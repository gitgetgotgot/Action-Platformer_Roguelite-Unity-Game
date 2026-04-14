using UnityEngine;

public static class GameSettings
{
    //screen
    public static Resolution[] resolutions = Screen.resolutions;
    public static bool isFullscreen = Screen.fullScreen;
    public static bool VsyncOn = QualitySettings.vSyncCount > 0;
    //audio
    public static int MusicVolume = 0;
    public static int SoundsVolume = 0;
}
