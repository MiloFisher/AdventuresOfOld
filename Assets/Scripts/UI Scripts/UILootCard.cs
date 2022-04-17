using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILootCard : MonoBehaviour
{
    public string cardName;
    public GameObject collectCardButton;
    public GameObject cardButton;
    public GameObject cardBack;

    public float fadeLength = 0.004f;

    private bool collectButtonActive;

    public void ClickCard(int id)
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
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
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

        for(int i = 1; i <= 100; i++)
        {
            SetAlpha(collectCardButton, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
        }

        collectButtonActive = true;
    }

    IEnumerator FadeOutButton()
    {
        collectButtonActive = false;

        for (int i = 99; i >= 0; i--)
        {
            SetAlpha(collectCardButton, i * 0.01f);
            yield return new WaitForSeconds(fadeLength);
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
