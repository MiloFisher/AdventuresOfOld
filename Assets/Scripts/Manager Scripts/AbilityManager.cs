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

    public bool usingAbility;

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
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(5000, 560 + 340 * textContainer.textInfo.lineCount);
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
            if (PlayManager.Instance.GetAbilityCharges(p) < s.cost)
                return false;
            // Special case for all utility abilities
            return s.skillName switch
            {
                "Holy" => CombatManager.Instance.InCombat() && !CombatManager.Instance.usedHoly && CombatManager.Instance.IsCombatant(PlayManager.Instance.localPlayer),
                "Taunt" => CombatManager.Instance.PlayerRequestedTaunt(),
                "Rally" => CombatManager.Instance.InCombat(),
                "Bless" => !CombatManager.Instance.choice.activeInHierarchy,
                "Purification Circle" => CombatManager.Instance.InCombat(),
                "Vanish" => CombatManager.Instance.InCombat() && CombatManager.Instance.IsCombatant(PlayManager.Instance.localPlayer) && !CombatManager.Instance.GetCombatantFromPlayer(PlayManager.Instance.localPlayer).HasVanish(),
                _ => false
            };
        }
        else
            return false;
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
        // Done in PlayManager GetAttack(Player p)
    }

    private void BalancedStrike()
    {
        AttackSkillUsed(GetSkill("Balanced Strike"));
    }

    private void Revenge()
    {
        // Done in CombatManager AttackPlayer(Combatant c, Action OnComplete, List<Effect> debuffs = default)
    }

    private void CrushingBlow()
    {
        AttackSkillUsed(GetSkill("Crushing Blow"));
    }

    private void IronWill()
    {
        // Done in Player TakeDamage(int amount, int armor, bool isTrue = false)
    }

    private void Rally()
    {
        // *** Has special use case ***
        foreach (Combatant c in CombatManager.Instance.turnOrderCombatantList)
        {
            if(c.combatantType == CombatantType.PLAYER)
            {
                if (CombatManager.Instance.IsThisCombatantsTurn(c))
                    CombatManager.Instance.InflictEffect(c, new Effect("Power Up", 2, 1, true));
                else
                    CombatManager.Instance.InflictEffect(c, new Effect("Power Up", 1, 1, true));
            }
        }
    }
    #endregion

    // Paladin Abilities
    #region Paladin Abilties
    private void AngelicWrath()
    {
        // Done in Combatant RestoreHealth(int amount)
    }

    private void DivineStrike()
    {
        AttackSkillUsed(GetSkill("Divine Strike"));
    }

    private void Bless()
    {
        usingAbility = true;
        // *** Has special use case ***
        PlayManager.Instance.TargetPlayerSelection("Choose a player to Bless", true, true, true, (p) => {
            // OnSelect offer choice to restore 8 health or give +1 power
            CombatManager.Instance.MakeChoice("Restore +8 Health", "Give +1 Power", true, CombatManager.Instance.InCombat() && CombatManager.Instance.IsCombatant(p));
            CombatManager.Instance.ChoiceListener((a) => {
                if(a == 1)
                {
                    if (CombatManager.Instance.InCombat())
                        CombatManager.Instance.HealPlayer(p, 8);
                    else
                        p.RestoreHealth(8);
                    usingAbility = false;
                }
                else
                {
                    Combatant c = CombatManager.Instance.GetCombatantFromPlayer(p);
                    if(CombatManager.Instance.IsThisCombatantsTurn(c) && !p.HasYetToAttack.Value)
                        CombatManager.Instance.InflictEffect(c, new Effect("Power Up", 2, 1, true));
                    else
                        CombatManager.Instance.InflictEffect(c, new Effect("Power Up", 1, 1, true));
                    usingAbility = false;
                }
            });
        }, (p) => {
            // Requires being alive
            return PlayManager.Instance.GetHealth(p) > 0;
        }, false);
    }

    private void HolyBlade()
    {
        AttackSkillUsed(GetSkill("Holy Blade"));
    }

    private void JusticarsVow()
    {
        // Done in Player TakeDamage(int amount, int armor, bool isTrue = false) and in TransitionStartOfCombat OnDisable()
    }

    private void PurificationCircle()
    {
        // *** Has special use case ***
        foreach (Combatant c in CombatManager.Instance.turnOrderCombatantList)
        {
            if (c.combatantType == CombatantType.PLAYER)
            {
                CombatManager.Instance.CleanseAllEffectsFromPlayer(c.player);
                CombatManager.Instance.HealPlayer(c.player, 10);
            }
        }
    }
    #endregion

    // Rogue Abilities
    #region Rogue Abilties
    private void Stealth()
    {
        // Done in UIEncounterCard FightMonster()
    }

    private void Backstab()
    {
        if (!CombatManager.Instance.monsterTookTurn)
            CombatManager.Instance.InflictEffect(CombatManager.Instance.GetCombatantFromPlayer(PlayManager.Instance.localPlayer), new Effect("Power Up", 1, 1, true));
        AttackSkillUsed(GetSkill("Backstab"));
    }

    private void Lacerate()
    {
        AttackSkillUsed(GetSkill("Lacerate"));
    }

    private void CombinationStrike()
    {
        // Done in CombatManager AttackMonster(Combatant c, int damage, List<Effect> debuffs = default, Action OnAttack = default)
    }

    private void ShadowStep()
    {
        // Done in CombatManager GetMonsterTargets()
    }

    private void Vanish()
    {
        // *** Has special use case ***
        CombatManager.Instance.InflictEffect(CombatManager.Instance.GetCombatantFromPlayer(PlayManager.Instance.localPlayer), new Effect("Vanish", 2));
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
        // *** Has special use case ***
        PlayManager.Instance.localPlayer.SetValue("Taunting", true);
        foreach(Player p in PlayManager.Instance.playerList)
        {
            if (p.RequestedTaunt.Value)
            {
                p.SetValue("RequestedTaunt", false);
                PlayManager.Instance.localPlayer.SendTauntReceivedNotification(p);
            }
        }
    }

    private void BattleCharge()
    {
        PlayManager.Instance.localPlayer.SetValue("ParticipatingInCombat", 1);
        PlayManager.Instance.localPlayer.SetPosition(PlayManager.Instance.turnOrderPlayerList[PlayManager.Instance.turnMarker].Position.Value);
    }

    private void Dodge()
    {
        // Done in UIDefenseManager and in UIDodgeRoll
    }

    private void TreasureHunter()
    {
        // Done in LootManager AnimateClosing()
    }

    private void Focus()
    {
        // Done in UIAttackRoll AnimateDiceRoll()
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
        // Done in Player RestoreHealth(int amount) and in PlayManager GetAttack(Player p) and in CombatManager HealPlayer(Player p, int amount)
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
        // Done in Combatant TakeDamage(int amount, bool isTrue = false)
    }

    private void FleetFooted()
    {
        // Done in Character Creation Scene
    }

    private void Generalist()
    {
        // Done in PlayManager GetStatModFromType(string statRollType)
    }

    private void Healthy()
    {
        // Done in Character Creation Scene
    }

    private void Highborn()
    {
        // Done in Character Creation Scene and in Player LevelUpCheckClientRPC(ClientRpcParams clientRpcParams = default)
    }

    private void Holy()
    {
        // *** Has special use case ***
        CombatManager.Instance.UseHoly(PlayManager.Instance.localPlayer);
    }

    private void Looter()
    {
        // Done in CombatManager EndCombat(int result)
    }

    private void Mystical()
    {
        // Done in Character Creation Scene
    }

    private void Powerful()
    {
        // Done in Character Creation Scene
    }
    #endregion
}
