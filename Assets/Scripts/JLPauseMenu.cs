using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class JLPauseMenu : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject pauseCanvas;
    public GameObject[] pauseMenus;
    public Slider animSlider;
    public TextMeshProUGUI animText;
    public Slider frameSlider;
    public TextMeshProUGUI frameText;

    private int currentID;

    private void Start()
    {
        animSlider.value = -1 * Global.animSpeed;
        AnimationSpeed(Global.animSpeed);

        frameSlider.value = Global.animSteps;
        FramerateCap(Global.animSteps);

        ResolutionChange(Global.screenLayout);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !TextChatManager.Instance.IsActive()) {
            Pause();
        }
    }

    public void Pause()
    {
        if (!pauseCanvas.activeSelf)
        {
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
            SwapMenus(0);
        }
        else
        {
            ResumeButton();
        }
        JLAudioManager.Instance.PlaySound("PageTurn");
    }

    public void SwapMenus(int id)
    {
        if (id >= pauseMenus.Length || id < 0)
            return;
        foreach (GameObject g in pauseMenus)
            g.SetActive(false);
        pauseMenus[id].SetActive(true);
        currentID = id;
    }

    public void ResumeButton() {
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        pauseMenus[currentID].SetActive(false);
    }

    public void QuitButton() {
        PlayManager.Instance.DisconnectFromGame();
    }

    public void ResolutionChange(string viewType)
    {
        if (viewType == "Standard") {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            pauseCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else if (viewType == "Widescreen") {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            pauseCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
    }

    public void AnimationSpeed(float speed) {
        float animSpeed = Mathf.Abs(speed);
        Global.animSpeed = animSpeed;

        switch (animSpeed) {
            case >= 0.75f:
                animText.text = "Normal";
                break;
            case >= 0.50f:
                animText.text = "Quick";
                break;
            case >= 0.25f:
                animText.text = "Fast";
                break;
            case >= 0f:
                animText.text = "Speedrun";
                break;
            default:
                animText.text = "Not Working";
                break;
        }

        PlayerPrefs.SetFloat("animSpeed", Global.animSpeed);
    }

    public void FramerateCap(float value)
    {
        Global.animSteps = (int)value;
        Global.frameCap = 3 * (int)value;
        Global.animRate = 1f / Global.animSteps;
        Global.animTimeMod = 100f / Global.animSteps;

        Application.targetFrameRate = Global.frameCap;
        frameText.text = Global.frameCap + " fps";

        PlayerPrefs.SetInt("animSteps", Global.animSteps);
    }
}
