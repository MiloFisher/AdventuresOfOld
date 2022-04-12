using UnityEngine;

public enum MonsterType {BASIC, ELITE, MINIBOSS, BOSS, MINION};

[CreateAssetMenu(fileName = "New Monster Card", menuName = "Cards/Monster Card")]
public class MonsterCard : EncounterCard
{
    public int gold;
    public MonsterType type;
    public int health;
    public int attack;
    public int speed;
    public int physicalPower;
    public int magicalPower;
    public string skillName;
    public string skillDescription;
    public string passiveName;
    public string passiveDescription;
    public void Skill() { MonsterAbilityManager.Instance.CallSkill(name); }
    public void Passive() { MonsterAbilityManager.Instance.CallPassive(name); }
}
