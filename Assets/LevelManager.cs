using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DirectionData
{
    public int parkingAreas; // Number of parking areas in this direction
}

[System.Serializable]
public class LevelData
{
    public int directionsCount; // Number of directions in this level
    public List<DirectionData> parkingAreasPerDirection = new List<DirectionData>(); // Parking areas per direction
}

public class LevelManager : MonoBehaviour
{
    private int currentLevelIndex = 0;                 // Current active level index
    private int currentDirectionIndex = 0;             // Current active direction index within the level
    private int currentParkingCount = 0;                // Count of completed parking areas in the current direction

    private Transform[] levels;                        // Array to store the level GameObjects
    private Transform[] directions;                    // Array to store the Directions GameObjects in the current level

    public List<LevelData> levelDataList = new List<LevelData>(); // List to store data for each level

    private void Start()
    {
        InitializeLevels();
    }

    // Initialize all levels at the start of the game
    void InitializeLevels()
    {
        levels = new Transform[transform.childCount];

        // Loop through each level in the parent GameObject
        for (int i = 0; i < transform.childCount; i++)
        {
            levels[i] = transform.GetChild(i);
            levels[i].gameObject.SetActive(false); // Disable all levels initially
        }

        // Enable the first level and initialize its directions
        if (levels.Length > 0)
        {
            levels[0].gameObject.SetActive(true); // Enable the first level
            currentLevelIndex = 0;
            InitializeDirectionsForLevel(currentLevelIndex);
        }
    }

    // Initialize the Directions for the current level
    void InitializeDirectionsForLevel(int levelIndex)
    {
        Transform level = levels[levelIndex];

        // Check if there are enough parking areas data
        if (levelIndex < levelDataList.Count)
        {
            int directionCount = levelDataList[levelIndex].directionsCount;

            // Initialize the directions array for the current level
            directions = new Transform[directionCount];

            for (int i = 0; i < directionCount; i++)
            {
                // Find each direction GameObject
                Transform directionTransform = level.Find("Directions" + (i + 1));

                // Check if the direction Transform was found
                if (directionTransform != null)
                {
                    directions[i] = directionTransform;
                    directions[i].gameObject.SetActive(false); // Disable all directions initially
                }
                else
                {
                    Debug.LogError($"Direction GameObject 'Directions{i + 1}' not found in Level {levelIndex + 1}");
                }
            }

            // Enable the first direction and reset the parking area count
            if (directions.Length > 0)
            {
                currentDirectionIndex = 0;
                directions[currentDirectionIndex].gameObject.SetActive(true); // Enable the first direction
                currentParkingCount = 0; // Reset parking count for the first direction
            }
        }
        else
        {
            Debug.LogError("Not enough LevelData for the current level index.");
        }
    }

    // This method is called when a parking area is triggered
    public void ParkingAreaTriggered()
    {
        currentParkingCount++;

        // Check if all parking areas in the current direction are completed
        if (currentParkingCount >= levelDataList[currentLevelIndex].parkingAreasPerDirection[currentDirectionIndex].parkingAreas)
        {
            DisableCurrentDirectionAndEnableNext();
        }
    }

    // Disable the current direction and enable the next one
    void DisableCurrentDirectionAndEnableNext()
    {
        directions[currentDirectionIndex].gameObject.SetActive(false); // Disable the current direction

        currentDirectionIndex++; // Move to the next direction
        currentParkingCount = 0; // Reset the parking count for the next direction

        if (currentDirectionIndex < directions.Length)
        {
            directions[currentDirectionIndex].gameObject.SetActive(true); // Enable the next direction
        }
        else
        {
            LoadNextLevel(); // If all directions are completed, move to the next level
        }
    }

    // Load the next level when the current level is completed
    void LoadNextLevel()
    {
        // Disable the current level
        levels[currentLevelIndex].gameObject.SetActive(false);

        // Move to the next level
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            // Enable the next level
            levels[currentLevelIndex].gameObject.SetActive(true);
            InitializeDirectionsForLevel(currentLevelIndex); // Initialize directions for the new level
        }
        else
        {
            Debug.Log("All levels completed!"); // All levels are completed
        }
    }
}
