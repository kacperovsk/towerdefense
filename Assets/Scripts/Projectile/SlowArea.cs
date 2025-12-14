using UnityEngine;
using System.Collections.Generic;

public class SlowArea : MonoBehaviour
{
    [Header("Slow Effect Settings")]
    [Tooltip("Promień obszaru spowalniania.")]
    public float radius = 3f;
    [Tooltip("Współczynnik spowolnienia (1.0 = brak spowolnienia, 0.5 = 50% spowolnienia).")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.5f;
    [Tooltip("Czas trwania obszaru spowalniającego w sekundach.")]
    public float duration = 3f;

    private CircleCollider2D areaCollider;
    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>();

    void Awake()
    {
        // Upewnij się, że obiekt ma CircleCollider2D
        areaCollider = GetComponent<CircleCollider2D>();
        if (areaCollider == null)
        {
            areaCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        areaCollider.isTrigger = true;
        areaCollider.radius = radius;
        
        // Skalowanie wizualizacji
        float scale = radius * 2f;
        transform.localScale = new Vector3(scale, scale, 1f);

        // Zaplanowanie autodestrukcji po upływie czasu
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            // Aplikacja spowolnienia na wrogu
            enemy.ApplySlow(this, slowMultiplier);
            affectedEnemies.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            // Usunięcie spowolnienia
            enemy.RemoveSlow(this);
            affectedEnemies.Remove(enemy);
        }
    }

    void OnDestroy()
    {
        // Usunięcie efektu spowolnienia ze wszystkich wrogów przed zniszczeniem obszaru
        foreach (var enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.RemoveSlow(this);
            }
        }
        affectedEnemies.Clear();
    }
    
    // Wizualizacja zasięgu w edytorze
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}