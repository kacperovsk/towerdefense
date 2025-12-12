using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/PassiveGoldIncome")]
public class PassiveGoldIncomeEffect : CardEffect
{
    public int addAmount = 5; 

    public override void ApplyEffect(GameObject target)
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.passiveGoldIncome += addAmount;
            Debug.Log("Passive gold per wave increased by " + addAmount + ". Now: " + gm.passiveGoldIncome);
        }
    }
}
