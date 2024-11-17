using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : MonoBehaviour
{
    [System.Serializable]
    public class Level
    {
        [Header("Level Details")]
        public int levelNumber;
        public Transform startPoint;
        public GameObject levelObject;

        [Header("Cutscene Details")]
        public Cutscenes startCustscene;
        public Cutscenes endCustscene;
  //      public Cutscenes[] middleCutscenes;

        [Header("Direction Details")]
        public Directions[] directions;

    }

    [System.Serializable]
    public class Directions
    {
        public GameObject direction;
        public Cutscenes midCutscene;

    }


    [System.Serializable]
    public class Cutscenes
    {
        public GameObject cutscene;
        public float cutsceneTime;

        public bool loadAnimal;

        public AnimalPositionHolder.AnimalData[] animals;

        public bool activeCage_1;
        public bool activeCage_2;
   }


    public Level[] farmModeLevels;
    public GameObject farmAnimalMode;

    public Level[] parkingModeLevels;
    public GameObject parkingMode;

    public Level[] activeModeLevels;

    public bool farmMode = false;
    public bool parkMode = false;

    public static LevelManagerScript instance;
    [HideInInspector]public string LevelNo = "LevelNumber";
    [HideInInspector]public string Mode = "CurrentMode";

    int levelNo = 0;

    private void Awake()
    {
        instance = this;
        // Retrieve the current mode from PlayerPrefs
        string currentMode = PlayerPrefs.GetString(Mode, "Farm");

        // Initialize activeModeLevels based on the saved mode
        if (currentMode == "Farm")
        {
            farmMode = true;
            parkMode = false;
            activeModeLevels = farmModeLevels;
            farmAnimalMode.SetActive(true);
            parkingMode.SetActive(false);
        }
        else if (currentMode == "Park")
        {
            farmMode = false;
            parkMode = true;
            activeModeLevels = parkingModeLevels;
            farmAnimalMode.SetActive(false);
            parkingMode.SetActive(true);
        }

        levelNo = PlayerPrefs.GetInt(LevelNo, 0);


    }

    public void NextLevel()
    {
        levelNo++;
        if(levelNo >= activeModeLevels.Length)
        {
            levelNo = Random.Range(0, activeModeLevels.Length);
        }
        PlayerPrefs.SetInt(LevelNo, levelNo);
    }
}
