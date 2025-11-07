using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    public float speed = 4f;
    public float damage = 4f;
    private bool hasHit = false; // prevents multiple hits

    public void SetTarget(Enemy _target)
    {
        target = _target;
    }

    void OnEnable()
    {
        hasHit = false;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // Rotate towards target
        RotateTowards(dir);

        // Move towards target
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

    void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        gameObject.SetActive(false);
    }
}
