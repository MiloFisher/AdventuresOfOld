using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILootCard : MonoBehaviour
{
    [Header("General Components")]
    public string cardName;
    public int slot;
    public GameObject collectCardButton;
    public GameObject cardButton;
    public GameObject cardBack;
    public Image displayRarityBack;
    public Image displayImage;
    public TMP_Text displayName;
    public Color[] rarityColors;
    public TMP_Text rarity;
    public TMP_Text buyCost;
    public TMP_Text sellCost;
    public GameObject zoomSelector;

    [Header("Weapon Components")]
    public GameObject weaponComponents;
    public TMP_Text weaponEffect;
    public TMP_Text weaponAttackType;
    public TMP_Text weaponType;

    [Header("Other Components")]
    public GameObject lootComponents;
    public TMP_Text lootEffect;
    public TMP_Text lootType;

    public float fadeLength = 0.004f;

    private bool collectButtonActive;

    public void ClickCard()
    {
        if (!InventoryManager.Instance.maximized)
            InventoryManager.Instance.MaximizeInventory();
    }

    public void AddCardToHand()
    {
        if(collectButtonActive)
            LootManager.Instance.AddCardToInventory(gameObject);
    }

    public void SetVisuals(string card)
    {
        cardName = card;
        if (!PlayManager.Instance.itemReference.ContainsKey(card))
            return;
        LootCard loot = PlayManager.Instance.itemReference[card];
        if (loot.GetType() == typeof(WeaponCard))
        {
            WeaponCard w = loot as WeaponCard;
            weaponComponents.SetActive(true);
            lootComponents.SetActive(false);
            weaponEffect.text = w.effectDescription;
            weaponAttackType.text = w.attackType;
            weaponType.text = w.itemType;
        }
        else
        {
            weaponComponents.SetActive(false);
            lootComponents.SetActive(true);
            lootEffect.text = loot.effectDescription;
            lootType.text = loot.itemType;
        }
        switch(loot.rarity)
        {
            case Rarity.COMMON:
                rarity.text = "Common";
                rarity.color = rarityColors[0];
                displayRarityBack.color = rarityColors[0];
                break;
            case Rarity.UNCOMMON:
                rarity.text = "Uncommon";
                rarity.color = rarityColors[1];
                displayRarityBack.color = rarityColors[1];
                break;
            case Rarity.RARE:
                rarity.text = "Rare";
                rarity.color = rarityColors[2];
                displayRarityBack.color = rarityColors[2];
                break;
            case Rarity.EPIC:
                rarity.text = "Epic";
                rarity.color = rarityColors[3];
                displayRarityBack.color = rarityColors[3];
                break;
            case Rarity.LEGENDARY:
                rarity.text = "Legendary";
                rarity.color = rarityColors[4];
                displayRarityBack.color = rarityColors[4];
                break;
        }
        displayImage.sprite = loot.image;
        displayName.text = loot.cardName;
        buyCost.text = loot.buyCost + "";
        sellCost.text = loot.sellCost + "";
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
    }

    public void ActivateZoomSelector(bool active)
    {
        zoomSelector.SetActive(active);
    }

    public void ActivateCollectCardButton(bool active)
    {
        if (active)
            StartCoroutine(FadeInButton());
        else
            StartCoroutine(FadeOutButton());
    }

    IEnumerator FadeInButton()
    {
        collectCardButton.SetActive(true);

        for(int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(collectCardButton, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        collectButtonActive = true;
    }

    IEnumerator FadeOutButton()
    {
        collectButtonActive = false;

        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(collectCardButton, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        collectCardButton.SetActive(false);
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
