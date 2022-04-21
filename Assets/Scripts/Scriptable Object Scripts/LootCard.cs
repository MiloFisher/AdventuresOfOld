using UnityEngine;

public enum Rarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY }

public class LootCard : ScriptableObject
{
    public string cardName;
    public Rarity rarity;
    public Sprite image;
    public int copies;
    public string itemType;
    public string effectDescription;
    public int buyCost;
    public int sellCost;
}
