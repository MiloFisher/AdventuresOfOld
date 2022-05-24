using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIAbilityDisplay : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float baseHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public float waitTime = 0.5f;
    public float unitHeight = 230;
    public GameObject skillContainer;
    public GameObject abilityChargesBar;
    public TMP_Text abilityChargesText;
    public GameObject[] passives;
    public GameObject[] skills;
    public UICombatOptions combatOptions;

    private RectTransform rt;
    public bool opened;
    public bool animating;
    private float currentHeight;

    private void OnEnable()
    {
        ResetSize();
        SetupAbilities();
        StartCoroutine(AnimateOpening());
    }

    private void Update()
    {
        if(opened)
        {
            SetupAbilities();
        }
    }

    private void SetupAbilities()
    {
        List<Skill> passiveList = AbilityManager.Instance.GetPassives();
        List<Skill> skillList = AbilityManager.Instance.GetSkills();

        currentHeight = baseHeight;
        skillContainer.transform.localPosition = Vector3.zero;

        if (passiveList.Count <= 3)
        {
            currentHeight -= unitHeight;
            skillContainer.transform.localPosition += new Vector3(0, unitHeight, 0);
        }

        if(skillList.Count <= 6)
        {
            currentHeight -= unitHeight;
        }

        if (skillList.Count <= 3)
        {
            currentHeight -= unitHeight;
        }

        // Setup Passives
        int i;
        for(i = 0; i < passiveList.Count; i++)
        {
            passives[i].SetActive(true);
            passives[i].transform.localPosition = new Vector3(-425 * (GetSize(i, passiveList.Count) - 2 * (i%3) - 1), -320 - unitHeight * (i/3), 0);
            // Set skill text
            passives[i].GetComponentInChildren<TMP_Text>().text = passiveList[i].skillName;
            // Set skill color
            passives[i].GetComponent<Image>().color = AbilityManager.Instance.GetSkillColor(passiveList[i]);
            // Set button enabled (disabled always for passives)
            passives[i].GetComponent<Button>().enabled = false;
            // Set tooltip
            AbilityManager.Instance.FormatTooltip(passives[i].GetComponent<Tooltip>().display, passiveList[i]);
        }
        for (; i < passives.Length; i++)
            passives[i].SetActive(false);

        // Setup Skills
        for (i = 0; i < skillList.Count; i++)
        {
            skills[i].SetActive(true);
            skills[i].transform.localPosition = new Vector3(-425 * (GetSize(i, skillList.Count) - 2 * (i % 3) - 1), -320 - unitHeight * (i / 3), 0);
            // Set skill text
            skills[i].GetComponentInChildren<TMP_Text>().text = skillList[i].skillName;
            // Set skill color
            skills[i].GetComponent<Image>().color = AbilityManager.Instance.GetSkillColor(skillList[i]);
            // Set button enabled
            if(AbilityManager.Instance.CanUseSkill(skillList[i]))
            {
                skills[i].GetComponent<Button>().enabled = true;
                skills[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                skills[i].GetComponent<Button>().enabled = false;
                skills[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            // Set button press effect
            skills[i].GetComponent<Button>().onClick.RemoveAllListeners();
            skills[i].GetComponent<Button>().onClick.AddListener(AbilityManager.Instance.GetAbilityCall(skillList[i]));
            // Set tooltip
            AbilityManager.Instance.FormatTooltip(skills[i].GetComponent<Tooltip>().display, skillList[i]);
        }
        for (; i < skills.Length; i++)
            skills[i].SetActive(false);

        // Setup Ability Charge bar
        abilityChargesText.text = PlayManager.Instance.GetAbilityCharges(PlayManager.Instance.localPlayer) + " / " + PlayManager.Instance.GetMaxAbilityCharges(PlayManager.Instance.localPlayer);
        abilityChargesBar.transform.localPosition = new Vector3(3329f * PlayManager.Instance.GetAbilityCharges(PlayManager.Instance.localPlayer) / PlayManager.Instance.GetMaxAbilityCharges(PlayManager.Instance.localPlayer) - 3329f, 0, 0);

        // Update RectTransform
        if(!animating)
        {
            if(opened)
                rt.sizeDelta = new Vector2(endWidth, currentHeight);
            else
                rt.sizeDelta = new Vector2(startWidth, currentHeight);
        }
    }

    private int GetSize(int i, int count)
    {
        if (count <= 3)
            return count;
        if (count <= 6)
        {
            if (i < 3)
                return 3;
            return count - 3;
        }
        if (count <= 9)
        {
            if (i < 6)
                return 3;
            return count - 6;
        }
        return 3;
    }

    public void Close()
    {
        if (!opened)
            return;

        StartCoroutine(AnimateClosing());
    }

    public void Close(Action OnComplete)
    {
        if (!opened)
            return;

        StartCoroutine(AnimateClosing(OnComplete));
    }

    IEnumerator AnimateOpening()
    {
        animating = true;

        // First grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, currentHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        if (combatOptions.opened)
        {
            combatOptions.SetLockInput(true);
            CombatManager.Instance.SetCanUseAttackAbilities(true);
        }
        else
            CombatManager.Instance.SetCanUseAttackAbilities(false);

        // Finally set opened to true
        opened = true;
        animating = false;
    }

    IEnumerator AnimateClosing(Action OnComplete = default)
    {
        animating = true;
        opened = false;

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, currentHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Next shrink the object
        dif = endScale - startScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        yield return new WaitForSeconds(waitTime * Global.animSpeed);

        gameObject.SetActive(false);
        animating = false;

        if (combatOptions.opened)
            combatOptions.SetLockInput(false);

        if (OnComplete != default)
            OnComplete();
    }

    public void ResetSize()
    {
        animating = false;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
    }
}
