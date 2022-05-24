using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JLCreditsMenu : MonoBehaviour
{
    public GameObject mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        JLAudioManager.Instance.PlaySound("Death");

        if (Global.screenLayout != default)
            ResolutionChange(Global.screenLayout);
        else
            ResolutionChange("Standard");
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.anyKeyDown) {
           SceneManager.LoadScene("JLMainMenu");
       } 
    }

    public void ResolutionChange(string viewType)
    {
        if (viewType == "Standard")
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else if (viewType == "Widescreen")
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
    }
}
