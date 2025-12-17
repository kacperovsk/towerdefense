using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public CardDropTable dropTable;
    public bool isBoss;

    [Header("Boss Drop")]
    [Range(0, 100)] public float secondCardChance = 20f;
}
