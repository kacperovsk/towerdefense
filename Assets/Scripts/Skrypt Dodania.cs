using UnityEngine;
using UnityEngine.InputSystem; 

public class SkryptDodania: MonoBehaviour
{
    public GameObject towerPrefab; 
    [Header("Tower Cost")]
    public int towerCost = 50;
    public float placementCheckRadius = 1.0f;
    public Color validColor = new Color(0.2f, 1f, 0.2f, 0.5f); // Green, semi-transparent
    public Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Red, semi-transparent
    private GameObject currentGhostTower;
    private bool isPlacing = false;
    private bool isValidPlacement = false;
    public void OnDodajButtonClick()
    {
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
                towerScript.enabled = false;

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

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Sprawdź czy można tu postawić wieżę
                if (isValidPlacement)
                {
                    if (GameManager.Instance.SpendMoney(towerCost)){
                    // Jak tak to postaw wieżę na stałe
                    GameObject placedTower = Instantiate(towerPrefab, mouseWorldPosition, Quaternion.identity); // Wieża dopiero po postawieniu strzela
                    Tower towerScript = placedTower.GetComponent<Tower>();
                    if (towerScript != null)
                        towerScript.enabled = true;

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
        // Physics2D.OverlapCircle sprawdza czy jakiekolwiek kolidery znajdują się w określonym promieniu od pozycji.
        // Sprawdzamy tagi tego co się styka z wieżą widmo.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, placementCheckRadius);

        foreach (Collider2D collider in colliders)
        {
            // Tylko po to by nie kolidować z samym sobą
            if (collider.gameObject == currentGhostTower) 
                continue;

            // Check 1: Tower Overlap
            // Sprawdzamy czy nazwa obiektu zawiera nazwę prefaba wieży
            if (collider.gameObject.name.Contains(towerPrefab.name))
            {
                return false; // Jeśli tak to blokuje
            }

            // Check 2: Pathpoint Overlap
            // To samo ale sprawdzamy tag drogi
            if (collider.gameObject.CompareTag("Pathpoint"))
            {
                return false; // Tutaj też blokuje
            }
        }

        // Jak przeszło przez ify to znaczy, że można postawić wieżę
        return true;
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