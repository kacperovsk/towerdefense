using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public class EnemyGroupData
{
    public string prefab;
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class WaveDataJSON
{
    public EnemyGroupData[] enemyGroups;
}

[System.Serializable]
public class WavePack
{
    public WaveDataJSON[] waves;
}

public class WaveLoader : MonoBehaviour
{
    public WaveSystem waveSystem;

    void Awake()
    {
        TextAsset json;
        if (SceneManager.GetActiveScene().name == "GameMap0")
        {
             json = Resources.Load<TextAsset>("waves2"); // do tutorialu tylko...
        }

        else
        {
            json = Resources.Load<TextAsset>("waves");
        }
        WavePack pack = JsonUtility.FromJson<WavePack>(json.text);

        waveSystem.waves = new WaveSystem.WaveData[pack.waves.Length];

        for (int i = 0; i < pack.waves.Length; i++)
        {
            var w = new WaveSystem.WaveData();
            w.enemyGroups = new WaveSystem.EnemyGroup[pack.waves[i].enemyGroups.Length];

            for (int g = 0; g < w.enemyGroups.Length; g++)
            {
                var groupJson = pack.waves[i].enemyGroups[g];

                var group = new WaveSystem.EnemyGroup();


                group.enemyPrefab = Resources.Load<GameObject>("Enemies/" + groupJson.prefab);

                if (group.enemyPrefab == null)
                {
                    Debug.LogError($" Nie znaleziono prefabu: Enemies/{groupJson.prefab}");
                }

                group.count = groupJson.count;
                group.spawnInterval = groupJson.spawnInterval;

                w.enemyGroups[g] = group;
            }

            waveSystem.waves[i] = w;
        }
    }
}
