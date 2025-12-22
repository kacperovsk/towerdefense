using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject optionsPanel;
    public Toggle autoStartToggle;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            optionsPanel.SetActive(false);
        }
    }
    public void StartGame()
    {
        // Tymczasowe, potem do zastapienia jakims wyborem map.
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
        // do implementacji
    }
    public void CancelOptions()
    {
        optionsPanel.SetActive(false);
    }
}
