using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILootSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject display;
    public Vector3 displayPosition;
    public float displayScale;

    private GameObject copy;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled || !InventoryManager.Instance.maximized)
            return;

        copy = Instantiate(display, display.transform.parent);
        copy.transform.localScale = new Vector3(displayScale, displayScale, 1);
        copy.transform.localPosition = displayPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled)
            return;
        if (copy)
            Destroy(copy);
    }

    private void OnDisable()
    {
        if (copy)
            Destroy(copy);
    }
}
