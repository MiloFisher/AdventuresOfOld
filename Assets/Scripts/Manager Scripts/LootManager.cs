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
    public bool treasureTile;
    private List<GameObject> displayCards = new List<GameObject>();
    private bool endTurnAfter;

    public void AddLootCardToDraw(string cardName)
    {
        cardsToDraw.Add(cardName);
    }

    public void DrawCard(int amount, bool _endTurnAfter)
    {
        if(amount > 0 && !lootBanner.activeInHierarchy)
        {
            endTurnAfter = _endTurnAfter;
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
            g.GetComponent<UILootCard>().ActivateCollectCardButton(false);
        }

        // Next fade out cards
        for (int i = 99; i >= 0; i--)
        {
            foreach(GameObject g in displayCards)
            {
                SetAlpha(g, i * 0.01f);
            }
            yield return new WaitForSeconds(fadeLength);
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
        lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth, bannerConstHeight);
        lootBanner.transform.localScale = new Vector3(bannerStartScale, bannerStartScale, 1);

        // Then grow the object
        float dif = bannerEndScale - bannerStartScale;
        for (int i = 1; i <= 100; i++)
        {
            lootBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * 0.01f, bannerStartScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(bannerGrowingLength);
        }

        // Next open the scroll
        dif = bannerEndWidth - bannerStartWidth;
        for (int i = 1; i <= 100; i++)
        {
            lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * 0.01f, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength);
        }

        // Finally animate cards
        StartCoroutine(AnimateCardDraw(0, amount));
    }

    IEnumerator AnimateClosing()
    {
        discardCardsButton.SetActive(false);

        // First close the scroll
        float dif = bannerEndWidth - bannerStartWidth;
        for (int i = 99; i >= 0; i--)
        {
            lootBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * 0.01f, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength);
        }

        // Then shrink the object
        dif = bannerEndScale - bannerStartScale;
        for (int i = 99; i >= 0; i--)
        {
            lootBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * 0.01f, bannerStartScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(bannerGrowingLength);
        }

        // Finally deactivate banner
        lootBanner.SetActive(false);
        treasureTile = false;

        if (endTurnAfter)
            PlayManager.Instance.EndTurn();
    }

    IEnumerator AnimateCardDraw(int current, int amount)
    {
        GameObject travelCard = Instantiate(cardPrefab, transform.parent);
        travelCard.GetComponent<UILootCard>().SetVisuals(cardsToDraw[current]);
        travelCard.GetComponent<UILootCard>().ActivateCardButton(false);
        travelCard.GetComponent<UILootCard>().ActivateCardBack(true);

        float distX = endX - startX - gap * (2 * current + 1 - amount);
        float x = distX * 0.01f - 1;
        float distY = endY - startY;
        float y = Mathf.Log10(1 + Mathf.Abs(x));
        float scale;

        for (int i = 1; i <= 100; i++)
        {
            // Move Position
            travelCard.transform.localPosition = new Vector3(startX + distX * i * 0.01f, startY + Mathf.Log10(1 + Mathf.Abs(x * i * 0.01f)) / y * distY, 0);
            // Rotate X, Y, Z
            travelCard.transform.localRotation = Quaternion.Euler(new Vector3(i <= 50 ? 22.5f * i * 0.02f - 45 : 22.5f * (50-(i-50)) * 0.02f, i <= 50 ? -90 * i * 0.02f : 90 * (50 - (i - 50)) * 0.02f, i <= 50 ? 17.6f * (50-i) * 0.02f + 17.6f : -17.6f * (100-i) * 0.02f)); //45 * i * 0.01f - 45, -180 * i * 0.01f, 35.2f * (100-i) * 0.01f
            // Scale up
            scale = 0.25f * i * 0.01f + 0.75f;
            travelCard.transform.localScale = new Vector3(scale, scale, 1);
            // Flip card face halfway through
            if (i == 50)
                travelCard.GetComponent<UILootCard>().ActivateCardBack(false);
            yield return new WaitForSeconds(travelLength);
        }

        GameObject card = Instantiate(cardPrefab, travelCard.transform.position, Quaternion.identity, transform.parent);
        card.GetComponent<UILootCard>().SetVisuals(cardsToDraw[current]);
        card.GetComponent<UILootCard>().ActivateCardButton(false);
        card.GetComponent<UILootCard>().ActivateCollectCardButton(true);
        displayCards.Add(card);
        Destroy(travelCard);

        if (current < amount - 1)
            StartCoroutine(AnimateCardDraw(current + 1, amount));
        else
        {
            cardsToDraw.Clear();
            discardCardsButton.SetActive(true);
        }  
    }

    private void SetAlpha(GameObject card, float a)
    {
        // Set transparency of all card features here
        Image[] images = card.GetComponentsInChildren<Image>();
        TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
        foreach (Image img in images)
        {
            if (!img.GetComponent<Button>())
                img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        }
        foreach (TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }
}
