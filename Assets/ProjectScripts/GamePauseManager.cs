using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button backToHubButton;
    public Button exitButton;

    [Header("Scene Names")]
    public string mainHubSceneName = "MainHubScene";

    const string KEY_CURRENT_LEVEL = "CurrentLevelScene";
    const string KEY_IS_PLAYING = "IsInGame";

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (backToHubButton != null)
            backToHubButton.onClick.AddListener(BackToMainHub);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Debug.Log("Game Resumed");
    }

    public void BackToMainHub()
    {
        Time.timeScale = 1f;
        isPaused = false;

        string currentScene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString(KEY_CURRENT_LEVEL, currentScene);
        PlayerPrefs.SetInt(KEY_IS_PLAYING, 1);
        PlayerPrefs.Save();

        Debug.Log("Saved current level: " + currentScene);

        SceneManager.LoadScene(mainHubSceneName);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // EXIT means start everything afresh
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Game reset completely");

        SceneManager.LoadScene(mainHubSceneName);
    }
}