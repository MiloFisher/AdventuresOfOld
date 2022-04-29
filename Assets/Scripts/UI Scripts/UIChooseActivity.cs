using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;

public class UIChooseActivity : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public Image[] buttons;
    public GameObject selection;
    public float selectionStartWidth;
    public float selectionStartHeight;
    public float selectionEndWidth;
    public float selectionEndHeight;
    public float selectionEndY;
    public float selectionShiftLength = 0.004f;
    public GameObject playerNames;
    public GameObject activities;
    public float waitTime = 0.5f;

    private RectTransform rt;
    private bool opened;
    private bool selectionMade;

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void Update()
    {
        // Update player activities after selection is made
        if(selectionMade)
        {
            for (int i = 0; i < PlayManager.Instance.turnOrderPlayerList.Count; i++)
            {
                activities.transform.GetChild(i).GetComponent<TMP_Text>().text = ParseActivity(PlayManager.Instance.turnOrderPlayerList[i]);
            }
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
        {
            PlayManager.Instance.StartOfDay();
        }
    }

    public string ParseActivity(Player p)
    {
        if (p.Ready.Value)
        {
            switch (p.EndOfDayActivity.Value)
            {
                case 0: return "Visiting the Store";
                case 1: return "Taking a Short Rest";
                case 2: return "Taking a Long Rest";
                case 3: return "Visiting the Shrine";
            }
        } 
        return "Waiting...";
    }

    public void SelectOption(int option)
    {
        if(!selectionMade && opened)
        {
            selectionMade = true;
            selection.SetActive(true);
            selection.transform.localPosition = buttons[option].transform.localPosition;
            selection.GetComponentInChildren<TMP_Text>().text = buttons[option].GetComponentInChildren<TMP_Text>().text;
            buttons[option].gameObject.SetActive(false);
            StartCoroutine(AnimateSelection(option));
        }
    }

    IEnumerator AnimateSelection(int option)
    {
        // First fade out buttons and shift selection over
        float startY = selection.transform.localPosition.y;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            foreach (Image b in buttons)
            {
                SetAlpha(b, (Global.animSteps-i) * Global.animRate);
                SetAlpha(b.GetComponentInChildren<TMP_Text>(), (Global.animSteps-i) * Global.animRate);
            }
            selection.transform.localPosition = new Vector3(0, startY + (selectionEndY - startY) * i * Global.animRate, 0);
            selection.GetComponent<RectTransform>().sizeDelta = new Vector2(selectionStartWidth + (selectionEndWidth - selectionStartWidth) * i * Global.animRate, selectionStartHeight + (selectionEndHeight - selectionStartHeight) * i * Global.animRate);
            yield return new WaitForSeconds(selectionShiftLength * Global.animTimeMod);
        }

        // Next fade in playerNames and activities
        playerNames.SetActive(true);
        activities.SetActive(true);
        for (int i = 1; i <= Global.animSteps; i++)
        {
            for(int j = 0; j < PlayManager.Instance.turnOrderPlayerList.Count; j++)
            {
                SetAlpha(playerNames.transform.GetChild(j).GetComponent<TMP_Text>(), i * Global.animRate);
                SetAlpha(activities.transform.GetChild(j).GetComponent<TMP_Text>(), i * Global.animRate);
            }
            yield return new WaitForSeconds(selectionShiftLength * Global.animTimeMod);
        }

        PlayManager.Instance.localPlayer.SetValue("EndOfDayActivity", option);
        PlayManager.Instance.localPlayer.ReadyUp();

        yield return new WaitUntil(() => {
            bool allReady = true;
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (!p.Ready.Value)
                    allReady = false;
            }
            return allReady;
        });

        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateOpening()
    {
        // First grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod);
        }

        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod);
        }

        // Finally set opened to true
        opened = true;
    }

    public void ResetSize()
    {
        selectionMade = false;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        selection.GetComponent<RectTransform>().sizeDelta = new Vector2(selectionStartWidth, selectionStartHeight);
        foreach (Image img in buttons)
        {
            img.gameObject.SetActive(true);
            SetAlpha(img, 1);
            SetAlpha(img.GetComponentInChildren<TMP_Text>(), 1);
        }
        playerNames.SetActive(false);
        activities.SetActive(false);
        int i;
        for(i = 0; i < PlayManager.Instance.turnOrderPlayerList.Count; i++)
        {
            // Setup PlayerNames
            GameObject g = playerNames.transform.GetChild(i).gameObject;
            g.SetActive(true);
            g.GetComponent<TMP_Text>().text = "<color=" + PlayManager.Instance.turnOrderPlayerList[i].Color.Value + ">" + PlayManager.Instance.turnOrderPlayerList[i].Name.Value + "</color>";
            SetAlpha(g.GetComponent<TMP_Text>(), 0);
            // Setup Activities
            g = activities.transform.GetChild(i).gameObject;
            g.SetActive(true);
            g.GetComponent<TMP_Text>().text = "Waiting...";
            SetAlpha(g.GetComponent<TMP_Text>(), 0);
        }
        for(; i < 6; i++)
        {
            playerNames.transform.GetChild(i).gameObject.SetActive(false);
            activities.transform.GetChild(i).gameObject.SetActive(false);
        }
        selection.SetActive(false);
    }

    IEnumerator AnimateClosing()
    {
        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod);
        }

        // Next shrink the object
        dif = endScale - startScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod);
        }

        yield return new WaitForSeconds(waitTime);

        gameObject.SetActive(false);
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
