using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public Image optionImage;
        public Button optionBtn;
        public GameObject optionSettings;
    }
    [Header("Options Buttons")]
    public Settings audioSettings;
    public Settings controlSettings;
    public Settings graphicSettings;

    [Header("Audio")]
    public Slider musicSlider;
    public Slider buttonClickSlider;

    private void Awake()
    {
        LoadControlSettings();
        audioSettings.optionBtn.onClick.AddListener(() => OpenAudioSettings());
        controlSettings.optionBtn.onClick.AddListener(() => OpenControlSettings());
        graphicSettings.optionBtn.onClick.AddListener(() => OpenGraphicsSettings());

        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.MusicVolumeKey, 1f); // Default to 1 if not set
        buttonClickSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.ButtonVolumeKey, 1f); // Default to 1 if not set

        OpenAudioSettings();
    }

    public void OpenAudioSettings()
    {
        DisableAllOptions();
        audioSettings.optionImage.color = new Color(1f, 1f, 1f, 1f);
        audioSettings.optionSettings.SetActive(true);
    }

    public void OpenControlSettings()
    {
        DisableAllOptions();
        controlSettings.optionImage.color = new Color(1f, 1f, 1f, 1f);
        controlSettings.optionSettings.SetActive(true);
    }

    public void OpenGraphicsSettings()
    {
        DisableAllOptions();
        graphicSettings.optionImage.color = new Color(1f, 1f, 1f, 1f);
        graphicSettings.optionSettings.SetActive(true);
    }

    public void DisableAllOptions()
    {
        audioSettings.optionImage.color = new Color(1f, 1f, 1f, 0f);
        audioSettings.optionSettings.SetActive(false);
        controlSettings.optionImage.color = new Color(1f, 1f, 1f, 0f);
        controlSettings.optionSettings.SetActive(false);
        graphicSettings.optionImage.color = new Color(1f, 1f, 1f, 0f);
        graphicSettings.optionSettings.SetActive(false);
    }

    public void AdjustMusic()
    {
        AudioManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void AdjustButtonClickSound()
    {
        AudioManager.instance.SetButtonVolume(buttonClickSlider.value);
    }
    public Sprite selectedSprite;
    public Sprite unSelectedSprite;
    public Image ArrowBtn;
    public Image GyroBtn;
    public Image SteeringBtn;
    public void SelectArrows()
    {
        ArrowBtn.sprite = selectedSprite;
        GyroBtn.sprite = unSelectedSprite;
        SteeringBtn.sprite = unSelectedSprite;
        PlayerPrefs.SetInt("MobileControlsIndex", 0);
    }

    public void SelectGYro()
    {
        GyroBtn.sprite = selectedSprite;
        ArrowBtn.sprite = unSelectedSprite;
        SteeringBtn.sprite = unSelectedSprite;
        PlayerPrefs.SetInt("MobileControlsIndex", 1);
    }

    public void SelectSteering()
    {
        SteeringBtn.sprite = selectedSprite;
        GyroBtn.sprite = unSelectedSprite;
        ArrowBtn.sprite = unSelectedSprite;
        PlayerPrefs.SetInt("MobileControlsIndex", 2);
    }

    public void LoadControlSettings()
    {
        int control = PlayerPrefs.GetInt("MobileControlsIndex", 0);
        if (control == 0)
        {
            SelectArrows();
        }
        else if (control == 1)
        {
            SelectGYro();
        }
        else if (control == 2)
        {
            SelectSteering();
        }
    }

    public Sprite unselectedGraphic;
    public Sprite selectedGraphic;
    public Image lowGraphicBtn;
    public Image mediumGraphicBtn;
    public Image highGraphicBtn;

    public void UnselectGraphic(Image btn)
    {
        lowGraphicBtn.sprite = unselectedGraphic;
        mediumGraphicBtn.sprite = unselectedGraphic;
        highGraphicBtn.sprite = unselectedGraphic;


        btn.sprite = selectedGraphic;
    }
    public void SetGraphicsLow()
    {
        QualitySettings.SetQualityLevel(1); 
        UnselectGraphic(lowGraphicBtn);
    }

    public void SetGraphicsMedium()
    {
        QualitySettings.SetQualityLevel(2);
        UnselectGraphic(mediumGraphicBtn);
    }

    public void SetGraphicsHigh()
    {
        QualitySettings.SetQualityLevel(3);
        UnselectGraphic(highGraphicBtn);
    }
}
