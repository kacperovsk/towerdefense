using System.Collections.Generic;
using UnityEngine;

public class Karczma : Tower
{
    [Header("Buff Settings")]
    public float buffRadius = 3f;       
    public float buffMultiplier = 1.1f; // 1.1 = +10% bazowej wartości

    [HideInInspector] public Dictionary<Tower, float> towerBuffs = new Dictionary<Tower, float>();

    private void UpdateBuffs()
    {
        Tower[] allTowers = Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);
        HashSet<Tower> currentFrameTowers = new HashSet<Tower>();

        foreach (var t in allTowers)
        {
            if (t == this || t is Farma || t.isGhost) continue;

            float dist = Vector2.Distance(transform.position, t.transform.position);
            if (dist <= buffRadius)
            {
                currentFrameTowers.Add(t);
                // Było wiele problemów z karczmą że daje np. 2 buffy tej samej wieży, więc to jest sprawdzenie czy ma już buff i jeśli nie to dodaje do listy tą wieże
                if (!towerBuffs.ContainsKey(t))
                    towerBuffs[t] = buffMultiplier;
            }
        }
        // Usuń buffy tych których nie ma w zasięgu, aka jak ktoś doda sprzedawanie wież to sprzedanie karczmy POWINNO usunąć buffy
        List<Tower> toRemove = new List<Tower>();
        foreach (var t in towerBuffs.Keys)
            if (!currentFrameTowers.Contains(t))
                toRemove.Add(t);

        foreach (var t in toRemove)
            towerBuffs.Remove(t);
        // Sumowanie buffów z kilku karczm do jednej wieży, bo zakładam że nie chcemy mieć liczb w stylu 4,84 albo 5,324
        // Technicznie niezbyt optymalne przy bardzo wielu wieżach, osobiście nie napotkałem problemów a nwm jak to inaczej zrobić więc na ten moment jest tak
        foreach (var t in currentFrameTowers)
        {
            float totalMultiplier = 1f;
            // Pobierz wszystkie karczmy w scenie
            Karczma[] allKarczmy = Object.FindObjectsByType<Karczma>(FindObjectsSortMode.None);

            foreach (var k in allKarczmy)
            {
                if (k == null || k.isGhost) continue;

                float dist = Vector2.Distance(k.transform.position, t.transform.position);
                // I tutaj ta nieoptymalna część. Przeszukuje liste karczm, sprawdzam czy wieże są w zasięgu, jak są to dostają buffa.
                // Imagine mieć problem z 100 wież, skill issue
                // Ale jokes aside, nie wydaje mi się że będzie z tym problem
                if (dist <= k.buffRadius && k.towerBuffs.ContainsKey(t))
                    totalMultiplier += (k.towerBuffs[t] - 1f);
                    //obviously jak jest ta wieża to dostaje buffa
            }
            // Tutaj wartość statystyk wieży przestawiona na sume buffów
            t.damage = t.baseDamage * totalMultiplier;
            t.fireRate = t.baseFireRate * totalMultiplier;
        }
    }

    void Update()
    {
        if (justPlaced)
        {
            justPlaced = false;
            return;
        }

        UpdateBuffs();
        // Osobne rysowanie zasięgu karczmy bo orginalne z tower.cs nie chciało działać, who cares
        if (IsShowingRange())
            DrawRangeCircle(buffRadius);
    }

    private void OnDestroy()
    {
        towerBuffs.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, buffRadius);
    }
}
