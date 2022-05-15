using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerSelection : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public GameObject playerDisplay;
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
        if (playerDisplay.transform.localScale != zoomScale)
            playerDisplay.transform.localScale = zoomScale;
        else
            HideSelection();
    }

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
        playerDisplay.transform.localScale = normalScale;
    }

    IEnumerator Close()
    {
        yield return new WaitForEndOfFrame();
        if (!TooltipOpen())
            HideSelection();
    }

    private bool TooltipOpen()
    {
        for (int i = 0; i < tooltips.Length; i++)
        {
            if (tooltips[i].IsHoveringOver())
                return true;
        }
        return false;
    }
}