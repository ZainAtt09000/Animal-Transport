using UnityEngine;

public class CustomizationApplier : MonoBehaviour
{
    public string carName;
    public GarageCarSelection.Options[] customizationOptions;

    [System.Serializable]
    public class CustomizationSettings
    {
        public Color color;
        public Texture2D texture;
        public Color windowColor;
        public GameObject wheelPrefab;
        public Color wheelColor;
        public GameObject currentSpoiler;
        public Texture2D specialSkin;
        public bool specialSkinApplied = false;
        public GameObject currentNeonLight;
    }

    public Material carBodyMat;
    public Material windowMat;
    public Transform FL_Wheel;
    public Transform FR_Wheel;
    public Transform RL_Wheel;
    public Transform RR_Wheel;
    public GameObject[] availableWheelsPrefabs;
    public Material wheelMat;
    public Transform spoilerPosition;
    public GameObject[] availableSpoilerPrefabs;
    public Transform neonLightPos;
    public GameObject[] neonLightPrefabs;
    public Texture2D[] specialSkinsTextures;

    public CustomizationSettings defaultCustomization;
    public CustomizationSettings currentAppliedCustomization;
    public CustomizationSettings toApplyCustomization;

    public static CustomizationApplier instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadCurrentCustomization();
    }

    #region Apply Functions
    public void ApplyColor(Color color)
    {
        if (carBodyMat != null)
        {
            carBodyMat.color = color;
            currentAppliedCustomization.color = color;
            PlayerPrefs.SetString(carName+"CarColor", ColorUtility.ToHtmlStringRGBA(color));
        }
    }

    public void ApplyWindowColor(Color color)
    {
        if (windowMat != null)
        {
            windowMat.color = color;
            currentAppliedCustomization.windowColor = color;
            PlayerPrefs.SetString(carName+"WindowColor", ColorUtility.ToHtmlStringRGBA(color));
        }
    }

    public void ApplyWheel(GameObject wheelPrefab)
    {
        if (wheelPrefab != null)
        {
            currentAppliedCustomization.wheelPrefab = wheelPrefab;
            PlayerPrefs.SetInt(carName+"WheelPrefabIndex", System.Array.IndexOf(availableWheelsPrefabs, wheelPrefab));
            VisualizeWheel(wheelPrefab);
        }
    }

    public void ApplyWheelColor(Color color)
    {
        if (wheelMat != null)
        {
            wheelMat.color = color;
            currentAppliedCustomization.wheelColor = color;
            PlayerPrefs.SetString(carName + "WheelColor", ColorUtility.ToHtmlStringRGBA(color));
        }
    }

    public void ApplySpoiler(GameObject spoilerPrefab)
    {
        if (spoilerPrefab != null && spoilerPosition != null)
        {
            currentAppliedCustomization.currentSpoiler = spoilerPrefab;
            PlayerPrefs.SetInt(carName + "SpoilerPrefabIndex", System.Array.IndexOf(availableSpoilerPrefabs, spoilerPrefab));
            VisualizeSpoiler(spoilerPrefab);
        }
    }

    public void ApplySpecialSkin(Texture2D skin)
    {
        if (skin != null && carBodyMat != null)
        {
            carBodyMat.mainTexture = skin;
            currentAppliedCustomization.specialSkin = skin;
            PlayerPrefs.SetString(carName + "SpecialSkin", skin.name);
            currentAppliedCustomization.specialSkinApplied = true;
            PlayerPrefs.SetInt(carName + "SpecialSkinApplied", 1);
        }
    }

    public void ApplyNeonLight(GameObject neonLightPrefab)
    {
        if (neonLightPrefab != null && neonLightPos != null)
        {
            currentAppliedCustomization.currentNeonLight = neonLightPrefab;
            PlayerPrefs.SetInt(carName + "NeonLightPrefabIndex", System.Array.IndexOf(neonLightPrefabs, neonLightPrefab));
            VisualizeNeonLight(neonLightPrefab);
        }
    }
    #endregion

    #region Visualize Functions
    public void VisualizeColor(Color color)
    {
        if (carBodyMat != null)
        {
            carBodyMat.color = color;
        }
    }

    public void VisualizeWindowColor(Color color)
    {
        if (windowMat != null)
        {
            windowMat.color = color;
//w            windowMat.color = new Color(color.r, color.g, color.b, 150f);
        }
    }

    public void VisualizeWheel(GameObject wheelPrefab)
    {
        if (wheelPrefab != null)
        {
            Instantiate(wheelPrefab, FL_Wheel.position, FL_Wheel.rotation, FL_Wheel);
            Instantiate(wheelPrefab, FR_Wheel.position, FR_Wheel.rotation, FR_Wheel);
            Instantiate(wheelPrefab, RL_Wheel.position, RL_Wheel.rotation, RL_Wheel);
            Instantiate(wheelPrefab, RR_Wheel.position, RR_Wheel.rotation, RR_Wheel);
        }
    }

    public void VisualizeWheelColor(Color color)
    {
        if (wheelMat != null)
        {
            wheelMat.color = color;
        }
    }

    public void VisualizeSpoiler(GameObject spoilerPrefab)
    {
        if (spoilerPrefab != null && spoilerPosition != null)
        {
            Instantiate(spoilerPrefab, spoilerPosition.position, spoilerPosition.rotation, spoilerPosition);
        }
    }

    public void VisualizeSpecialSkin(Texture2D skin)
    {
        if (skin != null && carBodyMat != null)
        {
            carBodyMat.mainTexture = skin;
        }
    }

    public void VisualizeNeonLight(GameObject neonLightPrefab)
    {
        if (neonLightPrefab != null && neonLightPos != null)
        {
            Instantiate(neonLightPrefab, neonLightPos.position, neonLightPos.rotation, neonLightPos);
        }
    }
    #endregion


    private void LoadCurrentCustomization()
    {
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(carName + "CarColor", ColorUtility.ToHtmlStringRGBA(defaultCustomization.color)), out color))
        {
            ApplyColor(color);
        }

        Color windowColor;
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(carName + "WindowColor", ColorUtility.ToHtmlStringRGBA(defaultCustomization.windowColor)), out windowColor))
        {
            ApplyWindowColor(windowColor);
        }

        int wheelIndex = PlayerPrefs.GetInt(carName + "WheelPrefabIndex", -1);
        if (wheelIndex >= 0 && wheelIndex < availableWheelsPrefabs.Length)
        {
            ApplyWheel(availableWheelsPrefabs[wheelIndex]);
        }
        else if (defaultCustomization.wheelPrefab != null)
        {
            ApplyWheel(defaultCustomization.wheelPrefab);
        }

        Color wheelColor;
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(carName + "WheelColor", ColorUtility.ToHtmlStringRGBA(defaultCustomization.wheelColor)), out wheelColor))
        {
            ApplyWheelColor(wheelColor);
        }

        int spoilerIndex = PlayerPrefs.GetInt(carName + "SpoilerPrefabIndex", -1);
        if (spoilerIndex >= 0 && spoilerIndex < availableSpoilerPrefabs.Length)
        {
            ApplySpoiler(availableSpoilerPrefabs[spoilerIndex]);
        }
        else if (defaultCustomization.currentSpoiler != null)
        {
            ApplySpoiler(defaultCustomization.currentSpoiler);
        }

        string specialSkinName = PlayerPrefs.GetString(carName + "SpecialSkin", null);
        if (!string.IsNullOrEmpty(specialSkinName))
        {
            Texture2D specialSkin = System.Array.Find(specialSkinsTextures, skin => skin.name == specialSkinName);
            if (specialSkin != null)
            {
                ApplySpecialSkin(specialSkin);
            }
        }
        else if (defaultCustomization.specialSkinApplied && defaultCustomization.specialSkin != null)
        {
            ApplySpecialSkin(defaultCustomization.specialSkin);
        }

        int neonIndex = PlayerPrefs.GetInt(carName + "NeonLightPrefabIndex", -1);
        if (neonIndex >= 0 && neonIndex < neonLightPrefabs.Length)
        {
            ApplyNeonLight(neonLightPrefabs[neonIndex]);
        }
        else if (defaultCustomization.currentNeonLight != null)
        {
            ApplyNeonLight(defaultCustomization.currentNeonLight);
        }
    }


    private void ApplyAllCustomizations()
    {
        ApplyColor(currentAppliedCustomization.color);
        ApplyWindowColor(currentAppliedCustomization.windowColor);
        ApplyWheel(currentAppliedCustomization.wheelPrefab);
        ApplyWheelColor(currentAppliedCustomization.wheelColor);
        ApplySpoiler(currentAppliedCustomization.currentSpoiler);
        ApplySpecialSkin(currentAppliedCustomization.specialSkin);
        ApplyNeonLight(currentAppliedCustomization.currentNeonLight);
    }

    private void SaveCustomization()
    {
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        currentAppliedCustomization = defaultCustomization;
        SaveCustomization();
        ApplyAllCustomizations();
    }
}
