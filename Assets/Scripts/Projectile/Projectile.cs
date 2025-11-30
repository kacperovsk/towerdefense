using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    public float speed = 4f;
    private float damage;
    private bool hasHit = false; // prevents multiple hits (for homing hit)
    // Jak ktoś to czyta, dużo w tym grzebałem aby naprawić piercing, dlatego te funkcje mogą wyglądać inaczej chociaż działają podobnie, głównie kombinowałem z rotacją
    // Już się pogubiłem co zmieniałem więc no


    // Potrzebowałem do pierca
    public event Action<Enemy> OnHit;
    [NonSerialized] public bool preventDestruction = false;
    [NonSerialized] public Vector2 lastDirection = Vector2.up;

    // Po przebiciu
    [NonSerialized] public bool stopHoming = false;
    [NonSerialized] public Vector2 straightDirection = Vector2.up;
    [NonSerialized] public float straightLife = 5f;
    private float straightTimer = 0f;
    // -----------------------------------------------------

    public void SetTarget(Enemy _target, float dmg)
    {
        target = _target;
        damage = dmg;
    }

    void OnEnable()
    {
        hasHit = false;
        stopHoming = false;
        straightTimer = 0f;
    }

    void Update()
    {
        // Jeśli jesteśmy w fazie prostego lotu -> leć prosto
        // I tak kurwa nie leci prosto, fml
        if (stopHoming)
        {
            float straightStep = speed * Time.deltaTime;
            transform.position += (Vector3)(straightDirection.normalized * straightStep);

            straightTimer += Time.deltaTime;
            if (straightTimer >= straightLife)
            Destroy(gameObject);

            return;
        }

        // Homing czyli tak jak wcześniej było
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir3 = target.transform.position - transform.position;
        Vector2 dir = new Vector2(dir3.x, dir3.y);
        float distanceThisFrame = speed * Time.deltaTime;

        // Zapisz stabilny kierunek do pierca
        if (dir.magnitude > 0.0001f)
            lastDirection = dir.normalized;

        // Obróć w kierunku celu
        RotateTowards(dir);

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void RotateTowards(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
    
    
    // ogólnie całe przerobione, trochę z pomocą GPT
    // Stara wersja rozwala pocisk od razu
    // Teraz jest opcja że nie, więc leci dalej na zasadzie pierce
    protected virtual void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        Enemy hitEnemy = null;
        if (target != null)
        {
            hitEnemy = target.GetComponent<Enemy>();
            if (hitEnemy != null)
                hitEnemy.TakeDamage(damage);
        }

        // powiadom behaviour-y (np. Piercing) że trafiono
        if (hitEnemy != null)
            OnHit?.Invoke(hitEnemy);

        // jeśli jakiś behaviour ustawił preventDestruction = true -> nie niszczemy tutaj
        if (preventDestruction)
        {
            // nie niszcz, reset flagi (behaviour może ustawić ponownie jeśli chce)
            preventDestruction = false;
            return;
        }

        Destroy(gameObject);
    }

    // Wywołane przez PiercingBehaviour, aby przełączyć pocisk w fazę prostego lotu
    public void EnterStraightPhase(Vector2 direction, float life)
    {
        stopHoming = true;

        if (direction.sqrMagnitude < 0.000001f && lastDirection.sqrMagnitude > 0.000001f)
            straightDirection = lastDirection.normalized;
        else if (direction.sqrMagnitude < 0.000001f)
            straightDirection = transform.up;
        else
            straightDirection = direction.normalized;

        straightLife = life;
        straightTimer = 0f;

        // Nie chce już homingu
        target = null;
    }

    // Dodatkowy collider z przeciwnikiem żeby na pewno był dmg
    // Wcześniej też działało, ale pierce bez tego miał problem z dmg
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!stopHoming) return;

        Enemy e = col.GetComponent<Enemy>();
        if (e == null) return;

        e.TakeDamage(damage);
        OnHit?.Invoke(e);

        if (!preventDestruction)
            Destroy(gameObject);
        else
            preventDestruction = false;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
