using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JLPauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject[] pauseMenus;

    private int currentID;

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
                swapMenus(0);
                Time.timeScale = 0;
            }
            else {
                resumeButton();
            }
        }
    }

    public void swapMenus(int id)
    {
        if (id >= pauseMenus.Length || id < 0)
            return;
        foreach (GameObject g in pauseMenus)
            g.SetActive(false);
        pauseMenus[id].SetActive(true);
        currentID = id;
    }

    public void resumeButton() {
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        pauseMenus[currentID].SetActive(false);
        Time.timeScale = 1;
    }

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
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
