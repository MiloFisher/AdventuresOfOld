using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEncounterSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject eventDisplay;
    public GameObject monsterDisplay;
    public Vector3 normalScale;
    public Vector3 zoomScale;

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventDisplay.transform.localScale = zoomScale;
        monsterDisplay.transform.localScale = zoomScale;
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
        eventDisplay.transform.localScale = normalScale;
        monsterDisplay.transform.localScale = normalScale;
    }
}