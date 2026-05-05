using UnityEngine;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Header("Score / Daily Energy")]
    public int score = 0;
    public TMP_Text scoreText;

    [Header("Health Percentage")]
    public TMP_Text healthText;

    const string KEY_ENERGY = "EnergyPoints";
    const string KEY_HEALTH = "HealthPoints";
    const string KEY_WELLNESS = "WellnessPoints";
    const string KEY_STARTER_SCORE = "StarterScore";

    public const int MAX_SCORE = 5000;

    void Start()
    {
        // Start level with food points from MainHub
        score = PlayerPrefs.GetInt(KEY_STARTER_SCORE, 0);
        score = Mathf.Clamp(score, 0, MAX_SCORE);

        // Reset starter so it does not repeat forever
        PlayerPrefs.SetInt(KEY_STARTER_SCORE, 0);

        SaveValues();
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score = Mathf.Clamp(score + amount, 0, MAX_SCORE);

        SaveValues();
        UpdateUI();
    }

    public void AddHealth(int amount)
    {
        // Keep this for compatibility with old scripts
        AddScore(amount);
    }

    void SaveValues()
    {
        // Daily Energy = current score points
        PlayerPrefs.SetInt(KEY_ENERGY, score);

        // Wellness Points = same as Daily Energy
        PlayerPrefs.SetInt(KEY_WELLNESS, score);

        // HealthPoints = score points from Groove
        // MainHub converts this into percentage
        PlayerPrefs.SetInt(KEY_HEALTH, score);

        PlayerPrefs.Save();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "DAILY ENERGY: " + score + " / " + MAX_SCORE;

        if (healthText != null)
            healthText.text = "HEALTH: " + GetHealthPercent() + "%";
    }

    public int GetHealthPercent()
    {
        return Mathf.RoundToInt((score / (float)MAX_SCORE) * 100f);
    }

    public int GetScorePoints()
    {
        return score;
    }

    public int GetScorePercent()
    {
        return GetHealthPercent();
    }

    public void ResetLevelPoints()
    {
        score = 0;

        PlayerPrefs.SetInt(KEY_ENERGY, 0);
        PlayerPrefs.SetInt(KEY_WELLNESS, 0);
        PlayerPrefs.SetInt(KEY_HEALTH, 0);
        PlayerPrefs.SetInt(KEY_STARTER_SCORE, 0);

        PlayerPrefs.Save();

        UpdateUI();
    }

    
}