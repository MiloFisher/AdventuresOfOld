using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILootSelection : MonoBehaviour, IPointerEnterHandler
{
    public GameObject display;
    public Vector3 displayPosition;
    public Vector3 displayScale;

    private GameObject copy;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled || !InventoryManager.Instance.maximized || InventoryManager.Instance.inAnimation)
            return;

        JLAudioManager.Instance.PlaySound("PageTurn");
        copy = Instantiate(display, display.transform.parent);
        copy.transform.localScale = displayScale;
        copy.transform.localPosition = displayPosition;
        InventoryManager.Instance.ShowOptions(GetComponentInParent<UILootCard>().slot);
    }

    private void OnDisable()
    {
        InventoryManager.Instance.HideOptions();
    }

    public void HideSelection()
    {
        if (copy)
            Destroy(copy);
    }
}
