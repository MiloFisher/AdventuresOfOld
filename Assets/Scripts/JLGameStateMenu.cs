using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdventuresOfOldMultiplayer;
using TMPro;
using UnityEngine.UI;

public class JLGameStateMenu : MonoBehaviour
{
    public GameObject[] gravestones;
    public TextMeshProUGUI[] graveTexts;

    public Player[] players;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "JLFailureMenu") {
           //players = GameObject.FindGameObjectsWithTag("Player");
            int playerList = 3;
            
            gravestones[playerList - 1].SetActive(true);
            SetPlayerName(playerList);
            //gravestones[players.Length - 1].SetActive(true);
            //SetPlayerName(players);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetPlayerName(int players) {
        for (int i = 0; i <= players - 1; i++) {
            
        }
    }

    private void SetPlayerName(Player[] players) {
        foreach (Player player in players) {
            
        }
    }

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
    }
}
