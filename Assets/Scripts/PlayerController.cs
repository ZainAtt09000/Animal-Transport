using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public GameObject rccCanvas;
    public GameObject rccCamera;
    public Rigidbody carRb;
    public RCC_CarControllerV3 RCC;

    public int currentLevel = 0;
    public static PlayerController instance;


    [System.Serializable]
    public class AnimalsPositions
    {
        public Transform[] transforms;
    }




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LevelManagerScript.instance.LevelNo);
        
        rccCanvas.SetActive(true);
        rccCamera.SetActive(true);
        PlayStartCutSceneOfCurrentLevel(LevelManagerScript.instance.farmModeLevels[currentLevel], false);
        TrafficPlayerFinder.Instance.gameObject.transform.parent = this.transform;
        TrafficPlayerFinder.Instance.gameObject.transform.localPosition = Vector3.zero;
        TrafficPlayerFinder.Instance.gameObject.transform.localEulerAngles = Vector3.zero;
        Invoke(nameof(CustomStart), 1f);
    }

    void CustomStart()
    {
        StartCoroutine(LevelStart());
    }

    public IEnumerator LevelStart()
    {
        rccCanvas.SetActive(false);
        rccCamera.SetActive(false);
        PlayStartCutSceneOfCurrentLevel(LevelManagerScript.instance.farmModeLevels[currentLevel], true);
        yield return new WaitForSeconds(LevelManagerScript.instance.farmModeLevels[currentLevel].startCustscene.cutsceneTime);
        if (LevelManagerScript.instance.farmModeLevels[currentLevel].startCustscene.loadAnimal)
        {
            AnimalPositionHolder.instance.SpawnAnimals(LevelManagerScript.instance.farmModeLevels[currentLevel].startCustscene.animals);
        }
        foreach (var level in LevelManagerScript.instance.farmModeLevels[currentLevel].directions)
        {
        level.direction.SetActive(false);
        }
        LevelManagerScript.instance.farmModeLevels[currentLevel].directions[0].direction.SetActive(true);
        rccCanvas.SetActive(true);
            RCC.StartEngine();
        rccCamera.SetActive(true);
        PlayStartCutSceneOfCurrentLevel(LevelManagerScript.instance.farmModeLevels[currentLevel], false);
    }

    public void PlayStartCutSceneOfCurrentLevel(LevelManagerScript.Level level, bool flag)
    {
        level.levelObject.SetActive(true);
        level.startCustscene.cutscene.SetActive(flag);
        
        if(level.startCustscene.activeCage_1)
        {
            AnimalPositionHolder.instance.cage_1.SetActive(true);
        }
        
        if(level.startCustscene.activeCage_2)
        {
            AnimalPositionHolder.instance.cage_2.SetActive(true);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LastParking"))
        {
            carRb.isKinematic = true;
            RCC.KillEngine();
            RCC.speed = 0;
            rccCanvas.SetActive(false);
            rccCamera.SetActive(false);
            if (!cutsceneStarted)
            {
                cutsceneStarted = true;
                StartCoroutine(StartCutscene(LevelManagerScript.instance.farmModeLevels[currentLevel].endCustscene,true));
            }
        }

        if(other.gameObject.CompareTag("MidParking"))
        {
            carRb.isKinematic = true;
            RCC.KillEngine();
            RCC.speed = 0;
            rccCanvas.SetActive(false);
            rccCamera.SetActive(false);
            if (!cutsceneStarted)
            {
                cutsceneStarted = true;

                LevelManagerScript.Cutscenes activeDirectionCutscene = null;
                foreach (var item in LevelManagerScript.instance.farmModeLevels[currentLevel].directions)
                {
                    if(item.direction.activeInHierarchy)
                    {
                        activeDirectionCutscene = item.midCutscene;
                        activeDirectionNumber++;
                    }
                }

                StartCoroutine(StartCutscene(activeDirectionCutscene,false));
            }
        }
    }
    public bool cutsceneStarted = false;
    int activeDirectionNumber = 0;
    public IEnumerator StartCutscene(LevelManagerScript.Cutscenes cutscene,bool Last)
    {
        if (!cutscene.loadAnimal)
        {
            AnimalPositionHolder.instance.DestroyAnimals();
        }
        cutscene.cutscene.SetActive(true);
        yield return new WaitForSeconds(cutscene.cutsceneTime);
        if(Last)
        {
            GameManager.instance.LevelCompleted();
            LevelManagerScript.instance.NextLevel();
        }
        else
        {
            cutscene.cutscene.SetActive(false);
            LevelManagerScript.instance.farmModeLevels[currentLevel].directions[activeDirectionNumber-1].direction.SetActive(false);
            LevelManagerScript.instance.farmModeLevels[currentLevel].directions[activeDirectionNumber].direction.SetActive(true);
            if(cutscene.loadAnimal)
            {
                AnimalPositionHolder.instance.DestroyAnimals(); 
                AnimalPositionHolder.instance.SpawnAnimals(cutscene.animals);
            }
            else
            {
                AnimalPositionHolder.instance.DestroyAnimals();
            }
            rccCanvas.SetActive(true);
            rccCamera.SetActive(true);
            cutsceneStarted = false; carRb.isKinematic = false;
            RCC.StartEngine();


            if (cutscene.activeCage_1)
            {
                AnimalPositionHolder.instance.cage_1.SetActive(true);
            }

            if (cutscene.activeCage_2)
            {
                AnimalPositionHolder.instance.cage_2.SetActive(true);
            }

        }
    }
}
