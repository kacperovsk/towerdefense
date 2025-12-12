using UnityEngine;


[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public string description;
    public Rarity rarity;
    public CardEffect effect;

    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

    
}
