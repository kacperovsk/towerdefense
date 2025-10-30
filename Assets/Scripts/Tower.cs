using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float range = 2f;
    public float fireRate = 1f;
    [SerializeField] private ObjectPooler projectilePooler;
    public Transform shootPoint;

    private float fireCountdown = 0f;
    private Enemy targetEnemy;
    // Update is called once per frame
    private void Start()
    {
        if (projectilePooler == null)
        {
            GameObject poolObj = GameObject.FindGameObjectWithTag("ProjectilePool");
            if (poolObj != null)
                projectilePooler = poolObj.GetComponent<ObjectPooler>();
        }
    }
    void Update()
    {
        UpdateTarget();

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
        GameObject projectileGO = projectilePooler.getPooledObject();
        projectileGO.transform.position = shootPoint.position;
        projectileGO.transform.rotation = Quaternion.identity;
        projectileGO.SetActive(true);

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(targetEnemy);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
