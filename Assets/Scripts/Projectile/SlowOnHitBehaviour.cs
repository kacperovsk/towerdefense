using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class SlowOnHitBehaviour : MonoBehaviour
{
    [Header("Slow Area Settings")]
    [Tooltip("Prefab obiektu, na którym znajduje się komponent SlowArea.")]
    public GameObject slowAreaPrefab;
    [Tooltip("Promień obszaru spowalniania.")]
    public float radius = 3f;
    [Tooltip("Współczynnik spowolnienia (0.1 do 1.0).")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.5f;
    [Tooltip("Czas trwania obszaru spowalniającego w sekundach.")]
    public float duration = 3f;

    private Projectile projectile;

    void Awake()
    {
        projectile = GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogWarning("SlowOnHitBehaviour wymaga komponentu Projectile!");
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
        if (slowAreaPrefab == null)
        {
            Debug.LogError("Nie ustawiono 'Slow Area Prefab' w SlowOnHitBehaviour!");
            return;
        }

        // 1. Utwórz obszar spowalniający w miejscu trafienia
        GameObject slowAreaGO = Instantiate(slowAreaPrefab, transform.position, Quaternion.identity);
        
        // 2. Skonfiguruj skrypt SlowArea
        SlowArea slowArea = slowAreaGO.GetComponent<SlowArea>();
        if (slowArea != null)
        {
            // Przekazanie ustawień z pocisku do obszaru
            slowArea.radius = radius;
            slowArea.slowMultiplier = slowMultiplier;
            slowArea.duration = duration;
        }
        else
        {
            Debug.LogWarning("Prefab obszaru spowalniającego nie ma komponentu SlowArea!");
        }
    }
}