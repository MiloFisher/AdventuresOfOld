using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILootZoom : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public GameObject display;
    public Vector3 zoomScale;
    private GameObject copy;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (display.transform.localScale != zoomScale)
        {
            copy = Instantiate(display, transform.parent.parent.parent);
            copy.transform.localPosition = display.transform.parent.localPosition;
            copy.transform.localScale = zoomScale;
        }
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
        if (display.transform.localScale == zoomScale)
            Destroy(display);
    }
}
