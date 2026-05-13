using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public static SettingsScript instance;
    public Button fullscreenButton;
    public TMP_Text fullscreenText;
    public Button vsyncButton;
    public TMP_Text vsyncText;
    public Slider musicSlider;
    public Slider soundsSlider;
    public TMP_Dropdown resolutionsDropdown;

    private int current_res_index;

    void Awake()
    {
        instance = this;
        LoadSettings();
        LoadResolutionsToDropdown();
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
        vsyncButton.onClick.AddListener(ToggleVsync);

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        soundsSlider.onValueChanged.AddListener(ChangeSoundsVolume);
    }

    public void LoadSettings()
    {
        if (GameSettings.isFullscreen) fullscreenText.text = "Fullscreen: ON";
        else fullscreenText.text = "Fullscreen: OFF";

        if (GameSettings.VsyncOn) vsyncText.text = "Vsync: ON";
        else vsyncText.text = "Vsync: OFF";

        musicSlider.value = GameSettings.MusicVolume;
        soundsSlider.value = GameSettings.SoundsVolume;
    }
    public void UpdateResolutionInDropdown()
    {
        resolutionsDropdown.value = GameSettings.active_resolution_index;
        ChangeResolution(GameSettings.active_resolution_index);
    }
    private void LoadResolutionsToDropdown()
    {
        List<string> resList = new();
        int active_res_index = 0;
        for(int i = 0; i < GameSettings.resolutions.Length; i++)
        {
            Resolution resolution = GameSettings.resolutions[i];
            if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height) active_res_index = i;
            resList.Add(resolution.width + "x" + resolution.height);
        }
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(resList);
        resolutionsDropdown.onValueChanged.AddListener(ChangeResolution);
        resolutionsDropdown.value = active_res_index;
    }
    private void ToggleFullscreen()
    {
        if (GameSettings.isFullscreen)
        {
            Screen.fullScreen = false;
            GameSettings.isFullscreen = false;
            fullscreenText.text = "Fullscreen: OFF";
        }
        else
        {
            Screen.fullScreen = true;
            GameSettings.isFullscreen = true;
            fullscreenText.text = "Fullscreen: ON";
        }
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void ToggleVsync()
    {
        if (GameSettings.VsyncOn)
        {
            QualitySettings.vSyncCount = 0;
            GameSettings.VsyncOn = false;
            vsyncText.text = "Vsync: OFF";
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            GameSettings.VsyncOn = true;
            vsyncText.text = "Vsync: ON";
        }
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void ChangeMusicVolume(float volume)
    {
        GameSettings.MusicVolume = (int)volume;
        AudioMixerManager.Instance.SetMusicVolume(volume);
    }
    private void ChangeSoundsVolume(float volume)
    {
        GameSettings.SoundsVolume = (int)volume;
        AudioMixerManager.Instance.SetSoundsVolume(volume);
    }
    private void ChangeResolution(int index)
    {
        current_res_index = index;
        Resolution res = GameSettings.resolutions[current_res_index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
