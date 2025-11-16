using UnityEngine;
using TMPro; 

//Jak coś ten plik jest po to by łatwiej zarządzać elementami gry takimi jak pieniądze itp. które nie pasują do innych skryptów i są bardziej globalne
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Początkowa ilość pieniędzy gracza
    [SerializeField] private int currentMoney = 100;
    public TextMeshProUGUI LicznikPieniedzy;
    // Początkowa ilość życia gracza
    [Header("Life")]
    [SerializeField] private int playerLife = 100;
    public TextMeshProUGUI LicznikŻycia;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //Przy starcie gry:
        //Ustawia zdrowie na domyślne
        UpdateLifeUI();
        //Ustawia kase na domyślną
        UpdateMoneyUI();
    }

    // Dodawanie pieniędzy przy śmierci przeciwnika
    public void AddMoney(int amount)
    {
        if (amount < 0) return;
        currentMoney += amount;
        UpdateMoneyUI();
    }

    // Wydawanie pieniędzy na wieże
    public bool SpendMoney(int amount)
    {
        if (amount < 0) return false;

        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }

        Debug.Log("GameManager: Brak środków na koncie!");
        return false;
    }

    // Do wywołania w innych skryptach gdy jest to potrzebne
    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    // Do pokazania tego w UI
    private void UpdateMoneyUI()
    {
        if (LicznikPieniedzy != null)
        {
            LicznikPieniedzy.text = currentMoney.ToString();
        }
    }
    // Metoda tracenia życia
    public void LoseLife(int damageAmount)
    {
        playerLife -= damageAmount;
        UpdateLifeUI();

        if (playerLife <= 0)
        {
            // TO JEST KLUCZOWA LOGIKA "KONIEC GRY"
            Debug.Log("KONIEC GRY! Straciłeś wszystkie życia.");
            Time.timeScale = 0f; // Zatrzymuje czas gry, co jest najprostszym warunkiem przegranej
        }
    }

    // NOWE: Do pokazania stanu żyć w UI
    private void UpdateLifeUI()
    {
        if (LicznikŻycia != null)
        {
            LicznikŻycia.text = playerLife.ToString();
        }
    }
}