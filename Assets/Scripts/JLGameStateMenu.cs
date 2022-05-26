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
    public GameObject[] gravestones; // For Failure Menu
    public GameObject[] playerSilhouettes; // For Success Menu
    public Sprite[] SpriteSheet;
    public GameObject mainCanvas;

    [Header("Success Screen Components")]
    public bool testMode;
    public GameObject[] layouts;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "JLFailureMenu") {
            players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length > 0)
            {
                gravestones[players.Length - 1].SetActive(true);
                SetPlayerName(players);
            }
        }

        else if (SceneManager.GetActiveScene().name == "JLSuccessMenu") {

            foreach (GameObject g in layouts)
                g.SetActive(false);
            players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length >= 3)
            {
                if (testMode)
                    StartCoroutine(AnimateSetupSuccessScreen());
                else
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

    private void SetPlayerName(GameObject[] players) {
        GameObject graveyard = gravestones[players.Length - 1];
        int i = 0;
        foreach (GameObject player in players) {
            GameObject currGrave = graveyard.transform.GetChild(i).gameObject; //Obtaining the TMP child
            TextMeshProUGUI graveText = currGrave.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>(); //Obtaining the text component of child
            graveText.text = player.GetComponent<Player>().Name.Value + "";
            i++;
        }
    }

    private void GetPlayerRace(GameObject[] players) {
        GameObject playerField = playerSilhouettes[players.Length - 1];
        int i = 0;
        foreach (GameObject player in players) {
            GameObject currPlayer = playerField.transform.GetChild(i).gameObject; //Obtaining the TMP child
            if (player.GetComponent<Player>().Race.Value + "" == "Human") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[0];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "Dwarf") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[1];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "Centaur") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[2];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "Leonin") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[3];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "High Elf" || player.GetComponent<Player>().Race.Value + "" == "Night Elf") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[4];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "Aasimar") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[5];
            }
            else {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[0];
            }
            i++;
        }
    }

    public void quitButton() {
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

    IEnumerator AnimateSetupSuccessScreen()
    {
        string[] raceList = {"Human", "Dwarf", "Leonin", "Centaur", "High Elf", "Aasimar" };
        int counter = 0;
        for(; ; )
        {
            SetupSuccessScreen("Spectacular Tester", raceList[counter++%raceList.Length]);
            yield return new WaitForSeconds(2);
        }
    }

    public void SetupSuccessScreen(string name = default, string race = default)
    {
        GameObject layout = layouts[players.Length - 3];
        layout.SetActive(true);
        for(int i = 0; i < players.Length; i++)
        {
            layout.transform.GetChild(i).GetComponent<Image>().enabled = false;
            GameObject silhouetteCollection = layout.transform.GetChild(i).GetChild(0).gameObject;
            Player p = players[i].GetComponent<Player>();
            GameObject silhouette;

            if(name == default)
                name = p.Name.Value + "";
            if(race == default)
                race = p.Race.Value + "";

            for (int j = 0; j < silhouetteCollection.transform.childCount; j++)
                silhouetteCollection.transform.GetChild(j).gameObject.SetActive(false);

            switch (race)
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
            silhouette.GetComponentInChildren<TMP_Text>().text = name;
        }
    }
}
