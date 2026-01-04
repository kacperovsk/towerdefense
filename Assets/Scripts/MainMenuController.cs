using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject optionsPanel;
    public Toggle autoStartToggle;
    public Slider musicSlider;


    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelOptions();
            optionsPanel.SetActive(false);
        }
    }
    private void Start()
    {
        bool autoStart = PlayerPrefs.GetInt("AutoStartNextWave", 0) == 1;
        autoStartToggle.SetIsOnWithoutNotify(autoStart);

        if (musicSlider != null && MusicManager.Instance != null)
        {
            // Pobierz aktualn¹ g³oœnoœæ z MusicManager
            float currentVolume = MusicManager.Instance.source.volume;
            musicSlider.SetValueWithoutNotify(currentVolume);

            // Listener
            musicSlider.onValueChanged.AddListener(value =>
            {
                MusicManager.Instance.SetVolume(value); // zmiana na ¿ywo
            });
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }
    public void QuitGame()
    {
        // Zadziala tylko w faktycznej gierce, w unity nie.
        Application.Quit();
    }

    public void ConfirmOptions()
    {
        PlayerPrefs.SetInt("AutoStartNextWave", autoStartToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        PlayerPrefs.Save();
        MusicManager.Instance.SetVolume(musicSlider.value);
        optionsPanel.SetActive(false);
    }
    public void CancelOptions()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);

        musicSlider.SetValueWithoutNotify(savedVolume);
        AudioListener.volume = savedVolume;

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(savedVolume);

        optionsPanel.SetActive(false);
    }

    public void ResetData()
    {
        ConfirmationMenu.Instance.Show(() =>
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            float defaultVolume = 0.2f;
            musicSlider.SetValueWithoutNotify(defaultVolume);
            MusicManager.Instance.SetVolume(defaultVolume);
            AudioListener.volume = defaultVolume;

            autoStartToggle.SetIsOnWithoutNotify(false);
        });
    }
    public void OnMusicVolumeChanged()
    {
        MusicManager.Instance.SetVolume(musicSlider.value);
    }
}
