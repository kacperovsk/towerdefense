using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    public float speed = 4f;
    //tymczasowo na public
    private float damage;
    public float BaseDamage => damage; //Potrzebne do przekazania obrażeń do innej klasy, bo chcieliście AOE bazowane na obrażeniach pocisku do karty
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
    [NonSerialized] public bool isFixedStraight = false;
    [NonSerialized] public Vector2 fixedDirection = Vector2.up;
    [NonSerialized] public float fixedLife = 5f;
    private float fixedTimer = 0f;

    public void SetTarget(Enemy _target, float dmg)
    {
        target = _target;
        damage = dmg;
    }
    //Muszę jakos przekazać damage dla systemu targetowania bez homingu
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void OnEnable()
    {
        hasHit = false;
        stopHoming = false;
        straightTimer = 0f;
    }

    public void ActivateFixedStraight(Vector2 direction, float life)
    {
        fixedDirection = direction.normalized;
        fixedLife = life;
        isFixedStraight = true;
        target = null;
        RotateTowards(fixedDirection);
    }

    void Update()
    {
        //Dodaje nowy sposób lotu pocisku, lecący do konkretnej pozycji na mapie. Na długie odległości będzie missować, dlatego stary też zostaje
        if (isFixedStraight)
        {
            float fixedStep = speed * Time.deltaTime;
            transform.position += (Vector3)(fixedDirection * fixedStep);

            fixedTimer += Time.deltaTime;
            if (fixedTimer >= fixedLife)
                Destroy(gameObject);

            return;
        }
        
        if (stopHoming)
        {
            // UŻYWAMY ISTNIEJĄCEJ ZMIENNEJ straightDirection DO KONTUACJI LOTU PO PIERCE
            float straightStep = speed * Time.deltaTime;
            transform.position += (Vector3)(straightDirection.normalized * straightStep); // Używamy straightDirection

            straightTimer += Time.deltaTime;
            if (straightTimer >= straightLife)
            Destroy(gameObject);

            // Utrzymujemy stałą rotację, aby pocisk wyglądał poprawnie w nowym kierunku
            RotateTowards(straightDirection); 

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
        // Dobrze że wcześniej ten dodatkowy collider robiłem bo teraz się przydaje do lotu prostego
        if (!stopHoming && !isFixedStraight) return; 

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
