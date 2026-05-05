using UnityEngine;
using TMPro;

public class GameTimeManager : MonoBehaviour
{
    [Header("Timer")]
    public float gameDuration = 600f;
    private float remainingTime;

    [Header("UI")]
    public TMP_Text timeText;

    [Header("References")]
    public RewardManager rewardManager;
    public LevelCompletePopup levelCompletePopup;
    public EducationManager educationManager;

    [Header("Rules")]
    public int unlockPercent = 80;
    public int badgePercent = 95;

    [Header("Level Keys")]
    public string timerKey = "GrooveRemainingTime";
    public string scoreKey = "GrooveScorePoints";
    public string finalPercentKey = "GrooveFinalPercent";
    public string badgeKey = "Badge_GrooveofAwareness";

    private bool gameEnded = false;

    const string KEY_LEVEL = "LearningLevel";
    const string KEY_COMPLETED = "CompletedLevels";
    const string KEY_HEALTH = "HealthPoints";

    void Start()
    {
        Time.timeScale = 1f;
        gameEnded = false;

        remainingTime = PlayerPrefs.GetFloat(timerKey, gameDuration);

        if (levelCompletePopup != null)
            levelCompletePopup.HidePopup();

        if (educationManager == null)
            educationManager = FindObjectOfType<EducationManager>();

        UpdateTimeUI();
    }

    void Update()
    {
        if (gameEnded) return;

        if (Time.timeScale > 0f)
        {
            remainingTime -= Time.deltaTime;
            PlayerPrefs.SetFloat(timerKey, remainingTime);
        }

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        if (timeText != null)
            timeText.text = "TIME: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;

        int scorePoints = 0;

        if (rewardManager != null)
            scorePoints = rewardManager.GetScorePoints();

        PlayerPrefs.SetInt(scoreKey, scorePoints);
        PlayerPrefs.SetInt(KEY_HEALTH, scorePoints);

        int healthPercent = Mathf.RoundToInt((scorePoints / (float)RewardManager.MAX_SCORE) * 100f);
        PlayerPrefs.SetInt(finalPercentKey, healthPercent);

        PlayerPrefs.DeleteKey(timerKey);

        if (healthPercent >= unlockPercent)
        {
            int level = PlayerPrefs.GetInt(KEY_LEVEL, 1);
            int completed = PlayerPrefs.GetInt(KEY_COMPLETED, 0);

            PlayerPrefs.SetInt(KEY_LEVEL, level + 1);
            PlayerPrefs.SetInt(KEY_COMPLETED, completed + 1);
        }

        if (healthPercent >= badgePercent)
            PlayerPrefs.SetInt(badgeKey, 1);

        PlayerPrefs.Save();

        if (educationManager != null)
            educationManager.ShowNutritionAtEnd();
        else if (levelCompletePopup != null)
            levelCompletePopup.ShowPopup();
    }

    public void ShowLevelCompleteAfterNutrition()
    {
        if (levelCompletePopup != null)
            levelCompletePopup.ShowPopup();
    }
}