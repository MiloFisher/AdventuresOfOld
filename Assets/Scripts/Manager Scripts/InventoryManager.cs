using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class InventoryManager : Singleton<InventoryManager>
{
    public GameObject[] cards;
    public GameObject minimizeButton;
    public GameObject minimizeButton1;
    public float minimizedY;
    public float maximizedY;
    public float minimizedGap;
    public float maximizedGap;
    public float auxiliaryCardY;
    public float movementTimeLength = 0.004f;
    public bool maximized;
    public bool forcedMaximize;
    public GameObject lootCardOptions;
    public GameObject forcedDiscard;
    public GameObject discardMany;

    private string[] gear = new string[9];
    private int[] gearPos = new int[9];
    private int activeCards;
    [HideInInspector] public string emptyValue = "empty";
    public bool inAnimation;
    private int selectedID = -1;

    private void Start()
    {
        SetGear(new string[] { emptyValue, emptyValue, emptyValue, emptyValue, emptyValue, emptyValue, emptyValue, emptyValue, emptyValue });
    }

    private void Update()
    {
        if(!inAnimation && GearChanged())
        {
            Player p = PlayManager.Instance.localPlayer;
            UpdateGear(new string[] { p.Weapon.Value + "", p.Armor.Value + "", p.Ring1.Value + "", p.Ring2.Value + "", p.Inventory1.Value + "", p.Inventory2.Value + "", p.Inventory3.Value + "", p.Inventory4.Value + "", p.Inventory5.Value + ""});
        }
    }

    private bool GearChanged()
    {
        Player p = PlayManager.Instance.localPlayer;
        if (p == null)
            return false;
        if (gear[0] != p.Weapon.Value)
            return true;
        if (gear[1] != p.Armor.Value)
            return true;
        if (gear[2] != p.Ring1.Value)
            return true;
        if (gear[3] != p.Ring2.Value)
            return true;
        if (gear[4] != p.Inventory1.Value)
            return true;
        if (gear[5] != p.Inventory2.Value)
            return true;
        if (gear[6] != p.Inventory3.Value)
            return true;
        if (gear[7] != p.Inventory4.Value)
            return true;
        if (gear[8] != p.Inventory5.Value)
            return true;
        return false;
    }

    private void SetGear(string[] g)
    {
        activeCards = 0;
        for(int i = 0; i < 9; i++)
        {
            gear[i] = g[i];
            if (g[i] != emptyValue)
            {
                if (gearPos[i] > -1)
                {
                    cards[i].SetActive(true);
                    SetAlpha(cards[i], 1);
                }
                gearPos[i] = activeCards;
                activeCards++;
            }
            else
            {
                if (gearPos[i] > -1)
                {
                    cards[i].SetActive(false);
                    SetAlpha(cards[i], 0);
                }
                gearPos[i] = -1;
            }
        }
    }

    public void UpdateGear(string[] newGear)
    {
        int newActiveCards = 0;
        int[] newGearPos = new int[9];
        for (int i = 0; i < 9; i++)
        {
            if (newGear[i] != emptyValue)
            {
                cards[i].GetComponent<UILootCard>().SetVisuals(newGear[i]);
                newGearPos[i] = newActiveCards;
                newActiveCards++;
                if(gearPos[i] == -1)
                {
                    cards[i].SetActive(true);
                    SetAlpha(cards[i], 0);
                }
            }
            else
            {
                newGearPos[i] = -1;
            }
        }

        StartCoroutine(AnimateCorrectionalMovement(newActiveCards, newGearPos, newGear));
    }

    public bool AddDrawnCardToInventory(GameObject cardToAdd)
    {
        int emptyCount = 0;
        bool[] emptySpaces = new bool[5];
        for(int i = 0; i < 5; i++)
        {
            if(gear[i + 4] == emptyValue)
            {
                emptySpaces[i] = true;
                emptyCount++;
            }
        }
        if (emptyCount == 0)
            return false;

        for (int i = 0; i < 5; i++)
        {
            if (emptySpaces[i])
            {
                PlayManager.Instance.localPlayer.SetValue("Inventory" + (i + 1), cardToAdd.GetComponent<UILootCard>().cardName);
                StartCoroutine(AnimateAddDrawnCard(i + 4, cardToAdd));
                break;
            }
        }

        return true;
    }

    IEnumerator AnimateAddDrawnCard(int id, GameObject card)
    {
        inAnimation = true;

        string[] newGear = new string[9];
        gear.CopyTo(newGear, 0);
        newGear[id] = card.GetComponent<UILootCard>().cardName;

        int newActiveCards = 0;
        int[] newGearPos = new int[9];
        for (int i = 0; i < 9; i++)
        {
            if (newGear[i] != emptyValue)
            {
                cards[i].GetComponent<UILootCard>().SetVisuals(newGear[i]);
                newGearPos[i] = newActiveCards;
                newActiveCards++;
            }
            else
            {
                newGearPos[i] = -1;
            }
        }
        cards[id].SetActive(true);
        SetAlpha(cards[id], 1);

        float cardStartX = card.transform.localPosition.x;
        float cardStartY = card.transform.localPosition.y - transform.localPosition.y;

        float gap = maximized ? maximizedGap : minimizedGap;
        float y = maximized ? maximizedY : minimizedY;
        // Animate moving to new positions for all cards
        for (int i = 1; i <= Global.animSteps; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                float startPos = gap * (2 * gearPos[j] + 1 - activeCards);
                float endPos = gap * (2 * newGearPos[j] + 1 - newActiveCards);
                // Move from one position to another
                if (gearPos[j] > -1 && newGearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos + (endPos - startPos) * i * Global.animRate, y, 0);
                }
                // Insert drawn card
                else if (gearPos[j] == -1 && newGearPos[j] > -1)
                {
                    float distX = endPos - cardStartX;
                    float distY = y - cardStartY;
                    float yVal = distY * Global.animRate - 1;
                    float xVal = Mathf.Log10(1 + Mathf.Abs(yVal));
                    cards[j].transform.localPosition = new Vector3(cardStartX + Mathf.Log10(1 + Mathf.Abs(yVal * i * Global.animRate)) / xVal * distX, cardStartY + (y - cardStartY) * i * Global.animRate, 0);
                }
            }
            yield return new WaitForSeconds(movementTimeLength * Global.animTimeMod * Global.animSpeed);
        }

        SetGear(newGear);
        inAnimation = false;
    }

    public void MaximizeInventory()
    {
        if (inAnimation || maximized)
            return;
        maximized = true;
        minimizeButton.SetActive(true);
        minimizeButton1.SetActive(true);
        StartCoroutine(AnimateMinMaxMovement());
    }

    public void MinimizeInventory()
    {
        if (inAnimation || !maximized || forcedMaximize)
            return;
        maximized = false;
        minimizeButton.SetActive(false);
        minimizeButton1.SetActive(false);
        StartCoroutine(AnimateMinMaxMovement());
    }

    public void ShowOptions(int id)
    {
        selectedID = id;
        lootCardOptions.GetComponent<UILootOptions>().Display(cards[id].GetComponent<UILootCard>(), id);
        lootCardOptions.transform.localPosition = cards[id].transform.localPosition;
    }

    public void HideOptions()
    {
        if(selectedID != -1)
        {
            lootCardOptions.SetActive(false);
            cards[selectedID].GetComponentInChildren<UILootSelection>().HideSelection();
            selectedID = -1;
        }
    }

    public bool IsShowingOptions()
    {
        return lootCardOptions.activeInHierarchy;
    }

    IEnumerator AnimateCorrectionalMovement(int newActiveCards, int[] newGearPos, string[] newGear)
    {
        inAnimation = true;
        float gap = maximized ? maximizedGap : minimizedGap;
        float y = maximized ? maximizedY : minimizedY;
        float auxHeight = auxiliaryCardY + y;
        // Animate moving to new positions for all cards
        for(int i = 1; i <= Global.animSteps; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                float startPos = gap * (2 * gearPos[j] + 1 - activeCards);
                float endPos = gap * (2 * newGearPos[j] + 1 - newActiveCards);
                // Move from one position to another
                if (gearPos[j] > -1 && newGearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos + (endPos - startPos) * i * Global.animRate, y, 0);
                }
                // Insert new card
                else if (gearPos[j] == -1 && newGearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(endPos, auxHeight + (y - auxHeight) * i * Global.animRate, 0);
                    SetAlpha(cards[j], i * Global.animRate);
                }
                // Remove old card
                else if (gearPos[j] > -1 && newGearPos[j] == -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos, y + (auxHeight - y) * i * Global.animRate, 0);
                    SetAlpha(cards[j], (Global.animSteps - i) * Global.animRate);
                }
            }
            yield return new WaitForSeconds(movementTimeLength * Global.animTimeMod * Global.animSpeed);
        }

        SetGear(newGear);
        inAnimation = false;
    }

    IEnumerator AnimateMinMaxMovement()
    {
        inAnimation = true;
        float startGap = maximized ? minimizedGap : maximizedGap;
        float endGap = maximized ? maximizedGap : minimizedGap;
        float startY = maximized ? minimizedY : maximizedY;
        float endY = maximized ? maximizedY : minimizedY;
        // Animate moving to new positions for all cards
        for (int i = 1; i <= Global.animSteps; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                float startPos = startGap * (2 * gearPos[j] + 1 - activeCards);
                float endPos = endGap * (2 * gearPos[j] + 1 - activeCards);
                // Move from one position to another
                if (gearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos + (endPos - startPos) * i * Global.animRate, startY + (endY - startY) * i * Global.animRate, 0);
                }
            }
            yield return new WaitForSeconds(movementTimeLength * Global.animTimeMod * Global.animSpeed);
        }
        inAnimation = false;
    }

    private void SetAlpha(GameObject card, float a)
    {
        // Set transparency of all card features here
        Image[] images = card.GetComponentsInChildren<Image>();
        TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
        foreach (Image img in images)
        {
            if(!img.GetComponent<Button>())
                img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        }
        foreach (TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }

    public bool InventoryIsFull(Player p)
    {
        return p.Inventory1.Value != emptyValue && p.Inventory2.Value != emptyValue && p.Inventory3.Value != emptyValue && p.Inventory4.Value != emptyValue && p.Inventory5.Value != emptyValue;
    }

    public bool HasCardInInventory(Player p)
    {
        return !(p.Inventory1.Value == emptyValue && p.Inventory2.Value == emptyValue && p.Inventory3.Value == emptyValue && p.Inventory4.Value == emptyValue && p.Inventory5.Value == emptyValue);
    }

    public bool HasSpaceInventory(Player p)
    {
        return p.Inventory1.Value == emptyValue || p.Inventory2.Value == emptyValue || p.Inventory3.Value == emptyValue || p.Inventory4.Value == emptyValue || p.Inventory5.Value == emptyValue;
    }

    public string[] GetGear()
    {
        return gear;
    }

    public void SetFirstEmptySlotInInventory(Player p, string equipment)
    {
        if (p.Inventory1.Value == emptyValue)
        {
            p.SetValue("Inventory1", equipment);
        }
        else if (p.Inventory2.Value == emptyValue)
        {
            p.SetValue("Inventory2", equipment);
        }
        else if (p.Inventory3.Value == emptyValue)
        {
            p.SetValue("Inventory3", equipment);
        }
        else if (p.Inventory4.Value == emptyValue)
        {
            p.SetValue("Inventory4", equipment);
        }
        else if (p.Inventory5.Value == emptyValue)
        {
            p.SetValue("Inventory5", equipment);
        }
    }

    public void Use(string specificCard = default)
    {
        if(specificCard != default)
        {
            for(int i = 0; i < cards.Length; i++)
            {
                if (cards[i].GetComponent<UILootCard>().cardName == specificCard)
                    selectedID = i;
            }
            if (selectedID == -1)
            {
                Debug.Log(specificCard + " not found in inventory!");
                return;
            }
        }
        string cardName = cards[selectedID].GetComponent<UILootCard>().cardName;
        ConsumableCard c = PlayManager.Instance.itemReference[cardName] as ConsumableCard;
        c.UseEffect();
    }

    public void Equip()
    {
        string cardName = cards[selectedID].GetComponent<UILootCard>().cardName;
        LootCard l = PlayManager.Instance.itemReference[cardName];
        Player p = PlayManager.Instance.localPlayer;
        if(l.GetType() == typeof(WeaponCard))
        {
            string weapon = p.Weapon.Value + "";
            p.SetValue("Weapon", cardName);
            p.SetValue("Inventory" + (selectedID - 3), weapon);
        }
        else if (l.GetType() == typeof(ArmorCard))
        {
            string armor = p.Armor.Value + "";
            int eng = (l as ArmorCard).energy;
            p.SetValue("Armor", cardName);
            p.SetValue("Inventory" + (selectedID - 3), armor);
            if(p.Armor.Value == emptyValue)
            {
                p.SetValue("AbilityCharges", PlayManager.Instance.GetAbilityCharges(p) + eng / 2);
            }
            else
            {
                string armor1 = p.Armor.Value + "";
                int eng1 = (PlayManager.Instance.itemReference[armor1] as ArmorCard).energy;
                int newAbilityCharge = PlayManager.Instance.GetAbilityCharges(p) + (eng - eng1) / 2;
                p.SetValue("AbilityCharges", newAbilityCharge > 0 ? newAbilityCharge : 0);
            }
        }
        else if (l.GetType() == typeof(RingCard))
        {
            int con1 = (l as RingCard).constitution;
            int eng1 = (l as RingCard).energy;
            if (p.Ring1.Value == emptyValue)
            {
                string ring1 = p.Ring1.Value + "";
                p.SetValue("Ring1", cardName);
                p.SetValue("Inventory" + (selectedID - 3), ring1);
                p.SetValue("Health", PlayManager.Instance.GetHealth(p) + con1 * 2);
                p.SetValue("AbilityCharges", PlayManager.Instance.GetAbilityCharges(p) + eng1 / 2);
            }
            else if (p.Ring2.Value == emptyValue)
            {
                string ring2 = p.Ring2.Value + "";
                p.SetValue("Ring2", cardName);
                p.SetValue("Inventory" + (selectedID - 3), ring2);
                p.SetValue("Health", PlayManager.Instance.GetHealth(p) + con1 * 2);
                p.SetValue("AbilityCharges", PlayManager.Instance.GetAbilityCharges(p) + eng1 / 2);
            }
            else
            {
                string ring1 = p.Ring1.Value + "";
                p.SetValue("Ring1", p.Ring2.Value + "");
                p.SetValue("Ring2", cardName);
                p.SetValue("Inventory" + (selectedID - 3), ring1);
                int con2 = (PlayManager.Instance.itemReference[ring1] as RingCard).constitution;
                int newHealth = PlayManager.Instance.GetHealth(p) + (con1 - con2) * 2;
                p.SetValue("Health", newHealth > 0 ? newHealth : 1);
                int eng2 = (PlayManager.Instance.itemReference[ring1] as RingCard).energy;
                int newAbilityCharge = PlayManager.Instance.GetAbilityCharges(p) + (eng1 - eng2) / 2;
                p.SetValue("AbilityCharges", newAbilityCharge > 0 ? newAbilityCharge : 0);
            }
        }
    }

    public void Unequip()
    {
        Player p = PlayManager.Instance.localPlayer;
        string equipment = cards[selectedID].GetComponent<UILootCard>().cardName;
        string location = "";
        int con = 0;
        int eng = 0;
        switch (selectedID)
        {
            case 0:
                location = "Weapon";
                break;
            case 1:
                location = "Armor";
                eng = (PlayManager.Instance.itemReference[equipment] as ArmorCard).energy;
                break;
            case 2:
                location = "Ring1";
                con = (PlayManager.Instance.itemReference[equipment] as RingCard).constitution;
                eng = (PlayManager.Instance.itemReference[equipment] as RingCard).energy;
                break;
            case 3:
                location = "Ring2";
                con = (PlayManager.Instance.itemReference[equipment] as RingCard).constitution;
                eng = (PlayManager.Instance.itemReference[equipment] as RingCard).energy;
                break;
        }
        if (p.Inventory1.Value == emptyValue)
        {
            p.SetValue(location, emptyValue);
            p.SetValue("Inventory1", equipment);
        }
        else if (p.Inventory2.Value == emptyValue)
        {
            p.SetValue(location, emptyValue);
            p.SetValue("Inventory2", equipment);
        }
        else if (p.Inventory3.Value == emptyValue)
        {
            p.SetValue(location, emptyValue);
            p.SetValue("Inventory3", equipment);
        }
        else if (p.Inventory4.Value == emptyValue)
        {
            p.SetValue(location, emptyValue);
            p.SetValue("Inventory4", equipment);
        }
        else if (p.Inventory5.Value == emptyValue)
        {
            p.SetValue(location, emptyValue);
            p.SetValue("Inventory5", equipment);
        }
        int newHealth = PlayManager.Instance.GetHealth(p) - con * 2;
        p.SetValue("Health", newHealth > 0 ? newHealth : 1);
        int newAbilityCharge = PlayManager.Instance.GetAbilityCharges(p) - eng / 2;
        p.SetValue("AbilityCharges", newAbilityCharge > 0 ? newAbilityCharge : 0);
    }

    public void Discard()
    {
        Player p = PlayManager.Instance.localPlayer;
        switch (selectedID)
        {
            case 0:
                p.SetValue("Weapon", emptyValue);
                break;
            case 1:
                p.SetValue("Armor", emptyValue);
                break;
            case 2:
                p.SetValue("Ring1", emptyValue);
                break;
            case 3:
                p.SetValue("Ring2", emptyValue);
                break;
            case 4:
                p.SetValue("Inventory1", emptyValue);
                break;
            case 5:
                p.SetValue("Inventory2", emptyValue);
                break;
            case 6:
                p.SetValue("Inventory3", emptyValue);
                break;
            case 7:
                p.SetValue("Inventory4", emptyValue);
                break;
            case 8:
                p.SetValue("Inventory5", emptyValue);
                break;
            default:
                Debug.Log("Invalid discard selectID: " + selectedID);
                break;
        }
        if (forcedDiscard.activeInHierarchy)
            forcedDiscard.GetComponent<UIForcedDiscard>().DiscardComplete();
        if (discardMany.activeInHierarchy)
            discardMany.GetComponent<UIDiscardManyCards>().CardDiscarded();
    }

    public void Trade()
    {
        PlayManager.Instance.TargetPlayerSelection("Choose Trade Partner", true, false, false, (p) => {
            // Trade to player p
            SetFirstEmptySlotInInventory(p, cards[selectedID].GetComponent<UILootCard>().cardName);
            Discard();
        }, (p) => {
            // Requirement is being in the store and having an empty slot in inventory
            return PlayManager.Instance.WentToStore(p) && HasSpaceInventory(p);
        }, true, "Cancel", () => {
            HideOptions();
        });
    }

    public void Sell()
    {
        string cardName = cards[selectedID].GetComponent<UILootCard>().cardName;
        LootCard l = PlayManager.Instance.itemReference[cardName];
        Player p = PlayManager.Instance.localPlayer;

        p.GainGold(l.sellCost);

        switch (selectedID)
        {
            case 0:
                p.SetValue("Weapon", emptyValue);
                break;
            case 1:
                p.SetValue("Armor", emptyValue);
                break;
            case 2:
                p.SetValue("Ring1", emptyValue);
                break;
            case 3:
                p.SetValue("Ring2", emptyValue);
                break;
            case 4:
                p.SetValue("Inventory1", emptyValue);
                break;
            case 5:
                p.SetValue("Inventory2", emptyValue);
                break;
            case 6:
                p.SetValue("Inventory3", emptyValue);
                break;
            case 7:
                p.SetValue("Inventory4", emptyValue);
                break;
            case 8:
                p.SetValue("Inventory5", emptyValue);
                break;
        }
    }
}
