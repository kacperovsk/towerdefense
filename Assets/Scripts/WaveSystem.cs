using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab; // prefab do stworzenia
        public int count; // ilu ma ich byc
        public float spawnInterval = 0.5f; // odstep czasowy miedzy spawnem pojedynczych jednostek
    }

    [System.Serializable]
    public class WaveData
    {
        public EnemyGroup[] enemyGroups; // tablica grup przeciwnikow w danej fali
    }

    [Header("Waves")]
    public WaveData[] waves; // lista wszystkich fal
    public Transform spawnPoint; // spawnpoint

    [Header("Settings")]
    public bool autoStartNextWave = false; 
    public float timeBetweenWaves = 5f;

    [Header("UI (>NOT< optional)")]
    public Button nextWaveButton;
    public Toggle autoStartToggle;
    public TextMeshProUGUI waveInfoText; // tekst informacyjny, mozemy tu wrzucic ktora fala albo co jest w danej fali

    // eventy do ktorych mozemy potem podlaczyc akcje w unity, jakis dzwiek lub animacje
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent<int> OnWaveCompleted;

    private int currentWaveIndex = -1; // numer aktualnej fali
    private bool isSpawning = false; // czy obecnie sie spawnia
    private int enemiesAlive = 0; // ilu zyje obecnie

    private void Start()
    {
        // przycisk nastepnej fali
        if (nextWaveButton != null)
            nextWaveButton.onClick.AddListener(() => StartNextWave());

        // checkbox autostartu
        if (autoStartToggle != null)
            autoStartToggle.onValueChanged.AddListener(value => autoStartNextWave = value);

        UpdateWaveInfo(); // update tekstu o aktualnej fali
    }

    public void StartNextWave()
    {
        // jesli fala trwa lub skonczyly sie fale to nic nie rob
        if (isSpawning || currentWaveIndex + 1 >= waves.Length)
            return;

        // przejdz do nastepnej fali
        currentWaveIndex++;
        UpdateWaveInfo();
        // uruchom spawnowanie sie przeciwnikow 
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        isSpawning = true; 
        OnWaveStarted?.Invoke(currentWaveIndex + 1); // wywolaj event startu fali

        // iterujemy po kazdej grupie w tej fali
        foreach (var group in wave.enemyGroups)
        {
            // tworzenie przeciwnikow jeden po drugim z odstepem
            for (int i = 0; i < group.count; i++)
            {
                // tworzenie prefabu przeciwnika
                GameObject enemyParent = GameObject.Find("Enemies");
                GameObject enemy = Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity, enemyParent != null ? enemyParent.transform : null);

                // zwiekszenie liczniku zywych przeciwnikow
                enemiesAlive++;

                // kiedy przeciwnik zostanie zniszczony to zmniejsz licznik
                EnemyDeathTracker tracker = enemy.AddComponent<EnemyDeathTracker>();
                tracker.OnEnemyDeath += OnEnemyDeath;

                // czekamy przed stworzeniem kolejnego przeciwnika
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
        // spawn zakonczony
        isSpawning = false;

        // passive gold co fale
        GameManager.Instance.GrantPassiveIncome();
        // wywolanie eventu zakonczenia fali
        OnWaveCompleted?.Invoke(currentWaveIndex + 1);

        // aktualizacja UI z informacja o numerze fali
        UpdateWaveInfo();
    }

    private void OnEnemyDeath()
    {
        enemiesAlive--;

        // jak przeciwnicy nie zyja to mozemy rozpoczac kolejna fale
        if (enemiesAlive <= 0 )
        {
            if (autoStartNextWave)
                StartCoroutine(AutoNextWave());
        }
    }

    private IEnumerator AutoNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }

    private void UpdateWaveInfo()
    {
        if (waveInfoText != null)
        {
            if (currentWaveIndex < 0)
                waveInfoText.text = $"Fala: 0 / {waves.Length}";
            else
                waveInfoText.text = $"Fala: {currentWaveIndex + 1} / {waves.Length}";
        }
    }
}

public class EnemyDeathTracker : MonoBehaviour // funkcja informujaca system o smierci przeciwnika
{
    public System.Action OnEnemyDeath;

    private void OnDestroy()
    {
        OnEnemyDeath?.Invoke();
    }
}
