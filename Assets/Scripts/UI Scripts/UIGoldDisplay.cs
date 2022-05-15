using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIGoldDisplay : MonoBehaviour
{
    public TMP_Text goldValue;
    public Image sendGoldButton;
    public TMP_InputField goldAmount;
    public Color enabledColor;
    public Color disabledColor;

    private int gold;

    private void OnEnable()
    {
        ClampGoldInput();
    }

    private void Update()
    {
        gold = PlayManager.Instance.GetGold(PlayManager.Instance.localPlayer);
        goldValue.text = gold + "";
        sendGoldButton.GetComponent<Button>().enabled = gold > 0;
        sendGoldButton.color = gold > 0 ? enabledColor : disabledColor;
    }

    public void SendGold()
    {
        PlayManager.Instance.TargetPlayerSelection("Choose Gold Recipient", true, false, false, (p) => {
            // Target player gains gold and this player loses gold
            p.GainGold(int.Parse(goldAmount.text));
            PlayManager.Instance.localPlayer.LoseGold(int.Parse(goldAmount.text));
        }, (p) => {
            // Requirement is player is at the store
            return PlayManager.Instance.WentToStore(p);
        }, true, "Cancel", () => {
            ClampGoldInput();
        });
    }

    public void ClampGoldInput()
    {
        if (goldAmount.text == "-")
            goldAmount.text = "";
        if (!string.IsNullOrWhiteSpace(goldAmount.text))
            goldAmount.text = Mathf.Clamp(int.Parse(goldAmount.text), 0, gold).ToString();
    }

    public void FormatGoldInputResult()
    {
        if (string.IsNullOrWhiteSpace(goldAmount.text))
            goldAmount.text = "0";
    }
}
