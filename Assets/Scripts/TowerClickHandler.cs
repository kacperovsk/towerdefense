using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour
{
    public Tower activeTower; // aktualnie zaznaczona wie¿a

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
            Tower clickedTower = hit.collider != null ? hit.collider.GetComponent<Tower>() : null;

            if (clickedTower != null)
            {
                // -2 godziny myslenia co bylo zepsute tutaj jest, prosze nie ruszac co by sie nie dzialo bo to moj cenny czas i nic mi go nie odda. w najgorszym wypadku zakomentowac i zostawic na samym dole.
                if (clickedTower.isGhost)
                    return;

                    // jeœli inna wie¿a by³a aktywna, ukryj jej zasiêg
                if (activeTower != null && activeTower != clickedTower)
                    activeTower.HideRange();

                // toggle klikniêtej wie¿y
                if (clickedTower == activeTower)
                {
                    clickedTower.HideRange();
                    activeTower = null;

                    // brak aktywnej wie¿y -> ukryj panel
                    if (StatsWindow.Instance != null)
                        StatsWindow.Instance.gameObject.SetActive(false);
                }
                else
                {
                    clickedTower.ShowRange();
                    activeTower = clickedTower;

                    // aktualizacja i pokaz panelu
                    TowerStats stats = clickedTower.GetStats();
                    if (StatsWindow.Instance != null)
                    {
                        StatsWindow.Instance.gameObject.SetActive(true);
                        StatsWindow.Instance.UpdateStats(stats);
                    }
                }
            }
            else
            {
                // klikniêto poza wie¿¹ – ukryj poprzedni¹
                if (activeTower != null)
                {
                    activeTower.HideRange();
                    activeTower = null;

                    // brak aktywnej wie¿y -> ukryj panel
                    if (StatsWindow.Instance != null)
                        StatsWindow.Instance.gameObject.SetActive(false);
                }
            }
        }
    }
}
