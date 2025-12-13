using UnityEngine;
using System;

[RequireComponent(typeof(Projectile))]
public class AoEOnHit : MonoBehaviour
{
    [Header("AOE Settings")]
    public float radius = 3f;          // promień AoE
    public float damageMultiplier = 1f;        // obrażenia centralne
    [Range(0f,1f)] public float falloff = 0.5f; // procent dmg dla pobliskich celów

    [Header("Visual Indicator")]
    public GameObject indicatorPrefab; // prefab przezroczystego okręgu
    public float displayTime = 0.3f;   // czas wyświetlania wskaźnika

    [NonSerialized] public Projectile projectile;

    void Awake()
    {
        projectile = GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogWarning("AoEOnHit wymaga komponentu Projectile!");
            enabled = false;
            return;
        }

        projectile.OnHit += HandleOnHit;
    }

    void OnDestroy()
    {
        if (projectile != null)
            projectile.OnHit -= HandleOnHit;
    }

    private void HandleOnHit(Enemy centerEnemy)
    {
        Vector2 center = transform.position;

        // Liczy obrażenia pocisku za pomocą mnożnika
        float finalCenterDamage = projectile.BaseDamage * damageMultiplier;
        // To samo dla fallof
        float finalFalloffDamage = finalCenterDamage * falloff;

        // Damage
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var c in hits)
        {
            Enemy e = c.GetComponent<Enemy>();
            if (e == null) continue;

            if (e == centerEnemy)
                e.TakeDamage(finalCenterDamage);
            else
                e.TakeDamage(finalFalloffDamage);
        }

        // Pokazanie range przy aktywacji
        if (indicatorPrefab != null)
        {
            GameObject go = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
            float scale = radius * 2f;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            Destroy(go, displayTime);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
