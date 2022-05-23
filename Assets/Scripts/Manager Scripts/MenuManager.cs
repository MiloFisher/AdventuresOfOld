using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject[] menuScenes;
    public Slider animSlider;
    public TextMeshProUGUI animText;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "JLMainMenu")
        {
            JLAudioManager.Instance.PlaySound("MainTheme");
        }
        if (SceneManager.GetActiveScene().name == "Character Creation")
        {
            JLAudioManager.Instance.PlaySound("Medieval");
        }
        SwapScene(0);
    }

    private void Start()
    {
        if (animSlider)
        {
            animSlider.value = -1 * Global.animSpeed;
            AnimationSpeed(Global.animSpeed);
        }
    }

    public void SwapScene(int id)
    {
        if (id >= menuScenes.Length || id < 0)
            return;
        foreach (GameObject g in menuScenes)
            g.SetActive(false);
        menuScenes[id].SetActive(true);
    }

    public void ResolutionChange(int resNumber)
    { // Set resNumber with Button function in Unity Editor Scene
        if (resNumber == 1920)
        {
            Screen.SetResolution(1920, 1080, true); // True == Fullscreen
        }
        else if (resNumber == 2560)
        {
            Screen.SetResolution(2560, 1440, true);
        }
    }

    public void CreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void CloseApplication()
    {
        Application.Quit();
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