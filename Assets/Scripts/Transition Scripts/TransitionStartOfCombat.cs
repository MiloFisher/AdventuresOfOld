using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionStartOfCombat : MonoBehaviour
{
    public Image background;
    public TMP_Text title;
    public Image divider;
    public Image dividerUnderlay;
    public TMP_Text turnOrderPrompt;
    public TMP_Text[] numberList;
    public TMP_Text[] playerList;
    public TMP_Text closePrompt;
    public float fadeLength = 0.004f;
    public float pulseLength = 0.01f;
    public float waitTime = 0.5f;

    private bool canClose;

    public void OnEnable()
    {
        canClose = false;
        ResetFade();
        StartCoroutine(FadeInSequence());
    }

    public void OnDisable()
    {
        
    }

    public void Close()
    {
        if (canClose)
        {
            StartCoroutine(FadeOutSequence());
        }
    }

    IEnumerator FadeInSequence()
    {
        // First fade in background + title + divider + divider underlay
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(background, i * 0.627f * Global.animRate);
            SetAlpha(title, i * Global.animRate);
            SetAlpha(divider, i * Global.animRate);
            SetAlpha(dividerUnderlay, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        // Next fade in turn order prompt
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(turnOrderPrompt, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        // Next fade in number + player list
        for (int x = 0; x < CombatManager.Instance.turnOrderCombatantList.Count; x++)
        {
            playerList[x].text = "<color=" + CombatManager.Instance.turnOrderCombatantList[x].GetColor() + ">" + CombatManager.Instance.turnOrderCombatantList[x].GetName() + "</color>";
            for (int i = 1; i <= Global.animSteps; i++)
            {
                SetAlpha(numberList[x], i * Global.animRate);
                SetAlpha(playerList[x], i * Global.animRate);
                yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
            }
        }

        canClose = true;

        // Animate close prompt
        while (canClose)
        {
            for (int i = 1; i <= Global.animSteps; i++)
            {
                if (!canClose)
                    break;
                SetAlpha(closePrompt, i * Global.animRate);
                yield return new WaitForSeconds(pulseLength * Global.animTimeMod);
            }
            for (int i = Global.animSteps - 1; i >= 0; i--)
            {
                if (!canClose)
                    break;
                SetAlpha(closePrompt, i * Global.animRate);
                yield return new WaitForSeconds(pulseLength * Global.animTimeMod);
            }
        }
    }

    IEnumerator FadeOutSequence()
    {
        canClose = false;

        // Fade everything out
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(background, i * 0.627f * Global.animRate);
            SetAlpha(title, i * Global.animRate);
            SetAlpha(divider, i * Global.animRate);
            SetAlpha(dividerUnderlay, i * Global.animRate);
            SetAlpha(turnOrderPrompt, i * Global.animRate);
            for (int x = 0; x < PlayManager.Instance.turnOrderPlayerList.Count; x++)
            {
                SetAlpha(numberList[x], i * Global.animRate);
                SetAlpha(playerList[x], i * Global.animRate);
            }
            SetAlpha(closePrompt, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        yield return new WaitForSeconds(waitTime);

        gameObject.SetActive(false);
    }

    private void ResetFade()
    {
        SetAlpha(background, 0);
        SetAlpha(title, 0);
        SetAlpha(divider, 0);
        SetAlpha(dividerUnderlay, 0);
        SetAlpha(turnOrderPrompt, 0);
        for (int i = 0; i < 6; i++)
        {
            SetAlpha(numberList[i], 0);
            SetAlpha(playerList[i], 0);
        }
        SetAlpha(closePrompt, 0);
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
