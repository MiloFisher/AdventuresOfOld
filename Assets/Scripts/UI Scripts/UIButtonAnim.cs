using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public float scaledWidth;
    public float scaledHeight;
    public float childScale;
    public bool allowClickOnEmptySpace = false;
    public bool disableHoverSound = false;
    public bool disableClickSound = false;

    private Image image;
    private RectTransform rt;
    private float defaultWidth;
    private float defaultHeight;
    private float defaultChildScale = 1;
    private string clickSound = "ClickSound";
    private string hoverSound = "PageTurn";

    void Start()
    {
        image = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        defaultWidth = rt.sizeDelta.x;
        defaultHeight = rt.sizeDelta.y;
        if(!allowClickOnEmptySpace)
            image.alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!disableClickSound)
            JLAudioManager.Instance.PlayOneShotSound(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled)
            return;
        if(!disableHoverSound)
            JLAudioManager.Instance.PlaySound(hoverSound);
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
