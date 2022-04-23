using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILootOptions : MonoBehaviour
{
    public GameObject useButton;
    public GameObject discardButton;
    public GameObject sellButton;
    public Color enabledColor;
    public Color disabledColor;
    public float fadeLength = 0.004f;

    private const string useOption = "Use";
    private const string equipOption = "Equip";
    private const string unequipOption = "Unequip";
    private const string discardOption = "Discard";
    private const string tradeOption = "Trade";
    private const string sellOption = "Sell";

    public void Display(UILootCard card, int slot)
    {
        gameObject.SetActive(true);
        LootCard l = PlayManager.Instance.itemReference[card.cardName];

        // If card is a Consumable, first option is "Use"
        if(l.GetType() == typeof(ConsumableCard))
        {
            useButton.GetComponentInChildren<TMP_Text>().text = useOption;
            useButton.GetComponent<Button>().enabled = true;
            useButton.GetComponent<Image>().color = enabledColor;
        }
        // If card is a Weapon, Ring, or Armor, first option is either "Equip" or "Unequip"
        else
        {
            // If the slot is in the last 5 positions, it is in the inventory and should display "Equip"
            if(slot > 3)
                useButton.GetComponentInChildren<TMP_Text>().text = equipOption;
            // If the slot is in the first 4 positions, it is equipped and should display "Unequip"
            else
                useButton.GetComponentInChildren<TMP_Text>().text = unequipOption;

            // If it is the start or end of the day, enable Equipping and Unequipping
            if(PlayManager.Instance.IsStartOrEndOfDay())
            {
                // If equip is selected, check to see if can use item
                if(slot > 3)
                {
                    // If it is a weapon player can use, enable
                    if(l.GetType() == typeof(WeaponCard) && PlayManager.Instance.CanUseWeapon(PlayManager.Instance.localPlayer, l as WeaponCard))
                    {
                        useButton.GetComponent<Button>().enabled = true;
                        useButton.GetComponent<Image>().color = enabledColor;
                    }
                    // Else if it is armor the player can use, enable
                    else if (l.GetType() == typeof(ArmorCard) && PlayManager.Instance.CanUseArmor(PlayManager.Instance.localPlayer, l as ArmorCard))
                    {
                        useButton.GetComponent<Button>().enabled = true;
                        useButton.GetComponent<Image>().color = enabledColor;
                    }
                    // Otherwise disable
                    else
                    {
                        useButton.GetComponent<Button>().enabled = false;
                        useButton.GetComponent<Image>().color = disabledColor;
                    }
                }
                // If unequip is selected, check to see if there is space in inventory
                else
                {
                    // If there is space, enable
                    if(InventoryManager.Instance.HasSpaceInventory(PlayManager.Instance.localPlayer))
                    {
                        useButton.GetComponent<Button>().enabled = true;
                        useButton.GetComponent<Image>().color = enabledColor;
                    }
                    // Otherwise, disable
                    else
                    {
                        useButton.GetComponent<Button>().enabled = false;
                        useButton.GetComponent<Image>().color = disabledColor;
                    }
                }
            }
            // Otherwise, disable Equipping and Unequipping
            else
            {
                useButton.GetComponent<Button>().enabled = false;
                useButton.GetComponent<Image>().color = disabledColor;
            }
        }

        // If in the store, second option is "Trade"
        if (PlayManager.Instance.IsInStore())
        {
            discardButton.GetComponentInChildren<TMP_Text>().text = tradeOption;

            // If there is another player in the store to trade with, enable Trading
            if(PlayManager.Instance.HasAllyInStore())
            {
                discardButton.GetComponent<Button>().enabled = true;
                discardButton.GetComponent<Image>().color = enabledColor;
            }
            // Otherwise, disable trading
            else
            {
                discardButton.GetComponent<Button>().enabled = false;
                discardButton.GetComponent<Image>().color = disabledColor;
            }
        }
        // Otherwise, second option is "Discard"
        else
        {
            discardButton.GetComponentInChildren<TMP_Text>().text = discardOption;
            discardButton.GetComponent<Button>().enabled = true;
            discardButton.GetComponent<Image>().color = enabledColor;
        }

        // Third option is "Sell"
        sellButton.GetComponentInChildren<TMP_Text>().text = sellOption;
        sellButton.GetComponent<Button>().enabled = true;
        sellButton.GetComponent<Image>().color = enabledColor;

        // If in store, set sell button active
        sellButton.SetActive(PlayManager.Instance.IsInStore());
    }

    public void ChooseOption(int id)
    {
        string option;
        if(id == 0)
           option = useButton.GetComponentInChildren<TMP_Text>().text;
        else if (id == 1)
            option = discardButton.GetComponentInChildren<TMP_Text>().text;
        else
            option = sellButton.GetComponentInChildren<TMP_Text>().text;

        switch(option)
        {
            case useOption:
                InventoryManager.Instance.Use();
                break;
            case equipOption:
                InventoryManager.Instance.Equip();
                break;
            case unequipOption:
                InventoryManager.Instance.Unequip();
                break;
            case discardOption:
                InventoryManager.Instance.Discard();
                break;
            case tradeOption:
                InventoryManager.Instance.Trade();
                break;
            case sellOption:
                InventoryManager.Instance.Sell();
                break;
        }
        InventoryManager.Instance.HideOptions();
    }
}
