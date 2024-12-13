using UnityEngine;
using UnityEngine.UI;

public class GarageCarSelection : MonoBehaviour
{
    public enum PurchaseWith
    {
        Gold,
        Diamond,
        Gold_Diamond,
        None
    }

    [System.Serializable]
    public class Car
    {
        public GameObject carPrefab;
        public Button carButton;
        public string controlCarName;
        public GameObject lockImage;
        public Image carButtonRefImage;
        public PurchaseWith canBePurchasedBy;
        public bool isUnlocked;
        public int goldPrice;
        public int diamondPrice;

        public Options[] availabeCustomizations;
        //  public string unlockKey; // Unique key for each car to save its unlock status in PlayerPrefs
    }


    [System.Serializable]
    public class Customizations
    {
        public GameObject button;
        public GameObject panel;
        public AvailableCustomizations[] customizations;
    }
    [System.Serializable]

    public class AvailableCustomizations
    {
        public Button customizationButton;
        public GameObject lockButton;
        public bool isUnlocked;

        public int unlockPrice; // only gold option available for purchase 
    }


    public enum Options
    {
        Paint,
        Wheels,
        WheelsColor,
        ClipperColor,
        WindowColor,
        SmokeColor,
        Neon,
        Spoiler,
        Skin
    }

    [Header("Customization")]
    public Customizations paint;
    public Customizations wheels;
    public Customizations wheelsColor;
    public Customizations windowColor;
    public Customizations ClipperColor;
    public Customizations smokeColor;
    public Customizations neon;
    public Customizations spoiler;
    public Customizations skins;
    public Button buyCustomizationWithGold;
    public Text buyCustomizationWithGoldText;
    [Header("UI Ref")]
    public Sprite selectedCarSprite;
    public Sprite unSelectedCarSprite;
    public Text unlockedCarsText;
    public Button customizeButton;
    public Button driveItButton;
    public GameObject unlockThisCarText;
    public Button goldBuyButton;
    public Text goldBuyAmountText;
    public Button diamondBuyButton;
    public Text diamondBuyAmountText;

    [Header("Cars Ref")]
    public Car[] availableCars;
    public Transform carSpawnPoint;

    [Header("Other Ref")]
    public GameObject garageCam;
    public GameObject garage;
    public GameObject customizationScreen;
    public static GarageCarSelection instance;

    private GameObject currentCarInstance;
    private int selectedCarIndex;
    private GameObject lastDrivenCarInstance; // Track the last driven car

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        selectedCarIndex = PlayerPrefs.GetInt("SelectedCar", 0);

