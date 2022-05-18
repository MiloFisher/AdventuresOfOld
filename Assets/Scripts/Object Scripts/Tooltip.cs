using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject display;
    public float gap = 100;
    public Action OnClick = default;

    private float targetScale = 0.1f;
    private bool hoveringOver;

    private void Start()
    {
        display.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOver = true;
        StartCoroutine(ShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOver = false;
        display.SetActive(false);
    }

    private void OnDisable()
    {
        hoveringOver = false;
        display.SetActive(false);
    }

    IEnumerator ShowTooltip()
    {
        float timerSeconds = 0.5f;
        int checks = 10;
        int counter = 0;
        while(hoveringOver && counter < checks)
        {
            yield return new WaitForSeconds(timerSeconds/checks);
            counter++;
        }

        if(counter == checks && hoveringOver)
        {
            display.SetActive(true);
            float scale = targetScale / GetCompositeParentsScale(transform);
            display.transform.localScale = new Vector3(scale, scale, 1);
            display.transform.localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * 0.5f + display.GetComponent<RectTransform>().sizeDelta.y * 0.5f * scale + gap * scale, 0);
        }
    }

    public bool IsOpen()
    {
        return display.activeInHierarchy;
    }

    public bool IsHoveringOver()
    {
        return hoveringOver;
    }

    private float GetCompositeParentsScale(Transform t)
    {
        if(t.GetComponent<Canvas>())
        {
            return 1;
        }
        return t.localScale.x * GetCompositeParentsScale(t.parent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClick != default)
            OnClick();
        display.SetActive(false);
    }
}
