using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject completePanel;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
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
