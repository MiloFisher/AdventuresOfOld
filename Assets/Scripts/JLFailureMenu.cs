using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JLFailureMenu : MonoBehaviour
{
    public GameObject[] gravestones;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playerList = 3;

        gravestones[playerList - 1].SetActive(true);
        //gravestones[players.Length - 1].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayerName(GameObject[] players) {
        foreach (GameObject player in players) {
            
        }
    }

    public void quitButton() {
        SceneManager.LoadScene("JLMainMenu", LoadSceneMode.Single);
    }
}
