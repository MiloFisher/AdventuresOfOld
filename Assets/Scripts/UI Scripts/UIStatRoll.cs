using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIStatRoll : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public Sprite[] diceFaces;
    public Image rollDisplay1;
    public Image rollDisplay2;
    public GameObject rollButton;
    public TMP_Text title;
    public TMP_Text subtitle;
    public float rollLength = 0.01f;
    public float rollDisplayTime = 1f;
    public float waitTime = 0.5f;
    public GameObject successText;
    public GameObject failureText;
    public GameObject toggleShowButton;
    public Vector3 hiddenPosition;
    public int success;

    private bool lockInput;
    private bool isHidden;
    private int hiddenSuccess;
    private RectTransform rt;
    private bool opened;
    private int roll1 = 0;
    private int roll2 = 0;

    private string statRollType;
    private int statRollValue;

    private Player player;

    public void MakeStatRoll(string _statRollType, int _statRollValue, Player p = default)
    {
        success = 0;
        statRollType = _statRollType;
        statRollValue = _statRollValue;
        player = p;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        success = hiddenSuccess;
    }

    IEnumerator AnimateOpening()
    {
        // Set title
        title.text = statRollType + " Roll (" + statRollValue + ")";
        subtitle.text = "Your current stat modifier: " + FormatStat(PlayManager.Instance.GetStatModFromType(statRollType, player));

        // Then grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Next open the scroll and fade in toggle show button
        dif = endWidth - startWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            if (toggleShowButton)
                SetAlpha(toggleShowButton, i * Global.animRate);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally set opened to true
        opened = true;
    }

    private string FormatStat(int value)
    {
        if (value > 0)
            return "<b><color=#267833>+" + value + "</color></b>";
        else if (value < 0)
            return "<b><color=#902E2E>" + value + "</color></b>";
        else
            return "<b><color=white>+" + value + "</color></b>";
    }

    public void ResetSize()
    {
        hiddenSuccess = 0;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        rollButton.SetActive(true);
        successText.SetActive(false);
        failureText.SetActive(false);
        isHidden = false;
        lockInput = false;
        transform.localPosition = Vector3.zero;
        if (toggleShowButton)
        {
            SetAlpha(toggleShowButton, 0);
            toggleShowButton.GetComponentInChildren<TMP_Text>().text = "Hide";
        } 
    }

    public void RollDice()
    {
        // Return if the scroll hasn't opened yet or if input is locked
        if (!opened || lockInput)
            return;

        lockInput = true;
        rollButton.SetActive(false);
        roll1 = Random.Range(1, 7);
        roll2 = Random.Range(1, 7);
        StartCoroutine(AnimateDiceRoll());
    }

    IEnumerator AnimateDiceRoll()
    {
        // Flash through random dice faces
        int rollTimes = Random.Range(80, 121);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % 4 == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", Random.Range(1.3f, 1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay1.sprite = diceFaces[Random.Range(0, 6)];
            rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End dice 1 first
        rollDisplay1.sprite = diceFaces[roll1 - 1];

        // Keep rolling dice 2
        rollTimes = Random.Range(20, 60);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % 4 == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", Random.Range(1.3f, 1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End dice 2
        rollDisplay2.sprite = diceFaces[roll2 - 1];

        // Display success or failure
        if (roll1 + roll2 + PlayManager.Instance.GetStatModFromType(statRollType, player) >= statRollValue)
        {
            successText.SetActive(true);
            hiddenSuccess = 1;
        }
        else
        {
            failureText.SetActive(true);
            hiddenSuccess = -1;
        }

        yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

        // Start closing scroll
        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateClosing()
    {
        opened = false;

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // First close the scroll and fade out toggle show button
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            if(toggleShowButton)
                SetAlpha(toggleShowButton, i * Global.animRate);
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

    public void ToggleShow()
    {
        if (!opened || lockInput)
            return;

        if(isHidden)
            StartCoroutine(Maximize());
        else
            StartCoroutine(Minimize());
    }

    IEnumerator Minimize()
    {
        lockInput = true;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localPosition =  i * Global.animRate * hiddenPosition;
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }
        lockInput = false;
        isHidden = true;
        toggleShowButton.GetComponentInChildren<TMP_Text>().text = "Show";
    }

    IEnumerator Maximize()
    {
        lockInput = true;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            transform.localPosition = i * Global.animRate * hiddenPosition;
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }
        lockInput = false;
        isHidden = false;
        toggleShowButton.GetComponentInChildren<TMP_Text>().text = "Hide";
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
