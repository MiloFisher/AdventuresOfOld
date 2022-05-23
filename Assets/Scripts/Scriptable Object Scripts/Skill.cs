using UnityEngine;

public enum SkillSchool { WARRIOR, PALADIN, ROGUE, RANGER, SORCERER, NECROMANCER, GENERAL_STR, GENERAL_DEX, GENERAL_INT, TRAIT, RACIAL_HUMAN, RACIAL_HIGH_ELF, RACIAL_NIGHT_ELF, RACIAL_DWARF, RACIAL_CENTAUR, RACIAL_LEONIN, RACIAL_AASIMAR };
public enum SkillType { PASSIVE, ATTACK, UTILITY };

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public int requirement;
    public SkillType type;
    public int cost;
    public SkillSchool school;
    public string description;
    public void UseSkill() {
        if (!AbilityManager.Instance.usingAbility)
        {
            AbilityManager.Instance.UseSkill(name);
            AbilityManager.Instance.PayCost(cost);
        }
        else
            Debug.Log(skillName + " failed.  A different ability is being used right now!");
    }
}