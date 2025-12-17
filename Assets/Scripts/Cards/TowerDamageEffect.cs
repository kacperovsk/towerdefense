using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/TowerDamage")]
public class TowerDamageEffect : CardEffect
{
    public float addAmount = 1.2f;

    private void OnEnable()
    {
        targetType = TargetType.Tower;
    }

    public override void ApplyEffect(GameObject target)
    {
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.ApplyDamageBuff(addAmount);
        }
    }
}
