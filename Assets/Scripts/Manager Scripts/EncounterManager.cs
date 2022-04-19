using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncounterManager : Singleton<EncounterManager>
{
    public GameObject cardPrefab;
    public float startX = -1300;
    public float startY = 545.8f;
    public float endX = 0;
    public float endY = 0;
    public float gap = 150;
    public float travelLength = 0.004f;
    public float fadeLength = 0.004f;
    public GameObject encounterBanner;
    public float bannerStartWidth;
    public float bannerEndWidth;
    public float bannerConstHeight;
    public float bannerStartScale;
    public float bannerEndScale;
    public float bannerGrowingLength = 0.004f;
    public float bannerOpeningLength = 0.004f;
    public List<string> cardsToDraw = new List<string>();
    private List<GameObject> displayCards = new List<GameObject>();
    private bool endTurnAfter;

    public void CompleteEncounter(bool _endTurnAfter)
    {
        endTurnAfter = _endTurnAfter;
        StartCoroutine(AnimateCardFade());
    }

    public void AddEncounterCardToDraw(string cardName)
    {
        cardsToDraw.Add(cardName);
    }

    public void DrawCard(int amount, bool animateOpening)
    {
        if (amount > 0)
        {
            if(animateOpening)
                StartCoroutine(AnimateOpening(amount));
            else
                StartCoroutine(AnimateCardDraw(0, amount));
        }
    }

    public void DisableCardButtons()
    {
        foreach (GameObject g in displayCards)
            g.GetComponent<UIEncounterCard>().SetButtonActive(false);
    }

    IEnumerator AnimateOpening(int amount)
    {
        // First setup banner
        encounterBanner.SetActive(true);
        encounterBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth, bannerConstHeight);
        encounterBanner.transform.localScale = new Vector3(bannerStartScale, bannerStartScale, 1);

        // Then grow the object
        float dif = bannerEndScale - bannerStartScale;
        for (int i = 1; i <= 100; i++)
        {
            encounterBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * 0.01f, bannerStartScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(bannerGrowingLength);
        }

        // Next open the scroll
        dif = bannerEndWidth - bannerStartWidth;
        for (int i = 1; i <= 100; i++)
        {
            encounterBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * 0.01f, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength);
        }

        // Finally animate cards
        StartCoroutine(AnimateCardDraw(0, amount));
    }

    IEnumerator AnimateClosing()
    {
        // First close the scroll
        float dif = bannerEndWidth - bannerStartWidth;
        for (int i = 99; i >= 0; i--)
        {
            encounterBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * 0.01f, bannerConstHeight);
            yield return new WaitForSeconds(bannerOpeningLength);
        }

        // Then shrink the object
        dif = bannerEndScale - bannerStartScale;
        for (int i = 99; i >= 0; i--)
        {
            encounterBanner.transform.localScale = new Vector3(bannerStartScale + dif * i * 0.01f, bannerStartScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(bannerGrowingLength);
        }

        // Finally deactivate banner
        encounterBanner.SetActive(false);

        if(endTurnAfter)
            PlayManager.Instance.EndTurn();
    }

    IEnumerator AnimateCardDraw(int current, int amount)
    {
        GameObject travelCard = Instantiate(cardPrefab, transform.parent);
        travelCard.GetComponent<UIEncounterCard>().SetVisuals(cardsToDraw[current]);
        travelCard.GetComponent<UIEncounterCard>().ActivateCardButton(false);
        travelCard.GetComponent<UIEncounterCard>().ActivateCardBack(true);

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
            travelCard.transform.localRotation = Quaternion.Euler(new Vector3(i <= 50 ? 22.5f * i * 0.02f - 45 : 22.5f * (50 - (i - 50)) * 0.02f, i <= 50 ? -90 * i * 0.02f : 90 * (50 - (i - 50)) * 0.02f, i <= 50 ? 17.6f * (50 - i) * 0.02f + 17.6f : -17.6f * (100 - i) * 0.02f)); //45 * i * 0.01f - 45, -180 * i * 0.01f, 35.2f * (100-i) * 0.01f
            // Scale up
            scale = 1.25f * i * 0.01f + 0.75f;
            travelCard.transform.localScale = new Vector3(scale, scale, 1);
            // Flip card face halfway through
            if (i == 50)
                travelCard.GetComponent<UIEncounterCard>().ActivateCardBack(false);
            yield return new WaitForSeconds(travelLength);
        }

        GameObject card = Instantiate(cardPrefab, travelCard.transform.position, Quaternion.identity, transform.parent);
        card.GetComponent<UIEncounterCard>().SetVisuals(cardsToDraw[current]);
        card.GetComponent<UIEncounterCard>().ActivateCardButton(false);
        card.GetComponent<UIEncounterCard>().ActivateOptionCardButton(true);
        card.transform.localScale = travelCard.transform.localScale;
        displayCards.Add(card);
        Destroy(travelCard);

        if (current < amount - 1)
            StartCoroutine(AnimateCardDraw(current + 1, amount));
        else
            cardsToDraw.Clear();
    }

    IEnumerator AnimateCardFade()
    {
        for(int i = 99; i >= 0; i--)
        {
            foreach (GameObject g in displayCards)
                SetAlpha(g, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        for (int i = 0; i < displayCards.Count; i++)
            Destroy(displayCards[i]);
        displayCards.Clear();

        StartCoroutine(AnimateClosing());
    }

    public void ForkInTheRoadHelper()
    {
        StartCoroutine(AnimateForkInTheRoad());
    }

    IEnumerator AnimateForkInTheRoad()
    {
        for (int i = 99; i >= 0; i--)
        {
            foreach (GameObject g in displayCards)
                SetAlpha(g, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        for (int i = 0; i < displayCards.Count; i++)
            Destroy(displayCards[i]);
        displayCards.Clear();

        PlayManager.Instance.localPlayer.DrawEncounterCards(2, PlayManager.Instance.localPlayer.UUID.Value, false);
    }

    private void SetAlpha(GameObject card, float a)
    {
        // Set transparency of all card features here
        Image[] images = card.GetComponentsInChildren<Image>();
        TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
        foreach(Image img in images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        foreach(TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }
}
