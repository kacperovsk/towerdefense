using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorController : MonoBehaviour
{
    public bool testUnlockAllMaps = true; // ustaw true na czas testów

    private int currentMap = 0;
    private int lastMapIndex;
    public Image mapImage;
    public TextMeshProUGUI mapText;
    private string mapPath;
    Button leftButton;
    Button rightButton;
    Button loadButton;
    public TextMeshProUGUI highscoreText;

    string[] mapNames = {
        "Droga",
        "Las",
        "Pustynia",
        "Zima"
    };

    string[] mapScenes = {
        "GameMap0",
        "GameMap1"
        // DO ROZSZERZENIA JAK BEDZIE WIECEJ MAP
    };

    bool[] unlocked;
    void Start()
    {
        lastMapIndex = mapScenes.Length - 1;
        leftButton = GameObject.Find("ButtonBack").GetComponent<Button>();
        rightButton = GameObject.Find("ButtonNext").GetComponent<Button>();
        loadButton = GameObject.Find("ButtonMap").GetComponent<Button>();

        LoadUnlocked();
        LoadNext();
    }
    
    public void GoRightButton()
    {
        currentMap++;
        LoadNext();
    }
    
    public void GoLeftButton()
    {
        currentMap--;
        LoadNext();
    }

    void LoadUnlocked()
    {
        int count = mapNames.Length;
        unlocked = new bool[count];

        for (int i = 0; i < count; i++)
        {
            if (testUnlockAllMaps)
            {
                unlocked[i] = true; // wszystkie odblokowane w trybie test
            }
            else
            {
                // Mapa 0 zawsze odblokowana
                if (i == 0)
                    unlocked[i] = true;
                else
                    unlocked[i] = PlayerPrefs.GetInt($"Map{i}Unlocked", 0) == 1;
            }
        }
    }


    public void LoadMapButton()
    {
        if (!unlocked[currentMap])
        {
            Debug.Log("Ta mapa nie jest odblokowana");
            return;
        }
        if (mapScenes[currentMap] != null)
        {
            SceneManager.LoadScene(mapScenes[currentMap]);
        }
        else
        {
            Debug.Log($"Brakuje sceny mapy nr.{currentMap}");
        }
    }

    public void GoToMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
     
    void LoadNext() // Wczytuje wszystkie wartosci dla danej mapy.
    {
        mapText.text = mapNames[currentMap];
        mapPath = $"Maps/mapa{currentMap}";
        Sprite s = Resources.Load<Sprite>(mapPath);

        if (s != null)
        {
            mapImage.sprite = s;
        }
        else
        {
            mapImage.sprite = null;
            Debug.Log($"Nie znaleziono mapy: [{mapPath}]");
        }

        if (highscoreText != null)
        {
            string sceneName = mapScenes[currentMap];
            int highscore = PlayerPrefs.GetInt(sceneName + "_Highscore", 0);
            highscoreText.text = $"Highscore: {highscore}";
        }

        // Wylaczanie przyciskow na skrajach.
        leftButton.interactable = currentMap>=1;
        rightButton.interactable = currentMap<lastMapIndex;

        loadButton.interactable = unlocked[currentMap];
        mapImage.color = unlocked[currentMap] ? Color.white : Color.gray;
    }
}
