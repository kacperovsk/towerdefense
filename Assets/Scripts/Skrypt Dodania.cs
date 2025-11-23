using UnityEngine;
using UnityEngine.InputSystem; 

public class SkryptDodania: MonoBehaviour
{
    public GameObject towerPrefab; 
    [Header("Tower Cost")]
    public float placementCheckRadius = 0.3f;
    public Color validColor = new Color(0.2f, 1f, 0.2f, 0.5f); // Green, semi-transparent
    public Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Red, semi-transparent
    private GameObject currentGhostTower;
    private bool isPlacing = false;
    private bool isValidPlacement = false;
    
    public void OnDodajButtonClick()
    {
        int towerCost = towerPrefab.GetComponent<Tower>().cost;
        if (isPlacing)
        {
            Debug.Log("Already in placement mode!");
            return;
        }
        if (GameManager.Instance != null && GameManager.Instance.GetCurrentMoney() < towerCost)
        {
            Debug.Log("Nie masz wystarczająco pieniędzy, aby rozpocząć stawianie!");
            return;
        }

        if (towerPrefab != null)
        {
            currentGhostTower = Instantiate(towerPrefab);
            Tower towerScript = currentGhostTower.GetComponent<Tower>(); // Wieża duch nie strzela
            if (towerScript != null)
            {
                towerScript.isGhost = true;

                //Pokazywanie range przy stawianiu
                towerScript.ShowRange();
            }
                

            if (currentGhostTower.GetComponent<SpriteRenderer>() == null)
            {
                Debug.LogError("Tower Prefab needs a SpriteRenderer component!");
                Destroy(currentGhostTower);
                return;
            }
            
            isPlacing = true;
            Debug.Log("Placement mode activated. Follow the mouse.");
        }
    }

    void Update()
    {
        
        if (isPlacing)
        {
            // Ustawienie kosztu wieży z prefaba.
            int towerCost = towerPrefab.GetComponent<Tower>().cost;

            // Get Mouse Position and set Z to 0
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = 0; 
            currentGhostTower.transform.position = mouseWorldPosition;

            // 1. --- VALIDATION CHECK ---
            //Czy miejsce jest poprawne?
            bool isPositionValid = CheckIfPlacementIsValid(mouseWorldPosition);
            // Czy dalej kasa się zgadza?
            bool hasEnoughMoney = GameManager.Instance != null && GameManager.Instance.GetCurrentMoney() >= towerCost;
            // 2 razy tak? Przechodzisz dalej
            isValidPlacement = isPositionValid && hasEnoughMoney;

            // 2. --- VISUAL FEEDBACK ---
            SetGhostColor(isValidPlacement ? validColor : invalidColor);

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Debug.Log("Placement canceled by right click");
                CleanupPlacement(); // już masz funkcję, która usuwa ghost i resetuje isPlacing
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Sprawdź czy można tu postawić wieżę
                if (isValidPlacement)
                {
                    if (GameManager.Instance.SpendMoney(towerCost)){
                    // Jak tak to postaw wieżę na stałe
                    GameObject towerParent = GameObject.Find("Towers");
                    GameObject placedTower = Instantiate(towerPrefab, mouseWorldPosition, Quaternion.identity, towerParent != null ? towerParent.transform : null); // Wieża dopiero po postawieniu strzela
                    Tower towerScript = placedTower.GetComponent<Tower>();
                    if (towerScript != null)
                        towerScript.isGhost = false;
                        towerScript.HideRange();

                        // I usuń wieżę widmo
                        CleanupPlacement();
                    Debug.Log("Tower placed successfully!");
                }
                else
                    {
                         // Ten fragment raczej nie zostanie nigdy wykorzystany, ale dodaje na wszelki wypadek gdybyśmy coś oprócz wież robili z kasą
                         // Bo gdyby jednocześnie gracz wydał kasę gdzie indziej to by się nie zgadzało
                         Debug.Log("Nie można postawić wieży: Brak środków (problem z walidacją)."); 
                         CleanupPlacement();
                    }
                }
                //Obsługa gdy nie można postawić
                else
                {
                    //Gdy nie ma kasy
                    if (!hasEnoughMoney)
                        {
                            Debug.Log("Nie można postawić: Za mało pieniędzy!");
                        }
                    else
                        {
                            // Jak nie to daj znać, że nie można tu postawić wieży bo koliduje z inną lub Pathpointem
                            Debug.Log("Cannot place here: Overlapping another tower or a Pathpoint.");
                        }
                }
            }
        }
    }

    private bool CheckIfPlacementIsValid(Vector3 position)
    {
        // BLOKADA UI
        if (IsUIInPlacementRadius(position))
            return false;

        // BLOKADA POZA KAMERĄ
        if (!IsPositionInsideCamera(position))
            return false;

        // SPRAWDZANIE FIZYKI — KOLIZJE TOWER / PATH
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, placementCheckRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == currentGhostTower)
                continue; // ignoruj własną wieżę

            // Kolizja z inną wieżą
            if (hit.gameObject.layer == LayerMask.NameToLayer("Tower"))
                return false;

            // Kolizja z drogą
            if (hit.gameObject.layer == LayerMask.NameToLayer("Path"))
                return false;
        }

        return true;
    }

    private bool IsUIInPlacementRadius(Vector3 position)
    {
        int points = 8; // liczba punktów w okręgu

        for (int i = 0; i < points; i++)
        {
            float angle = i * Mathf.PI * 2 / points;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * placementCheckRadius;
            Vector2 checkPos = Camera.main.WorldToScreenPoint(position + offset);

            var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventData.position = checkPos;

            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
                return true; // znalazło UI w promieniu
        }

        return false;
    }

    private bool IsPositionInsideCamera(Vector3 position)
    {
        int points = 8; // liczba punktów w okręgu

        for (int i = 0; i < points; i++)
        {
            float angle = i * Mathf.PI * 2 / points;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * placementCheckRadius;
            Vector3 checkPos = position + offset;
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(checkPos);

            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                return false; // punkt wychodzi poza ekran
        }

        return true; // wszystkie punkty mieszczą się w kamerze
    }

    private void SetGhostColor(Color color)
    {
        SpriteRenderer sr = currentGhostTower.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    private void CleanupPlacement()
    {
        if (currentGhostTower != null)
        {
            Destroy(currentGhostTower);
        }
        isPlacing = false;
    }
}