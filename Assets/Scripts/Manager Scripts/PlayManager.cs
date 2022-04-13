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

    public Dictionary<Vector3Int, Tile> gameboard = new Dictionary<Vector3Int, Tile>();

    [SerializeField] private List<QuestCard> questDeck;
    public List<QuestCard> quests = new List<QuestCard>();

    [SerializeField] private List<MonsterCard> chapterBossDeck;
    public MonsterCard chapterBoss;

    public List<MonsterCard> miniBossDeck;

    public List<MonsterCard> minionDeck;

    [SerializeField] private LootCard[] lootCardObjects;
    public List<LootCard> lootDeck = new List<LootCard>();

    [SerializeField] private EncounterCard[] encounterCardObjects;
    public List<EncounterCard> encounterDeck = new List<EncounterCard>();

    public int chaosCounter;

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
        BaseGameSetup();
        if(PlayerPrefs.GetString("gameType") == "New Game")
            NewGameSetup();
        else
            LoadGameSetup();
    }

    private void Update()
    {

    }

    private void BaseGameSetup()
    {
        // Construct Gameboard dictionary
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject g in tiles)
        {
            Tile t = g.GetComponent<Tile>();
            gameboard.Add(t.position, t);
        }

        // Run tests to find neighbors for each tile
        Vector3Int test1 = new Vector3Int(0, 1, -1);
        Vector3Int test2 = new Vector3Int(0, -1, 1);
        Vector3Int test3 = new Vector3Int(1, 0, -1);
        Vector3Int test4 = new Vector3Int(-1, 0, 1);
        Vector3Int test5 = new Vector3Int(1, -1, 0);
        Vector3Int test6 = new Vector3Int(-1, 1, 0);
        foreach (GameObject g in tiles)
        {
            Tile t = g.GetComponent<Tile>();
            if (gameboard.ContainsKey(t.position + test1)) t.neighbors.Add(gameboard[t.position + test1]);
            if (gameboard.ContainsKey(t.position + test2)) t.neighbors.Add(gameboard[t.position + test2]);
            if (gameboard.ContainsKey(t.position + test3)) t.neighbors.Add(gameboard[t.position + test3]);
            if (gameboard.ContainsKey(t.position + test4)) t.neighbors.Add(gameboard[t.position + test4]);
            if (gameboard.ContainsKey(t.position + test5)) t.neighbors.Add(gameboard[t.position + test5]);
            if (gameboard.ContainsKey(t.position + test6)) t.neighbors.Add(gameboard[t.position + test6]);
        }

    }

    private void NewGameSetup()
    {
        // 1) Set the Chaos Counter to 1
        chaosCounter = 1;

        // 2) Enable Treasure Tokens
        gameboard[new Vector3Int(3, 15, -18)].EnableTreasureToken();
        gameboard[new Vector3Int(5, 9, -14)].EnableTreasureToken();
        gameboard[new Vector3Int(3, 3, -6)].EnableTreasureToken();
        gameboard[new Vector3Int(8, -4, -4)].EnableTreasureToken();
        gameboard[new Vector3Int(7, 3, -10)].EnableTreasureToken();
        gameboard[new Vector3Int(8, 6, -14)].EnableTreasureToken();
        gameboard[new Vector3Int(16, 9, -25)].EnableTreasureToken();
        gameboard[new Vector3Int(21, 0, -21)].EnableTreasureToken();
        gameboard[new Vector3Int(15, -1, -14)].EnableTreasureToken();
        gameboard[new Vector3Int(16, -8, -8)].EnableTreasureToken();

        // 3) Deal Quest Cards
        ShuffleDeck(questDeck);
        for(int i = 0; i < Mathf.FloorToInt(playerList.Count/2); i++)
        {
            quests.Add(questDeck[i]);
        }

        // 4) Deal Chapter Boss Card
        ShuffleDeck(chapterBossDeck);
        chapterBoss = chapterBossDeck[0];

        // 5) Setup Loot and Encounter Decks
        foreach (LootCard lc in lootCardObjects)
        {
            for (int i = 0; i < lc.copies; i++) // Add copies depending on amount specified
                lootDeck.Add(lc);
        }
        ShuffleDeck(encounterDeck);
        foreach (EncounterCard ec in encounterCardObjects)
        {
            for (int i = 0; i < 4; i++) // Add 4 copies of each encounter card
                encounterDeck.Add(ec);
        }
        ShuffleDeck(encounterDeck);
    }

    private void LoadGameSetup()
    {

    }

    private void ShuffleDeck<CardType>(List<CardType> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            CardType temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}