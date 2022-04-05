using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JLMainMenu : MonoBehaviour
{
    // Menu Canvases
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject optionMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playButton() {
        mainMenu.SetActive(!mainMenu.activeSelf); // Flip-Flop switcheroo
        playMenu.SetActive(!playMenu.activeSelf); // Flip-Flop switcheroo x2
    }

    public void optionButton() {
        mainMenu.SetActive(!mainMenu.activeSelf); // Flip-Flop switcheroo
        optionMenu.SetActive(!optionMenu.activeSelf); // Flip-Flop switcheroo x2
    }

    public void quitButton() {
        Application.Quit();
    }

    public void backButton() { // Umbrella function that should work no matter where the back button is
        if (playMenu.activeSelf) {
            playButton();
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
