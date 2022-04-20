using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Class {
    private Stats stats;
    private string name;
    private List<Ability> abilities = new List<Ability>();
    private string class_armor;
    private string class_weapon;

    public Stats get_stats()
    {
        return this.stats;
    }

    public void setStats(Stats stats)
    {
        this.stats = stats;
    }

    public string getName()
    {
        return this.name;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public List<Ability> getAbilities()
    {
        return this.abilities;
    }

    public void setAbilities(List<Ability> abilities)
    {
        this.abilities = abilities;
    }

    public string getClass_armor()
    {
        return this.class_armor;
    }

    public void setClass_armor(string class_armor)
    {
        this.class_armor = class_armor;
    }

    public string getClass_weapon()
    {
        return this.class_weapon;
    }

    public void setClass_weapon(string class_weapon)
    {
        this.class_weapon = class_weapon;
    }


    public Class(string name) {
        this.name = name;

        //Adding class specific modifiers
        switch(name.ToLower()) {
            case "warrior":
            abilities.Add(new Ability("Battlelust"));
            abilities.Add(new Ability("Balanced Strike"));
            abilities.Add(new Ability("Revenge"));
            abilities.Add(new Ability("Crushing Blow"));
            abilities.Add(new Ability("Iron Will"));
            abilities.Add(new Ability("Rally"));
            stats = new Stats(4, 1, 0, 0, 4, 0);
            class_armor = "Simple Plate Armor";
            class_weapon = "Simple Sword & Shield";
            break;
            case "paladin":
            abilities.Add(new Ability("Angelic Wrath"));
            abilities.Add(new Ability("Divine Strike"));
            abilities.Add(new Ability("Bless"));
            abilities.Add(new Ability("Holy Blade"));
            abilities.Add(new Ability("Justicar's Vow"));
            abilities.Add(new Ability("Purification Circle"));
            stats = new Stats(4, 0, 2, 0, 3, 0);
            class_armor = "Simple Plate Armor";
            class_weapon = "Simple Greatsword";
            break;
            case "rogue":
            abilities.Add(new Ability("Stealth"));
            abilities.Add(new Ability("Backstab"));
            abilities.Add(new Ability("Lacerate"));
            abilities.Add(new Ability("Combination Strike"));
            abilities.Add(new Ability("Shadow Step"));
            abilities.Add(new Ability("Vanish"));
            stats = new Stats(0, 4, 2, 0, 1, 2);
            class_armor = "Simple Leather Armor";
            class_weapon = "Simple Daggers";
            break;
            case "ranger":
            abilities.Add(new Ability("Tactical Positioning"));
            abilities.Add(new Ability("Quick Shot"));
            abilities.Add(new Ability("Flaming Shot"));
            abilities.Add(new Ability("Arrow Barrage"));
            abilities.Add(new Ability("Nature's Charm"));
            abilities.Add(new Ability("Survival Kit"));
            stats = new Stats(0, 4, 1, 2, 0, 2);
            class_armor = "Simple Leather Armor";
            class_weapon = "Simple Bow";
            break;
            case "sorcerer":
            abilities.Add(new Ability("Infused Strikes"));
            abilities.Add(new Ability("Arcane Bolt"));
            abilities.Add(new Ability("Thunder Strike"));
            abilities.Add(new Ability("Fireball"));
            abilities.Add(new Ability("Elemental Conduit"));
            abilities.Add(new Ability("Energy Transfusion"));
            stats = new Stats(0, 0, 5, 0, 0, 4);
            class_armor = "Simple Cloth Robes";
            class_weapon = "Simple Magic Staff";
            break;
            case "necromancer":
            abilities.Add(new Ability("Siphon Life"));
            abilities.Add(new Ability("Necrotic Blast"));
            abilities.Add(new Ability("Raise Undead"));
            abilities.Add(new Ability("Blood Sacrifice"));
            abilities.Add(new Ability("Eternal Servitude"));
            abilities.Add(new Ability("Mass Resurrection"));
            stats = new Stats(1, 0, 4, 0, 0, 4);
            class_armor = "Simple Cloth Robes";
            class_weapon = "Simple Wand & Shield";
            break;
            default:
            abilities.Add(new Ability("Cultivate"));
            abilities.Add(new Ability("Seek Revenge"));
            stats = new Stats(0, 0, 0, 0, 0, 0);
            class_armor = "None";
            class_weapon = "None";
            break;
        }
    }
}