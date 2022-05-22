using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AdventuresOfOldMultiplayer;

public class UITargetPlayer : MonoBehaviour
{
    [Header("Banner")]
    public GameObject banner;
    public TMP_Text bannerText;
    public float bannerStartWidth;
    public float bannerEndWidth;
    public float bannerConstHeight;
    public float bannerStartScale;
    public float bannerEndScale;
    public float bannerGrowingLength = 0.004f;
    public float bannerOpeningLength = 0.004f;

    [Header("General")]
    public GameObject[] playerDisplays;
    public GameObject cancelButton;
    public TMP_Text cancelButtonText;
    public Image inputBlocker;

    private bool open;
    private List<Player> targets;
    private bool closeAfterSelect;
    private bool updatePlayerDisplays;
    private bool showCancelButton;
    private bool includeSelf;
    private Action<Player> OnSelect;
    private Func<Player, bool> MeetsRequirement;
    private Action OnClose;
    private bool selectPaused;

    public void Setup(string bannerDisplayText, bool closeAfterSelect, bool updatePlayerDisplays, bool includeSelf, Action<Player> OnSelect, Func<Player, bool> MeetsRequirement, bool showCancelButton, string cancelButtonText = "Cancel", Action OnClose = default)
    {
        bannerText.text = bannerDisplayText;
        this.closeAfterSelect = closeAfterSelect;
        this.updatePlayerDisplays = updatePlayerDisplays;
        this.showCancelButton = showCancelButton;
        this.includeSelf = includeSelf;
        this.OnSelect = OnSelect;
        this.MeetsRequirement = MeetsRequirement;
        this.cancelButtonText.text = cancelButtonText;
        this.OnClose = OnClose;

        targets = GetTargets();

        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        if (!open)
            return;

        StartCoroutine(AnimateClosing());
    }

    public void Select(int id)
    {
        if (!open || selectPaused || !playerDisplays[id].GetComponent<UIPlayerDisplay>().IsVisible() || playerDisplays[id].GetComponent<UIPlayerDisplay>().IsFading())
            return;

        OnSelect(targets[id]);

        if(closeAfterSelect)
            StartCoroutine(AnimateClosing());
        else
            StartCoroutine(SelectPause());
    }

    IEnumerator SelectPause()
    {
        selectPaused = true;
        yield return new WaitForSeconds(0.5f);
        selectPaused = false;
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void Update()
    {
        if (updatePlayerDisplays && open)
        {
            targets = GetTargets();
            DrawPlayerDisplays();
        }    
    }

    public void ResetSize()
    {
        open = false;
        GetComponent<Image>().enabled = true;
        SetAlpha(GetComponent<Image>(), 0);
        SetAlpha(inputBlocker, 0);
        cancelButton.SetActive(showCancelButton);
        SetAlpha(cancelButton, 0);
        foreach (GameObject g in playerDisplays)
            g.SetActive(false);
    }

    private List<Player> GetTargets()
    {
        List<Player> players = new List<Player>();
        foreach(Player p in PlayManager.Instance.playerList)
        {
            if(MeetsRequirement(p) && !(!includeSelf && p.UUID.Value == PlayManager.Instance.localPlayer.UUID.Value))
            {
                players.Add(p);
            }
        }
        return players;
    }

    private void DrawPlayerDisplays()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(18500 - 3000 * (6 - targets.Count), GetComponent<RectTransform>().sizeDelta.y);
        int i;
        for(i = 0; i < targets.Count; i++)
        {
            playerDisplays[i].SetActive(true);
            playerDisplays[i].GetComponent<UIPlayerDisplay>().UpdatePanel(targets[i]);
            playerDisplays[i].GetComponent<UIPlayerDisplay>().FadeInDisplay();
            playerDisplays[i].transform.localPosition = new Vector3(-150 * (targets.Count - 2 * i - 1), 45, 0);
        }
        for(; i < playerDisplays.Length; i++)
        {
            playerDisplays[i].SetActive(false);
        }
        GetComponent<Image>().enabled = targets.Count > 0;
    }

    IEnumerator AnimateOpening()
    {
        // First setup banner
        banner.SetActive(true);
        banner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth, bannerConstHeight);
        banner.transform.localScale = new Vector3(bannerStartScale, bannerStartScale, 1);

        // Then grow the object
        float dif = bannerEndScale - bannerStartScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            banner.transform.localScale = new Vector3(bannerStartScale + dif * i * Global.animRate, bannerStartScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(bannerGrowingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Draw player objects
        DrawPlayerDisplays();

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Next open the scroll and fade in object (and cancel button if active)
        dif = bannerEndWidth - bannerStartWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            banner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * Global.animRate, bannerConstHeight);
            SetAlpha(GetComponent<Image>(), i * Global.animRate);
            SetAlpha(inputBlocker, i * Global.animRate * 0.5f);
            if (showCancelButton)
                SetAlpha(cancelButton, i * Global.animRate);
            yield return new WaitForSeconds(bannerOpeningLength * Global.animTimeMod * Global.animSpeed);
        }

        open = true;

        DrawPlayerDisplays();
    }

    IEnumerator AnimateClosing()
    {
        open = false;

        foreach(GameObject g in playerDisplays)
            g.GetComponent<UIPlayerDisplay>().FadeOutDisplay();

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // First close the scroll and fade out object (and cancel button if active)
        float dif = bannerEndWidth - bannerStartWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            banner.GetComponent<RectTransform>().sizeDelta = new Vector2(bannerStartWidth + dif * i * Global.animRate, bannerConstHeight);
            SetAlpha(GetComponent<Image>(), i * Global.animRate);
            SetAlpha(inputBlocker, i * Global.animRate * 0.5f);
            if (showCancelButton)
                SetAlpha(cancelButton, i * Global.animRate);
            yield return new WaitForSeconds(bannerOpeningLength * Global.animTimeMod * Global.animSpeed);
        }

        cancelButton.SetActive(false);

        // Then shrink the object
        dif = bannerEndScale - bannerStartScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            banner.transform.localScale = new Vector3(bannerStartScale + dif * i * Global.animRate, bannerStartScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(bannerGrowingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally deactivate banner
        banner.SetActive(false);

        if(OnClose != default)
            OnClose();

        gameObject.SetActive(false);
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image i = g.GetComponent<Image>();
        TMP_Text t = g.GetComponentInChildren<TMP_Text>();
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
