using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Transform cardBar;

    [Header("Aiming")]
    public GameObject aimPrefab;

    [Header("Card Database")]
    public List<CardData> availableCards;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DropRandomCard();
        }
    }
    public void DropRandomCard()
    {
        CardData data = availableCards[Random.Range(0, availableCards.Count)];

        GameObject newCard = Instantiate(cardPrefab, cardBar);
        newCard.transform.localScale = Vector3.one;

        var view = newCard.GetComponent<CardView>();
        view.Setup(data);

        var ui = newCard.GetComponent<CardUI>();
        ui.cardData = data;
    }
    public CardUI GetCardUnderMouseUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult r in results)
        {
            var ui = r.gameObject.GetComponent<CardUI>();
            if (ui != null)
                return ui;
        }

        return null;
    }

    public Tower GetTowerUnderMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero);
        if (hit.collider != null)
            return hit.collider.GetComponent<Tower>();

        return null;
    }

    public bool UseCardOnTower(CardUI cardUI, Tower tower)
    {
        var card = cardUI.cardData;
        if (card?.effect == null || tower == null)
            return false;

        card.effect.ApplyEffect(tower.gameObject);
        RemoveCard(cardUI);
        return true;
    }
    public bool UseGlobalCard(CardUI cardUI)
    {
        var card = cardUI.cardData;
        if (card?.effect == null)
            return false;

        card.effect.ApplyEffect(null);
        RemoveCard(cardUI);
        return true;
    }
    public bool UseInstantCard(CardUI cardUI)
    {
        var card = cardUI.cardData;
        if (card?.effect == null)
            return false;

        card.effect.ApplyEffect(null);
        RemoveCard(cardUI);
        return true;
    }
    public void RemoveCard(CardUI cardUI)
    {
        Destroy(cardUI.gameObject);
    }

    public void DropCardOfRarity(CardData.Rarity rarity)
    {
        var pool = availableCards.FindAll(c => c.rarity == rarity);
        if (pool.Count == 0) return;

        CardData data = pool[Random.Range(0, pool.Count)];
        SpawnCard(data);
    }

    void SpawnCard(CardData data)
    {
        GameObject newCard = Instantiate(cardPrefab, cardBar);
        newCard.transform.localScale = Vector3.one;

        newCard.GetComponent<CardView>().Setup(data);
        newCard.GetComponent<CardUI>().cardData = data;
    }

}
