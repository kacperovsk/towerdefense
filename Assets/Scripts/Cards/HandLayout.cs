using UnityEngine;

public class HandLayout : MonoBehaviour
{
    public RectTransform barRect;
    public float baseSpacing = 160f;  
    public float minSpacing = 60f;
    private float cardWidth;

    void Start()
    {
        if (transform.childCount > 0)
        {
            RectTransform rt = transform.GetChild(0).GetComponent<RectTransform>();
            cardWidth = rt.rect.width;
        }
        else
            cardWidth = 120f; 
    }
    void LateUpdate()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float barWidth = barRect.rect.width;

        float maxSpacingAllowed = (barWidth - cardWidth) / (count - 1);

        float spacing = Mathf.Clamp(maxSpacingAllowed, minSpacing, baseSpacing);

        float startX = -((count - 1) * spacing) / 2;

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);

            CardUI ui = card.GetComponent<CardUI>();
            if (ui != null && ui.isHovered)
                continue; 
            Vector3 target = new Vector3(startX + i * spacing, 0, 0);
            card.localPosition = Vector3.Lerp(card.localPosition, target, Time.deltaTime * 10f);
        }
    }
}
