using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip musicClip;
    public AudioClip buttonClip;
    public static AudioManager instance;

    private float musicVolume = 1f;
    private float buttonVolume = 1f;

    [HideInInspector]public string MusicVolumeKey = "MusicVolume";
    [HideInInspector]public string ButtonVolumeKey = "ButtonVolume";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings(); // Load saved volume settings
        }
    }

    private void Start()
    {
        PlayMusic(); // Play background music on start
    }

    public void PlayMusic()
    {
        if (audioSource != null && musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.volume = musicVolume;
            audioSource.Play();
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClip != null)
        {
            if(Camera.main!= null)
            {
                AudioSource.PlayClipAtPoint(buttonClip, Camera.main.transform.position, buttonVolume);
            }
            else
            audioSource.PlayOneShot(buttonClip);
         //   AudioSource.PlayClipAtPoint(buttonClip, Camera.main.transform.position, buttonVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
        SaveVolumeSettings(); // Save the new music volume
    }

    public void SetButtonVolume(float volume)
    {
        buttonVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings(); // Save the new button volume
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(ButtonVolumeKey, buttonVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f); // Default to 1 if not set
        buttonVolume = PlayerPrefs.GetFloat(ButtonVolumeKey, 1f); // Default to 1 if not set
    }
}
