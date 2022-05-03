using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerSelection : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public GameObject playerDisplay;
    public Vector3 normalScale;
    public Vector3 zoomScale;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerDisplay.transform.localScale != zoomScale)
            playerDisplay.transform.localScale = zoomScale;
        else
            HideSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideSelection();
    }

    private void OnDisable()
    {
        HideSelection();
    }

    public void HideSelection()
    {
        playerDisplay.transform.localScale = normalScale;
    }
}