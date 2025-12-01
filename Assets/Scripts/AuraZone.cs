using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class AuraZone : MonoBehaviour
{
    [Header("Aura Settings")]
    public float radius = 3f;

    [Header("Multipliers")]
    public bool affectMaxHealth = false;
    public float maxHealthMultiplier = 1.2f;

    public bool affectMoveSpeed = false;
    public float moveSpeedMultiplier = 1.2f;

    public bool affectDamage = false;
    public float damageMultiplier = 1.2f;

    private CircleCollider2D col;

    // lista wrogów wewn¹trz aury
    private List<Enemy> affectedEnemies = new List<Enemy>();

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = radius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!enemy) return;

        if (!affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            ApplyAura(enemy);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!enemy) return;

        if (affectedEnemies.Contains(enemy))
        {
            RemoveAura(enemy);
            affectedEnemies.Remove(enemy);
        }
    }

    private void ApplyAura(Enemy enemy)
    {
        if (affectMaxHealth)
            enemy.ApplyAuraEffect(Enemy.AuraStat.MaxHealth, maxHealthMultiplier);

        if (affectMoveSpeed)
            enemy.ApplyAuraEffect(Enemy.AuraStat.MoveSpeed, moveSpeedMultiplier);

        if (affectDamage)
            enemy.ApplyAuraEffect(Enemy.AuraStat.Damage, damageMultiplier);
    }

    private void RemoveAura(Enemy enemy)
    {
        if (affectMaxHealth)
            enemy.RemoveAuraEffect(Enemy.AuraStat.MaxHealth, maxHealthMultiplier);

        if (affectMoveSpeed)
            enemy.RemoveAuraEffect(Enemy.AuraStat.MoveSpeed, moveSpeedMultiplier);

        if (affectDamage)
            enemy.RemoveAuraEffect(Enemy.AuraStat.Damage, damageMultiplier);
    }
}
