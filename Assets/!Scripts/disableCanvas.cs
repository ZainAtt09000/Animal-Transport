using System.Collections;
using UnityEngine;

public class disableCanvas : MonoBehaviour
{
    public GameObject canvas; // Reference to the Canvas GameObject
    public float disableDuration = 10f; // Duration for which the Canvas will remain disabled
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with is tagged as "ParkingArea"
        if (other.CompareTag("ParkingArea"))
        {
            // Stop the player movement and freeze the Rigidbody
            StopAndFreezePlayerMovement();

            // Start the coroutine to disable and re-enable the Canvas after a delay
            StartCoroutine(DisableAndEnableCanvas());

            // Notify LevelManager that a parking area has been triggered
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.ParkingAreaTriggered();
            }
        }
    }

    public void StopAndFreezePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            // Freeze the player's movement
            playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void UnfreezePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            // Unfreeze the player's movement
            playerRigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    private IEnumerator DisableAndEnableCanvas()
    {
        // Disable the Canvas
        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        // Wait for the specified duration
        yield return new WaitForSeconds(disableDuration);

        // Enable the Canvas after the delay
        if (canvas != null)
        {
            canvas.SetActive(true);
        }

        // Unfreeze the player's movement after the canvas is enabled
        UnfreezePlayerMovement();
    }
}
