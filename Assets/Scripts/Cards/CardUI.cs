using System.Collections;
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

    [Header("Preview")]
    public float previewScale = 3.0f;
    private bool previewActive;
    private GameObject previewInstance;
    private Canvas rootCanvas;
    [Header("Preview Animation")]
    public float previewAnimTime = 0.15f;
    private RectTransform previewVisual;



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
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    private void Update()
    {
        if (previewActive && !Input.GetMouseButton(1))
        {
            HidePreview();
            return;
        }

        if (previewActive && previewVisual != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                previewVisual,
                Input.mousePosition,
                null,
                out Vector2 localMouse
            );

            Vector2 normalized = localMouse / (previewVisual.rect.size * 0.5f);
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            float tiltStrength = 12f;

            Quaternion targetRot = Quaternion.Euler(
                -normalized.y * tiltStrength,
                 normalized.x * tiltStrength,
                0
            );

            previewVisual.localRotation = Quaternion.Lerp(
                previewVisual.localRotation,
                targetRot,
                Time.deltaTime * 10f
            );

            Vector3 targetPos = new Vector3(
                normalized.x * 12f,
                normalized.y * 12f,
                0
            );

            previewVisual.localPosition = Vector3.Lerp(
                previewVisual.localPosition,
                targetPos,
                Time.deltaTime * 10f
            );
        }

        Vector3 targetScale = IsHovered ? originalScale * hoverScale : originalScale;
        rect.localScale = Vector3.Lerp(
            rect.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );
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

        if (previewActive)
            return;
        if (eventData.button == PointerEventData.InputButton.Right)
            ShowPreview();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (State == CardInteractionState.Targeting &&
            eventData.button == PointerEventData.InputButton.Left)
        {
            ResolveTargeting();
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
    void ShowPreview()
    {
        if (previewInstance != null) return;

        previewActive = true;

        previewInstance = Instantiate(gameObject, rootCanvas.transform);
        Destroy(previewInstance.GetComponent<CardUI>());

        RectTransform r = previewInstance.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.localScale = Vector3.zero;
        StartCoroutine(AnimatePreviewIn(r, originalScale * previewScale));
        r.SetAsLastSibling();

        previewVisual = previewInstance
            .GetComponentInChildren<Image>()
            .rectTransform;

        previewVisual.localRotation = Quaternion.identity;
        previewVisual.localPosition = Vector3.zero;
    }

    void HidePreview()
    {
        if (!previewActive)
            return;

        previewActive = false;

        if (previewInstance != null)
        {
            StartCoroutine(FadeAndDestroy(previewInstance));
            previewInstance = null;
            previewVisual = null;
        }
    }
    IEnumerator AnimatePreviewIn(RectTransform r, Vector3 target)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / previewAnimTime;
            float eased = Mathf.SmoothStep(0, 1, t);
            r.localScale = Vector3.LerpUnclamped(Vector3.zero, target, eased);
            yield return null;
        }
        r.localScale = target;
    }
    IEnumerator FadeAndDestroy(GameObject obj)
    {
        if (obj == null) yield break;

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        float t = 1f;

        while (t > 0f)
        {
            if (obj == null || cg == null)
                yield break;

            t -= Time.unscaledDeltaTime / 0.12f;
            cg.alpha = t;
            yield return null;
        }

        if (obj != null)
            Destroy(obj);
    }


}
