using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/TowerRange")]
public class TowerRangeEffect : CardEffect
{
    public float addAmount = 1.2f; 

    public override void ApplyEffect(GameObject target)
    {
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.ApplyRangeBuff(addAmount);
        }
    }
}
