using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LootManager : Singleton<LootManager>
{
    public GameObject cardPrefab;
    public float startX = -1300;
    public float startY = -545.8f;
    public float endX = 0;
    public float endY = 0;
    public float gap = 150;
    public float travelLength = 0.004f;
    public GameObject lootBanner;
    public TMP_Text lootBannerText;
    public float bannerStartWidth;
    public float bannerEndWidth;
    public float bannerConstHeight;
    public float bannerStartScale;
    public float bannerEndScale;
    public float bannerGrowingLength = 0.004f;
    public float bannerOpeningLength = 0.004f;
    public float fadeLength = 0.004f;
    public List<string> cardsToDraw = new List<string>();
    public GameObject discardCardsButton;
    public GameObject leaveStoreButton;
    public GameObject goldDisplay;
    public bool treasureTile;

    private List<GameObject> displayCards = new List<GameObject>();
    private bool endTurnAfter;
    private bool forStore;

    public void AddLootCardToDraw(string cardName)
    {
        cardsToDraw.Add(cardName);
    }

    public void DrawCard(int amount, bool _endTurnAfter, bool _forStore)
    {
        if(amount > 0 && !lootBanner.activeInHierarchy)
        {
            endTurnAfter = _endTurnAfter;
            forStore = _forStore;
            PlayManager.Instance.inStore = forStore;
            StartCoroutine(AnimateOpening(amount));
        }
    }

    public void AddCardToInventory(GameObject card)
    {
        if (InventoryManager.Instance.AddDrawnCardToInventory(card))
        {
            Debug.Log("Successfully added Card to Inventory!");
            displayCards.Remove(card);
            Destroy(card);
            if(displayCards.Count == 0)
                StartCoroutine(AnimateClosing());
            else if(treasureTile)
            {
                DiscardRemaining();
                PlayManager.Instance.localPlayer.GainXP(2);
            }
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public void BuyCard(GameObject card, int cost, int slot)
    {
        if (InventoryManager.Instance.AddDrawnCardToInventory(card))
        {
            PlayManager.Instance.localPlayer.LoseGold(cost);
            PlayManager.Instance.localPlayer.RemoveStoreCardForOthers(slot);

            Debug.Log("Successfully Bought Card!");
            displayCards[slot] = null;
            Destroy(card);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public void LeaveStore()
    {
        PlayManager.Instance.inStore = false;
        leaveStoreButton.SetActive(false);
        goldDisplay.SetActive(false);
        for(int i = 0; i < displayCards.Count; i++)
        {
            if(displayCards[i] == null)
            {
                displayCards.RemoveAt(i);
                i--;
            }
        }
        StartCoroutine(FadeRemainingCards());
    }

    public void RemoveStoreCard(int slot)
    {
        StartCoroutine(FadeStoreCard(slot));
    }

    IEnumerator FadeStoreCard(int slot)
    {
        GameObject card = displayCards[slot];

        // First disable buy button
        card.GetComponent<UILootCard>().ActivateBuyCardButton(false);

        // Next fade out card
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(card, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        displayCards = null;
        Destroy(card);
    }

    public void DiscardRemaining()
    {
        discardCardsButton.SetActive(false);
        StartCoroutine(FadeRemainingCards());
    }

    IEnumerator FadeRemainingCards()
    {
        // First disable collect buttons
        foreach (GameObject g in displayCards)
        {
            if(forStore)
                g.GetComponent<UILootCard>().ActivateBuyCardButton(false);
            else
                g.GetComponent<UILootCard>().ActivateCollectCardButton(false);
        }

        // Next fade out cards
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            foreach(GameObject g in displayCards)
            {
                SetAlpha(g, i * Global.animRate);
            }
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Then destroy cards
        for (int i = 0; i < displayCards.Count; i++)
        {
            GameObject card = displayCards[i];
            displayCards.Remove(card);
            Destroy(card);
            i--;
        }

        // Start closing
        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateOpening(int amount)
    {
        // First setup banner
        lootBanner.SetActive(true);
        lootBannerText.text = treasureTile ? "Choose a Card" : "Drawn Cards";
        if (forStore)
            lootBannerText.text = "Store";
        lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth, bannerConstHeight);
        lootBanner.transform.localScale = new Vector3(bannerStartScale, bannerStartScale, 1);

        // Then grow the object
        float dif = bannerEndScale - bannerStartScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            lootBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * Global.animRate, bannerStartScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(bannerGrowingLength * Global.animTimeMod * Global.animSpeed);
        }

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Next open the scroll
        dif = bannerEndWidth - bannerStartWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * Global.animRate, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally animate cards
        StartCoroutine(AnimateCardDraw(0, amount));
    }

    IEnumerator AnimateClosing()
    {
        leaveStoreButton.SetActive(false);
        goldDisplay.SetActive(false);
        discardCardsButton.SetActive(false);

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // First close the scroll
        float dif = bannerEndWidth - bannerStartWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * Global.animRate, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength * Global.animTimeMod * Global.animSpeed);
        }

        // Then shrink the object
        dif = bannerEndScale - bannerStartScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            lootBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * Global.animRate, bannerStartScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(bannerGrowingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally deactivate banner
        lootBanner.SetActive(false);

        if (endTurnAfter)
        {
            if(treasureTile)
            {
                if(AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Treasure Hunter")))
                {
                    PlayManager.Instance.DefaultTile();
                }
                else
                {
                    PlayManager.Instance.ResetEncounterFails();
                    PlayManager.Instance.EndTurn();
                }
            }
            else
                PlayManager.Instance.EndTurn();
        }

        if (forStore)
            PlayManager.Instance.localPlayer.ReadyUp();

        treasureTile = false;
    }

    IEnumerator AnimateCardDraw(int current, int amount)
    {
        JLAudioManager.Instance.PlayOneShotSound("DrawCard");

        GameObject travelCard = Instantiate(cardPrefab, transform.parent);
        travelCard.GetComponent<UILootCard>().SetVisuals(cardsToDraw[current]);
        travelCard.GetComponent<UILootCard>().ActivateCardButton(false);
        travelCard.GetComponent<UILootCard>().ActivateCardBack(true);

        float distX = endX - startX - gap * (2 * current + 1 - amount);
        float x = distX * Global.animRate - 1;
        float distY = endY - startY;
        float y = Mathf.Log10(1 + Mathf.Abs(x));
        float scale;

        for (int i = 1; i <= Global.animSteps; i++)
        {
            // Move Position
            travelCard.transform.localPosition = new Vector3(startX + distX * i * Global.animRate, startY + Mathf.Log10(1 + Mathf.Abs(x * i * Global.animRate)) / y * distY, 0);
            // Rotate X, Y, Z
            travelCard.transform.localRotation = Quaternion.Euler(new Vector3(i <= Global.animSteps / 2 ? 22.5f * i * 2 * Global.animRate - 45 : 22.5f * (Global.animSteps / 2-(i-Global.animSteps / 2)) * 2 * Global.animRate, i <= Global.animSteps / 2 ? -90 * i * 2 * Global.animRate : 90 * (Global.animSteps / 2 - (i - Global.animSteps / 2)) * 2 * Global.animRate, i <= Global.animSteps / 2 ? 17.6f * (Global.animSteps / 2-i) * 2 * Global.animRate + 17.6f : -17.6f * (Global.animSteps-i) * 2 * Global.animRate)); //45 * i * Global.animRate - 45, -180 * i * Global.animRate, 35.2f * (Global.animSteps-i) * Global.animRate
            // Scale up
            scale = 0.25f * i * Global.animRate + 0.75f;
            travelCard.transform.localScale = new Vector3(scale, scale, 1);
            // Flip card face halfway through
            if (i == Global.animSteps / 2)
                travelCard.GetComponent<UILootCard>().ActivateCardBack(false);
            yield return new WaitForSeconds(travelLength * Global.animTimeMod * Global.animSpeed);
        }

        GameObject card = Instantiate(cardPrefab, travelCard.transform.position, Quaternion.identity, transform.parent);
        card.GetComponent<UILootCard>().SetVisuals(cardsToDraw[current], current);
        card.GetComponent<UILootCard>().ActivateCardButton(false);
        card.GetComponent<UILootCard>().ActivateZoomSelector(true);
        if(forStore)
            card.GetComponent<UILootCard>().ActivateBuyCardButton(true);
        else
            card.GetComponent<UILootCard>().ActivateCollectCardButton(true);
        displayCards.Add(card);
        Destroy(travelCard);

        if (current < amount - 1)
            StartCoroutine(AnimateCardDraw(current + 1, amount));
        else
        {
            cardsToDraw.Clear();
            if(forStore)
            {
                leaveStoreButton.SetActive(true);
                goldDisplay.SetActive(true);
                discardCardsButton.SetActive(false);
            }
            else
            {
                leaveStoreButton.SetActive(false);
                goldDisplay.SetActive(false);
                discardCardsButton.SetActive(true);
            }
        }  
    }

    private void SetAlpha(GameObject card, float a)
    {
        // Set transparency of all card features here
        Image[] images = card.GetComponentsInChildren<Image>();
        TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
        foreach (Image img in images)
        {
            if (!img.GetComponent<Button>() && !img.GetComponent<Tooltip>())
                img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        }
        foreach (TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }
}
