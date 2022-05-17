using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class JLPauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject[] pauseMenus;
    public Slider animSlider;
    public TextMeshProUGUI animText;

    private int currentID;

    private void Start()
    {
        animSlider.value = -1 * Global.animSpeed;
        AnimationSpeed(Global.animSpeed);
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

    public void ResolutionChange(int resNumber) { // Set resNumber with Button function in Unity Editor Scene
        if (resNumber == 1920) {
            Screen.SetResolution(1920, 1080, true); // True == Fullscreen
        }
        else if (resNumber == 2560) {
            Screen.SetResolution(2560, 1440, true);
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
