using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Card Settings")]
    public float hoverScale = 1.2f;
    public float animationSpeed = 10f;

    [Header("References")]
    public RectTransform rect;
    public Image cardImage;
    public CardData cardData;

    private Vector3 originalScale;
    private CardAim aim;

    public enum CardInteractionState
    {
        None,
        Hover,
        Targeting, // LPM
        Dragging   // PPM
    }

    public CardInteractionState State { get; private set; } = CardInteractionState.None;

    public bool IsHovered =>
        State == CardInteractionState.Hover ||
        State == CardInteractionState.Targeting;

    public bool IsActive =>
        State == CardInteractionState.Targeting ||
        State == CardInteractionState.Dragging;

    private void Awake()
    {
        originalScale = rect.localScale;
    }

    private void Update()
    {
        Vector3 targetScale = IsHovered ? originalScale * hoverScale : originalScale;
        rect.localScale = Vector3.Lerp(
            rect.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        if (State == CardInteractionState.Dragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect.parent as RectTransform,
                Input.mousePosition,
                null,
                out Vector2 localPos
            );
            rect.localPosition = localPos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (State == CardInteractionState.None)
            State = CardInteractionState.Hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (State == CardInteractionState.Hover)
            State = CardInteractionState.None;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            StartTargeting();

        if (eventData.button == PointerEventData.InputButton.Right)
            StartDragging();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (State == CardInteractionState.Targeting &&
            eventData.button == PointerEventData.InputButton.Left)
        {
            ResolveTargeting();
        }

        if (State == CardInteractionState.Dragging &&
            eventData.button == PointerEventData.InputButton.Right)
        {
            StopDragging();
        }
    }



    void StartTargeting()
    {
        State = CardInteractionState.Targeting;

        GameObject go = Instantiate(
            CardManager.Instance.aimPrefab,
            transform.root
        );

        aim = go.GetComponent<CardAim>();
        aim.startPoint = rect;
        aim.SetStartPointPosition();
    }

    void ResolveTargeting()
    {
        Tower tower = aim.StopAiming();

        bool used = false;

        switch (cardData.effect.targetType)
        {
            case CardEffect.TargetType.None:
                used = CardManager.Instance.UseInstantCard(this);
                break;

            case CardEffect.TargetType.Tower:
                if (tower != null)
                    used = CardManager.Instance.UseCardOnTower(this, tower);
                break;

            case CardEffect.TargetType.Global:
                used = CardManager.Instance.UseGlobalCard(this);
                break;
        }

        if (!used)
        {
            State = CardInteractionState.None;
        }

        if (aim != null)
        {
            Destroy(aim.gameObject);
            aim = null;
        }
    }



    void StartDragging()
    {
        State = CardInteractionState.Dragging;
    }

    void StopDragging()
    {
        State = CardInteractionState.None;
    }
}
