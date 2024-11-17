using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject truckPrefab;
    public Transform startPoint;
    public GameObject completePanel;
    public GameObject rccCanvas;
    public GameObject rccCamera;
    public static GameManager instance;

    public GameObject activeTruck;
    public int currentLvl = 0;

    public GameObject loadingScreen;

    public Transform animalPositionHolder;
    private void Awake()
    {
        instance = this;
        activeTruck = Instantiate(truckPrefab);
        activeTruck.GetComponent<PlayerController>().rccCamera = rccCamera;
        activeTruck.GetComponent<PlayerController>().rccCanvas = rccCanvas;
        animalPositionHolder.parent = activeTruck.transform;
        animalPositionHolder.localPosition = Vector3.zero;
        animalPositionHolder.localEulerAngles = Vector3.zero;
    }

    private void Start()
    {
        loadingScreen.SetActive(true);
        currentLvl = PlayerPrefs.GetInt(LevelManagerScript.instance.LevelNo);
        
        Invoke(nameof(Loading), 1f);
    }

    void Loading()
    {
        startPoint = LevelManagerScript.instance.activeModeLevels[currentLvl].startPoint;

        activeTruck.SetActive(false);

        activeTruck.transform.position = startPoint.position;
        activeTruck.transform.rotation = startPoint.rotation;
        activeTruck.SetActive(true);
        loadingScreen.SetActive(false);
    }

    public void LevelCompleted()
    {
        completePanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        PlayerController.instance.cutsceneStarted = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
