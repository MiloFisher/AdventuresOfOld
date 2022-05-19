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
    public Color enabledColor;
    public Color disabledColor;
    public GameObject strengthIncrementButton;
    public GameObject strengthDecrementButton;
    public GameObject dexterityIncrementButton;
    public GameObject dexterityDecrementButton;
    public GameObject intelligenceIncrementButton;
    public GameObject intelligenceDecrementButton;
    public GameObject speedIncrementButton;
    public GameObject speedDecrementButton;
    public GameObject constitutionIncrementButton;
    public GameObject constitutionDecrementButton;
    public GameObject energyIncrementButton;
    public GameObject energyDecrementButton;
    public GameObject confirmPointsButton;

    private int temporaryStrength;
    private int temporaryDexterity;
    private int temporaryIntelligence;
    private int temporarySpeed;
    private int temporaryConstitution;
    private int temporaryEnergy;

    public void UpdateCharacterSheet()
    {
        Player p = PlayManager.Instance.selectedPlayer;
        if (!p)
            return;

        portrait.sprite = PlayManager.Instance.portaitDictionary[p.Image.Value];
        characterName.text = "<color=" + PlayManager.Instance.GetPlayerColorString(p) + ">" + p.Name.Value + "</color>";
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

        string plus;
        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporaryStrength == 0)
        {
            strength.text = PlayManager.Instance.GetStrength(p) + "";
            strengthMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p));
        }
        else
        {
            strength.text = "<color=green>" + (PlayManager.Instance.GetStrength(p) + temporaryStrength) + "</color>";
            if(PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p) + temporaryStrength) > PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p)))
                strengthMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p) + temporaryStrength) + "</color>";
            else
                strengthMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(p) + temporaryStrength);
        }

        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporaryDexterity == 0)
        {
            dexterity.text = PlayManager.Instance.GetDexterity(p) + "";
            dexterityMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p));
        }
        else
        {
            dexterity.text = "<color=green>" + (PlayManager.Instance.GetDexterity(p) + temporaryDexterity) + "</color>";
            if (PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p) + temporaryDexterity) > PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p)))
                dexterityMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p) + temporaryDexterity) + "</color>";
            else
                dexterityMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetDexterity(p) + temporaryDexterity);
        }

        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporaryIntelligence == 0)
        {
            intelligence.text = PlayManager.Instance.GetIntelligence(p) + "";
            intelligenceMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p));
        }
        else
        {
            intelligence.text = "<color=green>" + (PlayManager.Instance.GetIntelligence(p) + temporaryIntelligence) + "</color>";
            if (PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p) + temporaryIntelligence) > PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p)))
                intelligenceMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p) + temporaryIntelligence) + "</color>";
            else
                intelligenceMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetIntelligence(p) + temporaryIntelligence);
        }

        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporarySpeed == 0)
        {
            speed.text = PlayManager.Instance.GetSpeed(p) + "";
            speedMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p));
        }
        else
        {
            speed.text = "<color=green>" + (PlayManager.Instance.GetSpeed(p) + temporarySpeed) + "</color>";
            if (PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p) + temporarySpeed) > PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p)))
                speedMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p) + temporarySpeed) + "</color>";
            else
                speedMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetSpeed(p) + temporarySpeed);
        }

        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporaryConstitution == 0)
        {
            constitution.text = PlayManager.Instance.GetConstitution(p) + "";
            constitutionMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p));
        }
        else
        {
            constitution.text = "<color=green>" + (PlayManager.Instance.GetConstitution(p) + temporaryConstitution) + "</color>";
            if (PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p) + temporaryConstitution) > PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p)))
                constitutionMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p) + temporaryConstitution) + "</color>";
            else
                constitutionMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetConstitution(p) + temporaryConstitution);
        }

        if (PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p)) >= 0)
            plus = "+";
        else
            plus = "";
        if (temporaryEnergy == 0)
        {
            energy.text = PlayManager.Instance.GetEnergy(p) + "";
            energyMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p));
        }
        else
        {
            energy.text = "<color=green>" + (PlayManager.Instance.GetEnergy(p) + temporaryEnergy) + "</color>";
            if (PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p) + temporaryEnergy) > PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p)))
                energyMod.text = "<color=green>" + plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p) + temporaryEnergy) + "</color>";
            else
                energyMod.text = plus + PlayManager.Instance.GetMod(PlayManager.Instance.GetEnergy(p) + temporaryEnergy);
        }

        if(PlayManager.Instance.GetLevelUpPoints(p) > 0 && p.UUID.Value == PlayManager.Instance.localPlayer.UUID.Value)
        {
            strengthIncrementButton.SetActive(true);
            strengthDecrementButton.SetActive(true);
            dexterityIncrementButton.SetActive(true);
            dexterityDecrementButton.SetActive(true);
            intelligenceIncrementButton.SetActive(true);
            intelligenceDecrementButton.SetActive(true);
            speedIncrementButton.SetActive(true);
            speedDecrementButton.SetActive(true);
            constitutionIncrementButton.SetActive(true);
            constitutionDecrementButton.SetActive(true);
            energyIncrementButton.SetActive(true);
            energyDecrementButton.SetActive(true);
            confirmPointsButton.SetActive(temporaryStrength + temporaryDexterity + temporaryIntelligence + temporarySpeed + temporaryConstitution + temporaryEnergy > 0);

            if (temporaryStrength + temporaryDexterity + temporaryIntelligence + temporarySpeed + temporaryConstitution + temporaryEnergy == PlayManager.Instance.GetLevelUpPoints(p))
            {
                strengthIncrementButton.GetComponent<Button>().enabled = false;
                strengthIncrementButton.GetComponent<Image>().color = disabledColor;
                dexterityIncrementButton.GetComponent<Button>().enabled = false;
                dexterityIncrementButton.GetComponent<Image>().color = disabledColor;
                intelligenceIncrementButton.GetComponent<Button>().enabled = false;
                intelligenceIncrementButton.GetComponent<Image>().color = disabledColor;
                speedIncrementButton.GetComponent<Button>().enabled = false;
                speedIncrementButton.GetComponent<Image>().color = disabledColor;
                constitutionIncrementButton.GetComponent<Button>().enabled = false;
                constitutionIncrementButton.GetComponent<Image>().color = disabledColor;
                energyIncrementButton.GetComponent<Button>().enabled = false;
                energyIncrementButton.GetComponent<Image>().color = disabledColor;
            }
            else
            {
                strengthIncrementButton.GetComponent<Button>().enabled = true;
                strengthIncrementButton.GetComponent<Image>().color = enabledColor;
                dexterityIncrementButton.GetComponent<Button>().enabled = true;
                dexterityIncrementButton.GetComponent<Image>().color = enabledColor;
                intelligenceIncrementButton.GetComponent<Button>().enabled = true;
                intelligenceIncrementButton.GetComponent<Image>().color = enabledColor;
                speedIncrementButton.GetComponent<Button>().enabled = true;
                speedIncrementButton.GetComponent<Image>().color = enabledColor;
                constitutionIncrementButton.GetComponent<Button>().enabled = true;
                constitutionIncrementButton.GetComponent<Image>().color = enabledColor;
                energyIncrementButton.GetComponent<Button>().enabled = true;
                energyIncrementButton.GetComponent<Image>().color = enabledColor;
            }
            strengthDecrementButton.GetComponent<Button>().enabled = temporaryStrength > 0;
            strengthDecrementButton.GetComponent<Image>().color = temporaryStrength > 0 ? enabledColor : disabledColor;
            dexterityDecrementButton.GetComponent<Button>().enabled = temporaryDexterity > 0;
            dexterityDecrementButton.GetComponent<Image>().color = temporaryDexterity > 0 ? enabledColor : disabledColor;
            intelligenceDecrementButton.GetComponent<Button>().enabled = temporaryIntelligence > 0;
            intelligenceDecrementButton.GetComponent<Image>().color = temporaryIntelligence > 0 ? enabledColor : disabledColor;
            speedDecrementButton.GetComponent<Button>().enabled = temporarySpeed > 0;
            speedDecrementButton.GetComponent<Image>().color = temporarySpeed > 0 ? enabledColor : disabledColor;
            constitutionDecrementButton.GetComponent<Button>().enabled = temporaryConstitution > 0;
            constitutionDecrementButton.GetComponent<Image>().color = temporaryConstitution > 0 ? enabledColor : disabledColor;
            energyDecrementButton.GetComponent<Button>().enabled = temporaryEnergy > 0;
            energyDecrementButton.GetComponent<Image>().color = temporaryEnergy > 0 ? enabledColor : disabledColor;
        }
        else
        {
            strengthIncrementButton.SetActive(false);
            strengthDecrementButton.SetActive(false);
            dexterityIncrementButton.SetActive(false);
            dexterityDecrementButton.SetActive(false);
            intelligenceIncrementButton.SetActive(false);
            intelligenceDecrementButton.SetActive(false);
            speedIncrementButton.SetActive(false);
            speedDecrementButton.SetActive(false);
            constitutionIncrementButton.SetActive(false);
            constitutionDecrementButton.SetActive(false);
            energyIncrementButton.SetActive(false);
            energyDecrementButton.SetActive(false);
            confirmPointsButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (PlayManager.Instance.characterDisplayOpen)
            UpdateCharacterSheet();
    }

    public void IncrementStrength()
    {
        temporaryStrength++;
    }
    public void DecrementStrength()
    {
        temporaryStrength--;
    }
    public void IncrementDexterity()
    {
        temporaryDexterity++;
    }
    public void DecrementDexterity()
    {
        temporaryDexterity--;
    }
    public void IncrementIntelligence()
    {
        temporaryIntelligence++;
    }
    public void DecrementIntelligence()
    {
        temporaryIntelligence--;
    }
    public void IncrementSpeed()
    {
        temporarySpeed++;
    }
    public void DecrementSpeed()
    {
        temporarySpeed--;
    }
    public void IncrementConstitution()
    {
        temporaryConstitution++;
    }
    public void DecrementConstitution()
    {
        temporaryConstitution--;
    }
    public void IncrementEnergy()
    {
        temporaryEnergy++;
    }
    public void DecrementEnergy()
    {
        temporaryEnergy--;
    }

    public void ConfirmPoints()
    {
        PlayManager.Instance.localPlayer.UpdateStats(temporaryStrength, temporaryDexterity, temporaryIntelligence, temporarySpeed, temporaryConstitution, temporaryEnergy);

        temporaryStrength = 0;
        temporaryDexterity = 0;
        temporaryIntelligence = 0;
        temporarySpeed = 0;
        temporaryConstitution = 0;
        temporaryEnergy = 0;
    }
}
