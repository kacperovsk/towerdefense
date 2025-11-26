using UnityEngine;
using System.Collections.Generic;

public class DamageAura : MonoBehaviour
{
    public float radius = 3f;
    public float damageMultiplier = 2f; 

    private List<Enemy> affectedEnemies = new List<Enemy>();
    private CircleCollider2D col;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e && !affectedEnemies.Contains(e))
        {
            affectedEnemies.Add(e);
            e.ModifyDamageMultiplier(damageMultiplier);
            Debug.Log("AURA DMG BUFF " + e.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e && affectedEnemies.Contains(e))
        {
            affectedEnemies.Remove(e);
            e.ModifyDamageMultiplier(1f / damageMultiplier); // cofniêcie buffa
            Debug.Log("AURA DMG DEBUFF " + e.name);
        }
    }
}
