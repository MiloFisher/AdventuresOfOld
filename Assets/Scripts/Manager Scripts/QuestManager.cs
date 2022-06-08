using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AdventuresOfOldMultiplayer;

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
    private bool onLastChunk;
    private bool onChoice;

    private Player localPlayer;

    public void LoadIntoQuest(bool isYourTurn, List<Action> chunks, Action OnComplete, Player p = default)
    {
        if (p == default)
            localPlayer = PlayManager.Instance.localPlayer;
        else
            localPlayer = p;
        this.isYourTurn = isYourTurn;
        this.chunks = chunks;
        this.OnComplete = OnComplete;
        ResetValues();
        StartCoroutine(FadeInScene());
    }

    public void SetChunk(int id)
    {
        JLAudioManager.Instance.PlayOneShotSound("DialogueChange");
        currentChunk = id;
        chunks[id]();
    }

    public void SetNextChunk()
    {
        currentChunk++;
        if (currentChunk < chunks.Count)
        {
            JLAudioManager.Instance.PlayOneShotSound("DialogueChange");
            chunks[currentChunk]();
        }
        else
            StartCoroutine(FadeOutScene());
    }

    public void EndDialogue()
    {
        currentChunk++;
        StartCoroutine(FadeOutScene());
    }

    public void PlayAudio(string name, float startTime = 0, float endTime = -1)
    {
        if(audioFileName != default)
            JLAudioManager.Instance.StopSound(audioFileName);
        audioFileName = name;
        JLAudioManager.Instance.PlaySound(name, startTime, endTime);
        if(endTime > startTime && isYourTurn)
            StartCoroutine(ContinueAudio(currentChunk, endTime - startTime));
    }

    IEnumerator ContinueAudio(int lastChunk, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (currentChunk == lastChunk && !onChoice)
        {
            if(onLastChunk)
               localPlayer.EndDialogue();
            else
               localPlayer.SetNextDialogueChunk();
        }
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
        bool noVisual = false;
        bool isLocation = false;
        switch(name)
        {
            case "Crazed Hermit":
                isLocation = false;
                npcImage.sprite = npcImages[2];
                break;
            case "Distressed Villager":
                isLocation = false;
                npcImage.sprite = npcImages[7];
                break;
            case "Forest Hag":
                isLocation = false;
                npcImage.sprite = npcImages[1];
                break;
            case "Four-Eyed Boy":
                isLocation = false;
                npcImage.sprite = npcImages[0];
                break;
            case "Pablo the Noob":
                isLocation = false;
                npcImage.sprite = npcImages[5];
                break;
            case "Shifty Peddler":
                isLocation = false;
                npcImage.sprite = npcImages[6];
                break;
            case "Suspicious Horse":
                isLocation = false;
                npcImage.sprite = npcImages[3];
                break;
            case "Veteran Hunter":
                isLocation = false;
                npcImage.sprite = npcImages[4];
                break;
            case "Fire Elemental":
                isLocation = false;
                npcImage.sprite = npcImages[8];
                break;
            case "Goblin Prince":
                isLocation = false;
                npcImage.sprite = npcImages[9];
                break;
            case "Abandoned Outpost":
                isLocation = true;
                locationImage.sprite = locationImages[0];
                break;
            case "Ancient Spring":
                isLocation = true;
                locationImage.sprite = locationImages[1];
                break;
            case "Bandit Hideout":
                isLocation = true;
                locationImage.sprite = locationImages[2];
                break;
            case "Howling Cave":
                isLocation = true;
                locationImage.sprite = locationImages[3];
                break;
            case "Ominous Clearing":
                isLocation = true;
                locationImage.sprite = locationImages[4];
                break;
            case "Overgrown Temple":
                isLocation = true;
                locationImage.sprite = locationImages[5];
                break;
            case "Webbed Forest":
                isLocation = true;
                locationImage.sprite = locationImages[6];
                break;
            case "Discord Kitten":
                isLocation = true;
                locationImage.sprite = locationImages[7];
                break;
            case "Raging Discord Kitten":
                isLocation = true;
                locationImage.sprite = locationImages[8];
                break;
            case "Goblin Horde":
                isLocation = true;
                locationImage.sprite = locationImages[9];
                break;
            case "Rainbow Slime":
                isLocation = true;
                locationImage.sprite = locationImages[10];
                break;
            case "Spooky Spider":
                isLocation = true;
                locationImage.sprite = locationImages[11];
                break;
            case "Bandit Weeb Lord":
                isLocation = true;
                locationImage.sprite = locationImages[12];
                break;
            case "Intro":
                isLocation = true;
                locationImage.sprite = locationImages[13];
                break;
            case "Corrupted Tree Spirit":
                isLocation = true;
                locationImage.sprite = locationImages[14];
                break;
            case "Wasteland":
                isLocation = true;
                locationImage.sprite = locationImages[15];
                break;
            case "Lush Greenery":
                isLocation = true;
                locationImage.sprite = locationImages[14];
                break;
            case "Victory":
                isLocation = true;
                locationImage.sprite = locationImages[14];
                break;
            case "Oracle1":
                isLocation = false;
                npcImage.sprite = npcImages[10];
                break;
            case "Oracle2":
                isLocation = false;
                npcImage.sprite = npcImages[11];
                break;
            case "Oracle3":
                isLocation = false;
                npcImage.sprite = npcImages[12];
                break;
            case "Oracle4":
                isLocation = false;
                npcImage.sprite = npcImages[13];
                break;
            case "Prophecy1":
                isLocation = false;
                npcImage.sprite = npcImages[14];
                break;
            case "Prophecy2":
                isLocation = false;
                npcImage.sprite = npcImages[15];
                break;
            case "Prophecy3":
                isLocation = false;
                npcImage.sprite = npcImages[16];
                break;
            case "Prophecy4":
                isLocation = false;
                npcImage.sprite = npcImages[17];
                break;
            case "Prophecy5":
                isLocation = false;
                npcImage.sprite = npcImages[18];
                break;
            case "Prophecy6":
                isLocation = false;
                npcImage.sprite = npcImages[19];
                break;
            case "Prophecy7":
                isLocation = false;
                npcImage.sprite = npcImages[20];
                break;
            default:
                noVisual = true;
                break;
        }
        locationImage.gameObject.SetActive(isLocation && !noVisual);
        npcImage.gameObject.SetActive(!isLocation && !noVisual);
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
            choiceButtons[i].GetComponentInChildren<TMP_Text>().text = choiceTexts[i];
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
        // Set onLastChunk if it is finish button
        if (buttonDisplay == ButtonDisplay.FINISH)
            onLastChunk = true;
        else
            onLastChunk = false;

        // Set onChoice if it is choices button
        if (buttonDisplay == ButtonDisplay.CHOICES)
            onChoice = true;
        else
            onChoice = false;

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
        if(OnComplete != default)
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

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
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
        onLastChunk = false;
        onChoice = false;
        SetAlpha(locationImage, 1);
        SetAlpha(npcImage, 1);
    }

    // Continue Button
    public void Continue()
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
       localPlayer.SetNextDialogueChunk();
    }

    // Finish Button
    public void Finish()
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
        JLAudioManager.Instance.StopSound(audioFileName);
       localPlayer.EndDialogue();
    }

    // Choice Buttons
    public void Choice(int id)
    {
        if (buttonFading || buttonOnCooldown)
            return;

        StartCoroutine(ButtonCooldown());
        OnChoiceMade[id]();
       localPlayer.SetNextDialogueChunk();
    }

    IEnumerator ButtonCooldown()
    {
        buttonOnCooldown = true;
        yield return new WaitForSeconds(0.5f);
        buttonOnCooldown = false;
    }

    public bool InQuest()
    {
        return questLayout.activeInHierarchy;
    }
}
