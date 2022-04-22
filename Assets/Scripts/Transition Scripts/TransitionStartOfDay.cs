using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionStartOfDay : MonoBehaviour
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
    public float waitTime = 1f;

    private bool canClose;

    public void OnEnable()
    {
        canClose = false;
        ResetFade();
        StartCoroutine(FadeInSequence());

        PlayManager.Instance.startOrEndOfDay = true;
    }

    public void OnDisable()
    {
        // If it is your turn, after displaying "Start of Day", display "Your Turn"
        if (PlayManager.Instance.isYourTurn)
            PlayManager.Instance.CallTransition(1);

        PlayManager.Instance.startOrEndOfDay = false;
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
        for (int i = 01; i <= 100; i++)
        {
            SetAlpha(background, i * 0.00627f);
            SetAlpha(title, i * 0.01f);
            SetAlpha(divider, i * 0.01f);
            SetAlpha(dividerUnderlay, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        // Next fade in turn order prompt
        for (int i = 1; i <= 100; i++)
        {
            SetAlpha(turnOrderPrompt, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        //Next fade in number + player list
        for(int x = 0; x < PlayManager.Instance.turnOrderPlayerList.Count; x++)
        {
            playerList[x].text = "<color=" + PlayManager.Instance.turnOrderPlayerList[x].Color.Value + ">" + PlayManager.Instance.turnOrderPlayerList[x].Name.Value + "</color>";
            for (int i = 1; i <= 100; i++)
            {
                SetAlpha(numberList[x], i * 0.01f);
                SetAlpha(playerList[x], i * 0.01f);
                yield return new WaitForSeconds(fadeLength);
            }
        }

        // Next fade in close prompt
        for (int i = 1; i <= 100; i++)
        {
            SetAlpha(closePrompt, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        canClose = true;

        // Animate close prompt
        while(canClose)
        {
            for (int i = 99; i >= 0; i--)
            {
                if (!canClose)
                    break;
                SetAlpha(closePrompt, i * 0.01f);
                yield return new WaitForSeconds(pulseLength);
            }
            if (!canClose)
                break;
            for (int i = 1; i <= 100; i++)
            {
                if (!canClose)
                    break;
                SetAlpha(closePrompt, i * 0.01f);
                yield return new WaitForSeconds(pulseLength);
            }
        }
    }

    IEnumerator FadeOutSequence()
    {
        canClose = false;

        // Fade everything out
        for (int i = 99; i >= 0; i--)
        {
            SetAlpha(background, i * 0.00627f);
            SetAlpha(title, i * 0.01f);
            SetAlpha(divider, i * 0.01f);
            SetAlpha(dividerUnderlay, i * 0.01f);
            SetAlpha(turnOrderPrompt, i * 0.01f);
            for (int x = 0; x < PlayManager.Instance.turnOrderPlayerList.Count; x++)
            {
                SetAlpha(numberList[x], i * 0.01f);
                SetAlpha(playerList[x], i * 0.01f);
            }
            SetAlpha(closePrompt, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
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
        for(int i = 0; i < 6; i++)
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
