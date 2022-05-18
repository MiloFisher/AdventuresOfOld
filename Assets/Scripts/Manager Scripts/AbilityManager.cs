using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AdventuresOfOldMultiplayer;
using TMPro;

public class AbilityManager : Singleton<AbilityManager>
{
    public GameObject abilityDisplay;
    public Skill[] skills;
    public Dictionary<string, Skill> skillReference = new Dictionary<string, Skill>();
    public Color warriorColor;
    public Color paladinColor;
    public Color rogueColor;
    public Color rangerColor;
    public Color sorcererColor;
    public Color necromancerColor;
    public Color generalColorSTR;
    public Color generalColorDEX;
    public Color generalColorINT;
    public Color traitColor;
    public Color racialColor;

    private void Start()
    {
        foreach (Skill s in skills)
            skillReference.Add(s.skillName, s);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && !abilityDisplay.GetComponent<UIAbilityDisplay>().animating)
        {
            if (abilityDisplay.GetComponent<UIAbilityDisplay>().opened)
                abilityDisplay.GetComponent<UIAbilityDisplay>().Close();
            else
                abilityDisplay.SetActive(true);
        }
    }

    public void ShowAbilityDisplay()
    {
        if (abilityDisplay.GetComponent<UIAbilityDisplay>().opened || abilityDisplay.GetComponent<UIAbilityDisplay>().animating)
            return;
        abilityDisplay.SetActive(true);
    }

    public Skill GetSkill(string skillName)
    {
        if (skillReference.ContainsKey(skillName))
            return skillReference[skillName];
        Debug.LogError("Skill not found: " + skillName);
        return default;
    }

    public List<Skill> GetPassives()
    {
        List<Skill> x = new List<Skill>();
        foreach(Skill s in skills)
        {
            if (HasAbilityUnlocked(s) && s.type == SkillType.PASSIVE)
                x.Add(s);
        }
        return x;
    }

    public List<Skill> GetSkills()
    {
        List<Skill> x = new List<Skill>();
        foreach (Skill s in skills)
        {
            if (HasAbilityUnlocked(s) && s.type != SkillType.PASSIVE)
                x.Add(s);
        }
        return x;
    }

    public bool HasAbilityUnlocked(Skill s, Player p = default)
    {
        if (s == default)
            return default;
        if (p == default)
            p = PlayManager.Instance.localPlayer;

        return s.school switch
        {
            SkillSchool.GENERAL_STR => PlayManager.Instance.GetStrength(p) >= s.requirement,
            SkillSchool.GENERAL_DEX => PlayManager.Instance.GetDexterity(p) >= s.requirement,
            SkillSchool.GENERAL_INT => PlayManager.Instance.GetIntelligence(p) >= s.requirement,
            SkillSchool.WARRIOR => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Warrior",
            SkillSchool.PALADIN => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Paladin",
            SkillSchool.ROGUE => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Rogue",
            SkillSchool.RANGER => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Ranger",
            SkillSchool.SORCERER => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Sorcerer",
            SkillSchool.NECROMANCER => PlayManager.Instance.GetLevel(p) >= s.requirement && p.Class.Value == "Necromancer",
            SkillSchool.TRAIT => s.skillName == p.Trait.Value,
            SkillSchool.RACIAL_HUMAN => p.Race.Value == "Human",
            SkillSchool.RACIAL_HIGH_ELF => p.Race.Value == "High Elf",
            SkillSchool.RACIAL_NIGHT_ELF => p.Race.Value == "Night Elf",
            SkillSchool.RACIAL_DWARF => p.Race.Value == "Dwarf",
            SkillSchool.RACIAL_CENTAUR => p.Race.Value == "Centaur",
            SkillSchool.RACIAL_LEONIN => p.Race.Value == "Leonin",
            SkillSchool.RACIAL_AASIMAR => p.Race.Value == "Aasimar",
            _ => false
        };
    }

    public bool CanUseSkill(Skill s, Player p = default)
    {
        if (s == default)
            return default;
        if (p == default)
            p = PlayManager.Instance.localPlayer;

        if (s.type == SkillType.ATTACK)
            return PlayManager.Instance.GetAbilityCharges(p) >= s.cost && CombatManager.Instance.InCombat() && CombatManager.Instance.CanUseAttackAbilities();
        else if (s.type == SkillType.UTILITY)
        {
            // Special case for all utility abilities
            return PlayManager.Instance.GetAbilityCharges(p) >= s.cost && false;
        }
        else
            return false;
    }

    public Color GetSkillColor(Skill s)
    {
        if (s == default)
            return default;
  
        return s.school switch
        {
            SkillSchool.GENERAL_STR => generalColorSTR,
            SkillSchool.GENERAL_DEX => generalColorDEX,
            SkillSchool.GENERAL_INT => generalColorINT,
            SkillSchool.WARRIOR => warriorColor,
            SkillSchool.PALADIN => paladinColor,
            SkillSchool.ROGUE => rogueColor,
            SkillSchool.RANGER => rangerColor,
            SkillSchool.SORCERER => sorcererColor,
            SkillSchool.NECROMANCER => necromancerColor,
            SkillSchool.TRAIT => traitColor,
            SkillSchool.RACIAL_HUMAN => racialColor,
            SkillSchool.RACIAL_HIGH_ELF => racialColor,
            SkillSchool.RACIAL_NIGHT_ELF => racialColor,
            SkillSchool.RACIAL_DWARF => racialColor,
            SkillSchool.RACIAL_CENTAUR => racialColor,
            SkillSchool.RACIAL_LEONIN => racialColor,
            SkillSchool.RACIAL_AASIMAR => racialColor,
            _ => default
        };
    }

    public UnityAction GetAbilityCall(Skill s)
    {
        if (s == default)
            return default;
        return s.UseSkill;
    }

    public void FormatTooltip(GameObject display, Skill s)
    {
        if (s == default || display == null)
            return;

        TMP_Text textContainer = display.GetComponentInChildren<TMP_Text>();
        textContainer.text = "<b><color=" + FormatColorString(s) + ">" + s.skillName + "</color></b>" + FormatCost(s.cost, s.type) + "\n" + FormatRequirement(s.requirement, s.school) + " " + FormatType(s.type) + "\n" + s.description;
        textContainer.ForceMeshUpdate(true, true);
        if(textContainer.textInfo != null)
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(5000, 580 + 320 * textContainer.textInfo.lineCount);
    }

    private string FormatCost(int cost, SkillType type)
    {
        return type != SkillType.PASSIVE ? ": (Cost " + cost + ")" : "";
    }

    private string FormatRequirement(int requirement, SkillSchool school)
    {
        return school switch
        {
            SkillSchool.WARRIOR => "Level " + requirement + " Warrior",
            SkillSchool.PALADIN => "Level " + requirement + " Paladin",
            SkillSchool.ROGUE => "Level " + requirement + " Rogue",
            SkillSchool.RANGER => "Level " + requirement + " Ranger",
            SkillSchool.SORCERER => "Level " + requirement + " Sorcerer",
            SkillSchool.NECROMANCER => "Level " + requirement + " Necromancer",
            SkillSchool.GENERAL_STR => "Strength (" + requirement + ")",
            SkillSchool.GENERAL_DEX => "Dexterity (" + requirement + ")",
            SkillSchool.GENERAL_INT => "Intelligence (" + requirement + ")",
            SkillSchool.TRAIT => "Trait",
            _ => "Racial"
        };
    }

    private string FormatType(SkillType type)
    {
        return type switch
        {
            SkillType.PASSIVE => "Passive",
            SkillType.UTILITY => "Utility",
            SkillType.ATTACK => "Attack",
            _ => ""
        };
    }

    private string FormatColorString(Skill s)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(GetSkillColor(s));
    }

    public void AttackSkillUsed(Skill s)
    {
        abilityDisplay.GetComponent<UIAbilityDisplay>().Close(() => {
            abilityDisplay.GetComponent<UIAbilityDisplay>().combatOptions.AttackAbilityUsed(s);
        });
    }

    public void PayCost(int cost)
    {
        PlayManager.Instance.localPlayer.LoseAbilityCharges(cost);
    }

    public void UseSkill(string name)
    {
        Invoke(name, 0);
    }

    // Warrior Abilities
    #region Warrior Abilities
    private void Battlelust()
    {

    }

    private void BalancedStrike()
    {
        AttackSkillUsed(GetSkill("Balanced Strike"));
    }

    private void Revenge()
    {

    }

    private void CrushingBlow()
    {
        AttackSkillUsed(GetSkill("Crushing Blow"));
    }

    private void IronWill()
    {

    }

    private void Rally()
    {

    }
    #endregion

    // Paladin Abilities
    #region Paladin Abilties
    private void AngelicWrath()
    {

    }

    private void DivineStrike()
    {
        AttackSkillUsed(GetSkill("Divine Strike"));
    }

    private void Bless()
    {

    }

    private void HolyBlade()
    {
        AttackSkillUsed(GetSkill("Holy Blade"));
    }

    private void JusticarsVow()
    {

    }

    private void PurificationCircle()
    {

    }
    #endregion

    // Rogue Abilities
    #region Rogue Abilties
    private void Stealth()
    {

    }

    private void Backstab()
    {
        AttackSkillUsed(GetSkill("Backstab Strike"));
    }

    private void Lacerate()
    {
        AttackSkillUsed(GetSkill("Lacerate"));
    }

    private void CombinationStrike()
    {

    }

    private void ShadowStep()
    {

    }

    private void Vanish()
    {

    }
    #endregion

    // Ranger Abilities
    #region Ranger Abilities
    private void TacticalPositioning()
    {

    }

    private void QuickShot()
    {
        AttackSkillUsed(GetSkill("Quick Shot"));
    }

    private void FlamingShot()
    {

    }

    private void ArrowBarrage()
    {
        AttackSkillUsed(GetSkill("Arrow Barrage"));
    }

    private void NaturesCharm()
    {

    }

    private void SurvivalKit()
    {

    }
    #endregion

    // Sorcerer Abilities
    #region Sorcerer Abilties
    private void InfusedStrikes()
    {

    }

    private void ArcaneBolt()
    {
        AttackSkillUsed(GetSkill("Arcane Bolt"));
    }

    private void ThunderStrike()
    {
        AttackSkillUsed(GetSkill("Thunder Strike"));
    }

    private void Fireball()
    {
        AttackSkillUsed(GetSkill("Fireball"));
    }

    private void ElementalConduit()
    {

    }

    private void EnergyTransfusion()
    {

    }
    #endregion

    // Necromancer Abilities
    #region Necromancer Abilities
    private void SiphonLife()
    {

    }

    private void NecroticBlast()
    {
        AttackSkillUsed(GetSkill("Necrotic Blast"));
    }

    private void RaiseUndead()
    {

    }

    private void BloodSacrifice()
    {

    }

    private void EternalServitude()
    {

    }

    private void MassResurrection()
    {

    }
    #endregion

    // General Abilities
    #region General Abilities
    private void Taunt()
    {

    }

    private void BattleCharge()
    {

    }

    private void Dodge()
    {

    }

    private void TreasureHunter()
    {

    }

    private void Focus()
    {

    }

    private void SapEnergy()
    {
        AttackSkillUsed(GetSkill("Sap Energy"));
    }
    #endregion

    // Racial Abilities
    #region Racial Abilities
    private void Adaptable()
    {
        // Done in PlayManager NewGameSetup()
    }

    private void BattleRoar()
    {
        // Done in CombatManager OnCharacterIsAttacked(Combatant c)
    }

    private void DwarvenDefense()
    {
        // Done in PlayManager GetArmor(Player p)
    }

    private void ElvenKnowledge()
    {
        // Done in PlayManager GetStatModFromType(string statRollType)
    }

    private void HeavensParagon()
    {
        // Done in Player RestoreHealth(int amount) and in PlayManager GetAttack(Player p)
    }

    private void HorsebackRiding()
    {
        // Done in PlayManager MovePhase(Vector3Int pos = default)
    }

    private void LoneWolf()
    {
        // Done in CombatManager LoadIntoCombat()
    }
    #endregion

    // Trait Abilities
    #region Trait Abilities
    private void Berserk()
    {

    }

    private void FleetFooted()
    {

    }

    private void Generalist()
    {

    }

    private void Healthy()
    {

    }

    private void Highborn()
    {

    }

    private void Holy()
    {

    }

    private void Looter()
    {

    }

    private void Mystical()
    {

    }

    private void Powerful()
    {

    }
    #endregion
}
