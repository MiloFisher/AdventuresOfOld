using UnityEngine;
using UnityEngine.SceneManagement;
using AdventuresOfOldMultiplayer;
using TMPro;

public class JLGameStateMenu : MonoBehaviour
{
    public GameObject[] gravestones;

    public GameObject[] players;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "JLFailureMenu") {
            //int playerList = 6;
            //gravestones[playerList - 1].SetActive(true);
            //SetPlayerName(playerList);
            
            players = GameObject.FindGameObjectsWithTag("Player");
            gravestones[players.Length - 1].SetActive(true);
            SetPlayerName(players);
        }
    }

    /*private void SetPlayerName(int players) {
        GameObject graveyard = gravestones[players - 1];
        for (int i = 0; i <= players - 1; i++) {
            GameObject currGrave = graveyard.transform.GetChild(i).gameObject;
            TextMeshProUGUI graveText = currGrave.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            graveText.text = i + "";
        }
    }*/

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

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
    }
}
