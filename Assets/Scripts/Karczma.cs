using System.Collections.Generic;
using UnityEngine;

public class Karczma : Tower
{
    [Header("Buff Settings")]
    public float buffRadius = 3f;
    public float buffMultiplier = 1.1f; // +10%

    private HashSet<Tower> buffedTowers = new HashSet<Tower>();

    private void Update()
    {
        if (justPlaced)
        {
            justPlaced = false;
            return;
        }

        UpdateBuffs();

        if (IsShowingRange())
            DrawRangeCircle(buffRadius);
    }

    private void UpdateBuffs()
    {
        Tower[] allTowers = Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);
        HashSet<Tower> towersInRange = new HashSet<Tower>();

        foreach (var t in allTowers)
        {
            if (t == this || t is Farma || t.isGhost)
                continue;

            float dist = Vector2.Distance(transform.position, t.transform.position);
            if (dist <= buffRadius)
            {
                towersInRange.Add(t);

                if (!buffedTowers.Contains(t))
                {
                    ApplyBuffToTower(t);
                    buffedTowers.Add(t);
                }
            }
        }

        List<Tower> toRemove = new List<Tower>();
        foreach (var t in buffedTowers)
        {
            if (!towersInRange.Contains(t))
            {
                RemoveBuffFromTower(t);
                toRemove.Add(t);
            }
        }

        foreach (var t in toRemove)
            buffedTowers.Remove(t);
    }

    private void ApplyBuffToTower(Tower t)
    {
        t.ApplyDamageBuff(buffMultiplier);
        t.ApplyAttackSpeedBuff(buffMultiplier);
    }

    private void RemoveBuffFromTower(Tower t)
    {
        t.RemoveDamageBuff(buffMultiplier);
        t.RemoveAttackSpeedBuff(buffMultiplier);
    }

    private void OnDestroy()
    {
  
        foreach (var t in buffedTowers)
        {
            if (t != null)
                RemoveBuffFromTower(t);
        }

        buffedTowers.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, buffRadius);
    }
}
