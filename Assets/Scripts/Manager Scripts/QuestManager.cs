using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum ButtonDisplay { NONE, CONTINUE, FINISH, CHOICES };

public class QuestManager : Singleton<QuestManager>
{
    [Header("General")]
    public GameObject questBackground;
    public float backgroundAlpha = 0.785f;
    public GameObject questLayout;
    public float fadeLength = 0.004f;

    [Header("Images")]
    public Image locationImage;
    public Image npcImage;
    public Sprite[] locationImages;
    public Sprite[] npcImages;

    [Header("Text")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Buttons")]
    public GameObject continueButton;
    public GameObject finishButton;
    public GameObject choiceButtonPrefab;
    public GameObject[] choiceButtons;
    public Color enabledColor;
    public Color disabledColor;

    private Action[] OnChoiceMade;
    private bool buttonFading;
    private bool isYourTurn;
    private List<Action> chunks;
    private Action OnComplete;
    private int currentChunk;
    private bool buttonOnCooldown;
    private string audioFileName;

    public void LoadIntoQuest(bool isYourTurn, List<Action> chunks, Action OnComplete)
    {
        this.isYourTurn = isYourTurn;
        this.chunks = chunks;
        this.OnComplete = OnComplete;
        ResetValues();
        StartCoroutine(FadeInScene());
    }

    public void SetChunk(int id)
    {
        currentChunk = id;
        chunks[id]();
    }

    public void SetNextChunk()
    {
        currentChunk++;
        if (currentChunk < chunks.Count)
            chunks[currentChunk]();
        else
            StartCoroutine(FadeOutScene());
    }

    public void PlayAudio(string name, float startTime = 0, float endTime = -1)
    {
        audioFileName = name;
        JLAudioManager.Instance.PlaySound(name, startTime, endTime);
        if(endTime > startTime)
            StartCoroutine(ContinueAudio(currentChunk, endTime - startTime));
    }

    IEnumerator ContinueAudio(int lastChunk, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (currentChunk == lastChunk)
            PlayManager.Instance.localPlayer.SetNextDialogueChunk();
    }

    public void SetDialogue(string text)
    {
        dialogueText.text = text;
    }

    public void SetSpeaker(string text)
    {
        speakerText.text = text;
    }

    public void SetImage(string name)
    {
        bool isLocation = false;
        switch(name)
        {
            case "Crazed Hermit":
                isLocation = false;
                npcImage.sprite = npcImages[2];
                break;
        }
        locationImage.gameObject.SetActive(isLocation);
        npcImage.gameObject.SetActive(!isLocation);
    }

    public void SetChoices(int choices, string[] choiceTexts, Action[] ChoiceResults, Func<bool>[] ChoiceRequirements)
    {
        // Clear old choices
        for(int i = 0; i < choiceButtons.Length; i++)
            Destroy(choiceButtons[i]);
        choiceButtons = new GameObject[choices];

        // Set new choices
        for (int i = 0; i < choices; i++)
        {
            choiceButtons[i] = Instantiate(choiceButtonPrefab, continueButton.transform.parent);
            choiceButtons[i].transform.localPosition = new Vector3(-4000 * (choices - i * 2 - 1), -1700, 0);
            choiceButtons[i].GetComponent<UIQuestChoice>().id = i;
            choiceButtons[i].GetComponent<TMP_Text>().text = choiceTexts[i];
            if(ChoiceRequirements[i]())
            {
                choiceButtons[i].GetComponent<Button>().enabled = true;
                choiceButtons[i].GetComponent<Image>().color = enabledColor;
            }
            else
            {
                choiceButtons[i].GetComponent<Button>().enabled = false;
                choiceButtons[i].GetComponent<Image>().color = disabledColor;
            }
        }
        OnChoiceMade = ChoiceResults;

        // Display choice buttons
        SetButtonDisplay(ButtonDisplay.CHOICES);
    }

    public void SetButtonDisplay(ButtonDisplay buttonDisplay)
    {
        // Disable buttons if its not your encounter
        if (!isYourTurn)
            buttonDisplay = ButtonDisplay.NONE;

        if(buttonDisplay == ButtonDisplay.CONTINUE && !continueButton.activeInHierarchy)
            StartCoroutine(FadeInButton(continueButton));
        else if(buttonDisplay != ButtonDisplay.CONTINUE && continueButton.activeInHierarchy)
            StartCoroutine(FadeOutButton(continueButton));

        if (buttonDisplay == ButtonDisplay.FINISH && !finishButton.activeInHierarchy)
            StartCoroutine(FadeInButton(finishButton));
        else if (buttonDisplay != ButtonDisplay.FINISH && finishButton.activeInHierarchy)
            StartCoroutine(FadeOutButton(finishButton));

        foreach (GameObject g in choiceButtons)
        {
            if (buttonDisplay == ButtonDisplay.CHOICES && !g.activeInHierarchy)
                StartCoroutine(FadeInButton(g));
            else if (buttonDisplay != ButtonDisplay.CHOICES && g.activeInHierarchy)
                StartCoroutine(FadeOutButton(g));
        }
    }

    IEnumerator FadeInButton(GameObject button)
    {
        button.SetActive(true);
        buttonFading = true;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(button, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }
        buttonFading = false;
    }

    IEnumerator FadeOutButton(GameObject button)
    {
        buttonFading = true;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(button, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }
        buttonFading = false;
        button.SetActive(false);
    }

    IEnumerator FadeInScene()
    {
        questBackground.SetActive(true);
        questLayout.SetActive(true);

        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(questBackground, backgroundAlpha * i * Global.animRate);
            SetAlpha(questLayout, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Start by calling chunk 0
        SetChunk(0);
    }

    IEnumerator FadeOutScene()
    {
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(questBackground, backgroundAlpha * i * Global.animRate);
            SetAlpha(questLayout, i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        questBackground.SetActive(false);
        questLayout.SetActive(false);

        // End by calling OnComplete
        OnComplete();
    }

    private void SetAlpha(GameObject g, float a)
    {
        Image[] images = g.GetComponentsInChildren<Image>();
        TMP_Text[] texts = g.GetComponentsInChildren<TMP_Text>();
        foreach (Image img in images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        foreach (TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }

    private void ResetValues()
    {
        continueButton.SetActive(false);
        finishButton.SetActive(false);
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].SetActive(false);
        speakerText.text = "";
        dialogueText.text = "";
        locationImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
        currentChunk = 0;
        buttonOnCooldown = false;
        buttonFading = false;
    }

    // Continue Button
    public void Continue()
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
        PlayManager.Instance.localPlayer.SetNextDialogueChunk();
    }

    // Finish Button
    public void Finish()
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
        JLAudioManager.Instance.StopSound(audioFileName);
        PlayManager.Instance.localPlayer.SetNextDialogueChunk();
    }

    // Choice Buttons
    public void Choice(int id)
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
        OnChoiceMade[id]();
        PlayManager.Instance.localPlayer.SetNextDialogueChunk();
    }

    IEnumerator ButtonCooldown()
    {
        buttonOnCooldown = true;
        yield return new WaitForSeconds(0.5f);
        buttonOnCooldown = false;
    }
}