using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [Header("Player Info")]
    public TMP_InputField nameInput;

    [Header("Avatar System")]
    public Image[] avatarImages;      // Drag the UI Image components here
    public Sprite[] avatarSprites;    // Drag your sprites here
    public Image previewImage;        // The large preview image

    private int selectedAvatarIndex = 0;

    [Header("Audio")]
    public Toggle soundToggle;
    public Toggle musicToggle;

    [Header("Buttons")]
    public Button saveButton;
    public Button backButton;

    // PlayerPrefs Keys
    const string KEY_NAME = "PlayerName";
    const string KEY_AVATAR = "AvatarIndex";
    const string KEY_SOUND = "SoundOn";
    const string KEY_MUSIC = "MusicOn";

    void Start()
    {
        LoadData();
        SetupAvatarClicks();
        UpdateAllAvatarDisplays();

        if (saveButton != null)
            saveButton.onClick.AddListener(SaveAndGoToMainHub);

        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMainHub);
    }

    void SetupAvatarClicks()
    {
        for (int i = 0; i < avatarImages.Length && i < avatarSprites.Length; i++)
        {
            int index = i; // Capture index for click event

            // Set the sprite on the UI Image
            if (avatarImages[i] != null && avatarSprites[i] != null)
            {
                avatarImages[i].sprite = avatarSprites[i];
            }
            
            // Make it clickable
            Button btn = avatarImages[i].GetComponent<Button>();
            if (btn == null)
                btn = avatarImages[i].gameObject.AddComponent<Button>();
            
            // Remove existing listeners and add new one
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectAvatar(index));
            
            // Add visual feedback
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            btn.colors = colors;
        }
    }

    void SelectAvatar(int index)
    {
        selectedAvatarIndex = index;
        UpdatePreview();
        Debug.Log("Avatar selected: " + index + " - " + avatarSprites[index].name);
    }

    void UpdatePreview()
    {
        if (previewImage != null && avatarSprites.Length > 0)
        {
            selectedAvatarIndex = Mathf.Clamp(selectedAvatarIndex, 0, avatarSprites.Length - 1);
            previewImage.sprite = avatarSprites[selectedAvatarIndex];
        }
    }

    void UpdateAllAvatarDisplays()
    {
        // Update all avatar images to show their sprites
        for (int i = 0; i < avatarImages.Length && i < avatarSprites.Length; i++)
        {
            if (avatarImages[i] != null && avatarSprites[i] != null)
            {
                avatarImages[i].sprite = avatarSprites[i];
            }
        }
        UpdatePreview();
    }

    void LoadData()
    {
        // Load name
        if (nameInput != null)
            nameInput.text = PlayerPrefs.GetString(KEY_NAME, "Player");

        // Load avatar index
        selectedAvatarIndex = PlayerPrefs.GetInt(KEY_AVATAR, 0);
        
        // Load audio settings
        if (soundToggle != null)
            soundToggle.isOn = PlayerPrefs.GetInt(KEY_SOUND, 1) == 1;
        
        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
    }

    void SaveAndGoToMainHub()
    {
        // Save all settings
        PlayerPrefs.SetString(KEY_NAME, nameInput.text);
        PlayerPrefs.SetInt(KEY_AVATAR, selectedAvatarIndex);
        PlayerPrefs.SetInt(KEY_SOUND, soundToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(KEY_MUSIC, musicToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Saved - Name: " + nameInput.text + ", Avatar Index: " + selectedAvatarIndex);
        
        // Load MainHubScene (correct scene name)
        SceneManager.LoadScene("MainHubScene");
    }

    void GoBackToMainHub()
    {
        Debug.Log("Going back without saving changes");
        
        // Load MainHubScene (correct scene name)
        SceneManager.LoadScene("MainHubScene");
    }
}