using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public enum TargetType { None, Tower, Global }
    public TargetType targetType = TargetType.None;
  
    public abstract void ApplyEffect(GameObject target);
}
