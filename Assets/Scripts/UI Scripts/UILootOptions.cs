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

        if (PlayManager.Instance.GetHealth(PlayManager.Instance.localPlayer) <= 0)
        {
            useButton.SetActive(false);
            discardButton.SetActive(false);
            sellButton.SetActive(false);
            return;
        }
        else
        {
            useButton.SetActive(true);
            discardButton.SetActive(true);
            sellButton.SetActive(true);
        }

        // If card is a Consumable, first option is "Use"
        if (l.GetType() == typeof(ConsumableCard))
        {
            useButton.GetComponentInChildren<TMP_Text>().text = useOption;

            // If forced maximize is active, disable using cards
            if(InventoryManager.Instance.forcedMaximize)
            {
                useButton.GetComponent<Button>().enabled = false;
                useButton.GetComponent<Image>().color = disabledColor;
            }
            // Otherwise check use condition
            else
            {
                switch((l as ConsumableCard).useCondition)
                {
                    case UseCondition.ANYTIME:
                        // If not in combat, enable
                        if (!CombatManager.Instance.InCombat())
                        {
                            useButton.GetComponent<Button>().enabled = true;
                            useButton.GetComponent<Image>().color = enabledColor;
                        }
                        // Else if in combat, and its your turn, and you haven't used and item yet, enable
                        else if (CombatManager.Instance.InCombat() && CombatManager.Instance.isYourTurn && !CombatManager.Instance.UsedItemThisTurn())
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
                        break;
                    case UseCondition.IN_COMBAT:
                        // If in combat, and its your turn, and you haven't used and item yet, enable
                        if (CombatManager.Instance.InCombat() && CombatManager.Instance.isYourTurn && !CombatManager.Instance.UsedItemThisTurn())
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
                        break;
                    case UseCondition.OUT_OF_COMBAT:
                        // If not in combat, enable
                        if(!CombatManager.Instance.InCombat())
                        {
                            useButton.GetComponent<Button>().enabled = true;
                            useButton.GetComponent<Image>().color = enabledColor;
                        }
                        // If in combat, disable
                        else
                        {
                            useButton.GetComponent<Button>().enabled = false;
                            useButton.GetComponent<Image>().color = disabledColor;
                        }
                        break;
                    case UseCondition.NEVER:
                        // Disable
                        useButton.GetComponent<Button>().enabled = false;
                        useButton.GetComponent<Image>().color = disabledColor;
                        break;
                }
            }
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
                    // If it is a ring, enable
                    if (l.GetType() == typeof(RingCard))
                    {
                        useButton.GetComponent<Button>().enabled = true;
                        useButton.GetComponent<Image>().color = enabledColor;
                    }
                    // Else if it is a weapon player can use, enable
                    else if(l.GetType() == typeof(WeaponCard) && PlayManager.Instance.CanUseWeapon(PlayManager.Instance.localPlayer, l as WeaponCard))
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

            // If there is another player in the store to trade with and this item is unequipped, enable Trading
            if(PlayManager.Instance.HasAllyInStore() && useButton.GetComponentInChildren<TMP_Text>().text != unequipOption)
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

            discardButton.SetActive(true);
        }
        // Otherwise, second option is "Discard"
        else
        {
            discardButton.GetComponentInChildren<TMP_Text>().text = discardOption;
            discardButton.GetComponent<Button>().enabled = true;
            discardButton.GetComponent<Image>().color = enabledColor;

            // If first option is "Unequip", set option 2 inactive
            discardButton.SetActive(useButton.GetComponentInChildren<TMP_Text>().text != unequipOption);
        }

        // Third option is "Sell"
        sellButton.GetComponentInChildren<TMP_Text>().text = sellOption;

        // If the first option is not unequip, enable sell button
        if (useButton.GetComponentInChildren<TMP_Text>().text != unequipOption)
        {
            sellButton.GetComponent<Button>().enabled = true;
            sellButton.GetComponent<Image>().color = enabledColor;
        }
        else
        {
            sellButton.GetComponent<Button>().enabled = false;
            sellButton.GetComponent<Image>().color = disabledColor;
        }

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
                InventoryManager.Instance.HideOptions();
                break;
            case unequipOption:
                InventoryManager.Instance.Unequip();
                InventoryManager.Instance.HideOptions();
                break;
            case discardOption:
                InventoryManager.Instance.Discard();
                InventoryManager.Instance.HideOptions();
                break;
            case tradeOption:
                InventoryManager.Instance.Trade();
                break;
            case sellOption:
                InventoryManager.Instance.Sell();
                InventoryManager.Instance.HideOptions();
                break;
        }
    }
}
