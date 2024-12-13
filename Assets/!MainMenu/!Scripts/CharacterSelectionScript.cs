using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionScript : MonoBehaviour
{
    [System.Serializable]
    public class Characters
    {
        public Image characterImage;
        public Sprite userProfileImage;
        public Button selectButton;
        public GameObject checkImage;
    }

    [Header("User Profile")]
    public Image userProfileImage;
    public InputField userName;
    public Text UserProfileName;
    public Button editNameButton;
    public Button nameSaveButton;

    [Header("Characters Selection")]
    public Characters[] availableCharacters;
    public Button characterSaveButton;

    [Header("MainMenu UserIcon & Name")]
    public Image userImage;
    public Text usersName;

    private int selectedCharacterIndex = -1;
    private string savedUserName;

    void Start()
    {
       

        LoadUserData();
        SetupCharacterSelection();
        userName.onValueChanged.AddListener(OnUserNameChanged);

        // Add listeners to the buttons
        editNameButton.onClick.AddListener(OnEditName);
        nameSaveButton.onClick.AddListener(SaveUserName);
        characterSaveButton.onClick.AddListener(SaveSelectedCharacter);

        // Hide the input field and save buttons initially
        userName.gameObject.SetActive(false);
        characterSaveButton.gameObject.SetActive(false);
        nameSaveButton.gameObject.SetActive(false);
       // PlayerPrefs.SetInt("FirstTime", 0);

        if (PlayerPrefs.GetInt("FirstTime", 0) == 0)
        {
            this.gameObject.SetActive(false);
            Invoke(nameof(ShowPanelAfter2Seconds), 1.7f);
            PlayerPrefs.SetInt("FirstTime", 1);

            // Save the updated currency values
            PlayerPrefs.SetInt("PlayerGold", 500);
            PlayerPrefs.SetInt("PlayerDiamond", 25);

        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    void ShowPanelAfter2Seconds()
    {
        this.gameObject.SetActive(true);
    }
    void LoadUserData()
    {
        selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);
        savedUserName = PlayerPrefs.GetString("UserName", "Guest");

        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < availableCharacters.Length)
        {
            userProfileImage.sprite = availableCharacters[selectedCharacterIndex].userProfileImage;
            userImage.sprite = availableCharacters[selectedCharacterIndex].userProfileImage;
        }

        userName.text = savedUserName;
        usersName.text = savedUserName;
        UserProfileName.text = savedUserName;
    }

    void SetupCharacterSelection()
    {
        for (int i = 0; i < availableCharacters.Length; i++)
        {
            int index = i;
            availableCharacters[i].selectButton.onClick.AddListener(() => OnCharacterSelected(index));
            availableCharacters[i].checkImage.SetActive(i == selectedCharacterIndex);
        }
    }

    void OnCharacterSelected(int index)
    {
        if (index != selectedCharacterIndex)
        {
            selectedCharacterIndex = index;

            // Update check images to show only for the selected character
            for (int i = 0; i < availableCharacters.Length; i++)
            {
                availableCharacters[i].checkImage.SetActive(i == selectedCharacterIndex);
            }

            characterSaveButton.gameObject.SetActive(true); // Show save button only if change detected
        }
    }

    public void SaveSelectedCharacter()
    {
        if (selectedCharacterIndex >= 0)
        {
            Sprite selectedSprite = availableCharacters[selectedCharacterIndex].userProfileImage;
            userProfileImage.sprite = selectedSprite;
            userImage.sprite = selectedSprite;

            // Save selected character to PlayerPrefs
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
            PlayerPrefs.Save();

            characterSaveButton.gameObject.SetActive(false); // Hide save button after saving
        }
    }

    void OnUserNameChanged(string newName)
    {
        nameSaveButton.gameObject.SetActive(newName != savedUserName);
    }

    public void SaveUserName()
    {
        savedUserName = userName.text;
        usersName.text = savedUserName;
        UserProfileName.text = savedUserName;

        // Save name to PlayerPrefs
        PlayerPrefs.SetString("UserName", savedUserName);
        PlayerPrefs.Save();

        nameSaveButton.gameObject.SetActive(false); // Hide save button after saving
        userName.gameObject.SetActive(false); // Hide the input field after saving
    }

    public void OnEditName()
    {
        userName.gameObject.SetActive(true); // Show the input field for editing
        userName.ActivateInputField(); // Optional: set focus on the input field
    }

    public void OnCloseProfile()
    {
        // Reset changes if not saved
        userName.text = savedUserName;
        usersName.text = savedUserName;
        UserProfileName.text = savedUserName;

        if (selectedCharacterIndex >= 0)
        {
            Sprite selectedSprite = availableCharacters[selectedCharacterIndex].userProfileImage;
            userProfileImage.sprite = selectedSprite;
            userImage.sprite = selectedSprite;
        }

        characterSaveButton.gameObject.SetActive(false);
        nameSaveButton.gameObject.SetActive(false);
        userName.gameObject.SetActive(false); // Hide the input field when closing
    }
}
