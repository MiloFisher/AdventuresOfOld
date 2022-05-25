using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject[] menuScenes;
    public Slider animSlider;
    public TextMeshProUGUI animText;
    public Slider frameSlider;
    public TextMeshProUGUI frameText;
    public GameObject mainCanvas;

    private void Awake()
    {
        SwapScene(0);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "JLMainMenu")
        {
            JLAudioManager.Instance.PlaySound("MainTheme");
        }
        if (SceneManager.GetActiveScene().name == "Character Creation")
        {
            JLAudioManager.Instance.PlaySound("Medieval");
        }

        if (animSlider)
        {
            animSlider.value = -1 * Global.animSpeed;
            AnimationSpeed(Global.animSpeed);
        }

        if(frameSlider)
        {
            frameSlider.value = PlayerPrefs.GetInt("animSteps", Global.animSteps);
            FramerateCap(Global.animSteps);
        }

        if(mainCanvas)
        {
            if (Global.screenLayout == default)
            {
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
            else
                ResolutionChange(Global.screenLayout);
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

    public void ResolutionChange(string viewType)
    {
        Global.screenLayout = viewType;
        if (viewType == "Standard")
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else if (viewType == "Widescreen")
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
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