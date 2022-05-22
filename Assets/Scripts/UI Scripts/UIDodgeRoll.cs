using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDodgeRoll : MonoBehaviour
{
    public UIDefensiveOptions defensiveOptions;
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public Sprite[] diceFaces;
    public TMP_InputField dodgePrediction;
    public Image rollDisplay;
    public GameObject rollButton;
    public float rollLength = 0.01f;
    public float rollDisplayTime = 1f;
    public float waitTime = 0.5f;
    public GameObject successText;
    public GameObject failureText;

    private int hiddenSuccess;
    private RectTransform rt;
    private bool opened;
    private int roll = 0;

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        if (hiddenSuccess == 1)
            defensiveOptions.HadSuccess();
        defensiveOptions.EnableOptions();
        defensiveOptions.SetLockInput(false);
    }

    IEnumerator AnimateOpening()
    {
        // Grow the object
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
        dodgePrediction.enabled = true;
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

        dodgePrediction.enabled = false;
        rollButton.SetActive(false);
        roll = Random.Range(1, 7);
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
            rollDisplay.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End dice roll
        rollDisplay.sprite = diceFaces[roll - 1];

        // Display success or failure
        if (roll == int.Parse(dodgePrediction.text))
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

        yield return new WaitForSeconds(waitTime * Global.animSpeed);

        gameObject.SetActive(false);
    }

    public void ClampDodgeInput()
    {
        if (dodgePrediction.text == "-")
            dodgePrediction.text = "";
        if(!string.IsNullOrWhiteSpace(dodgePrediction.text))
            dodgePrediction.text = Mathf.Clamp(int.Parse(dodgePrediction.text), 1, 6).ToString();
    }

    public void FormatDodgeInputResult()
    {
        if (string.IsNullOrWhiteSpace(dodgePrediction.text))
            dodgePrediction.text = "1";
    }
}
