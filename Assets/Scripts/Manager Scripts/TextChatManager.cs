using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        string message = "<color=" + PlayManager.Instance.GetPlayerColorString(PlayManager.Instance.localPlayer) + ">[" + PlayManager.Instance.localPlayer.Name.Value + "]</color> " + textInput.text;
        PlayManager.Instance.localPlayer.SendMessage(message);
        textInput.text = default;
    }
}
