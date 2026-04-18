using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainHubManager : MonoBehaviour
{
    [Header("Player Info")]
    public Image avatarImage;           // The avatar display in MainHub
    public TMP_Text playerNameText;

    [Header("Stats Text")]
    public TMP_Text pointsText;
    public TMP_Text totalLevelsText;
    public TMP_Text completedLevelsText;

    [Header("Progress Bars")]
    public Slider healthBar;
    public Slider energyBar;
    public TMP_Text healthText;
    public TMP_Text energyText;

    [Header("Avatars")]
    public Sprite[] avatarSprites;      // Drag your sprites here (SAME ORDER as SettingsManager)

    [Header("Buttons")]
    public Button settingsButton;
    public Button launchLevelButton;

    // PlayerPrefs Keys (MUST match SettingsManager)
    const string KEY_NAME = "PlayerName";
    const string KEY_AVATAR = "AvatarIndex";
    const string KEY_POINTS = "PlayerPoints";
    const string KEY_TOTAL = "TotalLevels";
    const string KEY_COMPLETED = "CompletedLevels";
    const string KEY_HEALTH = "PlayerHealth";
    const string KEY_ENERGY = "PlayerEnergy";

    void Start()
    {
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (launchLevelButton != null)
            launchLevelButton.onClick.AddListener(OpenChoosePath);

        LoadAllData();
    }

    void OnEnable()
    {
        // Reload data every time this scene becomes active (when coming back from Settings)
        LoadAllData();
    }

    void LoadAllData()
    {
        LoadPlayerName();
        LoadAvatar();
        LoadPoints();
        LoadLevels();
        LoadHealth();
        LoadEnergy();
    }

    void LoadPlayerName()
    {
        string playerName = PlayerPrefs.GetString(KEY_NAME, "Player");
        if (playerNameText != null)
            playerNameText.text = playerName;
        Debug.Log("Loaded Name: " + playerName);
    }

    void LoadAvatar()
    {
        int avatarIndex = PlayerPrefs.GetInt(KEY_AVATAR, 0);
        Debug.Log("Loaded Avatar Index: " + avatarIndex);

        if (avatarSprites != null && avatarSprites.Length > 0 && avatarImage != null)
        {
            avatarIndex = Mathf.Clamp(avatarIndex, 0, avatarSprites.Length - 1);
            avatarImage.sprite = avatarSprites[avatarIndex];
            Debug.Log("Avatar set to: " + avatarSprites[avatarIndex].name);
        }
        else
        {
            if (avatarSprites == null) Debug.LogError("avatarSprites array is NULL! Assign sprites in Inspector.");
            else if (avatarSprites.Length == 0) Debug.LogError("avatarSprites array is EMPTY! Assign sprites in Inspector.");
            if (avatarImage == null) Debug.LogError("avatarImage reference is MISSING! Assign in Inspector.");
        }
    }

    void LoadPoints()
    {
        int points = PlayerPrefs.GetInt(KEY_POINTS, 0);
        if (pointsText != null)
            pointsText.text = "Points: " + points.ToString();
    }

    void LoadLevels()
    {
        int total = PlayerPrefs.GetInt(KEY_TOTAL, 0);
        int completed = PlayerPrefs.GetInt(KEY_COMPLETED, 0);
        
        if (totalLevelsText != null)
            totalLevelsText.text = "Levels: " + total.ToString();
        
        if (completedLevelsText != null)
            completedLevelsText.text = "Completed: " + completed.ToString();
    }

    void LoadHealth()
    {
        float health = PlayerPrefs.GetFloat(KEY_HEALTH, 100f);
        if (healthBar != null)
            healthBar.value = health / 100f;
        if (healthText != null)
            healthText.text = "Health: " + Mathf.RoundToInt(health) + "/100";
    }

    void LoadEnergy()
    {
        float energy = PlayerPrefs.GetFloat(KEY_ENERGY, 100f);
        if (energyBar != null)
            energyBar.value = energy / 100f;
        if (energyText != null)
            energyText.text = "Energy: " + Mathf.RoundToInt(energy) + "/100";
    }

    // Public method to manually refresh avatar (call if needed)
    public void RefreshAvatar()
    {
        LoadAvatar();
    }

    void OpenSettings()
    {
        // Load SettingsScene (make sure this matches your scene name)
        SceneManager.LoadScene("SettingsScene");
    }

    void OpenChoosePath()
    {
        // Load ChoosePathScene (make sure this matches your scene name)
        SceneManager.LoadScene("ChoosePathScene");
    }
}