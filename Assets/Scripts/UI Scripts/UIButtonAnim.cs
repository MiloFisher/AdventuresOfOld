using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public float scaledWidth;
    public float scaledHeight;
    public float childScale;

    private Image image;
    private RectTransform rt;
    private float defaultWidth;
    private float defaultHeight;
    private float defaultChildScale;

    void Start()
    {
        image = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        defaultWidth = rt.sizeDelta.x;
        defaultHeight = rt.sizeDelta.y;
        image.alphaHitTestMinimumThreshold = 0.1f;
        if(transform.childCount > 0)
            defaultChildScale = 1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled)
            return;
        image.sprite = selectedSprite;
        rt.sizeDelta = new Vector2(scaledWidth,scaledHeight);
        if (transform.childCount > 0)
            transform.GetChild(0).localScale = new Vector3(childScale, childScale, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;
        rt.sizeDelta = new Vector2(defaultWidth, defaultHeight);
        if (transform.childCount > 0)
            transform.GetChild(0).localScale = new Vector3(defaultChildScale, defaultChildScale, 1);
    }

    private void OnDisable()
    {
        if(image)
            image.sprite = defaultSprite;
        if(rt)
            rt.sizeDelta = new Vector2(defaultWidth, defaultHeight);
        if (transform.childCount > 0)
            transform.GetChild(0).localScale = new Vector3(defaultChildScale, defaultChildScale, 1);
    }
}
