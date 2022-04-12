using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;

public class CharacterCreationManager : MonoBehaviour
{
    Player localPlayer;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Gets local player
        foreach(GameObject p in players)
        {
            if (p.GetComponent<Player>().IsLocalPlayer)
            {
                localPlayer = p.GetComponent<Player>();
            }
        }

        // Set Variable
        localPlayer.SetValue("Name", "Bob");
        localPlayer.SetValue("Gold", 100);

        // Leave scene
        if (NetworkManager.Singleton.IsServer)
        {
            localPlayer.ChangeScene("Core Game");
        }
    }
}
