using UnityEngine;

public class Special5 : MonoBehaviour
{
    [Header("Unik obra¿eñ")]
    [Range(0f, 1f)]
    public float dodgeChance = 0.3f;

    [Header("Kradzie¿ pieniêdzy po dojœciu do koñca")]
    public int moneyToSteal = 20;


    public bool TryDodge()
    {
        return Random.value < dodgeChance;
    }


    public void StealMoney()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpendMoney(moneyToSteal);
        }
    }
}
