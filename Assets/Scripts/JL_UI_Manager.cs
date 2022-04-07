using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JL_UI_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(false);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(false);
    }
}
