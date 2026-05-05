using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelCompletePopup : MonoBehaviour
{
    public GameObject gameOverPanel;

    public TMP_Text healthPercentText;
    public TMP_Text resultTitleText;
    public TMP_Text resultMessageText;

    public GameObject badgeImage;

    public Image treeImage;
    public Sprite poorBaobabSprite;
    public Sprite goodBaobabSprite;
    public Sprite betterBaobabSprite;
    public Sprite glowingBaobabSprite;

    public Button backToHubButton;
    public Button continueButton;

    public string currentLevelScene = "GrooveofAwareness";
    public string mainHubScene = "MainHubScene";
    public string nextLevelScene = "RiverOfNourishment";

    public string badgeKey = "Badge_GrooveofAwareness";

    public int unlockPercent = 80;
    public int badgePercent = 95;

    private int currentResultPercent;

    const string KEY_SCORE = "EnergyPoints";     // score points from food
    const string KEY_HEALTH = "HealthPoints";   // health percentage
    const int MAX_SCORE = 5000;

    void Awake()
    {
        if (gameOverPanel == null)
            gameOverPanel = gameObject;
    }

    void Start()
    {
        HidePopup();

        if (backToHubButton != null)
            backToHubButton.onClick.AddListener(BackToHub);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueNextLevel);
    }

    public void HidePopup()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowPopup()
    {
        // Get score points from gameplay
        int scorePoints = PlayerPrefs.GetInt(KEY_SCORE, 0);
        scorePoints = Mathf.Clamp(scorePoints, 0, MAX_SCORE);

        // Convert score to health percentage
        currentResultPercent = Mathf.RoundToInt((scorePoints / (float)MAX_SCORE) * 100f);
        currentResultPercent = Mathf.Clamp(currentResultPercent, 0, 100);

        // Save health percentage for MainHub/tree/health bar
        PlayerPrefs.SetInt(KEY_HEALTH, currentResultPercent);
        PlayerPrefs.Save();

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (healthPercentText != null)
            healthPercentText.text = currentResultPercent + "%";

        UpdateTreeImage(currentResultPercent);

        if (backToHubButton != null)
            backToHubButton.gameObject.SetActive(true);

        if (badgeImage != null)
            badgeImage.SetActive(currentResultPercent >= badgePercent);

        if (currentResultPercent < unlockPercent)
        {
            if (resultTitleText != null)
                resultTitleText.text = "Keep trying!";

            if (resultMessageText != null)
                resultMessageText.text = "Reach 80% to unlock the next level.";

            if (continueButton != null)
                continueButton.gameObject.SetActive(false);
        }
        else if (currentResultPercent < badgePercent)
        {
            if (resultTitleText != null)
                resultTitleText.text = "Great job!";

            if (resultMessageText != null)
                resultMessageText.text = "Next level unlocked.";

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
        else
        {
            if (resultTitleText != null)
                resultTitleText.text = "Excellent!";

            if (resultMessageText != null)
                resultMessageText.text = "Badge earned! Your Baobab is flourishing.";

            PlayerPrefs.SetInt(badgeKey, 1);
            PlayerPrefs.Save();

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
    }

    void UpdateTreeImage(int healthPercent)
    {
        if (treeImage == null) return;

        if (healthPercent >= 95)
            treeImage.sprite = glowingBaobabSprite;
        else if (healthPercent >= 80)
            treeImage.sprite = betterBaobabSprite;
        else if (healthPercent >= 60)
            treeImage.sprite = goodBaobabSprite;
        else
            treeImage.sprite = poorBaobabSprite;

        treeImage.color = Color.white;
        treeImage.preserveAspect = true;
    }

    void BackToHub()
    {
        Time.timeScale = 1f;

        if (currentResultPercent < unlockPercent)
        {
            PlayerPrefs.SetInt(KEY_SCORE, 0);
            PlayerPrefs.SetInt(KEY_HEALTH, 0);
            PlayerPrefs.SetInt("StarterScore", 0);

            PlayerPrefs.SetInt("IsInGame", 1);
            PlayerPrefs.SetString("CurrentLevelScene", currentLevelScene);
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene(mainHubScene);
    }

    void ContinueNextLevel()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt("IsInGame", 1);
        PlayerPrefs.SetString("CurrentLevelScene", nextLevelScene);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextLevelScene);
    }
}