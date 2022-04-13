using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Card", menuName = "Cards/Consumable Card")]
public class ConsumableCard : LootCard
{
    public void UseEffect() { ConsumableManager.Instance.CallEffect(name); }
}
