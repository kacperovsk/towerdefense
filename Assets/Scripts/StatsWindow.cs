using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsWindow : MonoBehaviour
{
    public static StatsWindow Instance;

    public Image towerIcon;
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackspeedText;
    public TextMeshProUGUI radiusText;
    public TextMeshProUGUI costText;
    public Image costIcon;
    public bool showCost = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // Aktualizuje dane wie¿y w panelu
    public void UpdateStats(TowerStats stats)
    {
        if (stats.towerIcon != null)
            towerIcon.sprite = stats.towerIcon;

        towerName.text = stats.towerName;
        damageText.text = $"Damage: {stats.damage}";
        attackspeedText.text = $"AS: {stats.fireRate}";
        radiusText.text = $"Range: {stats.range}";
        if (showCost)
        {
            costText.text = stats.cost.ToString();
            costText.gameObject.SetActive(true);
            costIcon.gameObject.SetActive(true);
        }
        else
        {
            costText.gameObject.SetActive(false);
            costIcon.gameObject.SetActive(false);
        }
    }
}
