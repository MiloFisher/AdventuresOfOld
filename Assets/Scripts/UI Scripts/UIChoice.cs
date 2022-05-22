using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIChoice : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public float fadeLength = 0.004f;
    public GameObject choiceButton1;
    public GameObject choiceButton2;
    public Color enabledColor;
    public Color disabledColor;
    public Color textEnabledColor;
    public Color textDisabledColor;
    public float waitTime = 0.5f;
    public int choice;

    private int hiddenChoice;
    private RectTransform rt;
    private bool opened;
    private bool condition1;
    private bool condition2;

    public void MakeChoice(string choice1, string choice2, bool _condition1, bool _condition2)
    {
        choiceButton1.GetComponentInChildren<TMP_Text>().text = choice1;
        choiceButton2.GetComponentInChildren<TMP_Text>().text = choice2;
        condition1 = _condition1;
        condition2 = _condition2;
        choice = 0;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        choice = hiddenChoice;
    }

    IEnumerator AnimateOpening()
    {
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
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally set opened to true
        opened = true;
    }

    public void ResetSize()
    {
        hiddenChoice = 0;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        choiceButton1.SetActive(true);
        choiceButton2.SetActive(true);
        choiceButton1.GetComponent<Button>().enabled = condition1;
        choiceButton1.GetComponent<Image>().color = condition1 ? enabledColor : disabledColor;
        choiceButton1.GetComponentInChildren<TMP_Text>().color = condition1 ? textEnabledColor : textDisabledColor;
        choiceButton2.GetComponent<Button>().enabled = condition2;
        choiceButton2.GetComponent<Image>().color = condition2 ? enabledColor : disabledColor;
        choiceButton2.GetComponentInChildren<TMP_Text>().color = condition2 ? textEnabledColor : textDisabledColor;
    }

    public void ChooseOption(int option)
    {
        // Return if the scroll hasn't opened yet
        if (!opened)
            return;

        hiddenChoice = option;
        choiceButton1.GetComponent<Button>().enabled = false;
        choiceButton2.GetComponent<Button>().enabled = false;

        StartCoroutine(AnimateChoice());
    }

    IEnumerator AnimateChoice()
    {
        // Fade out non-selected choice
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(hiddenChoice == 2 ? choiceButton1 : choiceButton2, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateClosing()
    {
        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
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
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
