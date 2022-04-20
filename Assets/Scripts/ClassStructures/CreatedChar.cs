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
        return this.gold;
    }

    public void setGold(int gold)
    {
        this.gold = gold;
    }

    public int getLevel()
    {
        return this.level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public int getXp()
    {
        return this.xp;
    }

    public void setXp(int xp)
    {
        this.xp = xp;
    }

    public int getStr()
    {
        return this.str;
    }

    public void setStr(int str)
    {
        this.str = str;
    }

    public int getDex()
    {
        return this.dex;
    }

    public void setDex(int dex)
    {
        this.dex = dex;
    }

    public int getInte()
    {
        return this.inte;
    }

    public void setInte(int inte)
    {
        this.inte = inte;
    }

    public int getSpd()
    {
        return this.spd;
    }

    public void setSpd(int spd)
    {
        this.spd = spd;
    }

    public int getCon()
    {
        return this.con;
    }

    public void setCon(int con)
    {
        this.con = con;
    }

    public int getEng()
    {
        return this.eng;
    }

    public void setEng(int eng)
    {
        this.eng = eng;
    }

    public int getHealth()
    {
        return this.health;
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
        return this.armor;
    }

    public void setArmor(string armor)
    {
        this.armor = armor;
    }

    public string getWeapon()
    {
        return this.weapon;
    }

    public void setWeapon(string weapon)
    {
        this.weapon = weapon;
    }

    public string getRing1()
    {
        return this.ring1;
    }

    public void setRing1(string ring1)
    {
        this.ring1 = ring1;
    }

    public string getRing2()
    {
        return this.ring2;
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