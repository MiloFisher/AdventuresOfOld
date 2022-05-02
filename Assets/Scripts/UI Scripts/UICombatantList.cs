using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;

public class UICombatantList : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public GameObject playerNames;
    public GameObject activities;
    public float waitTime = 0.5f;

    private RectTransform rt;
    private bool closing;

    public void CloseCombatantList()
    {
        StartCoroutine(AnimateClosing());
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void Update()
    {
        // Update player activities
        for (int i = 0; i < PlayManager.Instance.turnOrderPlayerList.Count; i++)
        {
            activities.transform.GetChild(i).GetComponent<TMP_Text>().text = ParseActivity(PlayManager.Instance.turnOrderPlayerList[i]);
        }
    }

    private void OnDisable()
    {
        // Load into combat
        CombatManager.Instance.LoadIntoCombat();
    }

    public string ParseActivity(Player p)
    {
        if (p.Ready.Value || closing)
        {
            switch (p.ParticipatingInCombat.Value)
            {
                case 0: return "Spectating";
                case 1: return "Joining the Fight";
            }
        }
        return "Waiting...";
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

        // Ready up player
        PlayManager.Instance.localPlayer.ReadyUp();

        // Host only, wait for all players to be ready before moving on
        if (NetworkManager.Singleton.IsServer)
        {
            yield return new WaitUntil(() =>
            {
                bool allReady = true;
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (!p.Ready.Value)
                        allReady = false;
                }
                return allReady;
            });

            // Unready all players and continue them
            foreach (Player p in PlayManager.Instance.playerList)
            {
                p.Unready();
                p.ContinueToCombat();
            }
        }
    }

    public void ResetSize()
    {
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        int i;
        for (i = 0; i < PlayManager.Instance.turnOrderPlayerList.Count; i++)
        {
            // Setup PlayerNames
            GameObject g = playerNames.transform.GetChild(i).gameObject;
            g.SetActive(true);
            g.GetComponent<TMP_Text>().text = "<color=" + PlayManager.Instance.GetPlayerColorString(PlayManager.Instance.turnOrderPlayerList[i]) + ">" + PlayManager.Instance.turnOrderPlayerList[i].Name.Value + "</color>";
            // Setup Activities
            g = activities.transform.GetChild(i).gameObject;
            g.SetActive(true);
            g.GetComponent<TMP_Text>().text = "Waiting...";
        }
        for (; i < 6; i++)
        {
            playerNames.transform.GetChild(i).gameObject.SetActive(false);
            activities.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator AnimateClosing()
    {
        closing = true;

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

        closing = false;
        gameObject.SetActive(false);
    }
}
