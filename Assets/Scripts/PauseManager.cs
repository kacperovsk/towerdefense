using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    public TextMeshProUGUI buttonText;
    public GameObject pauseMenu;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            // Wznawianie
            Time.timeScale = 1f;
            isPaused = false;
            buttonText.text = "||";
            pauseMenu.SetActive(false); // ukryj menu
        }
        else
        {
            // Pauza
            Time.timeScale = 0f;
            isPaused = true;
            buttonText.text = "||";
            pauseMenu.SetActive(true); // poka¿ menu
        }
    }
}
