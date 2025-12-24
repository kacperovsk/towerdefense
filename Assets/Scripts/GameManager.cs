using System.Collections;
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

    [Header("InfoTexts")]
    public TextMeshProUGUI placementText;
    public TextMeshProUGUI bossSpawnText;
    public TextMeshProUGUI goldText;

    [Header("UI")]
    public Slider musicSlider;
    public Toggle autoStartToggle;
    
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

        if(isPlacing)
            placementText.gameObject.SetActive(true);
        else
            placementText.gameObject.SetActive(false);

        goldText.text = "Gold per wave: " + passiveGoldIncome;
    }

    private void Start()
    {
        // Muzyka
        if (musicSlider != null && MusicManager.Instance != null)
        {
            musicSlider.SetValueWithoutNotify(MusicManager.Instance.source.volume);

            musicSlider.onValueChanged.AddListener(value =>
            {
                MusicManager.Instance.SetVolume(value); // zmiana na żywo
                PlayerPrefs.SetFloat("MusicVolume", value); // zapis do prefs zawsze
                PlayerPrefs.Save();
            });
        }

        // Toggle autostartu
        if (autoStartToggle != null)
        {
            bool autoStart = PlayerPrefs.GetInt("AutoStartNextWave", 0) == 1;
            autoStartToggle.SetIsOnWithoutNotify(autoStart);

            autoStartToggle.onValueChanged.AddListener(value =>
            {
                PlayerPrefs.SetInt("AutoStartNextWave", value ? 1 : 0);
                PlayerPrefs.Save();
            });
        }

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
            playerLife = 0; // zeby ladniej wygladalo. 0 na koniec.
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
            SceneManager.LoadScene("MainMenu");   // Wylaczone do testow.
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            PlayerPrefs.Save();
        });
    }

    public void GoToMenuNoConfirm()
    {
            Time.timeScale = 1f;
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

        scoreText.gameObject.SetActive(true);
        highscoreText.gameObject.SetActive(true);

        int finalScore = CalculateScore();
        string sceneName = SceneManager.GetActiveScene().name;

        int currentHighscore = PlayerPrefs.GetInt(sceneName + "_Highscore", 0);
        if (finalScore > currentHighscore)
        {
            PlayerPrefs.SetInt(sceneName + "_Highscore", finalScore);
            PlayerPrefs.Save();
        }

        if (scoreText != null)
            scoreText.text = $"Score: {finalScore}";
        if (highscoreText != null)
            highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(sceneName + "_Highscore", 0)}";
        UnlockNextMap();
    }

    public void ShowLoserPanel()
    {
        Time.timeScale = 0f;
        losePanel.SetActive(true);

        scoreText.gameObject.SetActive(true);
        highscoreText.gameObject.SetActive(true);

        int finalScore = CalculateScore();
        finalScore = finalScore / 2;    // na przegranej jest 0.5x pktow
        string sceneName = SceneManager.GetActiveScene().name;

        int currentHighscore = PlayerPrefs.GetInt(sceneName + "_Highscore", 0);

        if (finalScore > currentHighscore)
        {
            PlayerPrefs.SetInt(sceneName + "_Highscore", finalScore);
            PlayerPrefs.Save();
        }

        if (scoreText != null)
            scoreText.text = $"Score: {finalScore}";
        if (highscoreText != null)
            highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(sceneName + "_Highscore", 0)}";
    }

    private void UnlockNextMap()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        int currentIndex = -1;

        if (currentScene.StartsWith("GameMap"))
        {
            string number = currentScene.Substring("GameMap".Length);
            int.TryParse(number, out currentIndex);
        }

        if (currentIndex >= 0)
        {
            int nextIndex = currentIndex + 1;

            // zapisujemy odblokowanie następnej mapy w PlayerPrefs
            PlayerPrefs.SetInt($"Map{nextIndex}Unlocked", 1);
            PlayerPrefs.Save();

            Debug.Log($"Odblokowano mapę nr {nextIndex}");
        }
    }

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    private int totalEnemiesDefeated = 0;

    public void RegisterEnemyDefeat()
    {
        totalEnemiesDefeated++;
    }

    public int CalculateScore()
    {
        return (totalEnemiesDefeated * 5) + (playerLife * 10);
    }

    public void BossAnnouncement()
    {
        if (bossSpawnText == null) return;

        StopCoroutine("BossAnnouncementCoroutine"); // zatrzymanie poprzedniego komunikatu, jeśli jest
        StartCoroutine(BossAnnouncementCoroutine());
    }

    private IEnumerator BossAnnouncementCoroutine()
    {
        bossSpawnText.gameObject.SetActive(true);
        bossSpawnText.alpha = 1f;

        float displayTime = 3f;    // czas wyświetlania przed fade
        float fadeDuration = 1f;   // czas wygaszania

        // Czekaj przez displayTime sekund
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bossSpawnText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        bossSpawnText.alpha = 0f;
        bossSpawnText.gameObject.SetActive(false);
    }
}