using UnityEngine;

[RequireComponent(typeof(Projectile))]

// Jeśli ktokolwiek czyta ten skrypt: poddaje się. Nie mam pojęcia dlaczego wektory się pierdolą przy trafieniu i strzała nie leci prosto.
// Spróbowałem wielu rzeczy w ciągu wielu godzin, jedyne co zdziałałem to dodanie dobrego hitboxa do pocisku po trafieniu żeby faktycznie dmg robił i że strzała jest mniej losowa
// Błagam nie róbmy już tego :C
public class Piercing : MonoBehaviour
{
    [Tooltip("Ile dodatkowych trafień po pierwszym (0 = znika po pierwszym).")]
    public int additionalPierces = 1;

    [Tooltip("Ile sekund pocisk leci dalej po przestaniu homingu.")]
    public float lifeAfterPhase = 5f;

    private Projectile projectile;
    private int remainingPierces;

    void Awake()
    {
        projectile = GetComponent<Projectile>();
        remainingPierces = additionalPierces;
        projectile.OnHit += HandleOnHit;
    }

    void OnDestroy()
    {
        if (projectile != null)
            projectile.OnHit -= HandleOnHit;
    }

    private void HandleOnHit(Enemy e)
    {
        if (remainingPierces <= 0)
        {
            return; // pozwól pociskowi się zniszczyć
        }

        // Zablokuj automatyczne niszczenie
        projectile.preventDestruction = true;

        // Kierunek: preferujemy stabilny lastDirection ustawiony przez Projectile
        Vector2 dir = projectile.lastDirection;
        if (dir.magnitude < 0.0001f)
        {
            // fallback do transform.up jeśli lastDirection jest niepewny
            dir = projectile.transform.up;
        }

        projectile.EnterStraightPhase(dir, lifeAfterPhase);

        remainingPierces--;
    }
}