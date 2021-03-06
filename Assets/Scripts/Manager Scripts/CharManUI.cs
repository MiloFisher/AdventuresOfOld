using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class CharManUI : MonoBehaviour
{
    // Setting reference for animation manager
    public Anim_Charcreation AnimManager;

    public TMP_FontAsset font;
    public GameObject MenuManager;
    public GameObject SceneHeader;
    public GameObject stat_container;
    public GameObject race_selected_text;
    public GameObject race_details_text;
    public GameObject race_trait_text;
    public GameObject class_str_text;
    public GameObject class_dex_text;
    public GameObject class_int_text;
    public GameObject class_spd_text;
    public GameObject class_con_text;
    public GameObject class_eng_text;
    public GameObject chosen_race_image;
    public GameObject chosen_class_image;
    public GameObject final_race_image;
    public GameObject final_class_image;
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
    public GameObject confirm_Class_Image;
    public GameObject ability_desc_text;
    public GameObject ability_cost_text;
    public GameObject ability_type_text;
    public GameObject ability_level_text;
    public GameObject ability_desc_panel;
    public GameObject trait_details_container;
    public GameObject trait_details_text;
    public GameObject trait_scroll;
    public GameObject fleet_footed_text;
    public GameObject healthy_text;
    public GameObject mystical_text;
    public GameObject berserk_text;
    public GameObject generalist_text;
    public GameObject highborn_text;
    public GameObject holy_text;
    public GameObject powerful_text;
    public GameObject class_confirm_button;
    public GameObject trait_confirm_button;
    public TMP_InputField inputname;
    public GameObject error_text;
    public GameObject trait_container;
    public bool gamestart = false;


    private CreatedChar createdchar = new CreatedChar();

    public void RaceButtonBuilder(Race race, Transform objectToSetTo) {
        GameObject racebutton = new GameObject(race.get_name(), typeof(RectTransform));
        racebutton.AddComponent<TextMeshProUGUI>();
        racebutton.GetComponent<TextMeshProUGUI>().text = race.get_name();
        racebutton.GetComponent<TextMeshProUGUI>().font = font;
        racebutton.GetComponent<TextMeshProUGUI>().fontSize = 72;
        racebutton.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.SmallCaps;
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
        // racebutton.GetComponent<Button>().onClick.AddListener(() => MenuManager.GetComponent<MenuManager>().SwapScene(1));
        racebutton.GetComponent<Button>().onClick.AddListener(() => SetRaceDetails(race));
        racebutton.GetComponent<Button>().onClick.AddListener(() => AnimManager.PlayAnim(0));
        racebutton.GetComponent<Button>().onClick.AddListener(() => JLAudioManager.Instance.PlaySound("OnClick"));
    }

    
    public void SetRaceDetails(Race race) {
        //Updating text for next confirmation scene
        createdchar.setChosen_race(race);
        chosen_race_image.SetActive(true);

        //Using a scuffed switch case since I don't want to mess with game_resource files
        //Resource.load is what I would have used here so that I wouldn't have to deal with names
        switch (race.get_name()) {
            case "Aasimar":
            chosen_race_image.GetComponent<Image>().sprite = aasimar_image;
            final_race_image.GetComponent<Image>().sprite = aasimar_image;
            createdchar.setImage("portrait_aasimar");
            break;
            case "Centaur":
            chosen_race_image.GetComponent<Image>().sprite = centaur_image;
            final_race_image.GetComponent<Image>().sprite = centaur_image;
            createdchar.setImage("portrait_centaur");
            break;
            case "Dwarf":
            chosen_race_image.GetComponent<Image>().sprite = dwarf_image;
            final_race_image.GetComponent<Image>().sprite = dwarf_image;
            createdchar.setImage("portrait_dwarf");
            break;
            case "High Elf":
            chosen_race_image.GetComponent<Image>().sprite = high_elf_image;
            final_race_image.GetComponent<Image>().sprite = high_elf_image;
            createdchar.setImage("portrait_high_elf");
            break;
            case "Night Elf":
            chosen_race_image.GetComponent<Image>().sprite = night_elf_image;
            final_race_image.GetComponent<Image>().sprite = night_elf_image;
            createdchar.setImage("portrait_night_elf");
            break;
            case "Leonin":
            chosen_race_image.GetComponent<Image>().sprite = leonin_image;
            final_race_image.GetComponent<Image>().sprite = leonin_image;
            createdchar.setImage("portrait_leonin");
            break;
            default:
            chosen_race_image.GetComponent<Image>().sprite = human_image;
            final_race_image.GetComponent<Image>().sprite = human_image;
            createdchar.setImage("portrait_human");
            break;
        }

        race_selected_text.GetComponent<TextMeshProUGUI>().text = race.get_name();
        race_details_text.GetComponent<TextMeshProUGUI>().text = race.get_desc();
        stat_container.SetActive(true);
        class_str_text.GetComponent<TextMeshProUGUI>().text = "STR:" + race.get_stats().get_str();
        class_dex_text.GetComponent<TextMeshProUGUI>().text = "DEX:" + race.get_stats().get_dex();
        class_spd_text.GetComponent<TextMeshProUGUI>().text = "SPD:" + race.get_stats().get_spd();
        class_int_text.GetComponent<TextMeshProUGUI>().text = "INT:" + race.get_stats().get_inte();
        class_con_text.GetComponent<TextMeshProUGUI>().text = "CON:" + race.get_stats().get_con();
        class_eng_text.GetComponent<TextMeshProUGUI>().text = "ENG:" + race.get_stats().get_eng();
        race_trait_text.GetComponent<TextMeshProUGUI>().text = race.get_unique_ability() + ":\n" + race.get_unique_ability_desc();
        
    }

    public void ConfirmRace() {
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText("Class Selection");
        stat_container.SetActive(true);
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + createdchar.getChosen_race().get_stats().get_str());
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + createdchar.getChosen_race().get_stats().get_dex());
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + createdchar.getChosen_race().get_stats().get_inte());
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + createdchar.getChosen_race().get_stats().get_spd());
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + createdchar.getChosen_race().get_stats().get_con());
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + createdchar.getChosen_race().get_stats().get_eng());
        chosen_class_image.SetActive(false);
    }

    public void BackToRace() {
        //stat_container.SetActive(false);
        chosen_race_image.SetActive(true);
        chosen_class_image.SetActive(false);
        // Edited by Ethan
        // Changed color and stats back to pre-select class
        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR: " + createdchar.getChosen_race().get_stats().get_str());
        class_str_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX: " + createdchar.getChosen_race().get_stats().get_dex());
        class_dex_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT: " + createdchar.getChosen_race().get_stats().get_inte());
        class_int_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD: " + createdchar.getChosen_race().get_stats().get_spd());
        class_spd_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON: " + createdchar.getChosen_race().get_stats().get_con());
        class_con_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG: " + createdchar.getChosen_race().get_stats().get_eng());
        class_eng_text.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        // End edit
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText("Race Selection");
    }

    public void ConfirmClass() {
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getName() + " abilities");
        Color32 black = new Color32(0, 0, 0, 255);
        //Set the color back to black so it is no longer highlighted green from preview
        class_str_text.GetComponent<TextMeshProUGUI>().color = black;
        class_dex_text.GetComponent<TextMeshProUGUI>().color = black;
        class_int_text.GetComponent<TextMeshProUGUI>().color = black;
        class_spd_text.GetComponent<TextMeshProUGUI>().color = black;
        class_con_text.GetComponent<TextMeshProUGUI>().color = black;
        class_eng_text.GetComponent<TextMeshProUGUI>().color = black;
        
        ability1_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[0].getName());
        
        ability2_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[1].getName());

        ability3_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[2].getName());
       
        ability4_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[3].getName());

        ability5_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[4].getName());

        ability6_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[5].getName());

        //Updating skill slab background image
        //By copying the scuffed switch case - Ethan
        //confirm_Class_Image.GetComponent<Image>().sprite = aasimar_image;'
        confirm_Class_Image.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/Icon_" + createdchar.getChosen_class().getName());
        ability_desc_text.GetComponent<TextMeshProUGUI>().SetText("Ability Description");
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
        //ability_desc_panel.SetActive(false);
        //class_confirm_button.SetActive(false);
        //chosen_class_image.SetActive(false);
        ability_desc_text.GetComponent<TextMeshProUGUI>().SetText("");
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText("Class Selection");
        chosen_class_image.SetActive(false);
    }

    public void ConfirmTrait() {
        Color32 black = new Color32(0, 0, 0, 255);
        class_str_text.GetComponent<TextMeshProUGUI>().color = black;
        class_dex_text.GetComponent<TextMeshProUGUI>().color = black;
        class_int_text.GetComponent<TextMeshProUGUI>().color = black;
        class_spd_text.GetComponent<TextMeshProUGUI>().color = black;
        class_con_text.GetComponent<TextMeshProUGUI>().color = black;
        class_eng_text.GetComponent<TextMeshProUGUI>().color = black;
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_trait().getName() + " "
        + createdchar.getChosen_race().get_name() + " " + createdchar.getChosen_class().getName());
        final_class_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/Icon_" + createdchar.getChosen_class().getName());
    }
    public void BackToTrait() {
        BackToClass();
        Color32 black = new Color32(0, 0, 0, 255);
        class_str_text.GetComponent<TextMeshProUGUI>().color = black;
        class_dex_text.GetComponent<TextMeshProUGUI>().color = black;
        class_int_text.GetComponent<TextMeshProUGUI>().color = black;
        class_spd_text.GetComponent<TextMeshProUGUI>().color = black;
        class_con_text.GetComponent<TextMeshProUGUI>().color = black;
        class_eng_text.GetComponent<TextMeshProUGUI>().color = black;
        //trait_details_container.SetActive(false);
        //trait_confirm_button.SetActive(false);
        //stat_container.SetActive(true);
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText("View Abilities");
        ability_desc_text.GetComponent<TextMeshProUGUI>().SetText("Ability Description");
        error_text.SetActive(false);
    }

    public void ViewedTraits() {
        SceneHeader.GetComponent<TextMeshProUGUI>().SetText("Trait Selection");
        trait_container.GetComponent<RectTransform>().localPosition = new Vector2(0,-273);
    }

    public void PreviewAbility(int abilitynum) {
        if (abilitynum == -1) {
            //ability_desc_panel.SetActive(false);
        }
        else {
            ability_desc_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_class().getAbilities()[abilitynum].getDesc());
            //Extra ability descriptions removed.
            /* ability_cost_text.GetComponent<TextMeshProUGUI>().SetText("Ability cost: " + createdchar.getChosen_class().getAbilities()[abilitynum].getCost());
            ability_type_text.GetComponent<TextMeshProUGUI>().SetText("Ability type: " + createdchar.getChosen_class().getAbilities()[abilitynum].getType());
            ability_level_text.GetComponent<TextMeshProUGUI>().SetText("Ability level: " + createdchar.getChosen_class().getAbilities()[abilitynum].getLevel()); */
            //ability_desc_panel.SetActive(true);
        }
    }

    //See changes to player stats before they confirm
    public void PreviewClass(string classname) {
        createdchar.setChosen_class(new Class(classname));
        class_confirm_button.SetActive(true);
        chosen_class_image.SetActive(true);
        chosen_class_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/Icon_" + createdchar.getChosen_class().getName());
        
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

    public void PreviewTrait(string traitname) {
        createdchar.setChosen_trait(new Trait(traitname));
        // trait_details_container.SetActive(true);
        // trait_confirm_button.SetActive(true);
        trait_details_text.GetComponent<TextMeshProUGUI>().SetText(createdchar.getChosen_trait().getDesc());
        Color32 green = new Color32(175, 255, 0, 255);
        Color32 black = new Color32(0, 0, 0, 255);
        Color32 red = new Color32(255, 0, 0, 255);

        class_str_text.GetComponent<TextMeshProUGUI>().SetText("STR:" + createdchar.getStr());
        if(createdchar.getChosen_trait().get_stats().strisgreen()) {
            class_str_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_str() < 0) {
            class_str_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_str_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        class_dex_text.GetComponent<TextMeshProUGUI>().SetText("DEX:" + createdchar.getDex());
        if(createdchar.getChosen_trait().get_stats().dexisgreen()) {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_dex() < 0) {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_dex_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        class_int_text.GetComponent<TextMeshProUGUI>().SetText("INT:" + createdchar.getInte());
        if(createdchar.getChosen_trait().get_stats().inteisgreen()) {
            class_int_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_inte() < 0) {
            class_int_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_int_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        class_spd_text.GetComponent<TextMeshProUGUI>().SetText("SPD:" + createdchar.getSpd());
        if(createdchar.getChosen_trait().get_stats().spdisgreen()) {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_spd() < 0) {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_spd_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        class_con_text.GetComponent<TextMeshProUGUI>().SetText("CON:" + createdchar.getCon());
        if(createdchar.getChosen_trait().get_stats().conisgreen()) {
            class_con_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_con() < 0) {
            class_con_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_con_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        class_eng_text.GetComponent<TextMeshProUGUI>().SetText("ENG:" + createdchar.getEng());
        if(createdchar.getChosen_trait().get_stats().engisgreen()) {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if(createdchar.getChosen_trait().get_stats().get_eng() < 0) {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = red;
        }
        else {
            class_eng_text.GetComponent<TextMeshProUGUI>().color = black;
        }

        if (traitname != "Musclehead" && traitname != "Bookworm" && traitname != "Delicate") {
            powerful_text.GetComponent<TextMeshProUGUI>().color = new Color32(0,0,0,255);
            powerful_text.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(3); //Anything not 0 1 2 will work here
        }

    }

    public void ViewPowerful() {
        //trait_details_container.SetActive(true);
        //trait_confirm_button.SetActive(false);
        trait_details_text.GetComponent<TextMeshProUGUI>().SetText(
            "Gain +3 in either STR, DEX, or INT, then lose -1 from the two Stats you didn???t choose.");
    }

    //Dropdown menu functionality. Manually sets the colors of the Powerful text (coz dropdown color was jank)
    //0,1,2 refers to the index position in the Unity Editor dropdown object
    public void PreviewPowerful(int type) {
        powerful_text.GetComponent<TextMeshProUGUI>().color = new Color32(59,255,0,255);
        //trait_confirm_button.SetActive(false);
        if(type == 0) {
            PreviewTrait("Musclehead");
        }
        else if(type == 1) {
            PreviewTrait("Bookworm");
        }
        else if(type == 2) {
            PreviewTrait("Delicate");
        }
        else {
            // do nothing dont need case to do for now
        }
        //Renable the confirm button and scroll after dropdown selection.
        // trait_confirm_button.SetActive(true);
    }

    public void setCharName() {
        string name = inputname.text;
        if (string.IsNullOrEmpty(name)) {
            error_text.SetActive(true);
        }
        else {
            createdchar.setName(inputname.text);
            error_text.SetActive(false);
            gamestart = true;
            //MenuManager.GetComponent<MenuManager>().SwapScene(6);
        }
    }

    public CreatedChar getChar() {
        return createdchar;
    }

    void Awake() {
        powerful_text.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(3);//Setting default value of dropdown menu to not selected
    }




}
