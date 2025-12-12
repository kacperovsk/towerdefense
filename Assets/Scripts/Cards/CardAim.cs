using UnityEngine;

public class CardAim : MonoBehaviour
{
    public Transform startPoint;    
    public LineRenderer line;       
    public Transform arrow;         

    public int curveResolution = 20;
    public float curveHeight = 2f;

    private Tower hoveredTower;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (line == null) line = GetComponent<LineRenderer>();
        if (arrow == null && transform.childCount > 0) arrow = transform.GetChild(0);


    }

    void Update()
    {
        if (startPoint == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector3 start = GetWorldPosition(startPoint);
        DrawCurve(start, mouseWorld);

        UpdateArrow(start, mouseWorld);

        DetectTower(mouseWorld);
    }

    void DrawCurve(Vector3 start, Vector3 end)
    {
        Vector3 control = (start + end) / 2f;
        control.y += curveHeight;

        line.positionCount = Mathf.Max(2, curveResolution);

        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);
            Vector3 p = Mathf.Pow(1 - t, 2) * start +
                        2f * (1 - t) * t * control +
                        Mathf.Pow(t, 2) * end;
            line.SetPosition(i, p);
        }
    }

    void UpdateArrow(Vector3 start, Vector3 end)
    {
        if (arrow != null)
        {
            arrow.position = end;

            Vector3 dir = end - start;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrow.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    void DetectTower(Vector3 mouseWorld)
    {
        Collider2D col = Physics2D.OverlapPoint(mouseWorld);
        Tower t = (col != null) ? col.GetComponent<Tower>() : null;

        if (t != hoveredTower)
        {
            if (hoveredTower != null)
                hoveredTower.Unhighlight();

            hoveredTower = t;

            if (hoveredTower != null)
                hoveredTower.Highlight();
        }
    }

    public Tower StopAiming()
    {
        if (hoveredTower != null)
        {
            hoveredTower.Unhighlight();
        }
        return hoveredTower;
    }

    Vector3 GetWorldPosition(Transform t)
    {
        RectTransform rt = t as RectTransform;

        if (rt != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);

            Vector3 world;
            world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
            world.z = 0;

            return world;
        }

        return t.position;
    }

    public void SetStartPointPosition()
    {
        if (startPoint == null) return;

        Vector3 start = GetWorldPosition(startPoint);

        if (arrow != null)
            arrow.position = start;

        if (line != null)
        {
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, start);
        }
    }
}
