using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    private float moveSpeedMultiplier = 1f;
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private Path currentPath;
    private Vector3 targetPosition;
    private int currentPosition;
    private bool facingRight = true;
    private float temporarySlowMultiplier = 1f;
    private HashSet<SlowArea> activeSlows = new HashSet<SlowArea>();

    [Header("Health Settings")]
    [SerializeField] private float baseMaxHealth = 10f;
    private float maxHpMultiplier = 1f;
    private float currentMaxHealth;
    [SerializeField] private float health;

    [Header("Health Bar")]
    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;

    [Header("Player Damage")]
    private float damageMultiplier = 1f;
    [SerializeField] private int playerDamage = 1; //Ile obrażeń zada przeciwnik gdy dojdzie do końca. Ustawiam na 1 bo edytuje w prefabie anyway
    [SerializeField] private int currentPlayerDamage;

    //Wartości kasy dla wrogów, nie jestem pewien czy to potrzebne ale buja
    [Header("Reward")]
    [SerializeField] private int moneyValue = 10;
    [Header("Drop")]
    [SerializeField] private EnemyData data;


    public enum AuraStat
    {
        MaxHealth,
        MoveSpeed,
        Damage
    }
    void Flip(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(float amount)
    {
        // szansa na unik
        if (TryGetComponent<Special5>(out Special5 s5))
        {
            if (s5.TryDodge())
            {
                // unik
                return;
            }
        }
        health -= amount;
        UpdateHealhBar();
        if (health <= 0f) Die();
    }

    private void Die()
    {
        TryDropCard();
        TrojanHorse trojan = GetComponent<TrojanHorse>();
        if (trojan != null)
        {
            trojan.SpawnEnemies();
        }

        //Teraz przed usunięciem dodajemy kasę do gracza
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(moneyValue);
        }
        Destroy(gameObject);
    }

    private void Awake()
    {
        currentPath = GameObject.Find("Path").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;
    }
    private void Start()
    {
        damageMultiplier = 1f;
        currentPlayerDamage = playerDamage;

        if (forcedSpawn)
        {
            transform.position = forcedSpawnPosition;

            if (forcedWaypoint >= 0)
                currentPosition = forcedWaypoint;
            else
                currentPosition = 0;

            targetPosition = currentPath.GetPosition(currentPosition);

            forcedSpawn = false;
            forcedWaypoint = -1;
        }
        else
        {
            currentPosition = 0;
            targetPosition = currentPath.GetPosition(currentPosition);
        }

        maxHpMultiplier = 1f;
        currentMaxHealth = baseMaxHealth;
        health = currentMaxHealth;
        UpdateHealhBar();
        if (gameObject.name.StartsWith("Boss"))
            GameManager.Instance.BossAnnouncement();
    }



    void Update()
    {
        currentMoveSpeed = moveSpeed * moveSpeedMultiplier * temporarySlowMultiplier;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            currentMoveSpeed * Time.deltaTime
            );


        float relativeDistance = (transform.position - targetPosition).magnitude;

        // Jak blisko waypointa to idzie do kolejnego.
        if (relativeDistance < 0.05f)
        {

            if (currentPosition < currentPath.Waypoints.Length - 1)
            {
                currentPosition++;
                targetPosition = currentPath.GetPosition(currentPosition);
                float dirX = targetPosition.x - transform.position.x;
                if (dirX > 0 && !facingRight)
                {
                    Flip(true); //prawo
                }
                else if (dirX < 0 && facingRight)
                {
                    Flip(false); //lewo
                }
            }
            //Z tego co wiem to jest ten skrypt usuwania wroga na końcu ścieżki
            else
            {
                // więc jak dotrze do końca
                if (TryGetComponent<Special5>(out Special5 s5))
                {
                    s5.StealMoney();
                }
                else if (GameManager.Instance != null)
                {
                    // To gracz traci HP
                    GameManager.Instance.LoseLife(currentPlayerDamage);
                }
                //i tutaj tak jak wcześniej wróg znika z puli
                Destroy(gameObject);
            }
        }

    }

    public float GetProgress() // nie mam pojecia co tu sie dzieje ale buja i dzia��
    {
        if (currentPath == null || currentPath.Waypoints == null || currentPath.Waypoints.Length < 2)
            return 0f;

        int prevIndex = Mathf.Clamp(currentPosition - 1, 0, currentPath.Waypoints.Length - 1);
        int nextIndex = Mathf.Clamp(currentPosition, 0, currentPath.Waypoints.Length - 1);

        Vector3 prevPos = currentPath.Waypoints[prevIndex].transform.position;
        Vector3 nextPos = currentPath.Waypoints[nextIndex].transform.position;

        float segmentLength = Vector3.Distance(prevPos, nextPos);
        float distanceFromPrev = Vector3.Distance(transform.position, prevPos);

        // ile procent trasy mi�dzy tymi waypointami wr�g pokona�
        float segmentProgress = (segmentLength > 0f) ? Mathf.Clamp01(distanceFromPrev / segmentLength) : 0f;

        // pe�ny progres = numer poprzedniego waypointa + u�amek segmentu
        float totalProgress = (prevIndex + segmentProgress) / (currentPath.Waypoints.Length - 1);

        return Mathf.Clamp01(totalProgress);
    }

    public void ModifyMaxHealthMultiplier(float mult)
    {
        maxHpMultiplier *= mult;

        float oldMax = currentMaxHealth;
        currentMaxHealth = baseMaxHealth * maxHpMultiplier;
        health = health / oldMax * currentMaxHealth;
        //float newMax = maxHealth * maxHpMultiplier;

        // zdrowie nie może przekraczać nowego maksymalnego
        health = Mathf.Min(health, currentMaxHealth);

        UpdateHealhBar();
    }
    public float GetMaxHealth() => currentMaxHealth;
    public float GetHealth() => health;
    public int GetCurrentWaypoint() => currentPosition;
    private void UpdateHealhBar()
    {
        if (currentMaxHealth <= 0f)
            return;

        float healthPercent = health / currentMaxHealth;
        healthPercent = Mathf.Clamp01(healthPercent);

        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }
    public void ModifySpeedMultiplier(float mult)
    {
        moveSpeedMultiplier *= mult;
        currentMoveSpeed = moveSpeed * moveSpeedMultiplier;
    }
    public void ModifyDamageMultiplier(float mult)
    {
        damageMultiplier *= mult;
        currentPlayerDamage = Mathf.RoundToInt(playerDamage * damageMultiplier);
    }

    private bool forcedSpawn = false;
    private Vector3 forcedSpawnPosition;

    private int forcedWaypoint = -1;
    public void SetStartPosition(Vector3 pos, int waypoint)
    {
        forcedSpawn = true;
        forcedSpawnPosition = pos;
        forcedWaypoint = waypoint;
    }

    public void ApplyAuraEffect(AuraStat stat, float mult)
    {
        switch (stat)
        {
            case AuraStat.MaxHealth:
                ModifyMaxHealthMultiplier(mult);
                break;

            case AuraStat.MoveSpeed:
                ModifySpeedMultiplier(mult);
                break;

            case AuraStat.Damage:
                ModifyDamageMultiplier(mult);
                break;
        }
    }

    public void RemoveAuraEffect(AuraStat stat, float mult)
    {
        switch (stat)
        {
            case AuraStat.MaxHealth:
                ModifyMaxHealthMultiplier(1f / mult);
                break;

            case AuraStat.MoveSpeed:
                ModifySpeedMultiplier(1f / mult);
                break;

            case AuraStat.Damage:
                ModifyDamageMultiplier(1f / mult);
                break;
        }
    }
    // Metoda wywoływana przez SlowArea po wejściu w obszar
    public void ApplySlow(SlowArea slowSource, float slowMult)
    {
        if (activeSlows.Add(slowSource))
        {
            // Gdyby jakiś slow był silniejszy, to tylko silniejszy zadziała (dodaje na wypadek kart)
            if (slowMult < temporarySlowMultiplier)
            {
                temporarySlowMultiplier = slowMult;
            }
        }
    }

    // Gdy coś wyjdzie z obszaru
    public void RemoveSlow(SlowArea slowSource)
    {
        if (activeSlows.Remove(slowSource))
        {
            // Tutaj bardzo ważne - przelicza jeszcze raz slowa na podstawie kolejnych obszarów slowa.
            // Czyli wyjdzie z jednego to zamiast usuwać debuff patrzy czy jest kolejny, wieża lubi stawiać ich dużo więc potrzebne
            float maxSlow = 1f;
            foreach (var slow in activeSlows)
            {
                if (slow.slowMultiplier < maxSlow)
                {
                    maxSlow = slow.slowMultiplier;
                }
            }
            temporarySlowMultiplier = maxSlow;
        }
    }

    CardData.Rarity RollRarity(CardDropTable table)
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var r in table.rarityChances)
        {
            cumulative += r.chance;
            if (roll <= cumulative)
                return r.rarity;
        }

        return CardData.Rarity.Common;
    }
    void TryDropCard()
    {
        if (data == null || data.dropTable == null)
            return;

        if (data.isBoss)
        {
            DropBossCard();

            if (UnityEngine.Random.Range(0f, 100f) <= data.secondCardChance)
            {
                StartCoroutine(DropSecondBossCard());
            }

            return;
        }

        if (UnityEngine.Random.Range(0f, 100f) > data.dropTable.overallDropChance)
            return;

        CardData.Rarity rarity = RollRarity(data.dropTable);
        CardManager.Instance.DropCardOfRarity(rarity);
    }


    void DropBossCard()
    {
        CardData.Rarity rarity = RollRarity(data.dropTable);
        CardManager.Instance.DropCardOfRarity(rarity);
    }

    IEnumerator DropSecondBossCard()
    {
        yield return new WaitForSeconds(0.3f);

        DropBossCard();
    }

}