        garage.SetActive(true);
        garageCam.SetActive(true);
        LoadUnlockedCars(); // Load unlocked status of cars when the garage is enabled
        LoadSelectedCar();
    }

    private void OnDisable()
    {
          //   garage.SetActive(false);
         //    garageCam.SetActive(false);
    }

    private void LoadSelectedCar()
    {
        selectedCarIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        UpdateCarDisplay();
    }
    private void LoadUnlockedCars()
    {
        foreach (var car in availableCars)
        {
            // Check if the car can be purchased with 'None' (this means it's unlocked by default)
            if (car.canBePurchasedBy == PurchaseWith.None)
            {
                car.isUnlocked = true;  // Set it as unlocked
                PlayerPrefs.SetInt(car.controlCarName, 1);  // Save the unlock status in PlayerPrefs
            }
            else
            {
                car.isUnlocked = PlayerPrefs.GetInt(car.controlCarName, 0) == 1;  // Load unlock status from PlayerPrefs
            }
        }
    }

    public void SelectCar(int index)
    {
        selectedCarIndex = index;
        UpdateCarDisplay();
    }

    private void UpdateCarDisplay()
    {
        // Destroy previous car instance if it exists
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
        }

        // Spawn the currently selected car if it's not the last driven car
        Car selectedCar = availableCars[selectedCarIndex];
    /*    if (lastDrivenCarInstance != null && lastDrivenCarInstance != currentCarInstance)
        {
            // If a car was previously driven, use it
            currentCarInstance = lastDrivenCarInstance;
            lastDrivenCarInstance.SetActive(true); // Make sure the last driven car is active
        }
        else
    */    {
            // Otherwise, spawn the selected car
            currentCarInstance = Instantiate(selectedCar.carPrefab, carSpawnPoint);
        }

        // Update button visuals and lock image visibility
        foreach (Car car in availableCars)
        {
            // Update car button sprite and lock image visibility
            if (car == selectedCar && car.isUnlocked)
            {
                car.carButtonRefImage.sprite = selectedCarSprite;
            }
            else
            {
                car.carButtonRefImage.sprite = unSelectedCarSprite;
                //if (!car.isUnlocked)
                //{
                //   car.lockImage.SetActive(true); // Show lock image if locked
                //}
            }

            if (car.isUnlocked)
            {
                car.lockImage.SetActive(false);  // Hide lock image if unlocked
            }
            else
            {
                car.lockImage.SetActive(true); // Show lock image if locked
            }
        }

        // Show respective UI elements based on car's lock status and purchase method
        unlockThisCarText.SetActive(!selectedCar.isUnlocked);
        driveItButton.gameObject.SetActive(selectedCar.isUnlocked);
        customizeButton.gameObject.SetActive(/*selectedCar.isUnlocked*/false);

        if (!selectedCar.isUnlocked)
        {
            ShowPurchaseOptions(selectedCar);
        }
        else
        {
            goldBuyButton.gameObject.SetActive(false);
            diamondBuyButton.gameObject.SetActive(false);
        }

        UpdateUnlockedCarsText();
    }
    private void ShowPurchaseOptions(Car car)
    {
        bool showGoldButton = car.canBePurchasedBy == PurchaseWith.Gold || car.canBePurchasedBy == PurchaseWith.Gold_Diamond;
        bool showDiamondButton = car.canBePurchasedBy == PurchaseWith.Diamond || car.canBePurchasedBy == PurchaseWith.Gold_Diamond;

        goldBuyButton.gameObject.SetActive(showGoldButton);
        goldBuyAmountText.text = showGoldButton ? car.goldPrice.ToString() : "";

        diamondBuyButton.gameObject.SetActive(showDiamondButton);
        diamondBuyAmountText.text = showDiamondButton ? car.diamondPrice.ToString() : "";
    }

    private void UpdateUnlockedCarsText()
    {
        int unlockedCount = 0;
        foreach (var car in availableCars)
        {
            if (car.isUnlocked)
            {
                unlockedCount++;
            }
        }
        unlockedCarsText.text = $"{unlockedCount}/{availableCars.Length}";
    }

    public void BuyWithGold()
    {
        Car selectedCar = availableCars[selectedCarIndex];
        int playerGold = PlayerPrefs.GetInt("PlayerGold", 0);

        if (playerGold >= selectedCar.goldPrice)
        {
            playerGold -= selectedCar.goldPrice;
            PlayerPrefs.SetInt("PlayerGold", playerGold);

            selectedCar.isUnlocked = true;
            // Save the unlock status to PlayerPrefs
            PlayerPrefs.SetInt(selectedCar.controlCarName, 1);

            UpdateCarDisplay();
        }
        else
        {
            Debug.Log("Not enough gold to unlock this car.");
        }
        MainMenuController.instance.UpdateCurrency();

    }

    public void BuyWithDiamond()
    {
        Car selectedCar = availableCars[selectedCarIndex];
        int playerDiamonds = PlayerPrefs.GetInt("PlayerDiamond", 0);

        if (playerDiamonds >= selectedCar.diamondPrice)
        {
            playerDiamonds -= selectedCar.diamondPrice;
            PlayerPrefs.SetInt("PlayerDiamond", playerDiamonds);

            selectedCar.isUnlocked = true;
            // Save the unlock status to PlayerPrefs
            PlayerPrefs.SetInt(selectedCar.controlCarName, 1);

            UpdateCarDisplay();
        }
        else
        {
            Debug.Log("Not enough diamonds to unlock this car.");
        }
        MainMenuController.instance.UpdateCurrency();

    }

    public void DriveButton()
    {
        // Save the currently selected car as the last driven car
        lastDrivenCarInstance = currentCarInstance;
        PlayerPrefs.SetInt("SelectedCar", selectedCarIndex);
        PlayerPrefs.SetString("SelectedCar2Spawn", availableCars[selectedCarIndex].controlCarName);
        // Here you would transition to the driving mode (e.g., open mode selection screen)
        MainMenuController.instance.OpenModeSelection();
    }
    public void CustomizeButton()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        customizationScreen.SetActive(true);
        LoadAvailableCustomizations();
        OpenCustomizationPanel(paint);

        LoadUnlockedCustomizations(paint);
        LoadUnlockedCustomizations(wheels);
        LoadUnlockedCustomizations(wheelsColor);
        LoadUnlockedCustomizations(windowColor);
        LoadUnlockedCustomizations(ClipperColor);
        LoadUnlockedCustomizations(smokeColor);
        LoadUnlockedCustomizations(neon);
        LoadUnlockedCustomizations(spoiler);
        LoadUnlockedCustomizations(skins);

    }
    private void Start()
    {
        // Assign listeners to car selection buttons
        for (int i = 0; i < availableCars.Length; i++)
        {
            int index = i; // Capture the index in a local variable to avoid closure issues
            availableCars[i].carButton.onClick.AddListener(() => SelectCar(index));
        }

        // Assign listeners for purchasing buttons
        goldBuyButton.onClick.AddListener(BuyWithGold);
        diamondBuyButton.onClick.AddListener(BuyWithDiamond);

        // Assign listeners for drive and customize buttons
        driveItButton.onClick.AddListener(DriveButton);
        customizeButton.onClick.AddListener(CustomizeButton);

        foreach (var availCustom in paint.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(paint,availCustom));
        }
        foreach (var availCustom in wheels.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(wheels,availCustom));
        }

        foreach (var availCustom in wheelsColor.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(wheelsColor,availCustom));
        }

        foreach (var availCustom in windowColor.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(windowColor,availCustom));
        }

        foreach (var availCustom in ClipperColor.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(ClipperColor,availCustom));
        }

        foreach (var availCustom in smokeColor.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(smokeColor,availCustom));
        }

        foreach (var availCustom in neon.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(neon,availCustom));
        }

        foreach (var availCustom in spoiler.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(spoiler,availCustom));
        }

        foreach (var availCustom in skins.customizations)
        {
            availCustom.customizationButton.onClick.AddListener(() => CheckLockStatus(skins,availCustom));
        }

    }


    #region Customization
    private void LoadAvailableCustomizations()
    {
        Car selectedCar = availableCars[selectedCarIndex];
        DisableAllCustomizationButtons();

        foreach (Options option in selectedCar.availabeCustomizations)
        {
            switch (option)
            {
                case Options.Paint:
                    SetCustomizationButton(paint);
                    break;
                case Options.Wheels:
                    SetCustomizationButton(wheels);
                    break;
                case Options.WheelsColor:
                    SetCustomizationButton(wheelsColor);
                    break;
                case Options.WindowColor:
                    SetCustomizationButton(windowColor);
                    break;
                case Options.ClipperColor:
                    SetCustomizationButton(ClipperColor);
                    break;
                case Options.SmokeColor:
                    SetCustomizationButton(smokeColor);
                    break;
                case Options.Neon:
                    SetCustomizationButton(neon);
                    break;
                case Options.Spoiler:
                    SetCustomizationButton(spoiler);
                    break;
                case Options.Skin:
                    SetCustomizationButton(skins);
                    break;
            }
        }
    }

    private void SetCustomizationButton(Customizations customization)
    {
        customization.button.SetActive(true);
        customization.button.GetComponent<Button>().onClick.RemoveAllListeners();
        customization.button.GetComponent<Button>().onClick.AddListener(() => OpenCustomizationPanel(customization));
    }

    private void OpenCustomizationPanel(Customizations customization)
    {
        DisableAllCustomizationPanels();
        customization.panel.SetActive(true);
    }

    void CheckLockStatus(Customizations customize,AvailableCustomizations availCustomization)
    {
        availCustomization.lockButton.SetActive(!availCustomization.isUnlocked);

        if (!availCustomization.isUnlocked)
        {
            buyCustomizationWithGold.gameObject.SetActive(true);
            buyCustomizationWithGoldText.text = availCustomization.unlockPrice.ToString();

            buyCustomizationWithGold.onClick.RemoveAllListeners();
            buyCustomizationWithGold.onClick.AddListener(() => UnlockCustomization(customize,availCustomization));
        }
        else
        {
            buyCustomizationWithGold.gameObject.SetActive(false);
            ApplyCurrentCustomization();
        }
    }

    private void LoadUnlockedCustomizations(Customizations customize)
    {
        foreach (var item in customize.customizations)
        {
            // Construct the PlayerPrefs key dynamically based on car name, button name, and customization button name
            string key = availableCars[selectedCarIndex].controlCarName + customize.button.name + item.customizationButton.name;

            // Get the value from PlayerPrefs (default is 1, meaning unlocked)
            int unlocked = PlayerPrefs.GetInt(key, 0);
            if(unlocked == 1)
            {
                item.isUnlocked = true;
                item.lockButton.SetActive(false);
            }
        }

    }

    private void UnlockCustomization(Customizations customize,AvailableCustomizations customOption)
    {
        int playerGold = PlayerPrefs.GetInt("PlayerGold", 0);

        if (playerGold >= customOption.unlockPrice)
        {
            playerGold -= customOption.unlockPrice;
            PlayerPrefs.SetInt("PlayerGold", playerGold);

            customOption.isUnlocked = true;
            customOption.lockButton.SetActive(false);

            buyCustomizationWithGold.gameObject.SetActive(false);
            PlayerPrefs.SetInt(availableCars[selectedCarIndex].controlCarName + customize.button.name + customOption.customizationButton.name, 1);

            ApplyCurrentCustomization();
        }
        else
        {
            Debug.Log("Not enough gold to unlock this customization.");
        }
        MainMenuController.instance.UpdateCurrency();

    }

    private void DisableAllCustomizationButtons()
    {
        paint.button.SetActive(false);
        wheels.button.SetActive(false);
        wheelsColor.button.SetActive(false);
        windowColor.button.SetActive(false);
        ClipperColor.button.SetActive(false);
        smokeColor.button.SetActive(false);
        neon.button.SetActive(false);
        spoiler.button.SetActive(false);
        skins.button.SetActive(false);
    }

    private void DisableAllCustomizationPanels()
    {
        paint.panel.SetActive(false);
        wheels.panel.SetActive(false);
        wheelsColor.panel.SetActive(false);
        windowColor.panel.SetActive(false);
        ClipperColor.panel.SetActive(false);
        smokeColor.panel.SetActive(false);
        neon.panel.SetActive(false);
        spoiler.panel.SetActive(false);
        skins.panel.SetActive(false);
    }
    #endregion


    #region Customization Applier

    Color paintColor;
    Color prevPaintColor;

    Color windowsColor;
    Color prevWindowsColor;

    GameObject wheelPrefab;
    GameObject prevWheelPrefab;

    Color wheelColor;
    Color prevWheelColor;

    GameObject spoilerPrefab;
    GameObject prevSpoilerPrefab;

    Texture2D skin;
    Texture2D prevSkin;

    GameObject neonLight;
    GameObject prevNeonLight;

    // Function to visualize paint with Color object
    public void VisualizePaint(Color paintColor)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeColor(paintColor);
            this.paintColor = paintColor;
        }
    }

    // Function to visualize paint with color code string
    public void VisualizePaint(string colorCode)
    {
            Debug.Log("Applying Paint");
        if (ColorUtility.TryParseHtmlString("#"+colorCode, out Color color))
        {

            VisualizePaint(color);
        }
        else
        {
            Debug.LogWarning("Invalid color code: " + colorCode);
        }
    }

    // Function to visualize window color with Color object
    public void VisualizeWindowColor(Color color)
    {
        if (CustomizationApplier.instance)
        {
            // Set the new alpha value to 150 (normalized to 0-1 range, so 150/255)
            Color newCol = new Color(color.r, color.g, color.b, 150f / 255f);

            // Pass the color to the instance
            CustomizationApplier.instance.VisualizeWindowColor(newCol);

            // Store the new color with adjusted alpha
            windowsColor = newCol;
        }
    }

    // Function to visualize window color with color code string
    public void VisualizeWindowColor(string colorCode)
    {
        if (ColorUtility.TryParseHtmlString("#" + colorCode, out Color color))
        {
            color.a = 150f;
            VisualizeWindowColor(color);
        }
        else
        {
            Debug.LogWarning("Invalid color code: " + colorCode);
        }
    }

    public void VisualizeWheel(GameObject wheelPrefab)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeWheel(wheelPrefab);
            this.wheelPrefab = wheelPrefab;
        }
    }

    public void VisualizeWheelColor(Color color)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeWheelColor(color);
            wheelColor = color;
        }
    }

    public void VisualizeWheelColor(string colorCode)
    {
        if (ColorUtility.TryParseHtmlString("#" + colorCode, out Color color))
        {
            VisualizeWheelColor(color);
        }
        else
        {
            Debug.LogWarning("Invalid color code: " + colorCode);
        }
    }

    public void VisualizeSpoiler(GameObject spoilerPrefab)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeSpoiler(spoilerPrefab);
            this.spoilerPrefab = spoilerPrefab;
        }
    }

    public void VisualizeSpecialSkin(Texture2D skin)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeSpecialSkin(skin);
            this.skin = skin;
        }
    }

    public void VisualizeNeonLight(GameObject neonLightPrefab)
    {
        if (CustomizationApplier.instance)
        {
            CustomizationApplier.instance.VisualizeNeonLight(neonLightPrefab);
            neonLight = neonLightPrefab;
        }
    }


    public void ApplyCurrentCustomization()
    {
        if (CustomizationApplier.instance)
        {
            // Apply paint color only if it's different from the previous color
            if (prevPaintColor == null || prevPaintColor != paintColor)
            {
                CustomizationApplier.instance.ApplyColor(paintColor);
                prevPaintColor = paintColor;
            }

            // Apply window color only if it's different from the previous color
            if (prevWindowsColor == null || prevWindowsColor != windowsColor)
            {
                CustomizationApplier.instance.ApplyWindowColor(windowsColor);
                prevWindowsColor = windowsColor;
            }

            // Apply wheel prefab only if it's different from the previous prefab
            if (prevWheelPrefab != wheelPrefab)
            {
                CustomizationApplier.instance.ApplyWheel(wheelPrefab);
                prevWheelPrefab = wheelPrefab;
            }

            // Apply wheel color only if it's different from the previous color
            if (prevWheelColor == null || prevWheelColor != wheelColor)
            {
                CustomizationApplier.instance.ApplyWheelColor(wheelColor);
                prevWheelColor = wheelColor;
            }

            // Apply spoiler only if it's different from the previous spoiler
            if (prevSpoilerPrefab != spoilerPrefab)
            {
                CustomizationApplier.instance.ApplySpoiler(spoilerPrefab);
                prevSpoilerPrefab = spoilerPrefab;
            }

            // Apply special skin only if it's different from the previous skin
            if (prevSkin != skin)
            {
                CustomizationApplier.instance.ApplySpecialSkin(skin);
                prevSkin = skin;
            }

            // Apply neon light only if it's different from the previous light
            if (prevNeonLight != neonLight)
            {
                CustomizationApplier.instance.ApplyNeonLight(neonLight);
                prevNeonLight = neonLight;
            }
        }
    }
    #endregion

}
