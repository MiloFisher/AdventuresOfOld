using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRollToEncounter : MonoBehaviour
{
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
        PlayManager.Instance.ProcessEncounterRoll(roll);
    }

    IEnumerator AnimateOpening()
    {
        // First grow the object
        float dif = endScale - startScale;
        for(int i = 1; i <= 100; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * 0.01f, startScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(growingLength);
        }

        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= 100; i++)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * 0.01f, constHeight);
            yield return new WaitForSeconds(openingLength);
        }

        // Finally set opened to true
        opened = true;
    }

    public void ResetSize()
    {
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        rollButton.SetActive(true);
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

    IEnumerator AnimateDiceRoll(int roll)
    {
        // Flash through random dice faces
        for(int i = 0; i < 100; i++)
        {
            rollDisplay.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength);
        }

        // End on rolled value face
        rollDisplay.sprite = diceFaces[roll - 1];

        yield return new WaitForSeconds(rollDisplayTime);

        // Start closing scroll
        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateClosing()
    {
        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = 99; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * 0.01f, constHeight);
            yield return new WaitForSeconds(openingLength);
        }

        // Next shrink the object
        dif = endScale - startScale;
        for (int i = 99; i >= 0; i--)
        {
            transform.localScale = new Vector3(startScale + dif * i * 0.01f, startScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(growingLength);
        }

        yield return new WaitForSeconds(waitTime);

        gameObject.SetActive(false);
    }
}
