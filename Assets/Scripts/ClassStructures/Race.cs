using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Race {
        private Stats stats;
        private string name;
        private string desc;
        private string unique_ability;
        private string unique_ability_desc;

        private const string HUMAN_DESC = 
        "Relative to other species, Humans have a very short lifespan, " +
        "however, despite this they have a reputation for being one of " +
        "the most adaptable species.  Humans have been known to produce " +
        "heroes of all kinds, including master magic users, legendary " +
        "fighters, and skilled adventurers.";
        private const string HIGH_ELF_DESC = 
        "Elves in general are known for their grace and mastery that " +
        "they develop over their extensive lifetimes. " +
        "High Elves are no exception, however, they are also known for " +
        "their magical aptitude due to the great energy and intelligence " +
        "they possess.  Some of the most iconic and powerful magical " +
        "casters of the current age have been of High Elf descent.";
        private const string NIGHT_ELF_DESC =
        "Night Elves came about long ago when a group of Ancient Elves made " +
        "a pact with the Midnight Forest.  This pact changed the Elves, altering " +
        "the skin to become shades of blues and purples, elongating their ears, " +
        "and growing their connection to nature.  Due to this pact, however, " + 
        "the other Elves cast out the newly formed Night Elves, forcing them " + 
        "to leave their homeland forever.  With no place to call home, Night Elves " + 
        "began living in solitude, still bitter over their ancestral banishment " + 
        "from the elven homeland.";
        private const string DWARF_DESC = 
        "Dwarves are known across the land for their short stature " +
        "and even shorter tempers.  Despite these features, Dwarves " + 
        "are renowned master craftsmen, miners, and warriors. " +  
        "What Dwarves lack in mobility, they make up for in sturdiness. " +  
        "The ancient Dwarven Sentinels were second to none in their " +
        "defensive skill, with some even considering it to be impenetrable.";
        private const string CENTAUR_DESC = 
        "A breed of ancient defenders of the Midnight Forest, Centaurs are half-man, " +
        "half-horse creatures that live well beyond the influence of the realms of mankind. " +
        "With incredible speed and dexterity to match, the Centaur Wardens of the forest " +
        "are a highly regarded, and highly feared order of rangers. " +  
        "While Centaurs historically have kept to themselves, as the spread of Chaos " +
        "encroaches on their home, Centaurs have begun allying themselves with other " +
        "races with hopes to curb the spread of the ever growing Chaos.";
        private const string LEONIN_DESC = 
        "The proud race of the Leonin have warred throughout the Sand Plains " +
        "for as long as history recalls.  Largely divided as a race, " +
        "the Leonin have formed various Dynasty Tribes that constantly " +
        "make war with each other over control of the Sand Plains. " + 
        "For many, war has become a way of life, and these Leonin seem " +
        "to lack knowledge of anything beyond combat prowess and their unwavering pride.";
        private const string AASIMAR_DESC = 
        "Aasimar are said to be descendants of the Angels, however, this may have been " +
        "a rumor started by Humans who viewed their pale skin and radiant eyes as otherworldly. " +
        "While the origin of the Aasimar is widely unknown, this uncommon race is speculated to " +
        "have a connection to holy magic.  While this may be a result of the Angel descendant views, " +
        "it is undeniable that there lies a power within the Aasimar bloodline.";

        //BEGINNING OF DEFAULT CATEGORIES
        //Default RACES
        private Dictionary<string, string> race_default_desc = 
            new Dictionary<string, string>(){
                                                {"human", HUMAN_DESC},
                                                {"high elf", HIGH_ELF_DESC},
                                                {"night elf", NIGHT_ELF_DESC},
                                                {"dwarf", DWARF_DESC},
                                                {"centaur", CENTAUR_DESC},
                                                {"leonin", LEONIN_DESC},
                                                {"aasimar", AASIMAR_DESC}
                                            };

        //Default RACE ABILITY
        private Dictionary<string, string> race_default_unique_ability =
            new Dictionary<string, string>(){
                                                {"human", "Adaptable"},
                                                {"high elf", "Elven Knowledge"},
                                                {"night elf", "Lone Wolf"},
                                                {"dwarf", "Dwarven Defense"},
                                                {"centaur", "Horseback Riding"},
                                                {"leonin", "Battle Roar"},
                                                {"aasimar", "Heaven's Paragon"}
                                            };

        private Dictionary<string, string> unique_ability_descriptions = 
            new Dictionary<string, string>(){
                                               {"Adaptable", "Gain +2 Attribute Points at the start of the game."},
                                               {"Elven Knowledge", "When making Stat Rolls, if your modifier is less than 0, ignore it when making the roll."},
                                               {"Lone Wolf", "When fighting alone, gain +1 Power for the duration of the combat."},
                                               {"Dwarven Defense", "You have +1 Armor."},
                                               {"Horseback Riding", "When moving, take an ally on your tile or on an adjacent tile with you to your destination.  (Limited to 1 ally per turn)."},
                                               {"Battle Roar", "After you get attacked in combat, gain +4 Damage and +2 Armor until the end of your next turn."},
                                               {"Heaven's Paragon", "You recover an extra +2 Health whenever you are healed.  While at full Health, you have +2 Damage."}
                                            };

        //END OF DEFAULT CATEGORIES
        public Race(string name, Stats stats, string desc, string unique_ability) {
            this.name = name;
            this.stats = stats;

            if(race_default_desc.ContainsKey(name.ToLower())) {
                this.desc = race_default_desc[name.ToLower()];
            }
            else {
                this.desc = desc;
            }

            if(race_default_unique_ability.ContainsKey(name.ToLower())) {
                this.unique_ability = race_default_unique_ability[name.ToLower()];
                this.unique_ability_desc = unique_ability_descriptions[race_default_unique_ability[name.ToLower()]];
            }
            else {
                this.unique_ability = unique_ability;
                this.unique_ability_desc = "Unknown";
            }

        }
        

        public Stats get_stats() {
            return stats;
        }
        public string get_name() {
            return name;
        }
        public string get_desc() {
            return desc;
        }
        public string get_unique_ability() {
            return unique_ability;
        }
        public string get_unique_ability_desc() {
            return unique_ability_desc;
        }
        
    }