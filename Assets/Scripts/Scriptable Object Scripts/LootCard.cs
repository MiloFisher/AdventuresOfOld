using UnityEngine;

public enum Rarity { COMMON, UNCOMMON, RARE, LEGENDARY }

public class LootCard : ScriptableObject
{
    public string cardName;
    public Rarity rarity;
    public int copies;
    public string itemType;
    public string effectDescription;
    public int buyCost;
    public int sellCost;
}
