using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : Singleton<MenuManager>
{
    public GameObject[] menuScenes;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "JLMainMenu")
        {
            JLAudioManager.Instance.PlaySound("MainTheme");
        }
        SwapScene(0);
    }

    private void Start()
    {

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
        Debug.Log(animSpeed);
    }
}