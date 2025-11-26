using UnityEngine;
using System.Collections.Generic;

public class MageAura : MonoBehaviour
{
    public float radius = 3f;
    public float maxHpMultiplier = 1.5f;  // +50%

    private List<Enemy> affectedEnemies = new List<Enemy>();
    private CircleCollider2D col;

    void Start()
    {
        //CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        //col.isTrigger = true;
        //col.radius = radius;
    }

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
            e.ModifyMaxHealthMultiplier(maxHpMultiplier);
            Debug.Log("AURA BUFF " + e.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e && affectedEnemies.Contains(e))
        {
            affectedEnemies.Remove(e);
            e.ModifyMaxHealthMultiplier(1f / maxHpMultiplier); // cofniêcie buffa
            Debug.Log("AURA DEBUFF " + e.name);
        }
    }
}
