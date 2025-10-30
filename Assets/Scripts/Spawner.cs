using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float spawnTimer;
    private float spawnInterval = 1f;
    [SerializeField] private ObjectPooler pool;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0 )
        {
            spawnTimer = spawnInterval;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject spawnedObject = pool.getPooledObject();
        spawnedObject.transform.position = transform.position;
        spawnedObject.SetActive(true);
    }
}
