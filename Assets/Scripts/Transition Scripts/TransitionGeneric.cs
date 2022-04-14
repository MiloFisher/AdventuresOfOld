using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EndEffect { START_MOVEMENT_PHASE, START_ENCOUNTER_PHASE };

public class TransitionGeneric : MonoBehaviour
{
    public Image background;
    public Image[] images;
    public TMP_Text[] texts;
    public float fadeLength = 0.004f;
    public float waitTime = 1f;
    public EndEffect endEffect;

    public void OnEnable()
    {
        ResetFade();
        StartCoroutine(FadeSequence());
    }

    public void OnDisable()
    {
        switch(endEffect)
        {
            case EndEffect.START_MOVEMENT_PHASE:
                PlayManager.Instance.MovePhase();
                break;
            case EndEffect.START_ENCOUNTER_PHASE:
                PlayManager.Instance.EncounterPhase();
                break;
        }
    }

    IEnumerator FadeSequence()
    {
        // Fade in background, images, and texts
        for (int i = 1; i <= 100; i++)
        {
            SetAlpha(background, i * 0.00627f);
            for (int j = 0; j < images.Length; j++)
                SetAlpha(images[j], i * 0.01f);
            for (int j = 0; j < texts.Length; j++)
                SetAlpha(texts[j], i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        yield return new WaitForSeconds(waitTime);

        for (int i = 99; i >= 0; i--)
        {
            SetAlpha(background, i * 0.00627f);
            for (int j = 0; j < images.Length; j++)
                SetAlpha(images[j], i * 0.01f);
            for (int j = 0; j < texts.Length; j++)
                SetAlpha(texts[j], i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        gameObject.SetActive(false);
    }

    private void ResetFade()
    {
        SetAlpha(background, 0);
        for (int j = 0; j < images.Length; j++)
            SetAlpha(images[j], 0);
        for (int j = 0; j < texts.Length; j++)
            SetAlpha(texts[j], 0);
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