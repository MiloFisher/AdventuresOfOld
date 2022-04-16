using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILootCard : MonoBehaviour
{
    public void ClickCard(int id)
    {
        if (!InventoryManager.Instance.maximized)
            InventoryManager.Instance.MaximizeInventory();
    }
}
