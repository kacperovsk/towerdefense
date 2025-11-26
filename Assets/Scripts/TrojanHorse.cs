using UnityEngine;

public class TrojanHorse : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemiesInside = 5;
    public float spawnSpread = 0.3f;

    public void SpawnEnemies()
    {
        Enemy selfEnemy = GetComponent<Enemy>();
        int nextWaypoint = selfEnemy.GetCurrentWaypoint();

        for (int i = 0; i < enemiesInside; i++)
        {
            Vector3 offset = new Vector3(
                (Random.value - 0.5f) * spawnSpread,
                (Random.value - 0.5f) * spawnSpread,
                0
            );

            Vector3 spawnPos = transform.position + offset;

            GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            Enemy e = obj.GetComponent<Enemy>();

            // Przeciwnicy kontynuuj¹ RUCH NASTÊPNEGO WAYPOINTU
            e.SetStartPosition(spawnPos, nextWaypoint);
        }
    }

}
