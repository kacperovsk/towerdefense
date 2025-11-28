using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorController : MonoBehaviour
{
    private int currentMap = 0;
    private int lastMapIndex = 3;
    public Image mapImage;
    public TextMeshProUGUI mapText;
    private string mapPath;
    Button leftButton;
    Button rightButton;

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
    void Start()
    {
        leftButton = GameObject.Find("ButtonBack").GetComponent<Button>();
        rightButton = GameObject.Find("ButtonNext").GetComponent<Button>();

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

    public void LoadMapButton()
    {
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

        // Wylaczanie przyciskow na skrajach.
        leftButton.interactable = currentMap>=1;
        rightButton.interactable = currentMap<lastMapIndex;
    }
}
