using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using Unity.Collections;
using AdventuresOfOldMultiplayer;

public class PlayManager : Singleton<PlayManager>
{
    public Player localPlayer;
    public List<Player> playerList;
    public List<Player> turnOrderPlayerList;

    public Dictionary<Vector3Int, Tile> gameboard = new Dictionary<Vector3Int, Tile>();

    public Dictionary<string, LootCard> itemReference = new Dictionary<string, LootCard>();

    [SerializeField] private List<QuestCard> questDeck;
    public List<QuestCard> quests = new List<QuestCard>();

    [SerializeField] private List<MonsterCard> chapterBossDeck;
    public MonsterCard chapterBoss;

    [SerializeField] private MonsterCard[] miniBossObjects;
    public Dictionary<string, MonsterCard> miniBossDeck = new Dictionary<string, MonsterCard>();

    [SerializeField] private MonsterCard[] minionObjects;
    public Dictionary<string, MonsterCard> minionDeck = new Dictionary<string, MonsterCard>();

    public List<LootCard> equipmentDeck;

    [SerializeField] private LootCard[] lootCardObjects;
    public List<LootCard> lootDeck = new List<LootCard>();

    [SerializeField] private EncounterCard[] encounterCardObjects;
    public List<EncounterCard> encounterDeck = new List<EncounterCard>();

    public int chaosCounter;

    public GameObject transitions;

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

