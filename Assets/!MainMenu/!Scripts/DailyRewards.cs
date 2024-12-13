using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewards : MonoBehaviour
{
    [Serializable]
    public class Reward
    {
        public int gold;
        public int diamonds;
        public Image claimedImage;
        public Image claimImage;
        public Image lockedImage;
        public Button claimButton;
        public Text timerText;
        public RewardState state;
    }

    public enum RewardState { Claimable, Claimed, Locked }

    public Reward[] rewards;
    public int currentDay;
    [SerializeField]
    private long lastClaimTimestamp;
    int currentGold;
    int currentDiamonds;
    private void OnEnable()
    {
        // Retrieve current player currency values
        currentGold = PlayerPrefs.GetInt("PlayerGold", 0);
        currentDiamonds = PlayerPrefs.GetInt("PlayerDiamond", 0);

        InitializeRewards();
    }

    void InitializeRewards()
    {
        // Load current day and timestamp from PlayerPrefs
        currentDay = PlayerPrefs.GetInt("CurrentDay", 0);
        lastClaimTimestamp = long.Parse(PlayerPrefs.GetString("LastClaimTimestamp", "0"));

        // Convert last claim timestamp to DateTime
        DateTime lastClaimDateTime = DateTimeOffset.FromUnixTimeSeconds(lastClaimTimestamp).DateTime;

        // Update visuals for each reward based on state
        UpdateRewardUI();

        // Check if the cooldown has elapsed since the last claim
        if (DateTime.Now > lastClaimDateTime.AddDays(1))
        {
            // A full day has passed, so we can advance the reward
            if (currentDay < rewards.Length)
            {
                // Only update the UI to show the correct reward states, but do not update currentDay yet
                UpdateRewardUI(); // This will show the first reward as locked with the timer
            }
        }
        else
        {
            // If a day hasn't passed yet, keep the current reward locked
            UpdateRewardUI();
        }

        // If the current day reward is locked, start the timer
        if (currentDay < rewards.Length && rewards[currentDay].state == RewardState.Locked)
        {
            StartCoroutine(StartRewardTimer(rewards[currentDay]));
        }
    }

    void UpdateRewardUI()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (i < currentDay)
            {
                // All previous days should be marked as claimed
                SetRewardState(rewards[i], RewardState.Claimed);
            }
            else if (i == currentDay)
            {
                // The current day reward should be locked until the timer is up
                if (DateTime.Now > DateTimeOffset.FromUnixTimeSeconds(lastClaimTimestamp).DateTime.AddDays(1))
                {
                    // After the cooldown, this reward becomes claimable
                    SetRewardState(rewards[i], RewardState.Claimable);
                }
                else
                {
                    // Otherwise, it remains locked with a timer
                    SetRewardState(rewards[i], RewardState.Locked);
                }
            }
            else
            {
                // Future rewards should be locked and have no timer
                SetRewardState(rewards[i], RewardState.Locked);
                rewards[i].timerText.gameObject.SetActive(false); // Disable timer text for rewards not yet unlocked
            }
        }
    }


    void SetRewardState(Reward reward, RewardState state)
    {
        reward.state = state;
        reward.claimedImage.gameObject.SetActive(state == RewardState.Claimed);
        reward.claimImage.gameObject.SetActive(state == RewardState.Claimable);
        reward.lockedImage.gameObject.SetActive(state == RewardState.Locked);
        reward.claimButton.gameObject.SetActive(state == RewardState.Claimable);
        reward.timerText.gameObject.SetActive(state == RewardState.Locked && reward == rewards[currentDay]);
    }

    public void ClaimReward(int index)
    {
        if (rewards[index].state == RewardState.Claimable)
        {
            Debug.Log($"Reward claimed! Gold: {rewards[index].gold}, Diamonds: {rewards[index].diamonds}");
            // Retrieve current player currency values
            currentGold = PlayerPrefs.GetInt("PlayerGold");
            currentDiamonds = PlayerPrefs.GetInt("PlayerDiamond");
            // Add the reward values for the claimed day to the player's currency
            int newGold = currentGold + rewards[index].gold;
            int newDiamonds = currentDiamonds + rewards[index].diamonds;

            // Save the updated currency values
            PlayerPrefs.SetInt("PlayerGold", newGold);
            PlayerPrefs.SetInt("PlayerDiamond", newDiamonds);

            // Update reward state and UI
            SetRewardState(rewards[index], RewardState.Claimed);

            // Increment the day only after claiming a reward
            currentDay = index + 1;

            // Check if we've reached the end of the reward cycle
            if (currentDay >= rewards.Length)
            {
                // Reset to the first reward
                currentDay = 0;
            }

            PlayerPrefs.SetInt("CurrentDay", currentDay);

            // Update last claim timestamp
            lastClaimTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            PlayerPrefs.SetString("LastClaimTimestamp", lastClaimTimestamp.ToString());

            // Refresh UI and start timer for the next reward
            InitializeRewards();

            // Update main menu or any UI showing currency
            MainMenuController.instance.UpdateCurrency();
        }
    }


    IEnumerator StartRewardTimer(Reward reward)
    {
        DateTime lastClaimDateTime = DateTimeOffset.FromUnixTimeSeconds(lastClaimTimestamp).DateTime;
        DateTime unlockTime = lastClaimDateTime.AddDays(1); // Set the unlock time to 24 hours after the last claim

        // Update the timer text until the reward is claimable
        while (DateTime.Now < unlockTime)
        {
            TimeSpan remaining = unlockTime - DateTime.Now;
            reward.timerText.text = $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            yield return new WaitForSeconds(1);
        }

        // Once the timer is up, transition the current reward to claimable
        SetRewardState(rewards[currentDay], RewardState.Claimable);
    }

    void SkipTime()
    {
        // Calculate the remaining time in the current day
        DateTime lastClaimDateTime = DateTimeOffset.FromUnixTimeSeconds(lastClaimTimestamp).DateTime;
        DateTime unlockTime = lastClaimDateTime.AddDays(1);  // Unlock time is 24 hours after the last claim

        // Set the new time to 10 seconds remaining
        DateTime newTime = unlockTime.AddSeconds(-10); // Set remaining time to 10 seconds
        lastClaimTimestamp = new DateTimeOffset(newTime.AddDays(-1)).ToUnixTimeSeconds(); // Update lastClaimTimestamp

        PlayerPrefs.SetString("LastClaimTimestamp", lastClaimTimestamp.ToString());

        // Immediately update the UI to reflect the changes
        UpdateRewardUI();

        // Start or restart the timer to count down from 10 seconds smoothly
        if (currentDay < rewards.Length && rewards[currentDay].state == RewardState.Locked)
        {
            StartCoroutine(SmoothCountdown(rewards[currentDay], TimeSpan.FromSeconds(1)));
        }
    }

    // A modified timer coroutine to handle a smooth 10-second countdown
    IEnumerator SmoothCountdown(Reward reward, TimeSpan countdownTime)
    {
        DateTime targetTime = DateTime.Now.Add(countdownTime);

        // Update the timer text until the reward is claimable
        while (DateTime.Now < targetTime)
        {
            TimeSpan remaining = targetTime - DateTime.Now;
            reward.timerText.text = $"{remaining.Seconds:D2}:{remaining.Milliseconds / 10:D2}"; // Showing seconds and 1/100th seconds for smoothness
            yield return null;
        }

        // Once the timer is up, make the reward claimable
        SetRewardState(rewards[currentDay], RewardState.Claimable);
        reward.timerText.text = "00:00"; // Clear the timer text
    }


    void Update()
    {
        // Check for key press (S key in this example)
        if (Input.GetKeyDown(KeyCode.S))
        {
            SkipTime();
        }
    }

}
