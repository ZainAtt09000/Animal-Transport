using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    public GameObject privacyPanel;

    private void Start()
    {
        if(PlayerPrefs.GetInt("PrivacyPrefs",0) == 0)
        {
            privacyPanel.SetActive(true);
        }
        else
        {
            Invoke(nameof(LoadMainMenuScene),2f);
       //     AdmobAdsManager.instance.InitializeAds();
        }
    }

    public void AcceptPrivacyPanel()
    {
        privacyPanel.SetActive(false);
        LoadMainMenuScene();
  //      AdmobAdsManager.instance.InitializeAds();
        PlayerPrefs.SetInt("PrivacyPrefs", 1);
    }

    public void RejectPrivacy()
    {
        Application.Quit();
    }

    public void OpenPrivacy()
    {
        Application.OpenURL("https://sites.google.com/view/epic-arcade-nitro-racing/home");
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(LoadGameplaySceneCoroutine());
    }

    public GameObject loadingScreen;
    public Slider loadingSlider;

    private IEnumerator LoadGameplaySceneCoroutine()
    {
        // Show the loading screen
        loadingScreen.SetActive(true);
        loadingSlider.value = 0;

        // Fake loading phase (3 seconds to 50%)
        float fakeLoadingDuration = 3f;
        float fakeProgress = 0f;

        while (fakeProgress < 0.5f)
        {
            fakeProgress += Time.deltaTime / fakeLoadingDuration;
            loadingSlider.value = fakeProgress;
            yield return null;
        }

        // Ensure slider is exactly at 50%
        loadingSlider.value = 0.5f;

        // Actual scene loading phase
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            // Update slider to progress from 50% to 100%
            float realProgress = Mathf.Lerp(0.5f, 1f, operation.progress / 0.9f);
            loadingSlider.value = realProgress;
            yield return null;
        }

        // Ensure the slider is fully filled before activating the scene
        loadingSlider.value = 1f;
        operation.allowSceneActivation = true;

        // Hide the loading screen after the scene is activated
     //   loadingScreen.SetActive(false);
    }


}
