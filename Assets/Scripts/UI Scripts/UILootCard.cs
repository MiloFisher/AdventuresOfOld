using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILootCard : MonoBehaviour
{
    public GameObject cardButton;
    public GameObject cardBack;

    public void ClickCard(int id)
    {
        if (!InventoryManager.Instance.maximized)
            InventoryManager.Instance.MaximizeInventory();
    }

    public void SetVisuals(string cardName)
    {

    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
    }
}
