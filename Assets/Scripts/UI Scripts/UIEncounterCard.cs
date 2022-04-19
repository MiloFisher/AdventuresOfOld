using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEncounterCard : MonoBehaviour
{
    [Header("Generic Components")]
    public string cardName;
    public GameObject collectCardButton;
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

    private bool collectButtonActive;

    public void ClickCard(int id)
    {
        
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
            eventOptions.text = "<b>" + e.optionNames[0] + "</b>\n" + e.optionDescriptions[0];
            for (int i = 1; i < e.optionNames.Length; i++)
            {
                eventOptions.text += "\n\n<b>" + e.optionNames[i] + "</b>\n" + e.optionDescriptions[i];
            }
            eventXP.text = e.xp + "";
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

    public void ActivateCollectCardButton(bool active)
    {
        if (active)
            StartCoroutine(FadeInButton());
        else
            StartCoroutine(FadeOutButton());
    }

    IEnumerator FadeInButton()
    {
        collectCardButton.SetActive(true);

        for (int i = 1; i <= 100; i++)
        {
            SetAlpha(collectCardButton, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        collectButtonActive = true;
    }

    IEnumerator FadeOutButton()
    {
        collectButtonActive = false;

        for (int i = 99; i >= 0; i--)
        {
            SetAlpha(collectCardButton, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        collectCardButton.SetActive(false);
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
