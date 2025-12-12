using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image rarityFrame;

    private RectTransform rect;
    private CanvasGroup cg;

    private CardAim aiming;
    private CardData cardData;

    public void Setup(CardData newData)
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (cg == null) cg = GetComponent<CanvasGroup>();

        cardData = newData;
        iconImage.sprite = newData.icon;
        nameText.text = newData.cardName;
        descriptionText.text = newData.description;

        switch (newData.rarity)
        {
            case CardData.Rarity.Common: rarityFrame.color = Color.white; break;
            case CardData.Rarity.Uncommon: rarityFrame.color = Color.green; break;
            case CardData.Rarity.Rare: rarityFrame.color = Color.blue; break;
            case CardData.Rarity.Epic: rarityFrame.color = new Color(0.6f, 0, 0.8f); break;
            case CardData.Rarity.Legendary: rarityFrame.color = new Color(1f, 0.5f, 0f); break;
        }

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = false;

        GameObject aimObj = Instantiate(CardManager.Instance.aimPrefab, transform.parent);
        aiming = aimObj.GetComponent<CardAim>();

        aiming.startPoint = this.transform;
 
        //LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

        aiming.SetStartPointPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        Tower selectedTower = aiming.StopAiming();

        if (selectedTower != null)
        {
            if (cardData.effect != null)
            {
                cardData.effect.ApplyEffect(selectedTower.gameObject);
            }

            Destroy(gameObject); // usun kartê
        }

        Destroy(aiming.gameObject);

    }
}
