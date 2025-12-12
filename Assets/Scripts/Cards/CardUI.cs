using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Card Settings")]
    public float hoverScale = 1.2f;
    public float animationSpeed = 10f;

    [Header("References")]
    public RectTransform rect;
    public Image cardImage;
    public CardData cardData; 

    private Vector3 originalScale;
    public bool isHovered = false;
    private bool isDragging = false;

    private GameObject aimIndicator;
    //private bool dragStarted = false;
    private int originalIndex;

    private void Awake()
    {
        originalScale = rect.localScale;
    }

    private void Update()
    {
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.deltaTime * animationSpeed);

        if (isDragging && aimIndicator != null)
        {
            Vector3 pos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rect, Input.mousePosition, null, out pos);
            aimIndicator.transform.position = pos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        originalIndex = transform.GetSiblingIndex();
        transform.SetSiblingIndex(999); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.SetSiblingIndex(originalIndex);
    }
    private void CleanupDrag()
    {
        Destroy(aimIndicator);
        aimIndicator = null;
        //dragStarted = false;     
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
        {
            //dragStarted = false; 
            if (cardData.effect.targetType == CardEffect.TargetType.None)
                CardManager.Instance.UseInstantCard(this);
            return;
        }

        isDragging = false;

        var type = cardData.effect.targetType;


        if (type == CardEffect.TargetType.Tower)
        {
            Tower tower = CardManager.Instance.GetTowerUnderMouse();
            if (tower != null)
            {
                CardManager.Instance.UseCardOnTower(this, tower);
                CleanupDrag();
                return;
            }
        }

        CleanupDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

}
