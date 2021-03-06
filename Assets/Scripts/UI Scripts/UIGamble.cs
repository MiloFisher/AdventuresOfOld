using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamble : MonoBehaviour
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
    public float rollLength = 0.01f;
    public float rollDisplayTime = 1f;
    public float waitTime = 0.5f;
    public GameObject successText;
    public GameObject failureText;
    public int success;

    private int hiddenSuccess;
    private RectTransform rt;
    private bool opened;
    private int roll1 = 0;
    private int roll2 = 0;

    public void MakeGamble()
    {
        success = 0;
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
        hiddenSuccess = 0;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        rollButton.SetActive(true);
        successText.SetActive(false);
        failureText.SetActive(false);
    }

    public void RollDice()
    {
        // Return if the scroll hasn't opened yet
        if (!opened)
            return;

        rollButton.SetActive(false);
        roll1 = Random.Range(1, 7);
        roll2 = Random.Range(1, 7);
        StartCoroutine(AnimateDiceRoll());
    }

    IEnumerator AnimateDiceRoll()
    {
        // Animate dice 1
        int rollTimes = (int)(Global.animSteps * Random.Range(1f, 2f));
        int soundTrigger = Mathf.FloorToInt(Global.animSteps * 0.2f);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % soundTrigger == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", Random.Range(1.3f, 1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay1.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animTimeMod * Global.animSpeed);
        }

        // End dice 1 first
        rollDisplay1.sprite = diceFaces[roll1 - 1];

        yield return new WaitForSeconds(rollDisplayTime * 0.5f * Global.animSpeed);

        // Animate dice 2
        rollTimes = (int)(Global.animSteps * Random.Range(1f, 2f));
        soundTrigger = Mathf.FloorToInt(Global.animSteps * 0.2f);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % soundTrigger == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", Random.Range(1.3f, 1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animTimeMod * Global.animSpeed);
        }

        // End dice 2
        rollDisplay2.sprite = diceFaces[roll2 - 1];

        // Display success or failure
        if (roll1 > roll2)
        {
            JLAudioManager.Instance.PlayOneShotSound("Success");
            successText.SetActive(true);
            hiddenSuccess = 1;
        }
        else
        {
            JLAudioManager.Instance.PlayOneShotSound("Failure");
            failureText.SetActive(true);
            hiddenSuccess = -1;
        }

        yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

        // Start closing scroll
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

        yield return new WaitForSeconds(waitTime);

        gameObject.SetActive(false);
    }
}
