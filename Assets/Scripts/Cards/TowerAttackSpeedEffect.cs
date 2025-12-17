using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/TowerAttackSpeed")]
public class TowerAttackSpeedEffect : CardEffect
{
    public float addAmount = 1.2f;

    public override void ApplyEffect(GameObject target)
    {
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.ApplyAttackSpeedBuff(addAmount);
        }
    }
}
