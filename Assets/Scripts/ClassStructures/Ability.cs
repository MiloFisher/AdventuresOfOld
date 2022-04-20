using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Ability {
    private string name;
    private int level;
    private string type;
    private int cost;
    private string desc;

    private struct default_ability_content {
        public int level;
        public string type;
        public int cost;
        public string desc;

        public default_ability_content(int level, string type, int cost, string desc) {
            this.level = level;
            this.type = type;
            this.cost = cost;
            this.desc = desc;
        }
    }

    //The whole dictionary for ability descriptions
    private Dictionary<string, default_ability_content> default_ability = new Dictionary<string, default_ability_content>() {
        {"Battlelust", new default_ability_content(1, "Passive", 0, "When under 50% Health, you have bonus damage equal to your Level.")},
        {"Balanced Strike", new default_ability_content(1, "Attack", 1, "Make an Attack Roll. On a success, deal your Damage + STR Mod.  On a failure, gain +3 Armor for this turn.")},
        {"Revenge", new default_ability_content(2, "Utility", 1, "When you fail an Attack Roll, in addition to getting hit by the Monster, the Monster receives your Damage as well. Your next attack this combat has +1 Power.")},
        {"Crushing Blow", new default_ability_content(3, "Attack", 1, "Make an Attack Roll.  On a success, deal your normal Damage and inflict Dazed. (Dazed Monsters miss their next attack on a player)")},
        {"Iron Will", new default_ability_content(4, "Passive", 0, "The first time you die in each combat, Revive with 1 Health.")},
        {"Rally", new default_ability_content(5, "Utility", 2, "All players in combat gain +1 Power until the end of their next turn.")},
        {"Angelic Wrath", new default_ability_content(1, "Passive", 0, "Whenever you recover Health in combat, gain +2 Damage (stacking) until the end of the combat.")},
        {"Divine Strike", new default_ability_content(1, "Attack", 1, "Make an Attack Roll. On a success, deal your normal Damage + STR Mod and recover Health equal to half the damage dealt (rounded up).")},
        {"Bless", new default_ability_content(2, "Utility", 1, "Target yourself or an ally, and choose to either restore +8 HP or give +1 Power on their next attack.")},
        {"Holy Blade", new default_ability_content(3, "Utility", 1, "Make an Attack Roll against the Monster’s lowest Power (Physical or Magical).  On a success, deal your Damage + your INT Mod.")},
        {"Justicar's Vow", new default_ability_content(4, "Passive", 0, "In combat, the first time you fall below 50% Health, recover +2 Ability Charges.")},
        {"Purification Circle", new default_ability_content(5, "Utility", 2, "All players in combat recover +10 Health and have all negative combat effects removed.")},
        {"Stealth", new default_ability_content(1, "Passive", 0, "When encountering a Monster, you may choose to skip combat instead of fighting.  This rewards no Gold or XP, but you draw 1 Loot Card. (Regardless of monster type).")},
        {"Backstab", new default_ability_content(1, "Attack", 1, "Make an Attack Roll, and if this is used before the Monster has taken a turn, gain +1 Power for this attack.  On a success, deal your Damage + your DEX Mod.")},
        {"Lacerate", new default_ability_content(2, "Attack", 1, "Make an Attack Roll.  On a success, deal your normal Damage and inflict Bleeding (3).  (Bleeding Monsters take additional damage equal to the player’s level for the next X times they are attacked)")},
        {"Combination Strike", new default_ability_content(3, "Utility", 1, "After successfully attacking a Monster with an Attack Ability, you can immediately make a Basic Attack with +1 Power.")},
        {"Shadow Step", new default_ability_content(4, "Passive", 0, "After successfully attacking a Monster, the Monster is unable to target you with its skill on its next turn.")},
        {"Vanish", new default_ability_content(5, "Utility", 2, "Ignore all incoming damage and effects when you fail an Attack Roll on your next 2 turns.")},
        {"Tactical Positioning", new default_ability_content(1, "Passive", 0, "In each combat, the first time you fail an Attack Roll, ignore all incoming damage and effects.")},
        {"Quick Shot", new default_ability_content(1, "Attack", 1, "Make an Attack Roll.  On a success, deal your Damage + your DEX Mod, then roll a dice.  If the dice is even, this ability costs 0 Ability Charges the next time it is used this combat.")},
        {"Flaming Shot", new default_ability_content(2, "Utility", 1, "Your next attack has +1 Power and inflicts Burning (3).  (Burning Monsters take damage equal to the player’s level on their turn for X turns)")},
        {"Arrow Barrage", new default_ability_content(3, "Attack", 1, "Make an Attack Roll.  On a success, deal your normal Damage and repeat this effect with -1 Power.  Subsequent rolls for this ability count as separate attacks, however, you do not take Retaliation damage from these attacks")},
        {"Nature's Charm", new default_ability_content(4, "Passive", 0, "You have +1 when making any Stat Roll.  You also recover an extra Ability Charge during Short Rests.")},
        {"Survival Kit", new default_ability_content(5, "Attack", 2, "Remove all negative combat effects from yourself and recover +15 Health.  You have +1 Power on your next Attack.")},
        {"Infused Strikes", new default_ability_content(1, "Passive", 0, "When using an ability in combat, gain Damage equal to the amount of Ability Charges spent (stacking for this combat).")},
        {"Arcane Bolt", new default_ability_content(1, "Attack", 1, "Make an Attack Roll. On a success, deal your normal Damage and your next attack this combat has +1 Power.")},
        {"Thunder Strike", new default_ability_content(2, "Attack", 2, "Make an Attack Roll.  On success, roll 2 dice, then deal your Damage + the sum of the dice.  Then, if the sum of the dice is even, inflict Dazed.  (Dazed Monsters miss their next attack on a player)")},
        {"Fireball", new default_ability_content(3, "Attack", 2, "Make an Attack Roll.  On a success, deal your Damage + your INT Mod and inflict Burning (3). (Burning Monsters take damage equal to the player’s level on their turn for X turns)")},
        {"Elemental Conduit", new default_ability_content(4, "Passive", 0, "After spending an Ability Charge, roll a dice. An even number restores +1 Ability Charge.")},
        {"Energy Transfusion", new default_ability_content(5, "Utlility", 3, "Give an ally +2 Ability Charges. You and the ally both recover +12 Health.  (Can only target an ally once per combat)")},
        {"Siphon Life", new default_ability_content(1, "Passive", 0, "Whenever you deal damage, recover Health equal to half of the Damage dealt (rounded down).")},
        {"Necrotic Blast", new default_ability_content(1, "Attack", 1, "Make an Attack Roll.  On a success, deal your Damage + your INT Mod + your Armor.  On a failure, your Armor is reduced by half (rounded up) this turn.")},
        {"Raise Undead", new default_ability_content(2, "Utility", 2, "Summon an Undead Minion to aid you in combat.  (Usable once per combat)")},
        {"Blood Sacriface", new default_ability_content(3, "Utility", 1, "Take true Damage equal to 2x the Chaos Tier, then gain +2 Power for your next Attack Roll.")},
        {"Eternal Servitude", new default_ability_content(4, "Passive", 0, "Whenever your Undead Minion dies in combat, you may spend 1 Ability Charge to revive it with 1 Health.")},
        {"Mass Resurrection", new default_ability_content(5, "Utility", 3, "Revive all fallen allies in combat with you.")}
    };

    public string getName()
    {
        return this.name;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public int getLevel()
    {
        return this.level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public string getType()
    {
        return this.type;
    }

    public void setType(string type)
    {
        this.type = type;
    }

    public string getDesc()
    {
        return this.desc;
    }

    public void setDesc(string desc)
    {
        this.desc = desc;
    }

    public int getCost()
    {
        return this.cost;
    }

    public void setCost(int cost)
    {
        this.cost = cost;
    }


    public Ability(string name, int level, string type, int cost, string desc) {
        this.name = name;
        this.level = level;
        this.type = type;
        this.desc = desc;
        this.cost = cost;
    }

    public Ability(string name) {
        if(default_ability.ContainsKey(name)) {
            this.name = name;
            this.level = default_ability[name].level;
            this.cost = default_ability[name].cost;
            this.type = default_ability[name].type;
            this.desc = default_ability[name].desc;
        }
        else {
            this.name = name;
            this.level = -1;
            this.cost = -1;
            this.type = "Unknown";
            this.desc = "Unknown";
        }
    }

}