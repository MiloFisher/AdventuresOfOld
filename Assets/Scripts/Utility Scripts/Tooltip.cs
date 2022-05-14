using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject display;
    public float gap = 100;
    public Action OnClick = default;

    private float targetScale = 0.1f;

    private void Start()
    {
        display.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        display.SetActive(true);
        float scale = targetScale / GetCompositeParentsScale(transform);
        display.transform.localScale = new Vector3(scale, scale, 1);
        display.transform.localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * 0.5f + display.GetComponent<RectTransform>().sizeDelta.y * 0.5f * scale + gap * scale, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        display.SetActive(false);
    }

    private void OnDisable()
    {
        display.SetActive(false);
    }

    public bool IsOpen()
    {
        return display.activeInHierarchy;
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
        OnClick();
        display.SetActive(false);
    }
}
