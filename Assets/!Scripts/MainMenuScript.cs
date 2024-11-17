using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [HideInInspector] public string Mode = "CurrentMode";
    public void FarmMode()
    {
        PlayerPrefs.SetString(Mode, "Farm");
    }
}