        // Construct Gameboard dictionary
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject g in tiles)
        {
            Tile t = g.GetComponent<Tile>();
            gameboard.Add(t.position, t);
        }

        // Construct Item Reference dictionary
        foreach (LootCard l in equipmentDeck)
        {
            itemReference.Add(l.cardName, l);
        }
        foreach (LootCard l in lootCardObjects)
        {
            itemReference.Add(l.cardName, l);
        }

        // Construct Miniboss and Minion dictionaries
        foreach (MonsterCard m in miniBossObjects)
        {
            miniBossDeck.Add(m.cardName, m);
        }
        foreach (MonsterCard m in minionObjects)
        {
            minionDeck.Add(m.cardName, m);
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

        // Ready up to let server know player has loaded in
        localPlayer.ReadyUp();

        // Disable Update() if client is not the host
        if (!NetworkManager.Singleton.IsHost)
        {
            enabled = false;
            return;
        }

        // Host needs to ready up any bots playing
        foreach(Player bot in playerList)
        {
            if (bot.isBot)
                bot.ReadyUp();
        }    

        // Otherwise, setup game
        if(PlayerPrefs.GetString("gameType") == "New Game")
            StartCoroutine(NewGameSetup());
        else
            LoadGameSetup();
    }

    private void Update()
    {

    }

    IEnumerator NewGameSetup()
    {
        // Wait for all players to load in first
        yield return new WaitUntil(() => {
            bool allReady = true;
            foreach (Player p in playerList)
            {
                if (!p.Ready.Value)
                    allReady = false;
            }
            return allReady;
        });
        foreach (Player p in playerList)
        {
            // 1) Set the Chaos Counter to 1 for (all players)
            p.SetChaosCounterClientRPC(1);

            // 2) Enable Treasure Tokens for (all players)
            p.EnableTreasureTokenClientRPC(new Vector3Int(3, 15, -18));
            p.EnableTreasureTokenClientRPC(new Vector3Int(5, 9, -14));
            p.EnableTreasureTokenClientRPC(new Vector3Int(3, 3, -6));
            p.EnableTreasureTokenClientRPC(new Vector3Int(8, -4, -4));
            p.EnableTreasureTokenClientRPC(new Vector3Int(7, 3, -10));
            p.EnableTreasureTokenClientRPC(new Vector3Int(8, 6, -14));
            p.EnableTreasureTokenClientRPC(new Vector3Int(16, 9, -25));
            p.EnableTreasureTokenClientRPC(new Vector3Int(21, 0, -21));
            p.EnableTreasureTokenClientRPC(new Vector3Int(15, -1, -14));
            p.EnableTreasureTokenClientRPC(new Vector3Int(16, -8, -8));

            // 7) Equip players with starting gear (all players
            switch (p.Class.Value + "")
            {
                case "Warrior":
                    p.SetValue("Armor", "Simple Plate Armor");
                    p.SetValue("Weapon", "Simple Sword & Shield");
                    break;
                case "Paladin":
                    p.SetValue("Armor", "Simple Plate Armor");
                    p.SetValue("Weapon", "Simple Greatsword");
                    break;
                case "Ranger":
                    p.SetValue("Armor", "Simple Leather Armor");
                    p.SetValue("Weapon", "Simple Bow");
                    break;
                case "Rogue":
                    p.SetValue("Armor", "Simple Leather Armor");
                    p.SetValue("Weapon", "Simple Daggers");
                    break;
                case "Sorcerer":
                    p.SetValue("Armor", "Simple Cloth Robes");
                    p.SetValue("Weapon", "Simple Magic Staff");
                    break;
                case "Necromancer":
                    p.SetValue("Armor", "Simple Cloth Robes");
                    p.SetValue("Weapon", "Simple Wand & Shield");
                    break;
            }
        }

        // 3) Deal Quest Cards (host only)
        ShuffleDeck(questDeck);
        for(int i = 0; i < Mathf.FloorToInt(playerList.Count/2); i++)
        {
            quests.Add(questDeck[i]);
        }

        // 4) Deal Chapter Boss Card (host only)
        ShuffleDeck(chapterBossDeck);
        chapterBoss = chapterBossDeck[0];

        // 5) Setup Loot and Encounter Decks (host only)
        foreach (LootCard lc in lootCardObjects)
        {
            for (int i = 0; i < lc.copies; i++) // Add copies depending on amount specified
                lootDeck.Add(lc);
        }
        ShuffleDeck(lootDeck);
        foreach (EncounterCard ec in encounterCardObjects)
        {
            for (int i = 0; i < 4; i++) // Add 4 copies of each encounter card
                encounterDeck.Add(ec);
        }
        ShuffleDeck(encounterDeck);

        // 6) Miniboss and minion decks already set up

        // Give each player a color (host only)
        string[] colorList = { "red", "blue", "green", "purple", "yellow", "orange" };
        for(int i = 0; i < playerList.Count; i++)
        {
            playerList[i].SetValue("Color", colorList[i]);
        }

        // Begin Game with Start of Day (host only)
        StartOfDay();
    }

    private void LoadGameSetup()
    {

    }

    private void ShuffleDeck<T>(List<T> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            T temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void StartOfDay()
    {
        // Calculate turn order player list
        turnOrderPlayerList = new List<Player>(playerList);
        ShuffleDeck(turnOrderPlayerList);
        turnOrderPlayerList.Sort((a, b) => b.Speed.Value - a.Speed.Value);

        FixedString64Bytes[] arr = new FixedString64Bytes[turnOrderPlayerList.Count];
        for (int i = 0; i < turnOrderPlayerList.Count; i++)
        {
            arr[i] = turnOrderPlayerList[i].UUID.Value;
        }

        foreach (Player p in playerList)
        {
            p.SetTurnOrderPlayerListClientRPC(arr);
            p.PlayTransitionClientRPC(0); // Transition 0 is Start of Day
        }
    }

    public void StartTurn()
    {

    }

    public void CallTransition(int id)
    {
        transitions.transform.GetChild(id).gameObject.SetActive(true);
    }

    public void SetTurnOrderPlayerList(FixedString64Bytes[] arr)
    {
        turnOrderPlayerList = new List<Player>();
        for(int i = 0; i < arr.Length; i++)
        {
            foreach(Player p in playerList)
            {
                if(p.UUID.Value == arr[i])
                {
                    turnOrderPlayerList.Add(p);
                }
            }
        }
    }
}