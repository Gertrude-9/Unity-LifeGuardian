using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EducationManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text messageText;

    [Header("Buttons")]
    public Button closeButton;

    [Header("End Game Nutrition Message")]
    [TextArea(5, 15)]
    public string nutritionMessage;

    [Header("References")]
    public GameTimeManager gameTimeManager;

    private bool showLevelPopupAfterClose = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);

        if (gameTimeManager == null)
            gameTimeManager = FindObjectOfType<GameTimeManager>();
    }

    // Shows only when the game ends
    public void ShowNutritionAtEnd()
    {
        if (panel == null) return;

        showLevelPopupAfterClose = true;

        panel.SetActive(true);
        Time.timeScale = 0f;

        if (titleText != null)
            titleText.text = "Nutrition Lesson";

        if (messageText != null)
            messageText.text = nutritionMessage;
    }

    // Shows when player hits food marked Make Player Fall
    public void ShowBadFoodMessage(string message)
    {
        if (panel == null) return;

        showLevelPopupAfterClose = false;

        panel.SetActive(true);

        // Do not pause because Remy must fall and stand up
        Time.timeScale = 1f;

        if (titleText != null)
            titleText.text = "Food Health Warning";

        if (messageText != null)
            messageText.text = message;
    }

    // Hide when Remy stands up
    public void HideBadFoodMessage()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ClosePopup()
    {
        if (panel != null)
            panel.SetActive(false);

        if (showLevelPopupAfterClose)
        {
            showLevelPopupAfterClose = false;

            if (gameTimeManager == null)
                gameTimeManager = FindObjectOfType<GameTimeManager>();

            if (gameTimeManager != null)
                gameTimeManager.ShowLevelCompleteAfterNutrition();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}