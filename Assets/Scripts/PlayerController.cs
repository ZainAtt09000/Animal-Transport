using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public GameObject rccCanvas;
    public GameObject rccCamera;
    public Rigidbody carRb;
    public RCC_CarControllerV3 RCC;

    private int currentLevel = 0;
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LevelManagerScript.instance.LevelNo);
        rccCanvas.SetActive(true);
        rccCamera.SetActive(true);
        StartCoroutine(LevelStart());
    }

    public IEnumerator LevelStart()
    {
        rccCanvas.SetActive(false);
        rccCamera.SetActive(false);
        PlayStartCutSceneOfCurrentLevel(LevelManagerScript.instance.farmModeLevels[currentLevel], true);
        yield return new WaitForSeconds(LevelManagerScript.instance.farmModeLevels[currentLevel].startCustscene.cutsceneTime);
        rccCanvas.SetActive(true);
        rccCamera.SetActive(true);
        PlayStartCutSceneOfCurrentLevel(LevelManagerScript.instance.farmModeLevels[currentLevel], false);
    }

    public void PlayStartCutSceneOfCurrentLevel(LevelManagerScript.Level level, bool flag)
    {
        level.levelObject.SetActive(true);
        level.startCustscene.cutscene.SetActive(flag);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ParkingArea"))
        {
            carRb.isKinematic = true;
            RCC.speed = 0;
            rccCanvas.SetActive(false);
            rccCamera.SetActive(false);
            if (!cutsceneStarted)
            {
                cutsceneStarted = true;
                StartCoroutine(StartCutscene());
            }
        }
    }
    public bool cutsceneStarted = false;
    public IEnumerator StartCutscene()
    {

        LevelManagerScript.instance.farmModeLevels[currentLevel].endCustscene.cutscene.SetActive(true);
        yield return new WaitForSeconds(LevelManagerScript.instance.farmModeLevels[currentLevel].endCustscene.cutsceneTime);
        GameManager.instance.LevelCompleted();
        LevelManagerScript.instance.NextLevel();
    }
}
