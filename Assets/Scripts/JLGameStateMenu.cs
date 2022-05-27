using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class JLGameStateMenu : MonoBehaviour
{
    private GameObject[] players;

    public GameObject mainCanvas;
    public GameObject[] layouts;
    public Sprite[] races;

    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject g in layouts)
            g.SetActive(false);

        if (players.Length >= 3)
        {
            if (SceneManager.GetActiveScene().name == "JLFailureMenu")
            {
                SetupFailureScreen();
            }
            else if (SceneManager.GetActiveScene().name == "JLSuccessMenu")
            {
                SetupSuccessScreen();
            }
        }
    }

    private void Start()
    {
        JLAudioManager.Instance.PlaySound("Theme");

        if (Global.screenLayout != default)
            ResolutionChange(Global.screenLayout);
        else
            ResolutionChange("Standard");
    }

    public void quitButton()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsConnectedClient)
            NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("JLMainMenu");
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

    public void SetupSuccessScreen()
    {
        GameObject layout = layouts[players.Length - 3];
        layout.SetActive(true);
        for (int i = 0; i < players.Length; i++)
        {
            layout.transform.GetChild(i).GetComponent<Image>().enabled = false;
            GameObject silhouetteCollection = layout.transform.GetChild(i).GetChild(0).gameObject;
            Player p = players[i].GetComponent<Player>();
            GameObject silhouette;

            for (int j = 0; j < silhouetteCollection.transform.childCount; j++)
                silhouetteCollection.transform.GetChild(j).gameObject.SetActive(false);

            switch (p.Race.Value + "")
            {
                case "Human":
                    silhouette = silhouetteCollection.transform.GetChild(0).gameObject;
                    break;
                case "Dwarf":
                    silhouette = silhouetteCollection.transform.GetChild(1).gameObject;
                    break;
                case "Leonin":
                    silhouette = silhouetteCollection.transform.GetChild(2).gameObject;
                    break;
                case "Centaur":
                    silhouette = silhouetteCollection.transform.GetChild(3).gameObject;
                    break;
                case "High Elf":
                    silhouette = silhouetteCollection.transform.GetChild(4).gameObject;
                    break;
                case "Night Elf":
                    silhouette = silhouetteCollection.transform.GetChild(4).gameObject;
                    break;
                case "Aasimar":
                    silhouette = silhouetteCollection.transform.GetChild(5).gameObject;
                    break;
                default:
                    silhouette = silhouetteCollection.transform.GetChild(0).gameObject;
                    break;
            }

            silhouette.SetActive(true);
            silhouette.GetComponentInChildren<TMP_Text>().text = p.Name.Value + "";
        }
    }

    public void SetupFailureScreen()
    {
        GameObject layout = layouts[players.Length - 3];
        layout.SetActive(true);
        for (int i = 0; i < players.Length; i++)
        {
            Transform tombstone = layout.transform.GetChild(i);
            Player p = players[i].GetComponent<Player>();

            tombstone.GetChild(0).GetComponent<TMP_Text>().text = p.Name.Value + "";

            switch (p.Race.Value + "")
            {
                case "Human":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[0];
                    break;
                case "Dwarf":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[1];
                    break;
                case "Leonin":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[2];
                    break;
                case "Centaur":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[3];
                    break;
                case "High Elf":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[4];
                    break;
                case "Night Elf":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[5];
                    break;
                case "Aasimar":
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[6];
                    break;
                default:
                    tombstone.GetChild(1).GetComponent<Image>().sprite = races[0];
                    break;
            }
        }
    }
}
