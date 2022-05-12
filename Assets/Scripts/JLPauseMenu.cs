using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class JLPauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject[] pauseMenus;

    private int currentID;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pauseCanvas.activeSelf) {
                pauseCanvas.SetActive(!pauseCanvas.activeSelf);
                SwapMenus(0);
                Time.timeScale = 0;
            }
            else {
                ResumeButton();
            }
        }
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
        Time.timeScale = 1;
    }

    public void QuitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
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

        GameObject animObject = GameObject.FindGameObjectWithTag("AnimHolder");
        TextMeshProUGUI animText = animObject.GetComponent<TextMeshProUGUI>();
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
