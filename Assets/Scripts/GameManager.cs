using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Jak coś ten plik jest po to by łatwiej zarządzać elementami gry takimi jak pieniądze itp. które nie pasują do innych skryptów i są bardziej globalne
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Początkowa ilość pieniędzy gracza
    [SerializeField] private int currentMoney = 250;
    public TextMeshProUGUI LicznikPieniedzy;
    [Header("Passive Gold")]
    public int passiveGoldIncome = 0;
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

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) // PAUZOWANIE
        {
            TogglePause();
        }
    }

    private void Start()
    {
        UpdateLifeUI();
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

        // Jeśli gracz ma mniej niż amount to zabieramy wszystko
        int moneyTaken = Mathf.Min(currentMoney, amount);

        currentMoney -= moneyTaken;
        UpdateMoneyUI();

        return moneyTaken > 0; // zwraca true jeśli cokolwiek zabrał
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
    public void GrantPassiveIncome()
    {
        if (passiveGoldIncome > 0)
        {
            AddMoney(passiveGoldIncome);
            Debug.Log("Passive gold granted: " + passiveGoldIncome);
        }
    }
    // Metoda tracenia życia
    public void LoseLife(int damageAmount)
    {
        playerLife -= damageAmount;
        UpdateLifeUI();

        if (playerLife <= 0)
        {
            ShowLoserPanel();
        }
    }

    // Do pokazania stanu żyć w UI
    private void UpdateLifeUI()
    {
        if (LicznikŻycia != null)
        {
            LicznikŻycia.text = playerLife.ToString();
        }
    }

    // SPEED CONTROLLER
    private bool isDoubleSpeed = false; // czy jest włączone x2 speed
    [SerializeField] private Button speedUpButton;
    public void ToggleDoubleSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;

        if (isDoubleSpeed)
        {
            Time.timeScale = 2f; // podwójna prędkość
            speedUpButton.image.color = Color.green;
        }
        else
        {
            Time.timeScale = 1f; // normalna prędkość
            speedUpButton.image.color = Color.white;
        }
    }

    // PAUZOWANIE GRY
    public bool isPaused = false;
    public GameObject pauseMenu;

    public void TogglePause()
    {
        if (isPaused)
        {
            // Wznawianie
            if (isDoubleSpeed)
                Time.timeScale = 2f;
            else
                Time.timeScale = 1f;
            isPaused = false;
            pauseMenu.SetActive(false); // ukryj menu
        }
        else
        {
            // Pauza
            Time.timeScale = 0f;
            isPaused = true;
            pauseMenu.SetActive(true); // pokaż menu
        }
    }

    public void GoToMenu()
    {
        ConfirmationMenu.Instance.Show(() =>
        {
            Time.timeScale = 1f;
            //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);   // podobno resetuje scene przy okazji zmienienia na inna.
            SceneManager.LoadScene("MainMenu");   // Wylaczone do testow.
        });
    }

    public void GoToMenuNoConfirm()
    {
            Time.timeScale = 1f;
            //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);   // podobno resetuje scene przy okazji zmienienia na inna.
            SceneManager.LoadScene("MainMenu");   // Wylaczone do testow.
    }

    // Tymczasowe rozwiazanie wielu skryptów do stawiania wież.
    [Header("Tower Placement Shared State")]
    public bool isPlacing = false;

    public GameObject winPanel;
    public GameObject losePanel;
    public void ShowWinnerPanel()
    {
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    public void ShowLoserPanel()
    {
        Time.timeScale = 0f;
        losePanel.SetActive(true);
    }
}