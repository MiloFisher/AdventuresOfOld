using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : Singleton<InventoryManager>
{
    public GameObject[] cards;
    public GameObject minimizeButton;
    public float minimizedY;
    public float maximizedY;
    public float minimizedGap;
    public float maximizedGap;
    public float auxiliaryCardY;
    public float movementTimeLength = 0.004f;
    public bool maximized;

    private string[] gear = new string[9];
    private int[] gearPos = new int[9];
    private int activeCards;
    private string emptyValue = "empty";
    private bool inAnimation;

    public void SetGear(string[] g)
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

    public void MaximizeInventory()
    {
        if (inAnimation || maximized)
            return;
        maximized = true;
        minimizeButton.SetActive(true);
        StartCoroutine(AnimateMinMaxMovement());
    }

    public void MinimizeInventory()
    {
        if (inAnimation || !maximized)
            return;
        maximized = false;
        minimizeButton.SetActive(false);
        StartCoroutine(AnimateMinMaxMovement());
    }

    IEnumerator AnimateCorrectionalMovement(int newActiveCards, int[] newGearPos, string[] newGear)
    {
        inAnimation = true;
        float gap = maximized ? maximizedGap : minimizedGap;
        float y = maximized ? maximizedY : minimizedY;
        float auxHeight = auxiliaryCardY + y;
        // Animate moving to new positions for all cards
        for(int i = 1; i <= 100; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                float startPos = gap * (2 * gearPos[j] + 1 - activeCards);
                float endPos = gap * (2 * newGearPos[j] + 1 - newActiveCards);
                // Move from one position to another
                if (gearPos[j] > -1 && newGearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos + (endPos - startPos) * i * 0.01f, y, 0);
                }
                // Insert new card
                else if (gearPos[j] == -1 && newGearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(endPos, auxHeight + (y - auxHeight) * i * 0.01f, 0);
                    SetAlpha(cards[j], i * 0.01f);
                }
                // Remove old card
                else if (gearPos[j] > -1 && newGearPos[j] == -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos, y + (auxHeight - y) * i * 0.01f, 0);
                    SetAlpha(cards[j], (100 - i) * 0.01f);
                }
            }
            yield return new WaitForSeconds(movementTimeLength);
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
        for (int i = 1; i <= 100; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                float startPos = startGap * (2 * gearPos[j] + 1 - activeCards);
                float endPos = endGap * (2 * gearPos[j] + 1 - activeCards);
                // Move from one position to another
                if (gearPos[j] > -1)
                {
                    cards[j].transform.localPosition = new Vector3(startPos + (endPos - startPos) * i * 0.01f, startY + (endY - startY) * i * 0.01f, 0);
                }
            }
            yield return new WaitForSeconds(movementTimeLength);
        }
        inAnimation = false;
    }

    private void SetAlpha(GameObject card, float a)
    {
        // Set transparency of all card features here
        for(int i = 0; i < card.transform.childCount; i++)
        {
            Image img = card.transform.GetChild(0).GetComponent<Image>();
            TMP_Text txt = card.transform.GetChild(0).GetComponent<TMP_Text>();
            if (img)
                img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            else if (txt)
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
        }
    }
}
