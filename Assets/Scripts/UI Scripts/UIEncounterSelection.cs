using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEncounterSelection : MonoBehaviour, IPointerClickHandler, IPointerExitHandler //IPointerEnterHandler
{
    public GameObject eventDisplay;
    public GameObject monsterDisplay;
    public Vector3 normalScale;
    public Vector3 zoomScale;
    public Tooltip[] tooltips;

    private void Start()
    {
        for (int i = 0; i < tooltips.Length; i++)
        {
            tooltips[i].OnClick = OnClick;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnClick()
    {
        if (eventDisplay.transform.localScale != zoomScale)
        {
            eventDisplay.transform.localScale = zoomScale;
            monsterDisplay.transform.localScale = zoomScale;
        }
        else
            HideSelection();
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    eventDisplay.transform.localScale = zoomScale;
    //    monsterDisplay.transform.localScale = zoomScale;
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(Close());
    }

    private void OnDisable()
    {
        HideSelection();
    }

    public void HideSelection()
    {
        eventDisplay.transform.localScale = normalScale;
        monsterDisplay.transform.localScale = normalScale;
    }

    IEnumerator Close()
    {
        yield return new WaitForEndOfFrame();
        if (!TooltipOpen())
            HideSelection();
    }

    private bool TooltipOpen()
    {
        for(int i = 0; i < tooltips.Length; i++)
        {
            if (tooltips[i].IsHoveringOver())
                return true;
        }
        return false;
    }
}