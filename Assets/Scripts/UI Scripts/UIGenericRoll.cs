using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIGenericRoll : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public TMP_Text title;
    public Sprite[] diceFaces;
    public Image rollDisplay;
    public GameObject rollButton;
    public float rollLength = 0.01f;
    public float rollDisplayTime = 1f;
    public float waitTime = 0.5f;
    public GameObject successText;
    public GameObject failureText;

    protected RectTransform rt;
    protected bool opened;
    protected int roll = 0;

    private Func<int, bool> SuccessCondition;
    private Action OnSuccess;
    private Action OnFailure;
    private Action OnComplete;

    public void Setup(string title, Func<int, bool> SuccessCondition, Action OnSuccess, Action OnFailure = default)
    {
        this.title.text = title;
        this.SuccessCondition = SuccessCondition;
        this.OnSuccess = OnSuccess;
        this.OnFailure = OnFailure;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        if(OnComplete != default)
            OnComplete();
    }

    protected IEnumerator AnimateOpening()
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

    public void RollDice()
    {
        // Return if the scroll hasn't opened yet
        if (!opened)
            return;

        rollButton.SetActive(false);
        roll = UnityEngine.Random.Range(1, 7);
        StartCoroutine(AnimateDiceRoll(roll));
    }

    public void ResetSize()
    {
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        rollButton.SetActive(true);
        successText.SetActive(false);
        failureText.SetActive(false);
    }

    protected IEnumerator AnimateDiceRoll(int roll)
    {
        // Flash through random dice faces
        int rollTimes = (int)(Global.animSteps * UnityEngine.Random.Range(1f, 2f));
        int soundTrigger = Mathf.FloorToInt(Global.animSteps * 0.2f);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % soundTrigger == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", UnityEngine.Random.Range(1.3f, 1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay.sprite = diceFaces[UnityEngine.Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animTimeMod * Global.animSpeed);
        }

        // End on rolled value face
        rollDisplay.sprite = diceFaces[roll - 1];

        // Display success or failure
        if (SuccessCondition(roll))
        {
            JLAudioManager.Instance.PlayOneShotSound("Success");
            successText.SetActive(true);
            OnComplete = OnSuccess;
        }
        else
        {
            JLAudioManager.Instance.PlayOneShotSound("Failure");
            failureText.SetActive(true);
            OnComplete = OnFailure;
        }

        yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

        // Start closing scroll
        StartCoroutine(AnimateClosing());
    }

    protected IEnumerator AnimateClosing()
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
}
