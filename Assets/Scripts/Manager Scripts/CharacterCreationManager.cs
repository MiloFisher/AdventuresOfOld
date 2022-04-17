using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class CharacterCreationManager : MonoBehaviour
{
    Player localPlayer;
    private GameObject canvas;
    public TMP_FontAsset font;
    public GameObject MenuManager;
    public GameObject race_selected_text;
    public GameObject race_details_text;
    public GameObject race_stats_text;
    public GameObject race_trait_text;
    public GameObject class_str_text;
    public GameObject class_dex_text;
    public GameObject class_int_text;
    public GameObject class_spd_text;
    public GameObject class_con_text;
    public GameObject class_eng_text;
    private Race player_race;

    //Beginning of class creations for orgaization
    //Stat class contains all categories of stats and a way to print them
    public class Stats {
        private int str = 0;
        private int dex = 0;
        private int inte = 0;
        private int spd = 0;
        private int con = 0;
        private int eng = 0;

        public Stats(int str, int dex, int inte, int spd, int con, int eng) {
            this.str = str;
            this.dex = dex;
            this.inte = inte;
            this.spd = spd;
            this.con = con;
            this.eng =  eng;
        }

        public int get_str() {return str;}
        public int get_dex() {return dex;}
        public int get_inte() {return inte;}
        public int get_spd() {return spd;}
        public int get_con() {return con;}
        public int get_eng() {return eng;}
        public override string ToString() {
            return ("Strength: " + str
                    + " Dexterity: " + dex
                    + " Intelligence: " + inte
                    + " Speed: " + spd
                    + " Constitution: " + con
                    + " Energy: " + eng);
        }
        public void set_str(int str) {this.str = str;}
        public void set_dex(int dex) {this.dex = dex;}
        public void set_inte(int inte) {this.inte = inte;}
        public void set_spd(int spd) {this.spd = spd;}
        public void set_con(int con) {this.con = con;}
        public void set_eng(int eng) {this.eng = eng;}
    }

    //Race class to store race data - Allows for additional class adding if ever needed 
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

    //END OF CLASS DEFINITIONS

    //Creating Race Class for each default race
    //order: str, dex, int, spd, con, eng
    List<Race> default_races = new List<Race>() {
        new Race("Human", new Stats(10,10,10,18,10,12), "", ""),
        new Race("High Elf", new Stats(7,11,12,18,8,14), "", ""),
        new Race("Night Elf", new Stats(9,12,9,18,10,12), "", ""),
        new Race("Dwarf", new Stats(12,10,8,16,12,12), "", ""),
        new Race("Centaur", new Stats(8,13,9,20,8,12), "", ""),
        new Race("Leonin", new Stats(12,12,6,18,10,12), "", ""),
        new Race("Aasimar", new Stats(11,8,11,18,8,14), "", ""),
        //xd
        new Race("Dragon Amongst Men", new Stats(99,99,99,99,99,99), "Do not refuse a toast just to drink a forfeit.", "Peerless")
    };

    void RaceButtonBuilder(Race race, Transform objectToSetTo) {
        GameObject racebutton = new GameObject(race.get_name(), typeof(RectTransform));
        //racebutton.AddComponent<RawImage>();
        racebutton.AddComponent<TextMeshProUGUI>();
        racebutton.GetComponent<TextMeshProUGUI>().text = race.get_name();
        racebutton.GetComponent<TextMeshProUGUI>().font = font;
        racebutton.GetComponent<TextMeshProUGUI>().fontSize = 72;
        racebutton.GetComponent<TextMeshProUGUI>().enableAutoSizing = true;
        racebutton.GetComponent<TextMeshProUGUI>().color = new Color32(56,56,56,255);
        racebutton.AddComponent<Button>();
        ColorBlock buttoncolors = new ColorBlock();
        buttoncolors.colorMultiplier = 5;
        buttoncolors.normalColor = new Color32(0,0,0,255);
        buttoncolors.highlightedColor = new Color32(255,0,0,255);
        buttoncolors.pressedColor = new Color32(255,253,255,255);
        buttoncolors.selectedColor = new Color32(59,255,0,255);
        buttoncolors.disabledColor = new Color32(200,200,200,128);
        racebutton.GetComponent<Button>().colors = buttoncolors;
        racebutton.transform.SetParent(objectToSetTo, false);
        racebutton.GetComponent<Button>().onClick.AddListener(() => MenuManager.GetComponent<MenuManager>().SwapScene(1));
        racebutton.GetComponent<Button>().onClick.AddListener(() => SetRaceDetails(race));
    }

    void SetRaceDetails(Race race) {
        //Updating text for next confirmation scene
        player_race = race;
        race_selected_text.GetComponent<TextMeshProUGUI>().text = race.get_name();
        race_details_text.GetComponent<TextMeshProUGUI>().text = race.get_desc();
        race_stats_text.GetComponent<TextMeshProUGUI>().text = race.get_stats().ToString();
        race_trait_text.GetComponent<TextMeshProUGUI>().text = race.get_unique_ability() + ":\n" + race.get_unique_ability_desc();
        
        //Store for local player
        localPlayer.SetValue("Race", race.get_name());
        localPlayer.SetValue("Trait", race.get_unique_ability());
        localPlayer.SetValue("Strength", race.get_stats().get_str());
        localPlayer.SetValue("Dexterity", race.get_stats().get_dex());
        localPlayer.SetValue("Intelligence", race.get_stats().get_inte());
        localPlayer.SetValue("Speed", race.get_stats().get_spd());
        localPlayer.SetValue("Constitution", race.get_stats().get_con());
        localPlayer.SetValue("Energy", race.get_stats().get_eng());
        localPlayer.SetValue("Health", localPlayer.Constitution.Value * 2);
    }

    void ClassButtonBuilder(Race race, Transform objectToSetTo) {
        GameObject racebutton = new GameObject(race.get_name(), typeof(RectTransform));
        //racebutton.AddComponent<RawImage>();
        racebutton.AddComponent<TextMeshProUGUI>();
        racebutton.GetComponent<TextMeshProUGUI>().text = race.get_name();
        racebutton.GetComponent<TextMeshProUGUI>().font = font;
        racebutton.GetComponent<TextMeshProUGUI>().fontSize = 72;
        racebutton.GetComponent<TextMeshProUGUI>().enableAutoSizing = true;
        racebutton.GetComponent<TextMeshProUGUI>().color = new Color32(56,56,56,255);
        racebutton.AddComponent<Button>();
        ColorBlock buttoncolors = new ColorBlock();
        buttoncolors.colorMultiplier = 5;
        buttoncolors.normalColor = new Color32(0,0,0,255);
        buttoncolors.highlightedColor = new Color32(255,0,0,255);
        buttoncolors.pressedColor = new Color32(255,253,255,255);
        buttoncolors.selectedColor = new Color32(59,255,0,255);
        buttoncolors.disabledColor = new Color32(200,200,200,128);
        racebutton.GetComponent<Button>().colors = buttoncolors;
        racebutton.transform.SetParent(objectToSetTo, false);
        racebutton.GetComponent<Button>().onClick.AddListener(() => MenuManager.GetComponent<MenuManager>().SwapScene(1));
        racebutton.GetComponent<Button>().onClick.AddListener(() => SetRaceDetails(race));
    }

    void SetClassDetails(Race race) {
        //Updating text for next confirmation scene
        race_selected_text.GetComponent<TextMeshProUGUI>().text = race.get_name();
        race_details_text.GetComponent<TextMeshProUGUI>().text = race.get_desc();
        race_stats_text.GetComponent<TextMeshProUGUI>().text = race.get_stats().ToString();
        race_trait_text.GetComponent<TextMeshProUGUI>().text = race.get_unique_ability() + ":\n" + race.get_unique_ability_desc();
        
        //Store for local player
        localPlayer.SetValue("Race", race.get_name());
        localPlayer.SetValue("Trait", race.get_unique_ability());
        localPlayer.SetValue("Strength", race.get_stats().get_str());
        localPlayer.SetValue("Dexterity", race.get_stats().get_dex());
        localPlayer.SetValue("Intelligence", race.get_stats().get_inte());
        localPlayer.SetValue("Speed", race.get_stats().get_spd());
        localPlayer.SetValue("Constitution", race.get_stats().get_con());
        localPlayer.SetValue("Energy", race.get_stats().get_eng());
        localPlayer.SetValue("Health", localPlayer.Constitution.Value * 2);
    }

    //Confirm button for race sets up the information for the gui in the next scene
    public void ConfirmRace() {
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + player_race.get_stats().get_str());
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + player_race.get_stats().get_dex());
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + player_race.get_stats().get_inte());
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + player_race.get_stats().get_spd());
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + player_race.get_stats().get_con());
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + player_race.get_stats().get_eng());
    }

    //See changes to player stats befpre they confirm
    public void PreviewStat(string classname) {
        int strtoadd = 0;
        int dextoadd = 0;
        int inttoadd = 0;
        int spdtoadd = 0;
        int contoadd = 0;
        int engtoadd = 0;

        switch(classname) {
            case "Warrior":
                strtoadd = 4;
                dextoadd = 1;
                inttoadd = 0;
                spdtoadd = 0;
                contoadd = 4;
                engtoadd = 0;
                break;
            case "Paladin":
                strtoadd = 4;
                dextoadd = 0;
                inttoadd = 2;
                spdtoadd = 0;
                contoadd = 3;
                engtoadd = 0;
                break;
            case "Rogue":
                strtoadd = 0;
                dextoadd = 4;
                inttoadd = 2;
                spdtoadd = 0;
                contoadd = 1;
                engtoadd = 2;
                break;
            case "Ranger":
                strtoadd = 0;
                dextoadd = 4;
                inttoadd = 1;
                spdtoadd = 2;
                contoadd = 0;
                engtoadd = 2;
                break;
            case "Sorcerer":
                strtoadd = 0;
                dextoadd = 0;
                inttoadd = 5;
                spdtoadd = 0;
                contoadd = 0;
                engtoadd = 4;
                break;
            case "Necromancer":
                strtoadd = 1;
                dextoadd = 0;
                inttoadd = 4;
                spdtoadd = 0;
                contoadd = 0;
                engtoadd = 4;
                break;
            default:
                strtoadd = 0;
                dextoadd = 0;
                inttoadd = 0;
                spdtoadd = 0;
                contoadd = 0;
                engtoadd = 0;
                break;
        }
        
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + (player_race.get_stats().get_str() + strtoadd));
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + (player_race.get_stats().get_dex() + dextoadd));
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + (player_race.get_stats().get_inte() + inttoadd));
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + (player_race.get_stats().get_spd() + spdtoadd));
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + (player_race.get_stats().get_con() + contoadd));
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + (player_race.get_stats().get_eng() + engtoadd));
    }

    // Start is called before the first frame update
    void Start()
    {   
        //Scrollable container to spawn the buttons in
        canvas = GameObject.Find("RaceTextContainer");

        foreach(Race race in default_races) {
            RaceButtonBuilder(race, canvas.transform);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Gets local player
        foreach(GameObject p in players)
        {
            if (p.GetComponent<Player>().IsLocalPlayer)
            {
                localPlayer = p.GetComponent<Player>();
            }
        }

        // Set Variable
        localPlayer.SetValue("Name", localPlayer.Username.Value + "");
        localPlayer.SetValue("Race", "Human");
        localPlayer.SetValue("Class", "Warrior");
        localPlayer.SetValue("Trait", "Highborn");
        localPlayer.SetValue("Gold", 10);
        localPlayer.SetValue("Level", 1);
        localPlayer.SetValue("XP", 0);
        localPlayer.SetValue("Strength", 15);
        localPlayer.SetValue("Dexterity", 15);
        localPlayer.SetValue("Intelligence", 15);
        localPlayer.SetValue("Speed", 20);
        localPlayer.SetValue("Constitution", 15);
        localPlayer.SetValue("Energy", 15);
        localPlayer.SetValue("Health", localPlayer.Constitution.Value * 2);
        localPlayer.SetValue("Image", "portrait_human");

        // If host, generate random characters for bots
        if (NetworkManager.Singleton.IsHost)
        {
            foreach (GameObject g in players)
            {
                Player p = g.GetComponent<Player>();
                if (p.isBot)
                {
                    p.SetValue("Name", p.Username.Value + "");
                    p.SetValue("Race", "Human");
                    p.SetValue("Class", "Warrior");
                    p.SetValue("Trait", "Highborn");
                    p.SetValue("Gold", 10);
                    p.SetValue("Level", 1);
                    p.SetValue("XP", 0);
                    p.SetValue("Strength", 10);
                    p.SetValue("Dexterity", 10);
                    p.SetValue("Intelligence", 10);
                    p.SetValue("Speed", 10);
                    p.SetValue("Constitution", 10);
                    p.SetValue("Energy", 10);
                    p.SetValue("Health", p.Constitution.Value);
                    p.SetValue("Image", "portrait_human");
                }
            }
        }

        // Leave scene
        if (NetworkManager.Singleton.IsServer)
        {   
            Debug.Log("Stay here for now");
            //localPlayer.ChangeScene("Core Game");
        }
    }
}
