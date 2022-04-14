using UnityEngine;
using UnityEngine.EventSystems;

public class UIChaosCounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject description;
    public bool isDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isDescription)
            description.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isDescription)
            description.SetActive(false);
    }
}
