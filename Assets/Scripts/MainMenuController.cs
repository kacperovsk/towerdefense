using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    
    public void StartGame()
    {
        // Tymczasowe, potem do zastapienia jakims wyborem map.
        SceneManager.LoadScene("GameMap1");
    }

    public void QuitGame()
    {
        // Zadziala tylko w faktycznej gierce, w unity nie.
        Application.Quit();
    }
}
