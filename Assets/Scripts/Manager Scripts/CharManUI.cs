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
    public GameObject chosen_class_image;
    public Sprite aasimar_image;
    public Sprite centaur_image;
    public Sprite dwarf_image;
    public Sprite high_elf_image;
    public Sprite leonin_image;
    public Sprite human_image;
    public Sprite night_elf_image;
    public GameObject ability1_text;
    public GameObject ability2_text;
    public GameObject ability3_text;
    public GameObject ability4_text;
    public GameObject ability5_text;
    public GameObject ability6_text;
    public GameObject ability_desc_text;
    public GameObject ability_cost_text;
    public GameObject ability_type_text;
    public GameObject ability_level_text;
    public GameObject ability_desc_panel;

    private CreatedChar createdchar = new CreatedChar();

    public void RaceButtonBuilder(Race race, Transform objectToSetTo) {
        GameObject racebutton = new GameObject(race.get_name(), typeof(RectTransform));
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
        chosen_class_image.SetActive(true);

        //Using a scuffed switch case since I don't want to mess with game_resource files
        //Resource.load is what I would have used here so that I wouldn't have to deal with names
        switch (race.get_name()) {
            case "Aasimar":
            chosen_class_image.GetComponent<Image>().sprite = aasimar_image;
            createdchar.setImage("portrait_aasimar");
            break;
            case "Centaur":
            chosen_class_image.GetComponent<Image>().sprite = centaur_image;
            createdchar.setImage("portrait_centaur");
            break;
            case "Dwarf":
            chosen_class_image.GetComponent<Image>().sprite = dwarf_image;
            createdchar.setImage("portrait_dwarf");
            break;
            case "High Elf":
            chosen_class_image.GetComponent<Image>().sprite = high_elf_image;
            createdchar.setImage("portrait_high_elf");
            break;
            case "Night Elf":
            chosen_class_image.GetComponent<Image>().sprite = night_elf_image;
            createdchar.setImage("portrait_night_elf");
            break;
            case "Leonin":
            chosen_class_image.GetComponent<Image>().sprite = leonin_image;
            createdchar.setImage("portrait_leonin");
            break;
            default:
            chosen_class_image.GetComponent<Image>().sprite = human_image;
            createdchar.setImage("portrait_human");
            break;
        }

        race_selected_text.GetComponent<TextMeshProUGUI>().text = race.get_name();
        race_details_text.GetComponent<TextMeshProUGUI>().text = race.get_desc();
        race_stats_text.GetComponent<TextMeshProUGUI>().text = race.get_stats().ToString();
        race_trait_text.GetComponent<TextMeshProUGUI>().text = race.get_unique_ability() + ":\n" + race.get_unique_ability_desc();
        
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
        chosen_class_image.SetActive(false);
    }

    public void ConfirmClass() {
        Color32 black = new Color32(0, 0, 0, 255);
        //Set the color back to black so it is no longer highlighted green from preview
        class_str_text.GetComponent<TextMeshProUGUI>().color = black;
        class_dex_text.GetComponent<TextMeshProUGUI>().color = black;
        class_int_text.GetComponent<TextMeshProUGUI>().color = black;
        class_spd_text.GetComponent<TextMeshProUGUI>().color = black;
        class_con_text.GetComponent<TextMeshProUGUI>().color = black;
        class_eng_text.GetComponent<TextMeshProUGUI>().color = black;
        
        chosen_class_text.GetComponent<TextMeshProUGUI>().text = createdchar.getChosen_class().getName();
        ability1_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[0].getName());
        
        ability2_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[1].getName());

        ability3_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[2].getName());
       
        ability4_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[3].getName());

        ability5_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[4].getName());

        ability6_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[5].getName());
    }

    public void BackToClass() {
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + 
            createdchar.getChosen_race().get_stats().get_str());
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + 
            createdchar.getChosen_race().get_stats().get_dex());
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + 
            createdchar.getChosen_race().get_stats().get_inte());
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + 
            createdchar.getChosen_race().get_stats().get_spd());
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + 
            createdchar.getChosen_race().get_stats().get_con());
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + 
            createdchar.getChosen_race().get_stats().get_eng());
        ability_desc_panel.SetActive(false);
    }

    public void PreviewAbility(int abilitynum) {
        ability_desc_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[abilitynum].getDesc());
        ability_cost_text.GetComponent<TextMeshProUGUI>().SetText("Ability cost: " + createdchar.getChosen_class().getAbilities()[abilitynum].getCost());
        ability_type_text.GetComponent<TextMeshProUGUI>().SetText("Ability type: " + createdchar.getChosen_class().getAbilities()[abilitynum].getType());
        ability_level_text.GetComponent<TextMeshProUGUI>().SetText("Ability level: " + createdchar.getChosen_class().getAbilities()[abilitynum].getLevel());
        ability_desc_panel.SetActive(true);
    }

    //See changes to player stats before they confirm
    public void PreviewStat(string classname) {
        createdchar.setChosen_class(new Class(classname));
        
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + (createdchar.getChosen_race().get_stats().get_str() + 
        createdchar.getChosen_class().get_stats().get_str()));
        if(createdchar.getChosen_class().get_stats().strisgreen()) {
            class_str_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_str_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + (createdchar.getChosen_race().get_stats().get_dex() + 
        createdchar.getChosen_class().get_stats().get_dex()));
        if(createdchar.getChosen_class().get_stats().dexisgreen()) {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + (createdchar.getChosen_race().get_stats().get_inte() + 
        createdchar.getChosen_class().get_stats().get_inte()));
        if(createdchar.getChosen_class().get_stats().inteisgreen()) {
            class_int_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_int_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + (createdchar.getChosen_race().get_stats().get_spd() + 
        createdchar.getChosen_class().get_stats().get_spd()));
        if(createdchar.getChosen_class().get_stats().spdisgreen()) {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + (createdchar.getChosen_race().get_stats().get_con() + 
        createdchar.getChosen_class().get_stats().get_con()));
        if(createdchar.getChosen_class().get_stats().conisgreen()) {
            class_con_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_con_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + (createdchar.getChosen_race().get_stats().get_eng() + 
        createdchar.getChosen_class().get_stats().get_eng()));
        if(createdchar.getChosen_class().get_stats().engisgreen()) {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = new Color32(175, 255, 0, 255);
        }
        else {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }
    }

    public CreatedChar getChar() {
        return createdchar;
    }

}
