using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image rarityFrame;

    public void Setup(CardData data)
    {
        iconImage.sprite = data.icon;
        nameText.text = data.cardName;
        descriptionText.text = data.description;

        switch (data.rarity)
        {
            case CardData.Rarity.Common: rarityFrame.color = Color.white; break;
            case CardData.Rarity.Uncommon: rarityFrame.color = Color.green; break;
            case CardData.Rarity.Rare: rarityFrame.color = Color.blue; break;
            case CardData.Rarity.Epic: rarityFrame.color = new Color(0.6f, 0, 0.8f); break;
            case CardData.Rarity.Legendary: rarityFrame.color = new Color(1f, 0.5f, 0f); break;
        }
    }
}
