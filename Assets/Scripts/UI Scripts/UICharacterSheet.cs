using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UICharacterSheet : MonoBehaviour
{
    public Image portrait;
    public TMP_Text characterName;
    public TMP_Text characterDescription;
    public GameObject healthBar;
    public TMP_Text healthBarText;
    public GameObject abilityChargesBar;
    public TMP_Text abilityChargesText;
    public GameObject levelBar;
    public TMP_Text levelBarText;
    public TMP_Text currentLevel;
    public TMP_Text nextLevel;
    public TMP_Text attack;
    public TMP_Text armor;
    public TMP_Text physicalPower;
    public TMP_Text magicalPower;
    public TMP_Text gold;
    public TMP_Text strength;
    public TMP_Text strengthMod;
    public TMP_Text dexterity;
    public TMP_Text dexterityMod;
    public TMP_Text intelligence;
    public TMP_Text intelligenceMod;
    public TMP_Text speed;
    public TMP_Text speedMod;
    public TMP_Text constitution;
    public TMP_Text constitutionMod;
    public TMP_Text energy;
    public TMP_Text energyMod;

    public void UpdateCharacterSheet()
    {
        Player p = PlayManager.Instance.selectedPlayer;
        if (!p)
            return;

        portrait.sprite = PlayManager.Instance.portaitDictionary[p.Image.Value];
        characterName.text = "<color=" + p.Color.Value + ">" + p.Name.Value + "</color>";
        characterDescription.text = p.Trait.Value + " " + p.Race.Value + " " + p.Class.Value;

        healthBarText.text = PlayManager.Instance.GetHealth(p) + " / " + PlayManager.Instance.GetMaxHealth(p);
        healthBar.transform.localPosition = new Vector3(4454f * PlayManager.Instance.GetHealth(p) / PlayManager.Instance.GetMaxHealth(p) - 4454f, 0, 0);
        abilityChargesText.text = PlayManager.Instance.GetAbilityCharges(p) + " / " + PlayManager.Instance.GetMaxAbilityCharges(p);
        abilityChargesBar.transform.localPosition = new Vector3(3329f * PlayManager.Instance.GetAbilityCharges(p) / PlayManager.Instance.GetMaxAbilityCharges(p) - 3329f, 0, 0);
        levelBarText.text = PlayManager.Instance.GetXP(p) + " / " + PlayManager.Instance.GetNeededXP(p);
        levelBar.transform.localPosition = new Vector3(2204f * PlayManager.Instance.GetXP(p) / PlayManager.Instance.GetNeededXP(p) - 2204f, 0, 0);
        currentLevel.text = PlayManager.Instance.GetLevel(p) + "";
        nextLevel.text = PlayManager.Instance.GetLevel(p) == 5 ? "MAX" : PlayManager.Instance.GetLevel(p) + 1 + "";

        attack.text = ": " + PlayManager.Instance.GetAttack(p);
        armor.text = ": " + PlayManager.Instance.GetArmor(p);
        physicalPower.text = ": " + PlayManager.Instance.GetPhysicalPower(p);
        magicalPower.text = ": " + PlayManager.Instance.GetMagicalPower(p);
        gold.text = ": " + PlayManager.Instance.GetGold(p);

        strength.text = PlayManager.Instance.GetStrength(p) + "";
        strengthMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p)) + "";
        dexterity.text = PlayManager.Instance.GetDexterity(p) + "";
        dexterityMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p)) + "";
        intelligence.text = PlayManager.Instance.GetIntelligence(p) + "";
        intelligenceMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p)) + "";
        speed.text = PlayManager.Instance.GetSpeed(p) + "";
        speedMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p)) + "";
        constitution.text = PlayManager.Instance.GetConstitution(p) + "";
        constitutionMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p)) + "";
        energy.text = PlayManager.Instance.GetEnergy(p) + "";
        energyMod.text = PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p)) + "";
    }

    private void Update()
    {
        if (PlayManager.Instance.characterDisplayOpen)
            UpdateCharacterSheet();
    }
}
