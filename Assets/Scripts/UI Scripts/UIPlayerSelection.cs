using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject playerDisplay;
    public Vector3 normalScale;
    public Vector3 zoomScale;

    public void OnPointerEnter(PointerEventData eventData)
    {
        playerDisplay.transform.localScale = zoomScale;
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