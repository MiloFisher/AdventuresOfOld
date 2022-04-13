using UnityEngine;

public class ConsumableCard : LootCard
{
    public void UseEffect() { ConsumableManager.Instance.CallEffect(name); }
}
