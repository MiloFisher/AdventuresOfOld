using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;

public class EmoteManager : Singleton<EmoteManager>
{
    public GameObject emotePanel;
    public GameObject toggleShowButton;
    public Vector3 minimizePosition;
    public Vector3 maximizePosition;
    public Sprite minimize;
    public Sprite maximize;
    public float openingLength = 0.004f;
    public bool isMaximized = true;
    public Sprite[] emoteImages;
    public GameObject emotePrefab;
    public List<GameObject> currentEmotes = new List<GameObject>();
    public float emoteScale = 2f;
    public float emoteLifetime = 3f;
    public float emoteMinFloatSpeed = 150f;
    public float emoteMaxFloatSpeed = 250f;
    public float emoteMinSwerveWidth = 30f;
    public float emoteMaxSwerveWidth = 70f;

    private bool animating;

    private void Update()
    {
        for(int i = 0; i < currentEmotes.Count; i++)
        {
            float t = Time.deltaTime;
            Emote e = currentEmotes[i].GetComponent<Emote>();
            e.currentLifetime += t;
            if(e.currentLifetime >= e.maxLifetime)
            {
                Destroy(currentEmotes[i]);
                currentEmotes.RemoveAt(i);
                i--;
            }
            else
            {
                float lifetime = e.currentLifetime;
                currentEmotes[i].transform.localPosition = e.basePosition + new Vector3(Mathf.Sin(lifetime * Mathf.PI * 2) * e.swerveWidth, lifetime * e.floatSpeed, 0);
                if (e.currentLifetime >= e.maxLifetime * 0.5f)
                    SetAlpha(currentEmotes[i].GetComponent<Image>(), 1 - (e.currentLifetime - e.maxLifetime * 0.5f) / (e.maxLifetime * 0.5f));
            }
        }
    }

    public void CallEmote(int id)
    {
        PlayManager.Instance.localPlayer.Emote(id);
    }

    public void DrawEmote(Player p, int id)
    {
        JLAudioManager.Instance.PlayOneShotSound("Emote");
        Vector3 targetPosition;
        if(CombatManager.Instance.IsCombatant(p))
        {
            targetPosition = CombatManager.Instance.GetPlayerCardFromCombatant(CombatManager.Instance.GetCombatantFromPlayer(p)).transform.localPosition;
        }
        else
        {
            targetPosition = CombatManager.Instance.enemyCard.transform.localPosition;
        }
        GameObject g = Instantiate(emotePrefab, transform);
        g.transform.localScale = new Vector3(emoteScale, emoteScale, 1);
        g.GetComponent<Image>().sprite = GetEmoteSpriteFromId(id);
        g.GetComponent<Emote>().maxLifetime = emoteLifetime;
        g.GetComponent<Emote>().basePosition = targetPosition;
        g.GetComponent<Emote>().floatSpeed = Random.Range(emoteMinFloatSpeed, emoteMaxFloatSpeed);
        g.GetComponent<Emote>().swerveWidth = Random.Range(emoteMinSwerveWidth, emoteMaxSwerveWidth);
        g.transform.localPosition = targetPosition;
        currentEmotes.Add(g);
    }

    public Sprite GetEmoteSpriteFromId(int id)
    {
        return emoteImages[id];
    }

    public void ToggleShow()
    {
        if (animating)
            return;
        if(isMaximized)
        {
            isMaximized = false;
            toggleShowButton.GetComponent<Image>().sprite = maximize;
            StartCoroutine(Close());
        }
        else
        {
            isMaximized = true;
            toggleShowButton.GetComponent<Image>().sprite = minimize;
            StartCoroutine(Open());
        }
    }

    IEnumerator Open()
    {
        animating = true;
        for(int i = 1; i <= Global.animSteps; i++)
        {
            emotePanel.transform.localPosition = minimizePosition +  i * Global.animRate * (maximizePosition - minimizePosition);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }
        animating = false;
    }

    IEnumerator Close()
    {
        animating = true;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            emotePanel.transform.localPosition = maximizePosition + i * Global.animRate * (minimizePosition - maximizePosition);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }
        animating = false;
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }
}
