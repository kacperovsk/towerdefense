using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    public float speed = 4f;
    public float damage = 4f;
    private bool hasHit = false; // Czy juz uderzylo, Zapobiega multihitowi.
    public void SetTarget(Enemy _target)
    {
        target = _target;
    }
    void OnEnable()
    {
        hasHit = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
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
