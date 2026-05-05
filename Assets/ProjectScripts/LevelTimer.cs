using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float totalTime = 600f; // 10 minutes
    public TMP_Text timerText;

    public GameObject levelCompletePanel;

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        currentTime = totalTime;
        levelCompletePanel.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            EndLevel();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void EndLevel()
    {
        isRunning = false;
        Time.timeScale = 0f;

        levelCompletePanel.SetActive(true);
    }
}