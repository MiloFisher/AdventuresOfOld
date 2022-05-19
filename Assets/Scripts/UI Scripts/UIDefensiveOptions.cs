using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIDefensiveOptions : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public float waitTime = 0.5f;
    public GameObject dodgeButton;
    public GameObject requestTauntButton;
    public GameObject payUpButton;
    public GameObject takeDamageButton;
    public GameObject continueButton;
    public Color enabledColor;
    public Color disabledColor;
    public GameObject dodgeRoll;
    public GameObject toggleShowButton;
    public Vector3 hiddenPosition;
    public int resolution;
    public Player targetPlayer;

    private bool isHidden;
    private RectTransform rt;
    private bool opened;
    private bool lockInput;
    private int hiddenResolution;

    private bool dodgeUsed;
    private bool requestUsed;
    private bool payUpUsed;
    private bool hadSuccess;

    private void OnEnable()
    {
        ResetSize();
        EnableOptions();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        resolution = hiddenResolution;
    }

    public void HadSuccess()
    {
        hadSuccess = true;
    }

    private void Update()
    {
        if (CombatManager.Instance.isYourTurn && CombatManager.Instance.CombatOverCheck() > -1 && !lockInput && opened)
        {
            lockInput = true;
            hiddenResolution = 99;
            StartCoroutine(AnimateClosing());
        }
    }

    public void Dodge()
    {
        if (lockInput || !opened)
            return;
        dodgeUsed = true;
        lockInput = true;
        EnableOptions();
        dodgeRoll.SetActive(true);
    }

    public void RequestTaunt()
    {
        if (lockInput || !opened)
            return;
        requestUsed = true;
        lockInput = true;
        EnableOptions();

        string description = "You may wait for another player to taunt or continue, if none taunt";
        PlayManager.Instance.SendNotification(2, description, () => {
            SetLockInput(false);
        });

        PlayManager.Instance.localPlayer.SetValue("RequestedTaunt", true);
        PlayManager.Instance.localPlayer.SendRequestTauntNotifications();
    }

    public void TauntReceived()
    {
        foreach(Player p in PlayManager.Instance.playerList)
        {
            if (p.Taunting.Value)
            {
                targetPlayer = p;
                p.SetValue("Taunting", false);
            }
        }
        hadSuccess = true;
        EnableOptions();
    }

    public void PayUp()
    {
        if (lockInput || !opened)
            return;
        payUpUsed = true;
        lockInput = true;

        int goldAmount = 0;
        switch (CombatManager.Instance.OnPlayerBeingAttacked)
        {
            case OnAttacked.PAY_10_GOLD: goldAmount = 10; break;
            case OnAttacked.PAY_20_GOLD: goldAmount = 20; break;
        }

        PlayManager.Instance.localPlayer.LoseGold(goldAmount);
        HadSuccess();
        EnableOptions();

        string description = "You paid <color=#FFE200>" + goldAmount + " Gold</color> to not take any damage!";
        PlayManager.Instance.SendNotification(3, description, () => {
            SetLockInput(false);
        });
    }

    public void TakeDamage()
    {
        if (lockInput || !opened)
            return;
        lockInput = true;
        hiddenResolution = -1;
        StartCoroutine(AnimateClosing());
    }

    public void Continue()
    {
        if (lockInput || !opened)
            return;
        lockInput = true;
        hiddenResolution = 1;
        StartCoroutine(AnimateClosing());
    }

    public void SetLockInput(bool active)
    {
        lockInput = active;
    }

    public void EnableOptions()
    {
        // Enable dodge button if player meets requirement and has not used dodge yet
        if(AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Dodge"), PlayManager.Instance.localPlayer) && !dodgeUsed && !hadSuccess)
        {
            dodgeButton.GetComponent<Button>().enabled = true;
            dodgeButton.GetComponent<Image>().color = enabledColor;
        }
        else
        {
            dodgeButton.GetComponent<Button>().enabled = false;
            dodgeButton.GetComponent<Image>().color = disabledColor;
        }
        // Enable request taunt button if player has an ally who can taunt and has not requested a taunt yet
        if (CombatManager.Instance.GetAlliesWhoCanTaunt(PlayManager.Instance.localPlayer).Count > 0 && !requestUsed && !hadSuccess)
        {
            requestTauntButton.GetComponent<Button>().enabled = true;
            requestTauntButton.GetComponent<Image>().color = enabledColor;
        }
        else
        {
            requestTauntButton.GetComponent<Button>().enabled = false;
            requestTauntButton.GetComponent<Image>().color = disabledColor;
        }
        // Activate pay up button if monster allows for paying to avoid damage
        if(CombatManager.Instance.OnPlayerBeingAttacked != OnAttacked.NONE)
        {
            payUpButton.SetActive(true);
            switch (CombatManager.Instance.OnPlayerBeingAttacked)
            {
                case OnAttacked.PAY_10_GOLD: payUpButton.GetComponentInChildren<TMP_Text>().text = "<Pay 10 Gold>"; break;
                case OnAttacked.PAY_20_GOLD: payUpButton.GetComponentInChildren<TMP_Text>().text = "<Pay 20 Gold>"; break;
            }
            // Enable pay up button if player has not use pay up or dodge yet and has enough money
            if(!payUpUsed && !dodgeUsed && !hadSuccess && ((CombatManager.Instance.OnPlayerBeingAttacked == OnAttacked.PAY_10_GOLD && PlayManager.Instance.GetGold(PlayManager.Instance.localPlayer) >= 10) || (CombatManager.Instance.OnPlayerBeingAttacked == OnAttacked.PAY_20_GOLD && PlayManager.Instance.GetGold(PlayManager.Instance.localPlayer) >= 20)))
            {
                payUpButton.GetComponent<Button>().enabled = true;
                payUpButton.GetComponent<Image>().color = enabledColor;
            }
            else
            {
                payUpButton.GetComponent<Button>().enabled = false;
                payUpButton.GetComponent<Image>().color = disabledColor;
            }
        }
        else
        {
            payUpButton.SetActive(false);
        }

        // If player had success dodging, getting taunted, or paying up, activate continue button
        continueButton.SetActive(hadSuccess);
        // Otherwise activate take damage button
        takeDamageButton.SetActive(!hadSuccess);
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

        // Next open the scroll
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

    IEnumerator AnimateClosing()
    {
        PlayManager.Instance.localPlayer.SetValue("RequestedTaunt", false);
        opened = false;

        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            if (toggleShowButton)
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

    public void ResetSize()
    {
        targetPlayer = default;
        resolution = 0;
        hiddenResolution = 0;
        hadSuccess = false;
        dodgeUsed = false;
        requestUsed = false;
        payUpUsed = false;
        opened = false;
        SetLockInput(false);
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        transform.localPosition = Vector3.zero;
        if (toggleShowButton)
        {
            SetAlpha(toggleShowButton, 0);
            toggleShowButton.GetComponentInChildren<TMP_Text>().text = "Hide";
        }
    }

    public void ToggleShow()
    {
        if (!opened || lockInput)
            return;

        if (isHidden)
            StartCoroutine(Maximize());
        else
            StartCoroutine(Minimize());
    }

    IEnumerator Minimize()
    {
        lockInput = true;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localPosition = i * Global.animRate * hiddenPosition;
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
