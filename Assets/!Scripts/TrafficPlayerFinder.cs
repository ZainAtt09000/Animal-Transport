using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficPlayerFinder : MonoBehaviour
{
    public static TrafficPlayerFinder Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
