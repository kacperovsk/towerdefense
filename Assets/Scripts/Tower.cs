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
    [HideInInspector] public bool justPlaced = false;

    [SerializeField] private GameObject projectilePrefab;
    public Transform shootPoint;
    public enum ShotType
    {
        HomingTarget, // To jak po staremu
        TripleFixedAngle, // To do triple shot
        SingleFixedPiercing // Do starego pierca
    }
    
    [Header("Shot Configuration")]
    [SerializeField] private ShotType currentShotType = ShotType.HomingTarget; 
    [SerializeField] private float sideShotAngle = 20f;

    private LineRenderer rangeCircle;
    private bool showRange;

    private float fireCountdown = 0f;
    private Enemy targetEnemy;

    // Bazowe statystyki publiczne do odczytu przez buffy
    public float baseDamage { get; private set; }
    public float baseFireRate { get; private set; }

    private float buffMultiplierTotal = 1f;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

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
    public void Highlight()
    {
        sr.color = Color.yellow;
    }

    public void Unhighlight()
    {
        sr.color = originalColor;
    }

    public void ApplyCardDamageBuff(float value)
    {
        damage += value;
        Debug.Log($"{name} otrzymała buff damage +{value}");
    }
    public void ApplyAttackSpeedBuff(float value)
    {
        fireRate += value;
    }
    public void ApplyRangeBuff(float value)
    {
        range += value;
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

private void Shoot()
    {
        fireCountdown = 1f / fireRate;
        
        // Do przestawiania typu pocisku
        switch (currentShotType)
        {
            case ShotType.HomingTarget:
                // Do klasycznego homingu
                ShootHoming();
                break;
            
            case ShotType.TripleFixedAngle:
                // Do triple shot
                ShootTripleFixedAngle();
                break;
            
            case ShotType.SingleFixedPiercing: // Do single piercing shota
                ShootSingleFixed();
                break;
        }
    }

    // Metoda stara
    private void ShootHoming()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetTarget(targetEnemy, damage);
        }
    }
    // Logika triple shot
    private void ShootTripleFixedAngle()
    {
        // Pocisk centralny
        Vector3 targetPosition = targetEnemy.transform.position; 
        InstantiateTripleShotProjectile(0f, targetPosition);

        // Pociski boczne
        InstantiateTripleShotProjectile(-sideShotAngle, Vector3.zero); // Lewy
        InstantiateTripleShotProjectile(sideShotAngle, Vector3.zero);  // Prawy

        targetEnemy = null; 
    }
    private void ShootSingleFixed()
    {
        Vector3 targetPosition = targetEnemy.transform.position; 
        InstantiateTripleShotProjectile(0f, targetPosition);

        targetEnemy = null; 
    }
    private void InstantiateTripleShotProjectile(float angleOffset, Vector3 targetPos)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        
        // Korzysta z FixedPiercing aby mieć ten prosty lot bez homing
        FixedPiercingBehaviour fixedPierce = projectileGO.GetComponent<FixedPiercingBehaviour>();
        
        if (fixedPierce != null)
        {
            // dmg
            projectileGO.GetComponent<Projectile>().SetDamage(damage);

            // strzał centralny celuje
            if (targetPos != Vector3.zero)
            {
                // strzał
                fixedPierce.Initialize(shootPoint.position, targetPos);
            }
            else
            {
                // Strzały boczne (nie celują).
                
                // Kierunek na podstawie obrotu wieży
                Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
                Vector2 fixedDirection = rotation * shootPoint.up;
                
                // Tworzy odległy ceł w tym kierunku (rozwiązanie z neta)
                Vector3 virtualTarget = shootPoint.position + (Vector3)fixedDirection * 100f; 

                // Strzela
                fixedPierce.Initialize(shootPoint.position, virtualTarget);
            }
        }
        else
        {
            Destroy(projectileGO);
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
