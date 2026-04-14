using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource musicSource;
    public AudioSource soundsSource;
    [Header("----Music----")]
    public AudioClip mainMusicClip;
    [Header("----Sounds----")]
    public List<AudioClip> sounds = new();

    public static AudioMixerManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource.clip = mainMusicClip;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float value)
    {
        float normalized = value / 100f;
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(normalized, 0.0001f)) * 20);
    }
    public void SetSoundsVolume(float value)
    {
        float normalized = value / 100f;
        mixer.SetFloat("SoundsVolume", Mathf.Log10(Mathf.Max(normalized, 0.0001f)) * 20);
    }
    public void PlayMusic()
    {
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySound(int sound_id)
    {
        soundsSource.PlayOneShot(sounds[sound_id]);
    }
}
