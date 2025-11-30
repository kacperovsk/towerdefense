using System;
using UnityEngine;

public struct TowerStats
{
    public Sprite towerIcon;
    public string towerName;
    public float damage;
    public float fireRate;
    public float range;
    public float cost;
    public TowerStats(Sprite icon, string name, float dmg, float rate, float rng, float cost_)
    {
        towerIcon = icon;
        towerName = name;
        damage = dmg;
        fireRate = rate;
        range = rng;
        cost = cost_;
    }
}

public class Tower : MonoBehaviour
{
    [SerializeField] public Sprite towerIcon;
    [SerializeField] public string towerName;
    [SerializeField] public int cost;

    [SerializeField] private float range = 2f;
    public float fireRate = 1f;
    public float damage = 4f;

    [HideInInspector] public bool isGhost = false;

    [SerializeField] private GameObject projectilePrefab;
    public Transform shootPoint;

    private LineRenderer rangeCircle;
    private bool showRange;

    private float fireCountdown = 0f;
    private Enemy targetEnemy;

    // Bazowe statystyki publiczne do odczytu przez buffy
    public float baseDamage { get; private set; }
    public float baseFireRate { get; private set; }

    private float buffMultiplierTotal = 1f;

    private void Awake()
    {
        baseDamage = damage;
        baseFireRate = fireRate;

        if (rangeCircle == null)
        {
            rangeCircle = gameObject.AddComponent<LineRenderer>();
            rangeCircle.loop = true;
            rangeCircle.positionCount = 50;
            rangeCircle.material = new Material(Shader.Find("Sprites/Default"));
            rangeCircle.startWidth = 0.02f;
            rangeCircle.endWidth = 0.02f;
            rangeCircle.startColor = Color.grey;
            rangeCircle.endColor = Color.grey;
            rangeCircle.enabled = false;
            rangeCircle.sortingOrder = 2;
        }
    }

    void Update()
    {
        UpdateTarget();

        if (showRange)
            DrawRangeCircle(range);

        if (isGhost || targetEnemy == null) return;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Enemy furthestEnemy = null;
        float maxProgress = -1f;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemyObj in enemies)
        {
            if (!enemyObj.activeInHierarchy) continue;

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy == null) continue;

            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy > range) continue;

            float progress = enemy.GetProgress();

            if (progress > maxProgress || (Mathf.Approximately(progress, maxProgress) && distanceToEnemy < closestDistance))
            {
                maxProgress = progress;
                closestDistance = distanceToEnemy;
                furthestEnemy = enemy;
            }
        }

        targetEnemy = furthestEnemy;
    }

    void Shoot()
    {
        if (targetEnemy == null) return;

        GameObject projectileParent = GameObject.Find("Projectiles");
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity, projectileParent != null ? projectileParent.transform : null);

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            Vector2 dir = (targetEnemy.transform.position - shootPoint.position);
            if (dir.sqrMagnitude < 0.0001f) dir = (Vector2)transform.up;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectileGO.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            projectile.lastDirection = dir.normalized;
            projectile.SetTarget(targetEnemy, damage);
        }
    }

    public TowerStats GetStats()
    {
        return new TowerStats(towerIcon, towerName, damage, fireRate, range, cost);
    }

    public void ShowRange()
    {
        showRange = true;
        if (rangeCircle != null) rangeCircle.enabled = true;
    }

    public void HideRange()
    {
        showRange = false;
        if (rangeCircle != null) rangeCircle.enabled = false;
    }

    public void ApplyBuff(float multiplier)
    {
        buffMultiplierTotal *= multiplier;
        UpdateStats();
    }

    public void RemoveBuff(float multiplier)
    {
        buffMultiplierTotal /= multiplier;
        UpdateStats();
    }

    private void UpdateStats()
    {
        damage = baseDamage * buffMultiplierTotal;
        fireRate = baseFireRate * buffMultiplierTotal;
    }

    // Range dla karczm się pierdolił więc dodałem gettery żeby bezpośrednio u nich w skrypcie zrobić range, fml
    public float GetRange() => range;

    public bool IsShowingRange() => showRange;

    public LineRenderer GetRangeCircle() => rangeCircle;

    // Metoda też do tego
    public void DrawRangeCircle(float customRadius)
    {
        if (!showRange || rangeCircle == null) return;

        for (int i = 0; i < rangeCircle.positionCount; i++)
        {
            float angle = 2 * Mathf.PI * i / rangeCircle.positionCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * customRadius;
            rangeCircle.SetPosition(i, transform.position + pos);
        }
    }
}
