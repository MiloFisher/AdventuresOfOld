using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject display;

    private void Start()
    {
        display.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        display.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        display.SetActive(false);
    }

    private void OnDisable()
    {
        display.SetActive(false);
    }
}
