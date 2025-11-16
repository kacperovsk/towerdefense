using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Tower towerPrefab; // prefab przypisany w inspectorze

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Jak jest jakas aktywna (wybrana) to nic nie rób
        TowerClickHandler clickHandler = FindAnyObjectByType<TowerClickHandler>();
        if (clickHandler != null && clickHandler.activeTower != null)
            return;
        if (towerPrefab != null && StatsWindow.Instance != null)
        {
            StatsWindow.Instance.gameObject.SetActive(true);
            StatsWindow.Instance.UpdateStats(towerPrefab.GetStats());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Jak jest jakas aktywna (wybrana) to nic nie rób
        TowerClickHandler clickHandler = FindAnyObjectByType<TowerClickHandler>();
        if (clickHandler != null && clickHandler.activeTower != null)
            return;
        if (StatsWindow.Instance != null)
        {
            StatsWindow.Instance.gameObject.SetActive(false);
        }
    }
}
