using System;
using UnityEngine;

public struct TowerStats
{
    public Sprite towerIcon;
    public string towerName;
    public float damage;
    public float fireRate;
    public float range;

    public TowerStats(Sprite icon, string name, float dmg, float rate, float rng)
    {
        towerIcon = icon;
        towerName = name;
        damage = dmg;
        fireRate = rate;
        range = rng;
    }
}

public class Tower : MonoBehaviour
{
    [SerializeField] public Sprite towerIcon;
    [SerializeField] public string towerName;

    [SerializeField] private float range = 2f;
    public float fireRate = 1f;
    public float damage = 4f;
    public bool isGhost = false;

    [SerializeField] private GameObject projectilePrefab;
    public Transform shootPoint;

    private LineRenderer rangeCircle;
    private bool showRange;

    private float fireCountdown = 0f;
    private Enemy targetEnemy;
    private void Start()
    {
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
            rangeCircle.sortingLayerName = "Foreground";
            rangeCircle.sortingOrder = 10;
            rangeCircle.enabled = false; // na start niewidoczny
        }

    }
    void Update()
    {
        UpdateTarget();

        if (showRange)
            DrawCircle();

        if (targetEnemy == null )
        {
            return;
        }

        if(fireCountdown <= 0f)
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
            if (distanceToEnemy > range)
                continue;

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
        GameObject projectileParent = GameObject.Find("Projectiles");
        GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity, projectileParent != null ? projectileParent.transform : null);

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Pass tower's damage to the projectile
            projectile.SetTarget(targetEnemy, damage);
        }
    }

    void DrawCircle()
    {
        for (int i = 0; i < rangeCircle.positionCount; i++)
        {
            float angle = 2 * Mathf.PI * i / rangeCircle.positionCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * range;
            rangeCircle.SetPosition(i, transform.position + pos);
        }
    }

    public TowerStats GetStats()
    {
        return new TowerStats(towerIcon, towerName, damage, fireRate, range);
    }

    public void ShowRange()
    {
        showRange = true;
        if (rangeCircle != null)
            rangeCircle.enabled = true;
    }

    public void HideRange()
    {
        showRange = false;
        if (rangeCircle != null)
            rangeCircle.enabled = false;
    }
}
