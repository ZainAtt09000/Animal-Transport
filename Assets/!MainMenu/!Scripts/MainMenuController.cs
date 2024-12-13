using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // ========== Variables ==========
    #region Variables

    // Main Menu and Screens
    public GameObject mainMenu;
    [Header("Screens")]
    public GameObject mainMenuScreen;
    public GameObject dailyTasksScreen;
    public GameObject garageScreen;
    public GameObject storeScreen;
    public GameObject modeSelectionScreen;
    public GameObject levelSelectionScreen;

    // Panels
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject exitPanel;
    public GameObject dailyRewardPanel;
    public GameObject userPanel;
    public GameObject characterSelectionPanel;

    // Loading Screens
    [System.Serializable]
    public class LoadingScreen
    {
        public GameObject loadingScreen;
        public Slider loadingSlider;
        public float loadingTime;
    }
    [Header("Loading Screens")]
    public LoadingScreen fakeLoadingScreen_1;
    public LoadingScreen fakeLoadingScreen_2;
    public LoadingScreen loadToGameplayScene;

    private Stack<GameObject> screenHistory = new Stack<GameObject>(); // Stack to store screen history

    [Header("Currency Setup")]
    public Text goldText;
  //  public Text diamondText;

    private int goldAmount;
  //  private int diamondAmount;

    [Header("Mode Selection")]
    public Mode wildAnimalModeLevels;
    public Mode farmAnimalModeLevels;
    public Mode parkingModeLevels;
    

    [System.Serializable]
    public class Chapter
    {
        public Button chapterSelectionButton;
        public GameObject chapter;
    }


    [System.Serializable]
    public class Levels
    {
        public GameObject completeLevel;
        public GameObject lockedLevel;
        public GameObject unlockedLevel;
        public bool isUnlocked;
    }

    [System.Serializable]
    public class Mode
    {
        public Button modeButton;
        public GameObject levelsObject;
        public Levels[] levels;
        public Image modeLevelProgressFillImage;
        public Text modeLevelProgressText;
    }

    #endregion

    public static MainMenuController instance;


    #region Level Selection
  
    public void SelectLevel(int levelNumber)
    {
        string currentMode = PlayerPrefs.GetString("CurrentMode", "RaceTrack");

        // Get the highest unlocked level for the current mode
        int unlockedLevel = PlayerPrefs.GetInt($"{currentMode}_NewLevel", 0);

        // Validate selected level number
        if (levelNumber > unlockedLevel)
        {
            Debug.LogWarning($"Level {levelNumber} is locked. You can only select up to level {unlockedLevel}.");
            return; // Prevent selecting a locked level
        }

        // Save the selected level
        PlayerPrefs.SetInt($"{currentMode}_Level", levelNumber);

        // Start the game at the selected level
        PlayGame();
    }

    public void WildAnimalLevels()
    {
        SetMode("Wild", wildAnimalModeLevels);
    }

    public void FarmModeLevels()
    {
        SetMode("Farm", farmAnimalModeLevels);
    }

    public void ParkingModeLevels()
    {
        SetMode("Park", parkingModeLevels);
    }

    private void SetMode(string modeKey, Mode mode)
    {
        OpenLevelSelection();
        ActivateMode(mode);
        PlayerPrefs.SetString("CurrentMode", modeKey);

        UpdateLevelUI(mode, modeKey);
    }

    private void ActivateMode(Mode activeMode)
    {
        wildAnimalModeLevels.levelsObject.SetActive(activeMode == wildAnimalModeLevels);
        farmAnimalModeLevels.levelsObject.SetActive(activeMode == farmAnimalModeLevels);
        parkingModeLevels.levelsObject.SetActive(activeMode == parkingModeLevels);
      
    }

    public void CheckCompletedLevelsForMode(Mode mode, string modeKey)
    {
        // Fetch the current level and highest completed level for the mode
        int currentLevel = PlayerPrefs.GetInt($"{modeKey}_NewLevel", 0);
        int highestCompletedLevel = PlayerPrefs.GetInt($"{modeKey}_HighestLevel", -1);

        int unlockedLevels = 0;

        for (int i = 0; i < mode.levels.Length; i++)
        {
            if (i <= highestCompletedLevel) // Completed levels
            {
                mode.levels[i].isUnlocked = true;
                unlockedLevels++;
            }
            else if (i == currentLevel) // Current unlocked level
            {
                mode.levels[i].isUnlocked = false;
              //  unlockedLevels++;
            }
            else // Locked levels
            {
                mode.levels[i].isUnlocked = false;
            }
        }

        // Calculate the progress fill amount (0 to 1)
        float fillAmount = (float)unlockedLevels / mode.levels.Length;

        // Update the progress bar fill image
        mode.modeLevelProgressFillImage.fillAmount = fillAmount;

        // Update the progress text
        mode.modeLevelProgressText.text = $"{unlockedLevels}/{mode.levels.Length}";
    }



    private void UpdateLevelUI(Mode mode, string modeKey)
    {
        // Fetch the current level for the mode (the next level to unlock)
        int currentLevel = PlayerPrefs.GetInt($"{modeKey}_NewLevel", 0);
        // Fetch the highest completed level for the mode
        int highestCompletedLevel = PlayerPrefs.GetInt($"{modeKey}_HighestLevel", -1);

        for (int i = 0; i < mode.levels.Length; i++)
        {
            if (i <= highestCompletedLevel) // Completed levels
            {
                mode.levels[i].completeLevel.SetActive(true);
                mode.levels[i].isUnlocked = true;
                mode.levels[i].lockedLevel.SetActive(false);
                mode.levels[i].unlockedLevel.SetActive(false);
            }
            else if (i == currentLevel) // Current unlocked level
            {
                mode.levels[i].completeLevel.SetActive(false);
                mode.levels[i].lockedLevel.SetActive(false);
                mode.levels[i].unlockedLevel.SetActive(true);
            }
            else // Locked levels
            {
                mode.levels[i].completeLevel.SetActive(false);
                mode.levels[i].lockedLevel.SetActive(true);
                mode.levels[i].unlockedLevel.SetActive(false);
            }
        }
    }


    #endregion

    #region Currency Setup
    public void UpdateCurrency()
    {
        goldAmount = PlayerPrefs.GetInt("PlayerGold", 0);
       // diamondAmount = PlayerPrefs.GetInt("PlayerDiamond", 0);

        goldText.text = goldAmount.ToString();
      //  diamondText.text = diamondAmount.ToString();
    }

    #endregion

    // ========== Main Menu Controls ==========
    #region Main Menu Controls

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        UpdateCurrency();

        OpenMainMenu();

    }

    private void Start()
    {
        UpdateCurrency();
      //  parkingModeLevels.modeButton.onClick.AddListener(() => ParkingModeLevels());
      //  wildAnimalModeLevels.modeButton.onClick.AddListener(() => WildAnimalLevels());
     //   farmAnimalModeLevels.modeButton.onClick.AddListener(() => FarmModeLevels());
       
     //    CheckCompletedLevelsForMode(wildAnimalModeLevels, "GoldRaces");
    //    CheckCompletedLevelsForMode(farmAnimalModeLevels, "DiamondRaces");
    }

    public void OpenMainMenu()
    {
        CloseAllScreens();
        mainMenu.SetActive(true);
        mainMenuScreen.SetActive(true);
        screenHistory.Clear();  // Clear history when returning to main menu
        screenHistory.Push(mainMenuScreen);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL("https://sites.google.com/view/epic-arcade-nitro-racing/home");
    }

    #endregion

    // ========== Panel Controls (Open/Close Directly) ==========
    #region Panel Controls

    private void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        exitPanel.SetActive(false);
        dailyRewardPanel.SetActive(false);
        userPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);
    }

    public void OpenExitPanel()
    {
        CloseAllPanels();
        exitPanel.SetActive(true);  // Open exit panel directly without fake loading
    }

    public void OpenDailyRewardsPanel()
    {
        CloseAllPanels();
        dailyRewardPanel.SetActive(true);
    }

    public void OpenUserPanel()
    {
        CloseAllPanels();
        userPanel.SetActive(true);
    }

    public void OpenCharacterSelectionPanel()
    {
        CloseAllPanels();
        characterSelectionPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        CloseAllPanels();
    }

    #endregion

    // ========== Screen Controls with Random Loading ==========
    #region Screen Controls with Loading

    private void OpenScreenWithRandomLoading(GameObject screen)
    {
        // Randomly choose between fakeLoadingScreen_1 and fakeLoadingScreen_2
        LoadingScreen selectedLoadingScreen = Random.Range(0, 2) == 0 ? fakeLoadingScreen_1 : fakeLoadingScreen_2;

        ShowFakeLoading(selectedLoadingScreen, () =>
        {
            CloseAllScreens();
            screen.SetActive(true);
            screenHistory.Push(screen);  // Push new screen onto the history stack
        });
    }

    public void OpenDailyTasks()
    {
        OpenScreenWithRandomLoading(dailyTasksScreen);
    }

    public void OpenGarage()
    {
        garageScreen.transform.GetChild(0).gameObject.SetActive(true);
        garageScreen.transform.GetChild(1).gameObject.SetActive(false);

        // Set main menu transparency when opening garage
        SetMainMenuTransparency(true);

        OpenScreenWithRandomLoading(garageScreen);
    }


    public void OpenStoreScreen()
    {
        OpenScreenWithRandomLoading(storeScreen);
    }

    public void OpenModeSelection()
    {
        if (PlayerPrefs.GetInt("OtherModes", 0) == 1)
            OpenScreenWithRandomLoading(modeSelectionScreen);
        else
            PlayGame();
    }

    void PlayGame()
    {
        if (PlayerPrefs.GetInt($"RaceTrack_Level",0) == 9 && PlayerPrefs.GetString("CurrentMode", "RaceTrack") == "RaceTrack")
        {
            PlayerPrefs.SetString("SelectedCar2Spawn", "Car_T7_3_Player");
        }
        LoadGameplaySceneAsync();
    }

  

    public void OpenLevelSelection()
    {
        OpenScreenWithRandomLoading(levelSelectionScreen);
    }

    #endregion

    // ========== Navigation Back ==========
    #region Navigation Back
    private void SetMainMenuTransparency(bool isTransparent)
    {
        Color targetColor = isTransparent ? new Color(1f, 1f, 1f, 0f) : new Color(1f, 1f, 1f, 1f);
        mainMenu.GetComponent<Image>().color = targetColor;
    }

    public void BackButton()
    {
        if (screenHistory.Count > 1)
        {
            CloseAllPanels();  // Ensure all panels are closed
            // Close current screen and show previous screen
            GameObject currentScreen = screenHistory.Pop();
            currentScreen.SetActive(false);
            GameObject previousScreen = screenHistory.Peek();
            // Update transparency based on whether the previous screen is the garage
            SetMainMenuTransparency(previousScreen == garageScreen);
            ShowFakeLoading(fakeLoadingScreen_1, () => previousScreen.SetActive(true));  // Show loading for previous screen
        }
        else if (screenHistory.Count == 1)
        {
            // If only main menu screen is left in the stack, show the exit panel directly
            OpenExitPanel();
        }
        //    mainMenu.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

    }

    #endregion

    // ========== Loading and Scene Management ==========
    #region Loading and Scene Management

    public void LoadGameplaySceneAsync()
    {
        StartCoroutine(LoadGameplaySceneCoroutine());
    }

    private IEnumerator LoadGameplaySceneCoroutine()
    {
        // Show the loading screen
        loadToGameplayScene.loadingScreen.SetActive(true);
        loadToGameplayScene.loadingSlider.value = 0;

        // Fake loading phase (3 seconds to 50%)
        float fakeLoadingDuration = 3f;
        float fakeProgress = 0f;

        while (fakeProgress < 0.5f)
        {
            fakeProgress += Time.deltaTime / fakeLoadingDuration;
            loadToGameplayScene.loadingSlider.value = fakeProgress;
            yield return null;
        }

        // Ensure slider is exactly at 50%
        loadToGameplayScene.loadingSlider.value = 0.5f;

        // Actual scene loading phase
        AsyncOperation operation = SceneManager.LoadSceneAsync("Gameplay");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            // Update slider to progress from 50% to 100%
            float realProgress = Mathf.Lerp(0.5f, 1f, operation.progress / 0.9f);
            loadToGameplayScene.loadingSlider.value = realProgress;
            yield return null;
        }

        // Ensure the slider is fully filled before activating the scene
        loadToGameplayScene.loadingSlider.value = 1f;
        operation.allowSceneActivation = true;

        // Hide the loading screen after the scene is activated
      //  loadToGameplayScene.loadingScreen.SetActive(false);
    }

    #endregion

    // ========== Fake Loading with Callback ==========
    #region Fake Loading with Callback

    public void ShowFakeLoading(LoadingScreen fakeLoadingScreen, System.Action onComplete)
    {
        mainMenu.SetActive(false);
        StartCoroutine(FakeLoadingCoroutine(fakeLoadingScreen, onComplete));
    }

    private IEnumerator FakeLoadingCoroutine(LoadingScreen fakeLoadingScreen, System.Action onComplete)
    {
        fakeLoadingScreen.loadingScreen.SetActive(true);
        float elapsedTime = 0;

        while (elapsedTime < fakeLoadingScreen.loadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fakeLoadingScreen.loadingTime);
            fakeLoadingScreen.loadingSlider.value = progress;
            yield return null;
        }

        fakeLoadingScreen.loadingScreen.SetActive(false);
        onComplete?.Invoke();
        mainMenu.SetActive(true);
    }

    #endregion

    // ========== Utility Functions ==========
    #region Utility Functions

    private void CloseAllScreens()
    {

        mainMenuScreen.SetActive(false);
        dailyTasksScreen.SetActive(false);
        garageScreen.SetActive(false);
        storeScreen.SetActive(false);
        modeSelectionScreen.SetActive(false);
        levelSelectionScreen.SetActive(false);
    }

    #endregion


    public void ButtonClick()
    {
        AudioManager.instance.PlayButtonClick();
    }

}
