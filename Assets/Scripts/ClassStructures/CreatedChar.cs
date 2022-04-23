using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

//Class to store all information needed to be set
public class CreatedChar {

    private string image;
    private Race chosen_race;
    private Class chosen_class;
    private Trait chosen_trait;
    private int gold;
    private int level;
    private int xp;
    private int str;
    private int dex;
    private int inte;
    private int spd;
    private int con;
    private int eng;
    private int health;
    private int abilitycharges;
    private string armor;
    private string weapon;
    private string ring1;
    private string ring2;

    private string name;

    public string getName()
    {
        return this.name;
    }

    public void setName(string name)
    {
        this.name = name;
    }
    public string getImage()
    {
        return this.image;
    }

    public void setImage(string image)
    {
        this.image = image;
    }

    public Race getChosen_race()
    {
        return this.chosen_race;
    }

    public void setChosen_race(Race chosen_race)
    {
        this.chosen_race = chosen_race;
    }

    public Class getChosen_class()
    {
        return this.chosen_class;
    }

    public void setChosen_class(Class chosen_class)
    {
        this.chosen_class = chosen_class;
    }

    public Trait getChosen_trait()
    {
        return this.chosen_trait;
    }

    public void setChosen_trait(Trait chosen_trait)
    {
        this.chosen_trait = chosen_trait;
    }

    public int getGold()
    {
        return 10;
    }

    public void setGold(int gold)
    {
        this.gold = gold;
    }

    public int getLevel()
    {
        return 1;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public int getXp()
    {
        return 0;
    }

    public void setXp(int xp)
    {
        this.xp = xp;
    }

    public int getStr()
    {
        return chosen_race.get_stats().get_str()
                + chosen_class.get_stats().get_str()
                + chosen_trait.get_stats().get_str();
    }

    public void setStr(int str)
    {
        this.str = str;
    }

    public int getDex()
    {
        return chosen_race.get_stats().get_dex()
                + chosen_class.get_stats().get_dex()
                + chosen_trait.get_stats().get_dex();
    }

    public void setDex(int dex)
    {
        this.dex = dex;
    }

    public int getInte()
    {
        return chosen_race.get_stats().get_inte()
                + chosen_class.get_stats().get_inte()
                + chosen_trait.get_stats().get_inte();
    }

    public void setInte(int inte)
    {
        this.inte = inte;
    }

    public int getSpd()
    {
        return chosen_race.get_stats().get_spd()
                + chosen_class.get_stats().get_spd()
                + chosen_trait.get_stats().get_spd();
    }

    public void setSpd(int spd)
    {
        this.spd = spd;
    }

    public int getCon()
    {
        return chosen_race.get_stats().get_con()
                + chosen_class.get_stats().get_con()
                + chosen_trait.get_stats().get_con();
    }

    public void setCon(int con)
    {
        this.con = con;
    }

    public int getEng()
    {
        return chosen_race.get_stats().get_eng()
                + chosen_class.get_stats().get_eng()
                + chosen_trait.get_stats().get_eng();
    }

    public void setEng(int eng)
    {
        this.eng = eng;
    }

    public int getHealth()
    {
        return getCon() * 2;
    }

    public void setHealth(int health)
    {
        this.health = health;
    }

    public int getAbilitycharges()
    {
        return this.abilitycharges;
    }

    public void setAbilitycharges(int abilitycharges)
    {
        this.abilitycharges = abilitycharges;
    }

    public string getArmor()
    {
        return getChosen_class().getClass_armor();
    }

    public void setArmor(string armor)
    {
        this.armor = armor;
    }

    public string getWeapon()
    {
        return getChosen_class().getClass_weapon();
    }

    public void setWeapon(string weapon)
    {
        this.weapon = weapon;
    }

    public string getRing1()
    {
        return "empty";
    }

    public void setRing1(string ring1)
    {
        this.ring1 = ring1;
    }

    public string getRing2()
    {
        return "empty";
    }

    public void setRing2(string ring2)
    {
        this.ring2 = ring2;
    }


    public CreatedChar() {
        gold = 10;
        level = 1;
        xp = 0;
    }


}