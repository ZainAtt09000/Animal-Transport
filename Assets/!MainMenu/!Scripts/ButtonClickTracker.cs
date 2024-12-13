using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickTracker : MonoBehaviour
{

    public void TrackButtonClick(string eventName)
    {
        // Log an event to Firebase Analytics with the configurable event name
        FirebaseAnalytics.LogEvent(eventName, new Parameter("_button_click_", eventName));
    }
}
