using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainHubManager : MonoBehaviour
{
    [Header("Player Info")]
    public Image avatarImage;
    public TMP_Text playerNameText;

    [Header("Stats")]
    public TMP_Text pointsText;
    public TMP_Text totalLevelsText;
    public TMP_Text completedLevelsText;

    [Header("Bars")]
    public Slider healthBar;
    public Slider energyBar;
    public TMP_Text healthText;
    public TMP_Text energyText;

    [Header("Food UI")]
    public GameObject foodPanel;
    public TMP_InputField foodInput;
    public TMP_Text foodMessageText;
    public Button logMealButton;
    public Button submitFoodButton;
    public Button closeFoodButton;

    [Header("Food Prefabs")]
    public GameObject[] foodPrefabs;

    [Header("Avatars")]
    public Sprite[] avatarSprites;

    [Header("Buttons")]
    public Button settingsButton;
    public Button launchLevelButton;

    [Header("Scenes")]
    public string settingsSceneName = "SettingsScene";
    public string choosePathSceneName = "ChoosePathScene";

    [Header("Tree Display")]
    public Image treeImage;

    private Sprite poorTree;
    private Sprite goodTree;
    private Sprite betterTree;
    private Sprite glowingTree;

    const string KEY_NAME = "PlayerName";
    const string KEY_AVATAR = "AvatarIndex";

    // Saved from Groove of Awareness final score
    const string KEY_HEALTH = "HealthPoints";

    // Food score points
    const string KEY_ENERGY = "EnergyPoints";
    const string KEY_WELLNESS = "WellnessPoints";

    // Food points carried into the level as starting score
    const string KEY_STARTER_SCORE = "StarterScore";

    const string KEY_LEVEL = "LearningLevel";
    const string KEY_COMPLETED = "CompletedLevels";
    const string KEY_FOOD_LOG_COUNT = "FoodLogsThisSession";

    const int MAX_HEALTH_SCORE = 5000;
    const int MAX_ENERGY_POINTS = 5000;

    void Start()
    {
        LoadFoodPrefabs();

        poorTree = Resources.Load<Sprite>("Trees/poor");
        goodTree = Resources.Load<Sprite>("Trees/good");
        betterTree = Resources.Load<Sprite>("Trees/better");
        glowingTree = Resources.Load<Sprite>("Trees/glowing");

        if (!PlayerPrefs.HasKey(KEY_LEVEL))
            PlayerPrefs.SetInt(KEY_LEVEL, 1);

        if (!PlayerPrefs.HasKey(KEY_FOOD_LOG_COUNT))
            PlayerPrefs.SetInt(KEY_FOOD_LOG_COUNT, 0);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => SceneManager.LoadScene(settingsSceneName));

        if (launchLevelButton != null)
            launchLevelButton.onClick.AddListener(LaunchGame);

        if (logMealButton != null)
            logMealButton.onClick.AddListener(OpenFoodPanel);

        if (submitFoodButton != null)
            submitFoodButton.onClick.AddListener(LogFood);

        if (closeFoodButton != null)
            closeFoodButton.onClick.AddListener(CloseFoodPanel);

        if (foodPanel != null)
            foodPanel.SetActive(false);

        LoadAll();
    }

    void OnEnable()
    {
        LoadAll();
    }

    void LaunchGame()
    {
        string currentLevel = PlayerPrefs.GetString("CurrentLevelScene", "");

        if (!string.IsNullOrEmpty(currentLevel))
            SceneManager.LoadScene(currentLevel);
        else
            SceneManager.LoadScene(choosePathSceneName);
    }

    void LoadAll()
    {
        LoadName();
        LoadAvatar();
        LoadStats();
    }

    void LoadName()
    {
        if (playerNameText != null)
            playerNameText.text = PlayerPrefs.GetString(KEY_NAME, "Player");
    }

    void LoadAvatar()
    {
        int index = PlayerPrefs.GetInt(KEY_AVATAR, 0);

        if (avatarSprites != null && avatarSprites.Length > 0 && avatarImage != null)
        {
            index = Mathf.Clamp(index, 0, avatarSprites.Length - 1);
            avatarImage.sprite = avatarSprites[index];
        }
    }

    void LoadStats()
    {
        int healthScore = PlayerPrefs.GetInt(KEY_HEALTH, 0);
        int energyPoints = PlayerPrefs.GetInt(KEY_ENERGY, 0);
        int wellnessPoints = PlayerPrefs.GetInt(KEY_WELLNESS, 0);

        healthScore = Mathf.Clamp(healthScore, 0, MAX_HEALTH_SCORE);
        energyPoints = Mathf.Clamp(energyPoints, 0, MAX_ENERGY_POINTS);
        wellnessPoints = Mathf.Clamp(wellnessPoints, 0, MAX_ENERGY_POINTS);

        int healthPercent = Mathf.RoundToInt((healthScore / (float)MAX_HEALTH_SCORE) * 100f);

        if (pointsText != null)
            pointsText.text = "Wellness Points: " + wellnessPoints;

        if (healthBar != null)
        {
            healthBar.maxValue = 100;
            healthBar.value = healthPercent;
        }

        if (energyBar != null)
        {
            energyBar.maxValue = MAX_ENERGY_POINTS;
            energyBar.value = energyPoints;
        }

        if (healthText != null)
            healthText.text = "Health: " + healthPercent + "%";

        if (energyText != null)
            energyText.text = "Daily Energy: " + energyPoints + " pts";

        UpdateTree(healthPercent);
    }

    void UpdateTree(int healthPercent)
    {
        if (treeImage == null) return;

        if (healthPercent >= 95 && glowingTree != null)
            treeImage.sprite = glowingTree;
        else if (healthPercent >= 80 && betterTree != null)
            treeImage.sprite = betterTree;
        else if (healthPercent >= 60 && goodTree != null)
            treeImage.sprite = goodTree;
        else if (poorTree != null)
            treeImage.sprite = poorTree;
    }

    void OpenFoodPanel()
    {
        if (foodPanel != null)
            foodPanel.SetActive(true);

        if (foodInput != null)
            foodInput.text = "";

        if (foodMessageText != null)
            foodMessageText.text = "Enter food, for example matooke";
    }

    void CloseFoodPanel()
    {
        if (foodPanel != null)
            foodPanel.SetActive(false);
    }

    void LogFood()
    {
        if (foodInput == null) return;

        string input = foodInput.text.Trim();

        if (string.IsNullOrEmpty(input))
        {
            if (foodMessageText != null)
                foodMessageText.text = "Enter food name";
            return;
        }

        FoodItem food = FindFoodPrefab(input);

        if (food == null)
        {
            if (foodMessageText != null)
                foodMessageText.text = "Food not found";
            return;
        }

        int energyPoints = PlayerPrefs.GetInt(KEY_ENERGY, 0);
        int wellnessPoints = PlayerPrefs.GetInt(KEY_WELLNESS, 0);
        int starterScore = PlayerPrefs.GetInt(KEY_STARTER_SCORE, 0);

        // Food score points
        energyPoints += food.scoreValue;
        wellnessPoints += food.scoreValue;

        // This becomes starter score in the level
        starterScore += food.scoreValue;

        energyPoints = Mathf.Clamp(energyPoints, 0, MAX_ENERGY_POINTS);
        wellnessPoints = Mathf.Clamp(wellnessPoints, 0, MAX_ENERGY_POINTS);
        starterScore = Mathf.Clamp(starterScore, 0, MAX_ENERGY_POINTS);

        PlayerPrefs.SetInt(KEY_ENERGY, energyPoints);
        PlayerPrefs.SetInt(KEY_WELLNESS, wellnessPoints);
        PlayerPrefs.SetInt(KEY_STARTER_SCORE, starterScore);
        PlayerPrefs.Save();

        LoadStats();
        CloseFoodPanel();
    }

    FoodItem FindFoodPrefab(string name)
    {
        string typed = Normalize(name);

        foreach (GameObject prefab in foodPrefabs)
        {
            if (prefab == null) continue;

            FoodItem food = prefab.GetComponent<FoodItem>();

            if (food == null)
                food = prefab.GetComponentInChildren<FoodItem>(true);

            if (food == null) continue;

            string prefabName = Normalize(prefab.name);
            string foodName = Normalize(food.foodName);

            if (typed == prefabName || typed == foodName)
                return food;
        }

        return null;
    }

    void LoadFoodPrefabs()
    {
        foodPrefabs = Resources.LoadAll<GameObject>("Foods2");
        Debug.Log("Loaded food prefabs: " + foodPrefabs.Length);
    }

    string Normalize(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        value = value.ToLower();
        value = value.Replace("food_", "");
        value = value.Replace("food", "");
        value = value.Replace("_", "");
        value = value.Replace("-", "");
        value = value.Replace(" ", "");
        value = value.Replace("(clone)", "");

        return value.Trim();
    }
}