using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EncounterElementType { ROLL_TO_ENCOUNTER, EAT_THE_CANDY };

public class UIEncounterElement : MonoBehaviour
{
    public EncounterElementType type;
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
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

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        switch(type)
        {
            case EncounterElementType.ROLL_TO_ENCOUNTER:
                PlayManager.Instance.ProcessEncounterRoll(roll);
                break;
            case EncounterElementType.EAT_THE_CANDY:
                EventCard e = PlayManager.Instance.encounterReference["Trail of Candy"] as EventCard;
                int xp = e.xp;
                if (roll % 2 == 0)
                    xp += 4;
                else
                    PlayManager.Instance.localPlayer.TakeDamage(8, PlayManager.Instance.GetArmor(PlayManager.Instance.localPlayer));
                PlayManager.Instance.localPlayer.GainXP(xp);
                PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
                break;
        }
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
        roll = Random.Range(1, 7);
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
        int rollTimes = Random.Range(80, 121);
        for (int i = 0; i < rollTimes; i++)
        {
            if (i % 4 == 0)
            {
                JLAudioManager.Instance.SetPitch("RollDice", Random.Range(1.3f,1.7f));
                JLAudioManager.Instance.PlayOneShotSound("RollDice");
            }
            rollDisplay.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End on rolled value face
        rollDisplay.sprite = diceFaces[roll - 1];

        // Display success or failure
        switch (type)
        {
            case EncounterElementType.ROLL_TO_ENCOUNTER:
                if (roll % 2 == 0)
                {
                    JLAudioManager.Instance.PlayOneShotSound("Success");
                    successText.SetActive(true);
                }   
                else
                {
                    JLAudioManager.Instance.PlayOneShotSound("Failure");
                    failureText.SetActive(true);
                }
                break;
            case EncounterElementType.EAT_THE_CANDY:
                if (roll % 2 == 0)
                {
                    JLAudioManager.Instance.PlayOneShotSound("Success");
                    successText.SetActive(true);
                }
                else
                {
                    JLAudioManager.Instance.PlayOneShotSound("Failure");
                    failureText.SetActive(true);
                }
                break;
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
