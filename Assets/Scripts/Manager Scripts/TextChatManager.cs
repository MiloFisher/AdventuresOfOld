using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class TextChatManager : Singleton<TextChatManager>
{
    public GameObject display;
    public TMP_InputField textInput;
    public ScrollRect scrollRect;
    public Transform contentBlock;
    public GameObject messagePrefab;
    public GameObject pauseCanvas;

    private void Update()
    {
        if(!pauseCanvas.activeInHierarchy)
        {
            if (IsActive())
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendInputMessage();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(CloseAfterFrame());
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    SetTextChatActive(true);
                }
            }
        } 
    }

    public void ToggleTextChatActive()
    {
        SetTextChatActive(!IsActive());
    }

    IEnumerator CloseAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        SetTextChatActive(false);
    }

    public void SetTextChatActive(bool active)
    {
        display.transform.localPosition = new Vector3(0, active ? 0 : 10000, 0);
    }

    public bool IsActive()
    {
        return display.transform.localPosition == Vector3.zero;
    }

    public new void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        TMP_Text msg = Instantiate(messagePrefab, contentBlock).GetComponent<TMP_Text>();
        msg.text = message;

        StartCoroutine(Send(msg));
    }

    IEnumerator Send(TMP_Text msg)
    {
        yield return new WaitUntil(() => msg.textInfo != null);

        msg.ForceMeshUpdate(true, true);

        msg.GetComponent<RectTransform>().sizeDelta = new Vector2(2750, 75 * msg.textInfo.lineCount);
        contentBlock.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 75 * msg.textInfo.lineCount);

        yield return new WaitForEndOfFrame();

        scrollRect.verticalScrollbar.value = 0;
    }

    public void SendInputMessage()
    {
        if (IsCommand(textInput.text))
            return;
        string message = "<color=" + PlayManager.Instance.GetPlayerColorString(PlayManager.Instance.localPlayer) + ">[" + PlayManager.Instance.localPlayer.Name.Value + "] " + textInput.text + "</color>";
        PlayManager.Instance.localPlayer.SendMessage(message);
        textInput.text = default;
    }

    public bool IsCommand(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;
        string[] words = message.Split(" ");
        if(words.Length > 1)
        {
            switch(words[0])
            {
                case "/whisper":
                    Player p = GetPlayerFromName(words[1]);
                    if (p != default)
                    {
                        string msg = "";
                        for (int i = 2; i < words.Length; i++)
                            msg += " " + words[i];
                        SendMessage("<color=#8A3986>You whispered to " + p.Name.Value + ":" + msg + "</color>");
                        PlayManager.Instance.localPlayer.SendMessage("<color=#8A3986>" + PlayManager.Instance.localPlayer.Name.Value + " whispers:" + msg + "</color>", p.UUID.Value + "");
                    }
                    else
                    {
                        SendMessage("<color=red>[System] Whisper failed to send.  Unknown target player \"" + words[1] + "\"</color>");
                    }
                    textInput.text = default;
                    return true;
            }
        }
        
        return false;
    }

    private Player GetPlayerFromName(string name)
    {
        string[] nameWords = name.Split("_");
        name = nameWords[0];
        for (int i = 1; i < nameWords.Length; i++)
            name += " " + nameWords[i];

        for(int i = 0; i < PlayManager.Instance.playerList.Count; i++)
        {
            if (PlayManager.Instance.playerList[i].Name.Value == name)
                return PlayManager.Instance.playerList[i];
        }
        return default;
    }
}
