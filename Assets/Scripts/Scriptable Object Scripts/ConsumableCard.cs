using UnityEngine;

public enum UseCondition { ANYTIME, IN_COMBAT, OUT_OF_COMBAT, NEVER };

[CreateAssetMenu(fileName = "New Consumable Card", menuName = "Cards/Consumable Card")]
public class ConsumableCard : LootCard
{
    public UseCondition useCondition;
    public void UseEffect() { ConsumableManager.Instance.CallEffect(name); }
}
