using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AnimalPositionHolder : MonoBehaviour
{
    [System.Serializable]
    public class AnimalPositions
    {
        public AnimalData[] animalPrefabs;

    }

    [System.Serializable]
    public class AnimalData
    {
        public GameObject animal;
        public Transform animalPos;
    }

    public GameObject cage_1;
    public GameObject cage_2;

    public static AnimalPositionHolder instance;

    private void Awake()
    {
        instance = this;
    }


    public void SpawnAnimals(AnimalData[] animals)
    {
        foreach (var animal in animals)
        {
            GameObject go = Instantiate(animal.animal, animal.animalPos);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
        }
    }
    public void DestroyAnimals()
    {
        if (this.transform.GetChild(0).transform.childCount >0)
        {
            Destroy(this.transform.GetChild(0).GetChild(0).gameObject);
        }
        
        if (this.transform.GetChild(1).transform.childCount >0)
        {
            Destroy(this.transform.GetChild(1).GetChild(0).gameObject);
        }
    }
}
