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
        public GameObject levelObject;

        [Header("Cutscene Details")]
        public Cutscenes startCustscene;
        public Cutscenes endCustscene;
        public Cutscenes[] middleCutscenes;

        [Header("Direction Details")]
        public GameObject[] directions;

    }

    [System.Serializable]
    public class Cutscenes
    {
        public GameObject cutscene;
        public float cutsceneTime;
    }


    public Level[] farmModeLevels;
    public static LevelManagerScript instance;
    [HideInInspector]public string LevelNo = "LevelNumber";
    int levelNo = 0;
    private void Awake()
    {
        instance = this;
        levelNo = PlayerPrefs.GetInt(LevelNo, 0);
    }
    
    public void NextLevel()
    {
        levelNo++;
        PlayerPrefs.SetInt(LevelNo, levelNo);
    }
}
