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
        localPlayer.SetValue("Name", localPlayer.Username.Value + "");
        localPlayer.SetValue("Race", "Human");
        localPlayer.SetValue("Class", "Warrior");
        localPlayer.SetValue("Trait", "Highborn");
        localPlayer.SetValue("Gold", 10);
        localPlayer.SetValue("Level", 1);
        localPlayer.SetValue("XP", 0);
        localPlayer.SetValue("Strength", 15);
        localPlayer.SetValue("Dexterity", 15);
        localPlayer.SetValue("Intelligence", 15);
        localPlayer.SetValue("Speed", 20);
        localPlayer.SetValue("Constitution", 15);
        localPlayer.SetValue("Energy", 15);
        localPlayer.SetValue("Health", localPlayer.Constitution.Value * 2);
        localPlayer.SetValue("Image", "portrait_human");
        localPlayer.SetValue("Weapon", "empty");
        localPlayer.SetValue("Armor", "empty");
        localPlayer.SetValue("Ring1", "empty");
        localPlayer.SetValue("Ring2", "empty");
        localPlayer.SetValue("Inventory1", "empty");
        localPlayer.SetValue("Inventory2", "empty");
        localPlayer.SetValue("Inventory3", "empty");
        localPlayer.SetValue("Inventory4", "empty");
        localPlayer.SetValue("Inventory5", "empty");

        // If host, generate random characters for bots
        if (NetworkManager.Singleton.IsHost)
        {
            foreach (GameObject g in players)
            {
                Player p = g.GetComponent<Player>();
                if (p.isBot)
                {
                    p.SetValue("Name", p.Username.Value + "");
                    p.SetValue("Race", "Human");
                    p.SetValue("Class", "Warrior");
                    p.SetValue("Trait", "Highborn");
                    p.SetValue("Gold", 10);
                    p.SetValue("Level", 1);
                    p.SetValue("XP", 0);
                    p.SetValue("Strength", 10);
                    p.SetValue("Dexterity", 10);
                    p.SetValue("Intelligence", 10);
                    p.SetValue("Speed", 10);
                    p.SetValue("Constitution", 10);
                    p.SetValue("Energy", 10);
                    p.SetValue("Health", p.Constitution.Value);
                    p.SetValue("Image", "portrait_human");
                    p.SetValue("Armor", "empty");
                    p.SetValue("Ring1", "empty");
                    p.SetValue("Ring2", "empty");
                    p.SetValue("Inventory1", "empty");
                    p.SetValue("Inventory2", "empty");
                    p.SetValue("Inventory3", "empty");
                    p.SetValue("Inventory4", "empty");
                    p.SetValue("Inventory5", "empty");
                }
            }
        }

        // Leave scene
        if (NetworkManager.Singleton.IsServer)
        {
            localPlayer.ChangeScene("Core Game");
        }
    }
}
