using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using AdventuresOfOldMultiplayer;

public class PlayManager : Singleton<PlayManager>
{
    public Player localPlayer;
    public List<Player> playerList;

    [SerializeField] private EncounterCard[] encounterCardObjects;
    public List<EncounterCard> encounterDeck = new List<EncounterCard>();

    void Start()
    {
        // Populates the player list
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");

        // Exits game if no players found
        if(p.Length == 0)
        {
            Debug.LogError("No players found! Did you mean to load this scene directly?");
            return;
        }

        // Orders the player list and locates the local player
        Array.Sort(p, (a, b) => (int)a.GetComponent<Player>().NetworkObjectId - (int)b.GetComponent<Player>().NetworkObjectId);
        foreach (GameObject g in p)
        {
            playerList.Add(g.GetComponent<Player>());
            if (g.GetComponent<NetworkObject>().IsLocalPlayer)
                localPlayer = g.GetComponent<Player>();
        }

        // Disable Update() if client is not the host
        if(!NetworkManager.Singleton.IsHost)
        {
            enabled = false;
            return;
        }

        // Otherwise, setup game
        InitialGameSetup();
    }

    private void Update()
    {

    }

    private void InitialGameSetup()
    {
        // Add 4 copies of each encounter card to the encounter deck
        foreach(EncounterCard ec in encounterCardObjects)
        {
            for (int i = 0; i < 4; i++)
                encounterDeck.Add(ec);
        }
        // Shuffle encounter deck

    }
}