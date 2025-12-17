using System.Collections;
using UnityEngine;

public class HandLayout : MonoBehaviour
{
    public RectTransform barRect;
    public float baseSpacing = 160f;
    public float minSpacing = 60f;
    public float hoverYOffset = 80f;
    public float moveSpeed = 10f;

    private float cardWidth = 120f;

    void Start()
    {
        if (transform.childCount > 0)
            cardWidth = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
    }

    void LateUpdate()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float barWidth = barRect.rect.width;

        float maxSpacing = (barWidth - cardWidth) / Mathf.Max(1, count - 1);
        float spacing;
        if ((cardWidth + minSpacing) * count <= barWidth)
        {
            spacing = Mathf.Clamp(maxSpacing, minSpacing, baseSpacing);
        }
        else
        {
            spacing = (barWidth - cardWidth) / (count - 1);
        }
        float centerOffset = barRect.rect.width / 2f - cardWidth / 2f;
        float totalWidth = spacing * (count - 1);
        float startX = centerOffset;
        startX = -startX - 30f;

        int hoveredIndex = -1;

        for (int i = 0; i < count; i++)
        {
            CardUI ui = transform.GetChild(i).GetComponent<CardUI>();
            if (ui != null && ui.IsHovered)
            {
                hoveredIndex = i;
                break;
            }
        }

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);
            CardUI ui = card.GetComponent<CardUI>();

            if (ui != null && ui.IsActive)
                continue;

            float x = startX + i * spacing;
            float y = 0f;

            if (ui != null && ui.IsHovered)
                y = hoverYOffset;

            RectTransform rt = card as RectTransform;

            rt.anchoredPosition = Vector2.Lerp(
                rt.anchoredPosition,
                new Vector2(x, y),
                Time.deltaTime * moveSpeed
            );
        }
    }
}
