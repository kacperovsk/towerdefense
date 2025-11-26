using UnityEngine;
using System.Collections.Generic;

public class MSAura : MonoBehaviour
{
    public float radius = 3f;
    public float speedMultiplier = 1.1f;  // +10%

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
            e.ModifySpeedMultiplier(speedMultiplier);
            Debug.Log("AURA SPEED BUFF: " + e.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e && affectedEnemies.Contains(e))
        {
            affectedEnemies.Remove(e);
            e.ModifySpeedMultiplier(1f / speedMultiplier); // cofniêcie buffa
            Debug.Log("AURA SPEED DEBUFF: " + e.name);
        }
    }
}
