using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICombatOptions : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public float waitTime = 0.5f;
    public GameObject basicAttackButton;
    public GameObject attackAbilityButton;
    public GameObject fleeButton;
    public Color enabledColor;
    public Color disabledColor;
    public GameObject fleeRoll;
    public int resolution;

    private RectTransform rt;
    private bool opened;
    private bool lockInput;
    private int hiddenResolution;

    private bool fleeUsed;

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

    public void BasicAttack()
    {
        if (lockInput || !opened)
            return;
        lockInput = true;
        hiddenResolution = 1;
        StartCoroutine(AnimateClosing());
    }

    public void AttackAbility()
    {
        //if (lockInput || !opened)
        //    return;

    }

    public void Flee()
    {
        if (lockInput || !opened)
            return;
        fleeUsed = true;
        lockInput = true;
        EnableOptions();
        fleeRoll.SetActive(true);
    }

    public void SuccessfullyFled()
    {
        hiddenResolution = 2;
        StartCoroutine(AnimateClosing());
    }

    public void SetLockInput(bool active)
    {
        lockInput = active;
    }

    public void EnableOptions()
    {
        // Enable basic attack
        basicAttackButton.GetComponent<Button>().enabled = true;
        basicAttackButton.GetComponent<Image>().color = enabledColor;

        // Enable attack abilities
        attackAbilityButton.GetComponent<Button>().enabled = true;
        attackAbilityButton.GetComponent<Image>().color = enabledColor;

        // Enable flee if not attempted and not blocked
        if(!fleeUsed && !CombatManager.Instance.fleeingPrevented)
        {
            fleeButton.GetComponent<Button>().enabled = true;
            fleeButton.GetComponent<Image>().color = enabledColor;
        }
        else
        {
            fleeButton.GetComponent<Button>().enabled = false;
            fleeButton.GetComponent<Image>().color = disabledColor;
        }
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
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally set opened to true
        opened = true;
    }

    IEnumerator AnimateClosing()
    {
        opened = false;

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

    public void ResetSize()
    {
        resolution = 0;
        hiddenResolution = 0;
        opened = false;
        fleeUsed = false;
        SetLockInput(false);
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
    }
}
