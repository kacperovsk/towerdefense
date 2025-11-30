using UnityEngine;

public class Farma : Tower
{
    [Header("Farm Settings")]
    public int goldPerWave = 10; // ile złota daje farma na koniec fali

    private bool rewardGivenThisWave = false;
    private bool waveActive = false;

    private void Update()
    {
        // Szukamy wszystkich przeciwników w scenie
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            // Fala trwa
            waveActive = true;
            rewardGivenThisWave = false; // reset flagi na nową falę
        }
        else if (enemies.Length == 0 && waveActive && !rewardGivenThisWave)
        {
            // Fala zakończona więc czas na wypłate
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(goldPerWave);
                Debug.Log($"Farma dodała {goldPerWave} golda!");
            }

            rewardGivenThisWave = true;
            waveActive = false;
        }
    }
}
