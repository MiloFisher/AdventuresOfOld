using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JL_UI_Manager : Selectable
{
    BaseEventData m_BaseEvent;

    void Update()
    {
        //Check if the GameObject is being highlighted
        if (IsHighlighted())
        {
            Transform daggerTransform = this.gameObject.transform.GetChild(1);
            GameObject dagger = daggerTransform.gameObject;
            dagger.SetActive(true);
        }

        if (!IsHighlighted())
        {
            Transform daggerTransform = this.gameObject.transform.GetChild(1);
            GameObject dagger = daggerTransform.gameObject;
            dagger.SetActive(false);
        }
    }
}
