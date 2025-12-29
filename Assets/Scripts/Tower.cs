using System;
using UnityEngine;

public struct TowerStats
{
    public Sprite towerIcon;
    public string towerName;
    public string towerDescription;
    public float damage;
    public float fireRate;
    public float range;
    public float cost;

    public TowerStats(Sprite icon, string name, string desc, float dmg, float rate, float rng, float cost_)
    {
        towerIcon = icon;
        towerName = name;
        towerDescription = desc;
        damage = dmg;
        fireRate = rate;
        range = rng;
        cost = cost_;
    }
}

public class Tower : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] public Sprite towerIcon;
    [SerializeField] public string towerName;
    [SerializeField] public string towerDescription;
    [SerializeField] public int cost;

    [Header("Base Stats")]
    [SerializeField] private float range = 2f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float damage = 4f;

    [HideInInspector] public bool isGhost = false;
    [HideInInspector] public bool justPlaced = false;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    public Transform shootPoint;

    public enum ShotType
    {
        HomingTarget,
        TripleFixedAngle,
        SingleFixedPiercing
    }

    [Header("Shot Configuration")]
    [SerializeField] private ShotType currentShotType = ShotType.HomingTarget;
    [SerializeField] private float sideShotAngle = 20f;

    private float fireCountdown;
    private Enemy targetEnemy;

    public float baseDamage { get; private set; }
    public float baseFireRate { get; private set; }
    public float baseRange { get; private set; }

    private float damageMultiplier = 0f;
    private float fireRateMultiplier = 0f;
    private float rangeMultiplier = 0f;

    protected LineRenderer rangeCircle;
    protected bool showRange;

    private SpriteRenderer sr;
    private Color originalColor;
    public bool IsShowingRange() => showRange;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        baseDamage = damage;
        baseFireRate = fireRate;
        baseRange = range;

        RecalculateStats();
        SetupRangeCircle();
    }

    private void Update()
    {
        UpdateTarget();

        //== OBRACANIE, JAK CHCEMY TO ODKOEMNTOWAC.
        //if (targetEnemy != null)
        //{
        //    Vector2 dir = targetEnemy.transform.position - transform.position;
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        //}

        if (showRange)
            DrawRangeCircle(range);

        if (isGhost || targetEnemy == null)
            return;

        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }


    public void ApplyDamageBuff(float multiplier)
    {
        damageMultiplier += multiplier - 1f;
        RecalculateStats();
    }

    public void ApplyAttackSpeedBuff(float multiplier)
    {
        fireRateMultiplier += multiplier - 1f;
        RecalculateStats();
    }

    public void ApplyRangeBuff(float multiplier)
    {
        rangeMultiplier += multiplier - 1f;
        RecalculateStats();
    }

    public void RemoveDamageBuff(float multiplier)
    {
        damageMultiplier /= multiplier - 1f;
        RecalculateStats();
    }

    public void RemoveAttackSpeedBuff(float multiplier)
    {
        fireRateMultiplier /= multiplier - 1f;
        RecalculateStats();
    }

    public void RemoveRangeBuff(float multiplier)
    {
        rangeMultiplier /= multiplier - 1f;
        RecalculateStats();
    }

    private void RecalculateStats()
    {
        damage = baseDamage * (1f + damageMultiplier);
        fireRate = baseFireRate * (1f + fireRateMultiplier);
        range = baseRange * (1f + rangeMultiplier);
    }


    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Enemy bestEnemy = null;
        float maxProgress = -1f;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemyObj in enemies)
        {
            if (!enemyObj.activeInHierarchy) continue;

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > range) continue;

            float progress = enemy.GetProgress();

            if (progress > maxProgress ||
                (Mathf.Approximately(progress, maxProgress) && distance < closestDistance))
            {
                maxProgress = progress;
                closestDistance = distance;
                bestEnemy = enemy;
            }
        }

        targetEnemy = bestEnemy;
    }


    private void Shoot()
    {
        switch (currentShotType)
        {
            case ShotType.HomingTarget:
                ShootHoming();
                break;

            case ShotType.TripleFixedAngle:
                ShootTripleFixedAngle();
                break;

            case ShotType.SingleFixedPiercing:
                ShootSingleFixed();
                break;
        }
    }

    private void ShootHoming()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        projectileGO.GetComponent<Projectile>()?.SetTarget(targetEnemy, damage);
    }

    private void ShootTripleFixedAngle()
    {
        Vector2 baseDir = (targetEnemy.transform.position - shootPoint.position).normalized;

        ShootFixed(baseDir, 0f);
        ShootFixed(baseDir, -sideShotAngle);
        ShootFixed(baseDir, sideShotAngle);
    }

    private void ShootSingleFixed()
    {
        InstantiateTripleShotProjectile(0f, targetEnemy.transform.position);
        targetEnemy = null;
    }

    private void InstantiateTripleShotProjectile(float angleOffset, Vector3 targetPos)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        FixedPiercingBehaviour pierce = projectileGO.GetComponent<FixedPiercingBehaviour>();

        if (pierce == null)
        {
            Destroy(projectileGO);
            return;
        }

        projectileGO.GetComponent<Projectile>()?.SetDamage(damage);

        if (targetPos != Vector3.zero)
        {
            pierce.Initialize(shootPoint.position, targetPos);
        }
        else
        {
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * shootPoint.up;
            pierce.Initialize(shootPoint.position, shootPoint.position + (Vector3)dir * 100f);
        }
    }

    private void ShootFixed(Vector2 baseDir, float angle)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        Projectile proj = projectileGO.GetComponent<Projectile>();
        proj.SetDamage(damage);

        Vector2 finalDir = Quaternion.Euler(0, 0, angle) * baseDir;
        proj.ActivateFixedStraight(finalDir, 5f);
    }



    private void SetupRangeCircle()
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

    public void ShowRange()
    {
        showRange = true;
        rangeCircle.enabled = true;
    }

    public void HideRange()
    {
        showRange = false;
        rangeCircle.enabled = false;
    }

    public void DrawRangeCircle(float radius)
    {
        if (!showRange) return;

        for (int i = 0; i < rangeCircle.positionCount; i++)
        {
            float angle = 2 * Mathf.PI * i / rangeCircle.positionCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            rangeCircle.SetPosition(i, transform.position + pos);
        }
    }
    public void Highlight()
    {
        if (sr != null)
            sr.color = Color.yellow;
    }

    public void Unhighlight()
    {
        if (sr != null)
            sr.color = originalColor;
    }

    public TowerStats GetStats()
    {
        return new TowerStats(towerIcon, towerName,towerDescription, damage, fireRate, range, cost);
    }
}
