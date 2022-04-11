using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JLPauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject pauseMenu;
    public GameObject optionMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pauseCanvas.activeSelf) {
                pauseCanvas.SetActive(!pauseCanvas.activeSelf);
                pauseMenu.SetActive(!pauseMenu.activeSelf); // Flip-Flop switcheroo
                Time.timeScale = 0;
            }
            else {
                backButton();
            }
        }
    }

    public void resumeButton() {
        if (pauseMenu.activeSelf) {
            pauseCanvas.SetActive(!pauseMenu.activeSelf);
            pauseMenu.SetActive(!pauseMenu.activeSelf); // Flip-Flop switcheroo
            Time.timeScale = 1;
        }
    }

    public void optionButton() {
        pauseMenu.SetActive(!pauseMenu.activeSelf); // Flip-Flop switcheroo
        optionMenu.SetActive(!optionMenu.activeSelf); // Flip-Flop switcheroo x2
    }

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
    }

    public void backButton() { // Umbrella function that should work no matter where the back button is
        if (pauseMenu.activeSelf) {
            resumeButton();
        }
        else if (optionMenu.activeSelf) {
            optionButton();
        }
    }

    public void resolutionChange(int resNumber) { // Set resNumber with Button function in Unity Editor Scene
        if (resNumber == 1920) {
            Screen.SetResolution(1920, 1080, true); // True == Fullscreen
        }
        else if (resNumber == 2560) {
            Screen.SetResolution(2560, 1440, true);
        }
    }
}
