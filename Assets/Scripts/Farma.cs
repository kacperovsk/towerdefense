using UnityEngine;

public class Farma : Tower
{
    [Header("Farm Settings")]
    public int goldPerWave;

    //private bool rewardGivenThisWave = false;
    //private bool waveActive = false;

    private void Update()
    {
        //// Szukamy wszystkich przeciwników w scenie
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //if (enemies.Length > 0)
        //{
        //    // Fala trwa
        //    waveActive = true;
        //    rewardGivenThisWave = false; // reset flagi na nową falę
        //}
        //else if (enemies.Length == 0 && waveActive && !rewardGivenThisWave)
        //{
        //    // Fala zakończona więc czas na wypłate
        //    if (GameManager.Instance != null)
        //    {
        //        GameManager.Instance.AddMoney(goldPerWave);
        //        Debug.Log($"Farma dodała {goldPerWave} golda!");
        //    }

        //    rewardGivenThisWave = true;
        //    waveActive = false;
        //}
    }

    private void Start()
    {
        if (isGhost)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.passiveGoldIncome += goldPerWave;
            Debug.Log($"+{goldPerWave} passive gold (Farma placed)");
        }
    }

    private void OnDestroy()
    {
        if (isGhost)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.passiveGoldIncome -= goldPerWave;
            Debug.Log($"-{goldPerWave} passive gold (Farma removed)");
        }
    }
}
