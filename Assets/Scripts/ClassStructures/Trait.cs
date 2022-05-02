using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Trait {
    private Stats stats;
    private string name;
    private string desc;

    private struct default_trait {
        public Stats stats;
        public string desc;
        public default_trait(string desc, Stats stats) {
            this.stats = stats;
            this.desc = desc;
        }
    }
    private Dictionary<string, default_trait> default_trait_desc = new Dictionary<string, default_trait>() {
        {"Fleet-Footed", new default_trait("Gain +2 Speed (SPD).", new Stats(0,0,0,2,0,0))},
        {"Healthy", new default_trait("Gain +2 Constitution (CON).", new Stats(0,0,0,0,2,0))},
        {"Mystical", new default_trait("Gain +2 Energy (ENG).", new Stats(0,0,0,0,0,2))},
        {"Berserk", new default_trait("Whenever you take damage in combat, increase your Damage by +2 for this combat only.", new Stats())},
        {"Generalist", new default_trait("Add +1 when making Stat Rolls.", new Stats())},
        {"Highborn", new default_trait("Start the game with 100 gold instead of 10, and gain 20 Gold whenever you level up", new Stats())},
        {"Holy", new default_trait("Once per combat, recover Health equal to 2x your Level.", new Stats())},
        {"Looter", new default_trait("Take an extra loot card when defeating a Monster.", new Stats())},
        {"Musclehead", new default_trait("Gain +3 in STR, then lose -1 from DEX & INT.", new Stats(3,-1,-1,0,0,0))},
        {"Bookworm", new default_trait("Gain +3 in INT, then lose -1 from DEX & STR.", new Stats(-1,-1,3,0,0,0))},
        {"Delicate", new default_trait("Gain +3 in DEX, then lose -1 from STR & INT.", new Stats(-1,3,-1,0,0,0))}
    };

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

    public string getDesc()
    {
        return this.desc;
    }

    public void setDesc(string desc)
    {
        this.desc = desc;
    }

    public Trait(string name, string desc, Stats stats) {
        this.name = name;
        this.desc = desc;
        this.stats = stats;
    }

    public Trait(string name, string desc) {
        this.name = name;
        this.desc = desc;
        this.stats = new Stats();
    }

    public Trait(string name) {
        this.name = name;
        if(default_trait_desc.ContainsKey(name)) {
            if (name == "Musclehead" || name == "Bookworm" || name == "Delicate")
                this.name = "Powerful";
            this.desc = default_trait_desc[name].desc;
            this.stats = default_trait_desc[name].stats;
        }
        else {
            this.desc = "Unknown";
            this.stats = new Stats();
        }
    }

}