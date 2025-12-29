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
    public TextMeshProUGUI descriptionText;
    public Image costIcon;
    public bool hoveredOver = false;
    public Button sellButton;
    private float sellRefundPercent = 0.5f; // 50%
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
        descriptionText.text = stats.towerDescription.ToString();
        if (hoveredOver)
        {
            costText.text = stats.cost.ToString();
            costText.gameObject.SetActive(true);
            costIcon.gameObject.SetActive(true);
            sellButton.gameObject.SetActive(false);
        }
        else
        {
            costText.gameObject.SetActive(false);
            costIcon.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(true);
        }
    }

    public void SellActiveTower()
    {
        if (TowerClickHandler.Instance == null || TowerClickHandler.Instance.activeTower == null)
            return;

        Tower tower = TowerClickHandler.Instance.activeTower;

        // Oddaj hajs
        if (GameManager.Instance != null)
        {
            int refund = Mathf.RoundToInt(tower.cost * sellRefundPercent);
            GameManager.Instance.AddMoney(refund);
        }

        // Niszczenie wie¿y
        Destroy(tower.gameObject);

        // Reset UI
        TowerClickHandler.Instance.activeTower = null;
        gameObject.SetActive(false);
    }
}
