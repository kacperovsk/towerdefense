using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RarityChance
{
    public CardData.Rarity rarity;
    [Range(0, 100)] public float chance;
}

[CreateAssetMenu(menuName = "Cards/Drop Table")]
public class CardDropTable : ScriptableObject
{
    [Range(0, 100)] public float overallDropChance;
    public List<RarityChance> rarityChances;
}
