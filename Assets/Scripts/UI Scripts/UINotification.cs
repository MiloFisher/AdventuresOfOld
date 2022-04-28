using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UINotification : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public float waitTime = 1f;
    public TMP_Text description;

    protected RectTransform rt;
    protected bool opened;
    protected int roll = 0;

    private Action OnComplete;

    public void SendNotification(string descriptionText = default, Action _OnComplete = default)
    {
        gameObject.SetActive(true);
        description.text = descriptionText;
        ResetSize();
        OnComplete = _OnComplete;
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
        for (int i = 1; i <= 100; i++)
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

        yield return new WaitForSeconds(waitTime);

        // Finally start closing
        StartCoroutine(AnimateClosing());
    }

    public void ResetSize()
    {
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
    }

    protected IEnumerator AnimateClosing()
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

        gameObject.SetActive(false);
    }
}
