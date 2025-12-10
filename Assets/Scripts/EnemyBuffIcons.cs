using UnityEngine;
using System.Collections.Generic;

public class EnemyBuffIcons : MonoBehaviour
{
    [Header("UI")]
    public Transform iconAnchor;   // miejsce nad g³ow¹
    public GameObject iconPrefab;  // prefab pojedynczej ikonki buffa

    private Dictionary<Enemy.AuraStat, GameObject> activeIcons = new Dictionary<Enemy.AuraStat, GameObject>();

    public void ShowIcon(Enemy.AuraStat stat, Sprite sprite)
    {
        if (activeIcons.ContainsKey(stat))
            return;

        GameObject icon = Instantiate(iconPrefab, iconAnchor);
        icon.GetComponent<SpriteRenderer>().sprite = sprite;

        // zapamietaj ikone
        activeIcons[stat] = icon;

        //ustawienie w pionie jesli jest kilka naraz
        RepositionIcons();
    }

    public void HideIcon(Enemy.AuraStat stat)
    {
        if (!activeIcons.ContainsKey(stat))
            return;

        Destroy(activeIcons[stat]);
        activeIcons.Remove(stat);

        RepositionIcons();
    }

    private void RepositionIcons()
    {
        float offset = 0f;
        foreach (var entry in activeIcons)
        {
            entry.Value.transform.localPosition = new Vector3(offset, 0f, 0f);
            offset += 0.2f; // odstep miedzy ikonami
        }
    }
}
