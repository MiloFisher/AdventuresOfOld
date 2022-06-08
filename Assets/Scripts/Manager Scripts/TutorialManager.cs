using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : MonoBehaviour
{
    public Image tutorialImage;
    public TMP_Text pageNumber;
    public GameObject continueButton;
    public GameObject backButton;
    public GameObject closeButton;
    public Sprite[] tutorialPages;
    public bool isTutorial;

    private int page;

    private void OnEnable()
    {
        page = 1;
        DrawTutorial();
    }

    private void DrawTutorial()
    {
        tutorialImage.sprite = tutorialPages[page - 1];
        pageNumber.text = page + " / " + tutorialPages.Length;
        continueButton.SetActive(page < tutorialPages.Length);
        backButton.SetActive(page > 1);
        closeButton.SetActive(!isTutorial || page == tutorialPages.Length);
    }

    public void Continue()
    {
        page++;
        DrawTutorial();
    }

    public void Back()
    {
        page--;
        DrawTutorial();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
