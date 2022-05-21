using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class StatusEffectTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject display;
    public float gap = 100;
    public Action OnClick = default;

    private float targetScale = 0.1f;
    private bool hoveringOver;

    private void Start()
    {
        display.SetActive(false);
    }

    private void Update()
    {
        if (display.activeInHierarchy)
        {
            float scale = targetScale / GetCompositeParentsScale(transform);
            display.transform.localScale = new Vector3(scale, scale, 1);
            display.transform.localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * 0.5f + display.GetComponent<RectTransform>().sizeDelta.y * 0.5f * scale + gap * scale, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOver = true;
        StartCoroutine(ShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOver = false;
        display.SetActive(false);
    }

    private void OnDisable()
    {
        hoveringOver = false;
        display.SetActive(false);
    }

    IEnumerator ShowTooltip()
    {
        float timerSeconds = 0.3f;
        int checks = 10;
        int counter = 0;
        while (hoveringOver && counter < checks)
        {
            yield return new WaitForSeconds(timerSeconds / checks);
            counter++;
        }

        if (counter == checks)
        {
            display.SetActive(true);
            float scale = targetScale / GetCompositeParentsScale(transform);
            display.transform.localScale = new Vector3(scale, scale, 1);
            display.transform.localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * 0.5f + display.GetComponent<RectTransform>().sizeDelta.y * 0.5f * scale + gap * scale, 0);
        }
    }

    public bool IsOpen()
    {
        return display.activeInHierarchy;
    }

    public bool IsHoveringOver()
    {
        return hoveringOver;
    }

    private float GetCompositeParentsScale(Transform t)
    {
        if (t.GetComponent<Canvas>())
        {
            return 1;
        }
        return t.localScale.x * GetCompositeParentsScale(t.parent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != default)
            OnClick();
        display.SetActive(false);
    }

    public void SetupDisplay(Effect e)
    {
        TMP_Text textContainer = display.GetComponentInChildren<TMP_Text>();
        textContainer.text = "<b>Effect: <color=" + GetNameColor(e.name) + ">" + e.name + "</color>" + GetTurnCount(e.duration) + "\n" + GetDescription(e.name, e.potency, e.counter);
        textContainer.ForceMeshUpdate(true, true);
        if (textContainer.textInfo != null)
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(5000, 560 + 340 * textContainer.textInfo.lineCount);
    }

    private string GetDescription(string effectName, int potency, int counter)
    {
        return effectName switch
        {
            "Eaten" => "Deals half damage (rounded up) until the end of their next turn.",
            "Enwebbed" => "Misses their next turn.",
            "Plagued" => "Cannot recover Health.",
            "Poisoned" => "Takes " + GetFormattedNumber(potency) + " true damage at the start of each of their turns until they receive healing or leave combat.",
            "Weakened" => "Has their <color=#A1A2A5>Armor</color> reduced by " + GetFormattedNumber(potency) + ".",
            "Bleeding" => "Takes an additional " + GetFormattedNumber(potency) + " damage the next " + GetFormattedNumber(counter) + " times they are attacked.",
            "Burning" => "Takes " + GetFormattedNumber(potency) + " damage at the start of each of their turns.",
            "Dazed" => "Misses their next attack, dealing no damage and inflicting no effects.",
            "Power Up" => "Has their <color=#F16724>Physical Power</color> and <color=#76A5D8>Magical Power</color> increased by " + GetFormattedNumber(potency) + ".",
            "Attack Up" => "Has their <color=white>Attack</color> increased by " + GetFormattedNumber(potency) + ".",
            "Armor Up" => "Has their <color=#A1A2A5>Armor</color> increased by " + GetFormattedNumber(potency) + ".",
            "Power Down" => "Has their <color=#F16724>Physical Power</color> and <color=#76A5D8>Magical Power</color> reduced by " + GetFormattedNumber(potency) + ".",
            "Power Fantasy" => "Revives with +" + GetFormattedNumber(1) + " <color=#F16724>Physical Power</color>,\n+" + GetFormattedNumber(1) + " <color=#76A5D8>Magical Power</color>, and " + GetFormattedNumber(1) + " Health upon death.",
            "Vanish" => "Ignore all incoming damage and effects when you fail an Attack Roll.",
            "Flaming Shot" => "Your next attack has +" + GetFormattedNumber(potency) + " Power and inflicts <b><color=#F7941D>Burning (3)</color>",
            "Blessing" => "Has their <color=#F16724>Physical Power</color> and <color=#76A5D8>Magical Power</color> increased by " + GetFormattedNumber(potency) + ".",
            _ => "Unknown Effect: " + effectName
        };
    }

    private string GetNameColor(string effectName)
    {
        return effectName switch
        {
            "Eaten" => "#BA5DBA",
            "Enwebbed" => "#FFFFFF",
            "Plagued" => "#8DC63F",
            "Poisoned" => "#288931",
            "Weakened" => "#A1A2A5",
            "Bleeding" => "#ED1C24",
            "Burning" => "#F7941D",
            "Dazed" => "#FFF200",
            "Power Up" => "#00A14B",
            "Attack Up" => "#00A14B",
            "Armor Up" => "#00A14B",
            "Power Down" => "#D70B00",
            "Power Fantasy" => "#00A14B",
            "Vanish" => "#00A14B",
            "Flaming Shot" => "#F7941D",
            "Blessing" => "#DDA8D4",
            _ => "#000000"
        };
    }

    private string GetTurnCount(int duration)
    {
        if (duration == 1)
            return " (1 turn)";
        if (duration > 1)
            return " (" + duration + " turns)";
        return "";
    }

    private string GetFormattedNumber(int num)
    {
        return "<color=white>" + num + "</color>";
    }
}