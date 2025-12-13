using UnityEngine;

[RequireComponent(typeof(Projectile))]

// Skrypt do zarządzania pociskami, które od początku lecą na wprost do celu.
public class FixedPiercingBehaviour : MonoBehaviour
{
    [Tooltip("Ile dodatkowych trafień po pierwszym (0 = znika po pierwszym).")]
    public int additionalPierces = 1;

    [Tooltip("Ile sekund pocisk leci dalej po przebiciu (fixedDirection).")]
    public float lifeAfterPierce = 5f;

    private Projectile projectile;
    private int remainingPierces;
    private Vector2 initialDirection;
    
    public void Initialize(Vector3 startPosition, Vector3 targetPosition)
    {
        projectile = GetComponent<Projectile>();
        remainingPierces = additionalPierces;

        // Oblicz kierunek lotu na podstawie pozycji startowej i docelowej
        initialDirection = (targetPosition - startPosition).normalized;

        // Natychmiast przełączam pocisk w tryb lotu stałego
        projectile.ActivateFixedStraight(initialDirection, lifeAfterPierce);
        
        // Podpinam pod trafienie
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
            // jeśli pocisk ma zniknąć OD RAZU
            // zostawiam gdybym potrzebował do triple shota potem
            // Destroy(gameObject); 
            
            // jeśli ma zniknąć po fixedLife
            return; 
        }
        projectile.preventDestruction = true; 
        remainingPierces--;
    }
}