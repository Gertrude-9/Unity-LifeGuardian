using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Player Info")]
    public TMP_InputField nameInput;

    [Header("Avatar System")]
    public Image[] avatarImages;      // 4 avatar UI images
    public Image[] avatarFrames;      // 4 circle/frame images
    public Sprite[] avatarSprites;    // 4 avatar sprites
    public Image previewImage;

    private int selectedAvatarIndex = 0;

    [Header("Audio")]
    public Toggle soundToggle;
    public Toggle musicToggle;

    [Header("Buttons")]
    public Button saveButton;
    public Button backButton;

    const string KEY_NAME = "PlayerName";
    const string KEY_AVATAR = "AvatarIndex";
    const string KEY_SOUND = "SoundOn";
    const string KEY_MUSIC = "MusicOn";

    Color selectedColor = new Color(0.22f, 1f, 0.08f, 1f);   // bright green
    Color normalColor = new Color(0.25f, 0.45f, 0.25f, 1f);  // dim green

    void Start()
    {
        LoadData();
        SetupAvatarButtons();
        UpdateAvatarUI();

        if (saveButton != null)
            saveButton.onClick.AddListener(SaveAndGoToMainHub);

        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMainHub);
    }

    void SetupAvatarButtons()
    {
        int count = Mathf.Min(avatarImages.Length, avatarSprites.Length);

        for (int i = 0; i < count; i++)
        {
            int index = i;

            if (avatarImages[i] != null)
            {
                avatarImages[i].sprite = avatarSprites[i];

                Button btn = avatarImages[i].GetComponent<Button>();
                if (btn == null)
                    btn = avatarImages[i].gameObject.AddComponent<Button>();

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => SelectAvatar(index));
            }
        }
    }

    void SelectAvatar(int index)
    {
        selectedAvatarIndex = index;
        UpdateAvatarUI();

        Debug.Log("Avatar selected: " + selectedAvatarIndex);
    }

    void UpdateAvatarUI()
    {
        int count = Mathf.Min(avatarImages.Length, avatarSprites.Length);

        selectedAvatarIndex = Mathf.Clamp(selectedAvatarIndex, 0, count - 1);

        for (int i = 0; i < count; i++)
        {
            if (avatarImages[i] != null)
                avatarImages[i].sprite = avatarSprites[i];

            if (avatarFrames != null && i < avatarFrames.Length && avatarFrames[i] != null)
            {
                avatarFrames[i].color = i == selectedAvatarIndex ? selectedColor : normalColor;
            }
        }

        if (previewImage != null && count > 0)
            previewImage.sprite = avatarSprites[selectedAvatarIndex];
    }

    void LoadData()
    {
        if (nameInput != null)
            nameInput.text = PlayerPrefs.GetString(KEY_NAME, "Player");

        selectedAvatarIndex = PlayerPrefs.GetInt(KEY_AVATAR, 0);

        if (soundToggle != null)
            soundToggle.isOn = PlayerPrefs.GetInt(KEY_SOUND, 1) == 1;

        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
    }

    void SaveAndGoToMainHub()
    {
        if (nameInput != null)
            PlayerPrefs.SetString(KEY_NAME, nameInput.text);

        PlayerPrefs.SetInt(KEY_AVATAR, selectedAvatarIndex);

        if (soundToggle != null)
            PlayerPrefs.SetInt(KEY_SOUND, soundToggle.isOn ? 1 : 0);

        if (musicToggle != null)
            PlayerPrefs.SetInt(KEY_MUSIC, musicToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();

        SceneManager.LoadScene("MainHubScene");
    }

    void GoBackToMainHub()
    {
        SceneManager.LoadScene("MainHubScene");
    }
}