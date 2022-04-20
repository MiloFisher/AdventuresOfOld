using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class CharManUI : MonoBehaviour
{
    public TMP_FontAsset font;
    public GameObject MenuManager;
    public GameObject stat_container;
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
    public GameObject chosen_class_text;
    private CreatedChar createdchar = new CreatedChar();

    public void RaceButtonBuilder(Race race, Transform objectToSetTo) {
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

    
    public void SetRaceDetails(Race race) {
        //Updating text for next confirmation scene
        createdchar.setChosen_race(race);
        race_selected_text.GetComponent<TextMeshProUGUI>().text = race.get_name();
        race_details_text.GetComponent<TextMeshProUGUI>().text = race.get_desc();
        race_stats_text.GetComponent<TextMeshProUGUI>().text = race.get_stats().ToString();
        race_trait_text.GetComponent<TextMeshProUGUI>().text = race.get_unique_ability() + ":\n" + race.get_unique_ability_desc();
        
        /* //Store for local player
        localPlayer.SetValue("Race", race.get_name());
        localPlayer.SetValue("Trait", race.get_unique_ability());
        localPlayer.SetValue("Strength", race.get_stats().get_str());
        localPlayer.SetValue("Dexterity", race.get_stats().get_dex());
        localPlayer.SetValue("Intelligence", race.get_stats().get_inte());
        localPlayer.SetValue("Speed", race.get_stats().get_spd());
        localPlayer.SetValue("Constitution", race.get_stats().get_con());
        localPlayer.SetValue("Energy", race.get_stats().get_eng());
        localPlayer.SetValue("Health", localPlayer.Constitution.Value * 2); */
    }

    public void ConfirmRace() {
        stat_container.SetActive(true);
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + createdchar.getChosen_race().get_stats().get_str());
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + createdchar.getChosen_race().get_stats().get_dex());
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + createdchar.getChosen_race().get_stats().get_inte());
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + createdchar.getChosen_race().get_stats().get_spd());
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + createdchar.getChosen_race().get_stats().get_con());
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + createdchar.getChosen_race().get_stats().get_eng());
    }

    public void BackToRace() {
        stat_container.SetActive(false);
    }

    //See changes to player stats befpre they confirm
    public void PreviewStat(string classname) {
        createdchar.setChosen_class(new Class(classname));
        chosen_class_text.GetComponent<TextMeshProUGUI>().text = classname;
        //localPlayer.SetValue("Class", classname);
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
        
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + (createdchar.getChosen_race().get_stats().get_str() + strtoadd));
        //localPlayer.SetValue("Strength", player_race.get_stats().get_str() + strtoadd);
        if(strtoadd > 0) {
            class_str_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_str_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + (createdchar.getChosen_race().get_stats().get_dex() + dextoadd));
        //localPlayer.SetValue("Dexterity", player_race.get_stats().get_dex() + dextoadd);
        if(dextoadd > 0) {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + (createdchar.getChosen_race().get_stats().get_inte() + inttoadd));
        //localPlayer.SetValue("Intellegence", player_race.get_stats().get_inte() + inttoadd);
        if(inttoadd > 0) {
            class_int_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_int_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + (createdchar.getChosen_race().get_stats().get_spd() + spdtoadd));
        //localPlayer.SetValue("Speed", player_race.get_stats().get_spd() + spdtoadd);
        if(spdtoadd > 0) {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + (createdchar.getChosen_race().get_stats().get_con() + contoadd));
        //localPlayer.SetValue("Constitution", player_race.get_stats().get_con() + contoadd);
        if(contoadd > 0) {
            class_con_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_con_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + (createdchar.getChosen_race().get_stats().get_eng() + engtoadd));
        //localPlayer.SetValue("Energy", player_race.get_stats().get_eng() + engtoadd);
        if(engtoadd > 0) {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }
    }

}
