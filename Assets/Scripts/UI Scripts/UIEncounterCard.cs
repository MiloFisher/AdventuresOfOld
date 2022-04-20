using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIEncounterCard : MonoBehaviour
{
    [Header("Generic Components")]
    public string cardName;
    public GameObject cardButton;
    public GameObject cardBack;
    public Sprite[] monsterCardFaces;
    public float fadeLength = 0.004f;

    [Header("Event Card Components")]
    public GameObject eventComponents;
    public Image eventImage;
    public TMP_Text eventCardName;
    public TMP_Text eventFlavorText;
    public TMP_Text eventOptions;
    public TMP_Text eventXP;
    public GameObject eventOptionButton;
    public List<GameObject> eventOptionButtons = new List<GameObject>();

    [Header("Monster Card Components")]
    public GameObject monsterComponents;
    public Image monsterImage;
    public Image monsterCardFace;
    public TMP_Text monsterCardName;
    public TMP_Text monsterHealth;
    public TMP_Text monsterAttack;
    public TMP_Text monsterSpeed;
    public TMP_Text monsterPhysicalPower;
    public TMP_Text monsterMagicalPower;
    public TMP_Text monsterFlavorText;
    public TMP_Text monsterSkillName;
    public TMP_Text monsterSkillDescription;
    public TMP_Text monsterPassiveName;
    public TMP_Text monsterPassiveDescription;
    public TMP_Text monsterGold;
    public TMP_Text monsterXP;
    public GameObject monsterFightButton;

    private bool actionButtonActive;

    public void ClickCard(int id)
    {
        
    }

    public void FightMonster()
    {
        if (!actionButtonActive)
            return;

        EncounterManager.Instance.DisableCardButtons();

        MonsterCard m = PlayManager.Instance.encounterReference[cardName] as MonsterCard;

        ActivateOptionCardButton(false);

        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void ChooseOption()
    {
        if (!actionButtonActive)
            return;

        EncounterManager.Instance.DisableCardButtons();
        ActivateOptionCardButton(false);
    }

    public void SetVisuals(string card)
    {
        cardName = card;
        EncounterCard encounter = PlayManager.Instance.encounterReference[card];
        if (encounter.GetType() == typeof(MonsterCard))
        {
            MonsterCard m = encounter as MonsterCard;
            eventComponents.SetActive(false);
            monsterComponents.SetActive(true);
            monsterImage.sprite = m.image;
            monsterCardName.text = m.cardName;
            monsterHealth.text = m.health + "";
            monsterAttack.text = m.attack + "";
            monsterSpeed.text = m.speed + "";
            monsterPhysicalPower.text = m.physicalPower + "";
            monsterMagicalPower.text = m.magicalPower + "";
            monsterFlavorText.text = m.flavorText;
            monsterSkillName.text = m.skillName;
            monsterSkillDescription.text = m.skillDescription;
            monsterPassiveName.text = m.passiveName;
            monsterPassiveDescription.text = m.passiveDescription;
            switch (m.type)
            {
                case MonsterType.BASIC:
                    monsterCardFace.sprite =  monsterCardFaces[0];
                    monsterGold.gameObject.SetActive(true);
                    monsterXP.gameObject.SetActive(true);
                    monsterGold.text = "10";
                    monsterXP.text = "3";
                    break;
                case MonsterType.ELITE:
                    monsterCardFace.sprite = monsterCardFaces[1];
                    monsterGold.gameObject.SetActive(true);
                    monsterXP.gameObject.SetActive(true);
                    monsterGold.text = "20";
                    monsterXP.text = "6";
                    break;
                case MonsterType.MINIBOSS:
                    monsterCardFace.sprite = monsterCardFaces[2];
                    monsterGold.gameObject.SetActive(true);
                    monsterXP.gameObject.SetActive(true);
                    monsterGold.text = "20";
                    monsterXP.text = "6";
                    break;
                case MonsterType.BOSS:
                    monsterCardFace.sprite = monsterCardFaces[3];
                    monsterGold.gameObject.SetActive(false);
                    monsterXP.gameObject.SetActive(false);
                    break;
                case MonsterType.MINION:
                    monsterCardFace.sprite = monsterCardFaces[4];
                    monsterGold.gameObject.SetActive(false);
                    monsterXP.gameObject.SetActive(false);
                    break;
            }
        }
        else if (encounter.GetType() == typeof(EventCard))
        {
            EventCard e = encounter as EventCard;
            eventComponents.SetActive(true);
            monsterComponents.SetActive(false);
            eventImage.sprite = e.image;
            eventCardName.text = e.cardName;
            eventFlavorText.text = e.flavorText;
            for (int i = 0; i < eventOptionButtons.Count; i++)
                Destroy(eventOptionButtons[i]);
            eventOptionButtons.Clear();
            GameObject optionButton = Instantiate(eventOptionButton, eventOptionButton.transform.position, Quaternion.identity, eventOptionButton.transform.parent);
            optionButton.GetComponentInChildren<TMP_Text>().text = e.optionNames[0].Substring(0, e.optionNames[0].Length - 1);
            optionButton.GetComponent<Button>().onClick = e.OptionEffects[0];
            optionButton.GetComponent<Button>().onClick.AddListener(delegate { ChooseOption(); });
            eventOptionButtons.Add(optionButton);
            eventOptions.text = "<b>" + e.optionNames[0] + "</b>\n" + e.optionDescriptions[0];
            for (int i = 1; i < e.optionNames.Length; i++)
            {
                optionButton = Instantiate(eventOptionButton, eventOptionButton.transform.position, Quaternion.identity, eventOptionButton.transform.parent);
                optionButton.GetComponentInChildren<TMP_Text>().text = e.optionNames[i].Substring(0, e.optionNames[i].Length - 1);
                optionButton.GetComponent<Button>().onClick = e.OptionEffects[i];
                optionButton.GetComponent<Button>().onClick.AddListener(delegate { ChooseOption(); });
                optionButton.transform.localPosition += new Vector3(0, -55 * i, 0);
                eventOptionButtons.Add(optionButton);
                eventOptions.text += "\n\n<b>" + e.optionNames[i] + "</b>\n" + e.optionDescriptions[i];
            }
            if (e.xp < 0)
                eventXP.text = "0";
            else
                eventXP.text = e.xp + "";

            for(int i = 0; i < e.optionRequirements.Length; i++)
            {
                if (!RequirementMet(e.optionRequirements[i]))
                    eventOptionButtons[i].GetComponent<Button>().enabled = false;
            }
        }
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
    }

    public void ActivateOptionCardButton(bool active)
    {
        if (active)
            StartCoroutine(FadeInButton());
        else
            StartCoroutine(FadeOutButton());
    }

    public void SetButtonActive(bool active)
    {
        actionButtonActive = active;
    }

    IEnumerator FadeInButton()
    {
        monsterFightButton.SetActive(true);
        foreach (GameObject g in eventOptionButtons)
            g.SetActive(true);

        for (int i = 1; i <= 100; i++)
        {
            SetAlpha(monsterFightButton, i * 0.01f);
            foreach (GameObject g in eventOptionButtons)
                SetAlpha(g, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        actionButtonActive = true;
    }

    IEnumerator FadeOutButton()
    {
        actionButtonActive = false;

        for (int i = 99; i >= 0; i--)
        {
            SetAlpha(monsterFightButton, i * 0.01f);
            foreach (GameObject g in eventOptionButtons)
                SetAlpha(g, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        monsterFightButton.SetActive(false);
        foreach (GameObject g in eventOptionButtons)
            g.SetActive(false);
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }

    private bool RequirementMet(OptionRequirement o)
    {
        Player p = PlayManager.Instance.localPlayer;
        switch (o)
        {
            case OptionRequirement.NONE: return true;
            case OptionRequirement.ANGELKIN_OR_HOLY: return PlayManager.Instance.Angelkin(p) || PlayManager.Instance.Holy(p);
            case OptionRequirement.ANIMALKIN: return PlayManager.Instance.Animalkin(p);
            case OptionRequirement.BERSERK: return PlayManager.Instance.Berserk(p);
            case OptionRequirement.CHAOS_TIER_1_TO_3: return PlayManager.Instance.ChaosTier() <= 3;
            case OptionRequirement.CHAOS_TIER_4_TO_6: return PlayManager.Instance.ChaosTier() >= 4;
            case OptionRequirement.DWARF_OR_HIGHBORN: return PlayManager.Instance.Dwarf(p) || PlayManager.Instance.Highborn(p);
            case OptionRequirement.ELVEN: return PlayManager.Instance.Elven(p);
            case OptionRequirement.FLEET_FOOTED: return PlayManager.Instance.FleetFooted(p);
            case OptionRequirement.HAS_TORCH: return false; // IMPLEMENT LATER
            case OptionRequirement.LEONIN: return PlayManager.Instance.Leonin(p);
            case OptionRequirement.LOOTER: return PlayManager.Instance.Looter(p);
            case OptionRequirement.MYSTICAL: return PlayManager.Instance.Mystical(p);
            case OptionRequirement.POWERFUL: return PlayManager.Instance.Powerful(p);
        }
        return false;
    }
}