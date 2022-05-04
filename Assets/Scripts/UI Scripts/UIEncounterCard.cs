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
    public GameObject[] cardButtons;
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
    public Color eventOptionActiveColor;
    public Color eventOptionInactiveColor;

    [Header("Monster Card Components")]
    public GameObject monsterComponents;
    public GameObject monsterDisplay;
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
    public GameObject healthBarBack;
    public TMP_Text healthBarText;
    public GameObject healthBar;
    public GameObject turnMarker;
    public GameObject monsterDamaged;
    public GameObject damageNumber;
    public Transform statusEffectsContainer;
    public GameObject statusEffectPrefab;
    public Sprite[] statusEffectIcons;
    public float damageNumberFadeLength = 0.004f;

    private bool actionButtonActive;

    private List<GameObject> statusEffectList = new List<GameObject>();

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

        CombatManager.Instance.monsterCard = m;

        PlayManager.Instance.localPlayer.SendCombatNotifications();
        PlayManager.Instance.localPlayer.SetValue("ParticipatingInCombat", 1);
        PlayManager.Instance.CallEncounterElement(7);
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
            monsterHealth.text = m.health + PlayManager.Instance.HealthModifier() + "";
            monsterAttack.text = m.attack + PlayManager.Instance.AttackModifier() + "";
            monsterSpeed.text = m.speed + "";
            monsterPhysicalPower.text = m.physicalPower + PlayManager.Instance.PowerModifier() + "";
            monsterMagicalPower.text = m.magicalPower + PlayManager.Instance.PowerModifier() + "";
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
                    monsterXP.text = 3 + PlayManager.Instance.XPModifier() + "";
                    break;
                case MonsterType.ELITE:
                    monsterCardFace.sprite = monsterCardFaces[1];
                    monsterGold.gameObject.SetActive(true);
                    monsterXP.gameObject.SetActive(true);
                    monsterGold.text = "20";
                    monsterXP.text = 6 + PlayManager.Instance.XPModifier() + "";
                    break;
                case MonsterType.MINIBOSS:
                    monsterCardFace.sprite = monsterCardFaces[2];
                    monsterGold.gameObject.SetActive(true);
                    monsterXP.gameObject.SetActive(true);
                    monsterGold.text = "20";
                    monsterXP.text = 6 + PlayManager.Instance.XPModifier() + "";
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
                eventXP.text = e.xp + PlayManager.Instance.XPModifier() + "";

            for(int i = 0; i < e.optionRequirements.Length; i++)
            {
                if (!RequirementMet(e.optionRequirements[i]))
                {
                    eventOptionButtons[i].GetComponent<Button>().enabled = false;
                    eventOptionButtons[i].GetComponent<Image>().color = eventOptionInactiveColor;
                }
                else
                {
                    eventOptionButtons[i].GetComponent<Image>().color = eventOptionActiveColor;
                }
            }
        }
    }

    public void UpdateHealthBar(Combatant monster)
    {
        healthBarText.text = monster.GetHealth() + " / " + monster.GetMaxHealth();
        healthBar.transform.localPosition = new Vector3(6954f * monster.GetHealth() / monster.GetMaxHealth() - 6954f, 0, 0);
    }

    public void SetDisplayPosition(Vector3 pos)
    {
        monsterDisplay.transform.localPosition = pos;
    }

    public Vector3 GetDisplayPositionScaled()
    {
        return transform.localPosition + monsterDisplay.transform.localPosition * transform.localScale.x;
    }

    public void DrawStatusEffects(List<Effect> effects)
    {
        // First previous effects
        for (int i = 0; i < statusEffectList.Count; i++)
            Destroy(statusEffectList[i]);
        statusEffectList.Clear();

        // Then create new effects
        for (int i = 0; i < effects.Count; i++)
        {
            GameObject g = Instantiate(statusEffectPrefab, statusEffectsContainer);
            g.GetComponent<Image>().sprite = GetEffectSprite(effects[i]);
            g.transform.localPosition = new Vector3(0, -50 * i, 0);
            statusEffectList.Add(g);
        }

    }

    public Sprite GetEffectSprite(Effect e)
    {
        return e.name switch
        {
            "Bleeding" => statusEffectIcons[0],
            "Poisoned" => statusEffectIcons[1],
            "Weakened" => statusEffectIcons[2],
            _ => null
        };
    }

    public void ActivateTurnMarker(bool active)
    {
        turnMarker.SetActive(active);
    }

    public void ActivateHealthBar(bool active)
    {
        healthBarBack.SetActive(active);
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        foreach(GameObject g in cardButtons)
            g.SetActive(active);
    }

    public void ActivateDamaged(bool active)
    {
        monsterDamaged.SetActive(active);
    }

    public void DisplayDamageNumber(int amount)
    {
        if (amount < 0)
            amount = 0;
        Vector3 startPosition = healthBarBack.transform.localPosition + new Vector3(0.2f * (healthBar.GetComponent<RectTransform>().sizeDelta.x / 2 + healthBar.transform.localPosition.x), 0, 0);
        damageNumber.GetComponent<TMP_Text>().text = "-" + amount;
        damageNumber.SetActive(true);
        SetAlpha(damageNumber.GetComponent<TMP_Text>(), 1);
        StartCoroutine(AnimateDamageNumber(startPosition));
    }

    IEnumerator AnimateDamageNumber(Vector3 startPosition)
    {
        damageNumber.transform.localPosition = startPosition;

        // Fade in and float number
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(damageNumber.GetComponent<TMP_Text>(), 1 - i * Global.animRate);
            damageNumber.transform.localPosition = startPosition + new Vector3(i * Global.animRate * 50, i * Global.animRate * -100, 0);
            yield return new WaitForSeconds(damageNumberFadeLength * Global.animTimeMod);
        }

        damageNumber.SetActive(false);
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

        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(monsterFightButton, i * Global.animRate);
            foreach (GameObject g in eventOptionButtons)
                SetAlpha(g, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        actionButtonActive = true;
    }

    IEnumerator FadeOutButton()
    {
        actionButtonActive = false;

        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(monsterFightButton, i * Global.animRate);
            foreach (GameObject g in eventOptionButtons)
                SetAlpha(g, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
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

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }

    private bool RequirementMet(OptionRequirement o)
    {
        Player p = PlayManager.Instance.localPlayer;
        return o switch
        {
            OptionRequirement.NONE => true,
            OptionRequirement.ANGELKIN_OR_HOLY => PlayManager.Instance.Angelkin(p) || PlayManager.Instance.Holy(p),
            OptionRequirement.ANIMALKIN => PlayManager.Instance.Animalkin(p),
            OptionRequirement.BERSERK => PlayManager.Instance.Berserk(p),
            OptionRequirement.CHAOS_TIER_1_TO_3 => PlayManager.Instance.ChaosTier() <= 3,
            OptionRequirement.CHAOS_TIER_4_TO_6 => PlayManager.Instance.ChaosTier() >= 4,
            OptionRequirement.DWARF_OR_HIGHBORN => PlayManager.Instance.Dwarf(p) || PlayManager.Instance.Highborn(p),
            OptionRequirement.ELVEN => PlayManager.Instance.Elven(p),
            OptionRequirement.FLEET_FOOTED => PlayManager.Instance.FleetFooted(p),
            OptionRequirement.HAS_TORCH => PlayManager.Instance.HasTorch(p),
            OptionRequirement.LEONIN => PlayManager.Instance.Leonin(p),
            OptionRequirement.LOOTER => PlayManager.Instance.Looter(p),
            OptionRequirement.MYSTICAL => PlayManager.Instance.Mystical(p),
            OptionRequirement.POWERFUL => PlayManager.Instance.Powerful(p),
            _ => false
        };
    }
}
