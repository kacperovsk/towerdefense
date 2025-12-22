using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image rarityFrame;
    private Image cardBackground;

    void Awake()
    {
        cardBackground = GetComponent<Image>();
    }

    public void Setup(CardData data)
    {
        iconImage.sprite = data.icon;
        nameText.text = data.cardName;
        descriptionText.text = data.description;

        switch (data.rarity)
        {
            case CardData.Rarity.Common: cardBackground.color = Color.gray; break; 
            case CardData.Rarity.Uncommon: cardBackground.color = Color.green; break;
            case CardData.Rarity.Rare: cardBackground.color = Color.blue; break;
            case CardData.Rarity.Epic: cardBackground.color = Color.pink; break;
            case CardData.Rarity.Legendary: cardBackground.color = new Color(1f, 0.5f, 0f); break;
        }
        switch (data.rarity)
        {
            case CardData.Rarity.Common: rarityFrame.color = Color.gray; break;
            case CardData.Rarity.Uncommon: rarityFrame.color = Color.green; break;
            case CardData.Rarity.Rare: rarityFrame.color = Color.blue; break;
            case CardData.Rarity.Epic: rarityFrame.color = Color.purple; break;
            case CardData.Rarity.Legendary: rarityFrame.color = new Color(1f, 0.5f, 0f); break;
        }
    }
}
