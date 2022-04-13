using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Card", menuName = "Cards/Weapon Card")]
public class WeaponCard : LootCard
{
    public string attackType;
    public int damage;
    public int armor;
    public int range;
    public int crit;
}
