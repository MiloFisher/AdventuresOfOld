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

    private int currentID;

    private void Start()
    {
        animSlider.value = -1 * Global.animSpeed;
        AnimationSpeed(Global.animSpeed);

        float width = mainCanvas.GetComponent<RectTransform>().sizeDelta.x;
        float height = mainCanvas.GetComponent<RectTransform>().sizeDelta.y;
        if (width / height <= 16f / 9f)
        {
            ResolutionChange("Standard");
        }
        else
        {
            ResolutionChange("Widescreen");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
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
    }
}
