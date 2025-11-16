using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
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
            pauseMenu.SetActive(false); // ukryj menu
        }
        else
        {
            // Pauza
            Time.timeScale = 0f;
            isPaused = true;
            pauseMenu.SetActive(true); // poka¿ menu
        }
    }
}
