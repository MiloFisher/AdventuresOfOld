using UnityEngine;

public enum MonsterType {BASIC, ELITE, MINIBOSS, BOSS, MINION};
public enum Target {NONE, LOWEST_HEALTH, HIGHEST_HEALTH, ALL};

[CreateAssetMenu(fileName = "New Monster Card", menuName = "Cards/Monster Card")]
public class MonsterCard : EncounterCard
{
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
    public Sprite background;
    public Target target;
    public void Skill(Combatant c) { MonsterAbilityManager.Instance.CallSkill(name, c); }
    public void Passive() { MonsterAbilityManager.Instance.CallPassive(name); }
}
