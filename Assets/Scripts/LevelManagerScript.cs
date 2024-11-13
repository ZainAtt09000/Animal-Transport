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
        if(levelNo >= farmModeLevels.Length)
        {
            levelNo = Random.Range(0, farmModeLevels.Length);
        }
        PlayerPrefs.SetInt(LevelNo, levelNo);
    }
}
