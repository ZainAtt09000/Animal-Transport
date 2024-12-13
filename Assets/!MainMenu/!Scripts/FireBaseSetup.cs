using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class FireBaseSetup : MonoBehaviour
{
    private FirebaseApp app;

    void Start()
    {
        // Check and fix Firebase dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // Initialize Firebase
                app = FirebaseApp.DefaultInstance;

                // Enable Firebase Analytics
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Firebase initialized and analytics collection enabled.");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
}
