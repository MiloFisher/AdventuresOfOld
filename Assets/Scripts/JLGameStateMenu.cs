using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;
using TMPro;

public class JLGameStateMenu : MonoBehaviour
{
    private GameObject[] players;
    public GameObject[] gravestones; // For Failure Menu
    public GameObject[] playerSilhouettes; // For Success Menu
    
    public Sprite[] SpriteSheet;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "JLFailureMenu") {
            players = GameObject.FindGameObjectsWithTag("Player");
            gravestones[players.Length - 1].SetActive(true);
            SetPlayerName(players);
        }

        else if (SceneManager.GetActiveScene().name == "JLSuccessMenu") {      
            players = GameObject.FindGameObjectsWithTag("Player");
            playerSilhouettes[players.Length - 1].SetActive(true);
            GetPlayerRace(players);
        }
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
            else if (player.GetComponent<Player>().Race.Value + "" == "Leonin" || player.GetComponent<Player>().Race.Value + "" == "Aasimar") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[3];
            }
            else if (player.GetComponent<Player>().Race.Value + "" == "High Elf" || player.GetComponent<Player>().Race.Value + "" == "Night Elf") {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[4];
            }
            else {
                currPlayer.GetComponent<Image>().sprite = SpriteSheet[0];
            }
            i++;
        }
    }

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
    }
}
