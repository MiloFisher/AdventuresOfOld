using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using Unity.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using AdventuresOfOldMultiplayer;
using System.Security.Cryptography;
using System.Text;

public class PlayManager : Singleton<PlayManager>
{
    public Player localPlayer;
    public List<Player> playerList;
    public List<Player> turnOrderPlayerList;

    public Dictionary<Vector3Int, Tile> gameboard = new Dictionary<Vector3Int, Tile>();

    public Dictionary<string, LootCard> itemReference = new Dictionary<string, LootCard>();

    public Dictionary<string, EncounterCard> encounterReference = new Dictionary<string, EncounterCard>();

    public Dictionary<string, QuestCard> questReference = new Dictionary<string, QuestCard>();

    [SerializeField] private List<QuestCard> questDeck;
    public List<QuestCard> quests = new List<QuestCard>();

    [SerializeField] public List<MonsterCard> chapterBossDeck;
    public MonsterCard chapterBoss;

    [SerializeField] private MonsterCard[] miniBossObjects;
    public Dictionary<string, MonsterCard> miniBossDeck = new Dictionary<string, MonsterCard>();

    [SerializeField] private MonsterCard[] minionObjects;
    public Dictionary<string, MonsterCard> minionDeck = new Dictionary<string, MonsterCard>();

    public Dictionary<FixedString64Bytes, Sprite> portaitDictionary = new Dictionary<FixedString64Bytes, Sprite>();

    public List<LootCard> specialItems;
    public List<LootCard> equipmentDeck;

    [SerializeField] private LootCard[] lootCardObjects;
    public List<LootCard> lootDeck = new List<LootCard>();

    [SerializeField] private EncounterCard[] encounterCardObjects;
    public List<EncounterCard> encounterDeck = new List<EncounterCard>();

    public GameObject chaosMarker;

    public int chaosCounter;

    public int turnMarker;

    public bool isYourTurn;

    public GameObject transitions;

    public GameObject encounterElements;

    public GameObject endOfDayElements;

    public GameObject utilityMenus;

    public GameObject notifications;

    public GameObject[] playerPieces;

    public GameObject[] characterPanels;

    public GameObject characterTurnSelector;
    public GameObject characterDisplays;
    public GameObject characterDisplayMinimizeButton;
    public float minimizedX = -1600f;
    public float maximizedX = 518f;
    public float characterSheetOpenLength = 0.004f;
    public Player selectedPlayer;
    public bool characterDisplayOpen;

    public Sprite[] portraits;

    public GameObject loadingScreen;

    public GameObject questDisplay;

    public Color[] playerColors;

    // *** Testing Only ***
    public EncounterCard testingCard;
    public QuestCard testingCard1;

    public bool startOrEndOfDay;

    public bool movePhase;

    public bool inStore;

    public Player playerMovingWithYou;

    void Start()
    {
        // Activate loadingScreen
        loadingScreen.SetActive(true);

        // Populates the player list
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");

        // Exits game if no players found
        if (p.Length == 0)
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
        foreach (LootCard l in specialItems)
        {
            itemReference.Add(l.cardName, l);
        }
        foreach (LootCard l in equipmentDeck)
        {
            itemReference.Add(l.cardName, l);
        }
        foreach (LootCard l in lootCardObjects)
        {
            itemReference.Add(l.cardName, l);
        }

        // Construct Encounter Reference dictionary
        foreach (EncounterCard e in encounterCardObjects)
        {
            encounterReference.Add(e.cardName, e);
        }

        // Construct Quest Reference dictionary
        foreach (QuestCard q in questDeck)
        {
            questReference.Add(q.cardName, q);
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

        // Set up portrait dictionary
        foreach (Sprite s in portraits)
        {
            portaitDictionary.Add(s.name, s);
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

        // Stop here if client is not the host
        if (!NetworkManager.Singleton.IsHost)
        {
            return;
        }

        // Host needs to ready up any bots playing
        foreach (Player bot in playerList)
        {
            if (bot.isBot)
                bot.ReadyUp();
        }

        // Otherwise, setup game
        if (PlayerPrefs.GetString("gameType") == "New Game")
            StartCoroutine(NewGameSetup());
        else
            StartCoroutine(LoadGameSetup());
    }

    private void Update()
    {
        if (!loadingScreen.activeInHierarchy)
        {
            DrawPlayerPieces();
            UpdateCharacterPanels();
            if ((!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost) || PlayerDisconnected())
            {
                DisconnectFromGame();
            }
            // Game Over Check
            if(NetworkManager.Singleton.IsHost && !CombatManager.Instance.InCombat())
            {
                // Lose Check
                bool someoneAlive = false;
                foreach(Player p in playerList)
                {
                    if (GetHealth(p) > 0)
                        someoneAlive = true;
                }
                if (!someoneAlive)
                    GameOver(0);
            }
        }
    }

    // (Host Only)
    public void GameOver(int state)
    {
        // Lose = 0
        // Win = 1

        // Delete game save on game over
        DataManager d = new DataManager();
        d.DeleteSaveFile("GameData");

        if (state == 0)
        {
            localPlayer.ChangeScene("JLFailureMenu");
        }
        else
        {
            localPlayer.ChangeScene("JLSuccessMenu");
        }
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

        // 3) Deal Quest Cards (host only)
        ShuffleDeck(questDeck);
        for (int i = 0; i < Mathf.FloorToInt(playerList.Count / 2); i++)
        {
            quests.Add(questDeck[i]);
        }

        // Update Quest Cards
        localPlayer.UpdateQuests(quests);

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

            // If player is human, give them 2 level up points
            if (p.Race.Value == "Human")
                p.SetValue("LevelUpPoints", 2);

            EasterEggCheck(p);

            // Fill all of their ability charges
            p.RestoreAbilityCharges(999, true, true);

            // Set their health to full
            p.RestoreHealth(999, true, true);

            // Set player positions to starting tile
            p.SetPosition(new Vector3Int(0, 7, -7), true);
        }

        // 4) Deal Chapter Boss Card (host only)
        ShuffleDeck(chapterBossDeck);
        localPlayer.SetBoss(chapterBossDeck[0].cardName);

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

        // *** Testing Only ***
        if(testingCard != default)
            encounterDeck[0] = testingCard;
        if (testingCard1 != default)
        {
            quests[0] = testingCard1;
            localPlayer.UpdateQuests(quests);
        }

        // 6) Miniboss and minion decks already set up

        // Give each player a color (host only)
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].SetValue("Color", i);
        }

        yield return new WaitForSeconds(1);

        // Start background music
        JLAudioManager.Instance.PlaySound("BackgroundMusic");

        // Begin Game with Start of Day (host only)
        StartOfDay();
    }

    IEnumerator LoadGameSetup()
    {
        DataManager d = new DataManager();
        SaveFile s = d.GetSaveFile("GameData", true);

        // Load playerList
        List<Player> copy = new List<Player>(playerList);
        playerList.Clear();
        foreach(PlayerData p in s.playerList)
        {
            Player player = copy.Find((a) => a.UUID.Value == p.UUID);
            player.SetValue("Image", p.Image + "");
            player.SetValue("Name", p.Name + "");
            player.SetValue("Race", p.Race + "");
            player.SetValue("Class", p.Class + "");
            player.SetValue("Trait", p.Trait + "");
            player.SetValue("Gold", p.Gold);
            player.SetValue("Level", p.Level);
            player.SetValue("XP", p.XP);
            player.SetValue("Strength", p.Strength);
            player.SetValue("Dexterity", p.Dexterity);
            player.SetValue("Intelligence", p.Intelligence);
            player.SetValue("Speed", p.Speed);
            player.SetValue("Constitution", p.Constitution);
            player.SetValue("Energy", p.Energy);
            player.SetValue("Health", p.Health);
            player.SetValue("AbilityCharges", p.AbilityCharges);
            player.SetValue("Armor", p.Armor + "");
            player.SetValue("Weapon", p.Weapon + "");
            player.SetValue("Ring1", p.Ring1 + "");
            player.SetValue("Ring2", p.Ring2 + "");
            player.SetValue("Inventory1", p.Inventory1 + "");
            player.SetValue("Inventory2", p.Inventory2 + "");
            player.SetValue("Inventory3", p.Inventory3 + "");
            player.SetValue("Inventory4", p.Inventory4 + "");
            player.SetValue("Inventory5", p.Inventory5 + "");
            player.SetValue("LevelUpPoints", p.LevelUpPoints);
            player.SetValue("FailedEncounters", p.FailedEncounters);
            player.SetPosition(p.Position);
            //player.SetValue("TurnPhase", p.TurnPhase);
            player.SetValue("Color", p.Color);
            //player.SetValue("Ready", p.Ready);
            player.ReadyUp();
            player.SetValue("EndOfDayActivity", p.EndOfDayActivity);
            player.SetValue("ParticipatingInCombat", p.ParticipatingInCombat);
            player.SetValue("HasBathWater", p.HasBathWater);
            player.SetValue("BetrayedBoy", p.BetrayedBoy);
            player.SetValue("GrabbedHorse", p.GrabbedHorse);
            player.SetValue("KilledGoblin", p.KilledGoblin);
            player.SetValue("RequestedTaunt", p.RequestedTaunt);
            player.SetValue("IronWill", p.IronWill);
            player.SetValue("HasYetToAttack", p.HasYetToAttack);
            player.SetValue("JusticarsVow", p.JusticarsVow);
            player.SetValue("SuccessfullyAttackedMonster", p.SuccessfullyAttackedMonster);

            playerList.Add(player);
        }

        // Load turn order playerlist
        turnOrderPlayerList = new List<Player>();
        foreach (PlayerData p in s.playerList)
        {
            turnOrderPlayerList.Add(playerList.Find((a) => a.UUID.Value == p.UUID));
        }

        // Load quests
        quests = s.quests;
        localPlayer.UpdateQuests(quests);

        foreach (Player p in playerList)
        {
            // Load the Chaos Counter for (all players)
            p.SetChaosCounterClientRPC(s.chaosCounter);

            // Load Treasure Tokens for (all players)
            for(int i = 0; i < s.treasureTiles.Count; i++)
                p.EnableTreasureTokenClientRPC(s.treasureTiles[i]);
        }

        // Load chapter boss
        localPlayer.SetBoss(s.chapterBoss.cardName);

        // Load encounter deck
        encounterDeck = s.encounterDeck;

        // Load loot deck
        lootDeck = s.lootDeck;

        yield return new WaitUntil(() => {
            bool allReady = true;
            foreach (Player p in playerList)
            {
                if (!p.Ready.Value)
                    allReady = false;
            }
            return allReady;
        });

        yield return new WaitForSeconds(1);

        // Start background music
        JLAudioManager.Instance.PlaySound("BackgroundMusic");

        if(s.turnMarker == 0)
        {
            StartOfDay();
        }
        else
        {
            FixedString64Bytes[] arr = new FixedString64Bytes[turnOrderPlayerList.Count];
            for (int i = 0; i < turnOrderPlayerList.Count; i++)
            {
                arr[i] = turnOrderPlayerList[i].UUID.Value;
            }
            foreach (Player p in playerList)
            {
                // Close loading screen for all players
                p.CloseLoadingScreenClientRPC();
                // Reset ready up on players
                p.Unready();
                // Setup and draw player pieces
                p.SetupPlayerPiecesClientRPC();
                //p.DrawPlayerPiecesClientRPC();
                // Set turn order player list and turn marker
                p.SetTurnOrderPlayerListClientRPC(arr);
                p.SetTurnMarkerClientRPC(turnMarker);
                // Setup and update character panels
                p.SetupCharacterPanelsClientRPC();
            }

            if (s.turnMarker < s.playerList.Count)
            {
                turnOrderPlayerList[turnMarker].StartTurnClientRPC();
            }
            else
            {
                localPlayer.EndDayForPlayers();
            }
        }
    }

    public void SetBoss(string cardName)
    {
        foreach(MonsterCard m in chapterBossDeck)
        {
            if (m.cardName == cardName)
                chapterBoss = m;
        }
    }

    public void ShuffleDeck<T>(List<T> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            T temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // (Host Only)
    public void StartOfDay()
    {
        // Calculate turn order player list
        turnOrderPlayerList = new List<Player>(playerList);
        ShuffleDeck(turnOrderPlayerList);
        turnOrderPlayerList.Sort((a, b) => GetSpeed(b) - GetSpeed(a));
        FixedString64Bytes[] arr = new FixedString64Bytes[turnOrderPlayerList.Count];
        for (int i = 0; i < turnOrderPlayerList.Count; i++)
        {
            arr[i] = turnOrderPlayerList[i].UUID.Value;
        }

        // Set turn marker
        turnMarker = 0;

        foreach (Player p in playerList)
        {
            // Close loading screen for all players
            p.CloseLoadingScreenClientRPC();
            // Reset ready up on players
            p.Unready();
            // Setup and draw player pieces
            p.SetupPlayerPiecesClientRPC();
            //p.DrawPlayerPiecesClientRPC();
            // Set turn order player list and turn marker
            p.SetTurnOrderPlayerListClientRPC(arr);
            p.SetTurnMarkerClientRPC(turnMarker);
            // Setup and update character panels
            p.SetupCharacterPanelsClientRPC();
            // Play transition for all players
            p.PlayTransitionClientRPC(0); // Transition 0 is Start of Day
        }

        SaveGame();

        // Start turn of player who goes first
        turnOrderPlayerList[turnMarker].StartTurnClientRPC();
    }

    public void EndOfDay()
    {
        // Call Choose Activity prompt which is element 0
        startOrEndOfDay = true;
        CallEndOfDayElement(0);
    }

    // (Host Only)
    public void SendPlayersToEndOfDayActivities()
    {
        Player storeMaster = null;
        foreach (Player p in playerList)
        {
            p.Unready();
            switch (p.EndOfDayActivity.Value)
            {
                case 0:
                    storeMaster = p;
                    break;
                case 1:
                    p.TakeShortRestClientRPC();
                    break;
                case 2:
                    p.TakeLongRestClientRPC();
                    break;
                case 3:
                    p.GoToShrineClientRPC();
                    break;
                case 4:
                    // Is Dead
                    p.ReadyUp();
                    break;
            }
        }

        if (storeMaster != null)
            storeMaster.SetupStore();

        StartCoroutine(WaitForPlayersToEndDay());
    }

    public void TakeShortRest(Player p)
    {
        // Restores 1 Ability Charge and recovers Health equal to 3x the Chaos Tier
        int ac = 1;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Nature's Charm")))
            ac++;
        p.RestoreAbilityCharges(ac);
        p.RestoreHealth(3 * ChaosTier());

        p.ReadyUp();            
    }

    public void TakeLongRest(Player p)
    {
        // Fully restores a Character’s Ability Charges and Health.  Increases the Chaos Counter by 1 for every player that chooses this action
        p.RestoreAbilityCharges(999);
        p.RestoreHealth(999);
        p.IncreaseChaos(1);

        p.ReadyUp();
    }

    public void GoToShrine(Player player)
    {
        if(!player.isBot)
        {
            TargetPlayerSelection("Choose Players to Revive", false, true, false, (p) => {
                // This player spends gold and target player is revived
                player.LoseGold(50);
                p.Resurrect();
            }, (p) => {
                // Requirement is this player has 50 Gold and player is dead
                return GetGold(player) >= 50 && GetHealth(p) <= 0;
            }, true, "Leave Shrine", () => {
                // On Leave
                player.ReadyUp();
            });
        }
        else
        {
            player.ReadyUp();
        }
    }

    IEnumerator WaitForPlayersToEndDay()
    {
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
            p.Unready();
        }
        localPlayer.IncreaseChaos(1);
        StartOfDay();
    }

    public void StartTurn()
    {
        // Set the variable to mark it is this player's turn
        isYourTurn = true;
        if (!transitions.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if (GetHealth(localPlayer) > 0)
                CallTransition(1);
            else
                EndTurn();
        }
    }

    public void StartBotTurn()
    {
        EndBotTurn();
    }

    public void EndTurn()
    {
        // Deactivate variable marking your turn
        isYourTurn = false;

        // Increment turn marker for all players
        turnMarker++;
        localPlayer.UpdateTurnMarker(turnMarker);

        // Start next players turn or transition to end of day if turn marker is out of bounds
        if (turnMarker < turnOrderPlayerList.Count)
            localPlayer.StartNextPlayerTurn(turnMarker);
        else
            localPlayer.EndDayForPlayers();
    }

    public void EndBotTurn()
    {
        // Increment turn marker for all players
        turnMarker++;
        localPlayer.UpdateTurnMarker(turnMarker);

        // Start next players turn or transition to end of day if turn marker is out of bounds
        if (turnMarker < turnOrderPlayerList.Count)
            localPlayer.StartNextPlayerTurn(turnMarker);
        else
            localPlayer.EndDayForPlayers();
    }

    public void MovePhase(Vector3Int pos = default)
    {
        playerMovingWithYou = default;
        movePhase = true;
        // Activate all tiles within the player's move range
        if (pos == default)
        {
            if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Horseback Riding")) && HasAllyOnTileOrAdjacent(localPlayer))
            {
                MakeChoice("Take ally on move", "Move alone", true, true);
                ChoiceListener((a) => {
                    if (a == 1)
                    {
                        TargetPlayerSelection("Choose ally to take with", true, false, false, (p) => {
                            // On Select
                            playerMovingWithYou = p;
                            gameboard[localPlayer.Position.Value].Activate(GetMod(GetSpeed(localPlayer)));
                        }, (p) => {
                            // Requirement is being on the same or adjacent tile to localplayer
                            return InAssistingRange(localPlayer, p, 1);
                        }, false);
                    }
                    else
                    {
                        gameboard[localPlayer.Position.Value].Activate(GetMod(GetSpeed(localPlayer)));
                    }
                });
            }
            else
                gameboard[localPlayer.Position.Value].Activate(GetMod(GetSpeed(localPlayer)));
        }
        else
            gameboard[pos].Activate(GetMod(GetSpeed(localPlayer)));
    }

    public bool HasAllyOnTileOrAdjacent(Player p)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (p.UUID.Value != playerList[i].UUID.Value && InAssistingRange(p, playerList[i], 0))
                return true;
        }
        return false;
    }

    public void EncounterPhase()
    {
        // First Parse tile landed on
        Vector3Int tilePos = gameboard[localPlayer.Position.Value].position;

        // Check Boss tile first
        if (tilePos == new Vector3Int(17, 7, -24))
            BossFight();

        // Check NPC tiles next
        else if (tilePos == new Vector3Int(13, 0, -13))
            CrazedHermit();
        else if (tilePos == new Vector3Int(15, -4, -11))
            DistressedVillager();
        else if (tilePos == new Vector3Int(7, 13, -20))
            ForestHag();
        else if (tilePos == new Vector3Int(10, 8, -18))
            FourEyedBoy();
        else if (tilePos == new Vector3Int(5, 7, -12))
            PabloTheNoob();
        else if (tilePos == new Vector3Int(1, 1, -2))
            ShiftyPeddler();
        else if (tilePos == new Vector3Int(18, 4, -22))
            SuspiciousHorse();
        else if (tilePos == new Vector3Int(16, 1, -17))
            VeteranHunter();

        // Check Location tiles next
        else if (tilePos == new Vector3Int(14, 3, -17))
            AbandonedOutpost();
        else if (tilePos == new Vector3Int(21, -2, -19))
            AncientSpring();
        else if (tilePos == new Vector3Int(0, 4, -4))
            BanditHideout();
        else if (tilePos == new Vector3Int(11, 11, -22))
            HowlingCave();
        else if (tilePos == new Vector3Int(1, 15, -16))
            OminousClearing();
        else if (tilePos == new Vector3Int(10, -3, -7))
            OvergrownTemple();
        else if (tilePos == new Vector3Int(8, 4, -12))
            WebbedForest();

        // Check Treasure tile
        else if (gameboard[localPlayer.Position.Value].TreasureTokenIsEnabled())
            TreasureTile();

        // Else it's a Default tile
        else
            DefaultTile();
    }

    public void ProcessEncounterRoll(int roll)
    {
        if (roll % 2 == 0)
            GetEncounter();
        else
        {
            localPlayer.SetValue("FailedEncounters", localPlayer.FailedEncounters.Value + 1);
            EndTurn();
        }
    }

    public void CloseLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

    public void MoveToTile(Vector3Int pos)
    {
        movePhase = false;

        // Deactivate the selected tiles and move the player to the target position
        HideAllTiles();
        localPlayer.SetPosition(pos);
        if (playerMovingWithYou != default)
        {
            playerMovingWithYou.SetPosition(pos);
            playerMovingWithYou = default;
        }

        // Call Encounter Phase transition
        CallTransition(2);
    }

    public void HideAllTiles()
    {
        foreach (KeyValuePair<Vector3Int, Tile> t in gameboard)
            t.Value.Deactivate();
    }

    public void CallTransition(int id)
    {
        transitions.transform.GetChild(id).gameObject.SetActive(true);
        InventoryManager.Instance.HideOptions();
    }

    public void CallEncounterElement(int id)
    {
        encounterElements.transform.GetChild(id).gameObject.SetActive(true);
    }

    public void CallEndOfDayElement(int id)
    {
        endOfDayElements.transform.GetChild(id).gameObject.SetActive(true);
    }

    public void TargetPlayerSelection(string bannerDisplayText, bool closeAfterSelect, bool updatePlayerDisplays, bool includeSelf, Action<Player> OnSelect, Func<Player, bool> MeetsRequirement, bool showCancelButton, string cancelButtonText = "Cancel", Action OnClose = default)
    {
        utilityMenus.transform.GetChild(0).GetComponent<UITargetPlayer>().Setup(bannerDisplayText, closeAfterSelect, updatePlayerDisplays, includeSelf, OnSelect, MeetsRequirement, showCancelButton, cancelButtonText, OnClose);
    }

    public void SetTurnOrderPlayerList(FixedString64Bytes[] arr)
    {
        turnOrderPlayerList = new List<Player>();
        for (int i = 0; i < arr.Length; i++)
        {
            foreach (Player p in playerList)
            {
                if (p.UUID.Value == arr[i])
                {
                    turnOrderPlayerList.Add(p);
                }
            }
        }
    }

    public void SetupPlayerPieces()
    {
        int i;
        for (i = 0; i < playerList.Count; i++)
        {
            playerPieces[i].SetActive(true);
            playerPieces[i].GetComponent<Image>().color = GetPlayerColor(playerList[i]);
        }
        for (; i < 6; i++)
        {
            playerPieces[i].SetActive(false);
        }
    }

    public void SetupCharacterPanels()
    {
        int i;
        for (i = 0; i < turnOrderPlayerList.Count; i++)
        {
            characterPanels[i].SetActive(true);
            characterPanels[i].transform.localPosition = new Vector3(characterPanels[i].transform.localPosition.x, 114.5f * (turnOrderPlayerList.Count - 2 * i - 1), 0);
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateCharacterImage(portaitDictionary[turnOrderPlayerList[i].Image.Value]);
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateCharacterName(turnOrderPlayerList[i].Name.Value + "", GetPlayerColorString(turnOrderPlayerList[i]));
        }
        for (; i < 6; i++)
        {
            characterPanels[i].SetActive(false);
        }
    }

    public void UpdateCharacterPanels()
    {
        for (int i = 0; i < turnOrderPlayerList.Count; i++)
        {
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateHealthbar(GetHealth(turnOrderPlayerList[i]), GetMaxHealth(turnOrderPlayerList[i]));
        }
        characterTurnSelector.SetActive(turnMarker < playerList.Count);
        if(characterTurnSelector.activeInHierarchy)
            characterTurnSelector.transform.localPosition = characterPanels[turnMarker].transform.localPosition;
    }

    //public Color ColorLookUp(string color)
    //{
    //    switch (color)
    //    {
    //        case "red": return Color.red;
    //        case "blue": return Color.blue;
    //        case "green": return Color.green;
    //        case "purple": return new Color(0.627f, 0, 1);
    //        case "yellow": return Color.yellow;
    //        case "orange": return new Color(1, 0.627f, 0);
    //        default: return Color.black;
    //    }
    //}

    public void DrawPlayerPieces()
    {
        int playersOnTile;
        int positionOnTile;
        for (int i = 0; i < playerList.Count; i++)
        {
            playersOnTile = 0;
            positionOnTile = 0;
            for (int j = 0; j < playerList.Count; j++)
            {
                if (playerList[i].Position.Value == playerList[j].Position.Value)
                {
                    if (i == j)
                        positionOnTile = playersOnTile;
                    playersOnTile++;
                }
            }
            playerPieces[i].transform.localPosition = gameboard[playerList[i].Position.Value].transform.localPosition + new Vector3(0, 2.1f * positionOnTile - 2.1f * (playersOnTile - (positionOnTile + 1)), 0);
        }
    }

    public void SelectPortrait(int id)
    {
        selectedPlayer = turnOrderPlayerList[id];
        characterDisplayMinimizeButton.SetActive(true);

        if (!characterDisplayOpen)
        {
            characterDisplayOpen = true;
            StartCoroutine(OpenPlayerSheet());
        }
    }

    IEnumerator OpenPlayerSheet()
    {
        for (int i = 1; i <= Global.animSteps; i++)
        {
            characterDisplays.transform.localPosition = new Vector3(minimizedX + (maximizedX - minimizedX) * i * Global.animRate, 0, 0);
            yield return new WaitForSeconds(characterSheetOpenLength * Global.animTimeMod * Global.animSpeed);
        }
    }

    public void DeselectCharacterSheet()
    {
        if (characterDisplayOpen)
        {
            characterDisplayOpen = false;
            StartCoroutine(ClosePlayerSheet());
        }
    }

    IEnumerator ClosePlayerSheet()
    {
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            characterDisplays.transform.localPosition = new Vector3(minimizedX + (maximizedX - minimizedX) * i * Global.animRate, 0, 0);
            yield return new WaitForSeconds(characterSheetOpenLength * Global.animTimeMod * Global.animSpeed);
        }
        characterDisplayMinimizeButton.SetActive(false);
    }

    public int GetStatModFromType(string statRollType, Player p = default)
    {
        if (p == default)
            p = localPlayer;
        int result = 0;
        switch (statRollType)
        {
            case "STR": result = GetMod(GetStrength(p)); break;
            case "DEX": result = GetMod(GetDexterity(p)); break;
            case "INT": result = GetMod(GetIntelligence(p)); break;
            case "SPD": result = GetMod(GetSpeed(p)); break;
            case "CON": result = GetMod(GetConstitution(p)); break;
            case "ENG": result = GetMod(GetEnergy(p)); break;
            default: Debug.LogError("Unknown Stat Roll Type: " + statRollType); break;
        }
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Generalist"),p))
            result += 1;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Nature's Charm"),p))
            result += 1;
        if (result < 0 && AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Elven Knowledge"),p))
            result = 0;
        return result;
    }

    public void UpdateQuests(FixedString64Bytes[] questNames, int[] questSteps)
    {
        quests.Clear();
        for(int i = 0; i < questNames.Length; i++)
        {
            QuestCard q = questReference[questNames[i] + ""];
            q.questStep = questSteps[i];
            quests.Add(q);
        }
        questDisplay.GetComponent<UIQuestDisplay>().SetupQuests();
    }

    public GameObject GetNotification(int id)
    {
        return notifications.transform.GetChild(id).gameObject;
    }

    public void SendNotification(int id, string descriptionText = default, Action OnComplete = default)
    {
        GetNotification(id).GetComponent<UINotification>().SendNotification(descriptionText, OnComplete);
    }

    public void LevelUpNotificationOnComplete()
    {
        int id = 0;
        for (int i = 0; i < turnOrderPlayerList.Count; i++)
        {
            if (turnOrderPlayerList[i].UUID.Value == localPlayer.UUID.Value)
            {
                id = i;
                break;
            }
        }
        SelectPortrait(id);
    }

    public void LevelUpNotification()
    {
        JLAudioManager.Instance.PlayOneShotSound("LevelUp");
        string description = "You have <color=#006A9A><b>3</b></color> upgrade points available!";
        Action action = LevelUpNotificationOnComplete;
        SendNotification(0, description, action);
    }

    public void CombatNotificationOnComplete()
    {
        bool inRange = InAssistingRange(turnOrderPlayerList[turnMarker], localPlayer, GetRange(localPlayer));
        bool inBattleChargeRange = InAssistingRange(turnOrderPlayerList[turnMarker], localPlayer, 2);
        Skill battleCharge = AbilityManager.Instance.GetSkill("Battle Charge");
        if (inRange)
        {
            MakeChoice("Assist in Combat", "Spectate Combat", GetHealth(localPlayer) > 0, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    // Assist functionality
                    localPlayer.SetValue("ParticipatingInCombat", 1);
                }
                else
                {
                    // Spectate functionality
                    localPlayer.SetValue("ParticipatingInCombat", 0);
                }
                CallEncounterElement(7);
            });
        }
        else if(inBattleChargeRange && AbilityManager.Instance.HasAbilityUnlocked(battleCharge))
        {
            MakeChoice("Assist with Battle Charge", "Spectate Combat", GetAbilityCharges(localPlayer) >= battleCharge.cost && GetHealth(localPlayer) > 0, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    // Assist functionality
                    battleCharge.UseSkill();
                }
                else
                {
                    // Spectate functionality
                    localPlayer.SetValue("ParticipatingInCombat", 0);
                }
                CallEncounterElement(7);
            });
        }
        else
        {
            MakeChoice("Assist in Combat", "Spectate Combat", false, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    // Assist functionality
                    localPlayer.SetValue("ParticipatingInCombat", 1);
                }
                else
                {
                    // Spectate functionality
                    localPlayer.SetValue("ParticipatingInCombat", 0);
                }
                CallEncounterElement(7);
            });
        }
    }

    public void CombatNotification()
    {
        string description = turnOrderPlayerList[turnMarker].Name.Value + " is fighting a monster!";
        Action action = CombatNotificationOnComplete;
        SendNotification(1, description, action);
    }

    public void BossFightNotificationOnComplete()
    {
        MakeChoice("Assist in Combat", "Spectate Combat", GetHealth(localPlayer) > 0, GetHealth(localPlayer) <= 0);
        ChoiceListener((a) => {
            if (a == 1)
            {
                // Assist functionality
                localPlayer.SetValue("ParticipatingInCombat", 1);
            }
            else
            {
                // Spectate functionality
                localPlayer.SetValue("ParticipatingInCombat", 0);
            }
            CallEncounterElement(7);
        });
    }

    public void BossFightNotification()
    {
        string description = turnOrderPlayerList[turnMarker].Name.Value + " is fighting the boss!";
        Action action = BossFightNotificationOnComplete;
        SendNotification(1, description, action);
    }

    public void RequestTauntNotification()
    {
        string description = "You may use the ability \"Taunt\" to intercept the incoming monster attack!";
        SendNotification(2, description);
    }

    public void TauntReceivedNotification(FixedString64Bytes uuid)
    {
        CombatManager.Instance.defensiveOptions.GetComponent<UIDefensiveOptions>().TauntReceived(uuid + "");
        string description = "Another player has taunted for you!";
        SendNotification(5, description);
    }

    public void ContinueToCombat()
    {
        encounterElements.transform.GetChild(7).GetComponent<UICombatantList>().CloseCombatantList();
    }

    public string DrawFromLootDeck()
    {
        string cardName = lootDeck[0].cardName;
        lootDeck.RemoveAt(0);
        return cardName;
    }

    public string DrawFromEncounterDeck()
    {
        string cardName = encounterDeck[0].cardName;
        encounterDeck.RemoveAt(0);
        return cardName;
    }

    public void ResetEncounterFails()
    {
        localPlayer.SetValue("FailedEncounters", 0);
    }

    public void GetEncounter()
    {
        ResetEncounterFails();
        localPlayer.DrawEncounterCards(1, localPlayer.UUID.Value, true);
    }

    public void DefaultTile()
    {
        // If they have 2 fails give an encounter 
        if (localPlayer.FailedEncounters.Value == 2)
            GetEncounter();
        // Else if they have a torch, let them choose to use it or not
        else if(HasTorch(localPlayer))
        {
            MakeChoice("Use Torch", "Roll for Encounter", true, true);
            ChoiceListener((a) => {
                if(a == 1)
                {
                    InventoryManager.Instance.Use("Torch");
                }
                else
                    CallEncounterElement(0);
            });
        }
        // Otherwise roll for encounter
        else
            CallEncounterElement(0);
    }

    public void TreasureTile()
    {
        LootManager.Instance.treasureTile = true;
        localPlayer.DrawLootCards(3, localPlayer.UUID.Value, true);
        localPlayer.DisableTreasureToken(localPlayer.Position.Value);
    }

    public void BossFight()
    {
        if (AllAlivePlayerOnBossTile())
        {
            MakeChoice("Start Fight", "Ignore Fight", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    CombatManager.Instance.monsterCard = chapterBoss;
                    localPlayer.SendBossFightNotifications();
                    localPlayer.SetValue("ParticipatingInCombat", 1);
                    CallEncounterElement(7);
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public bool AllAlivePlayerOnBossTile()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].Position.Value != new Vector3Int(17, 7, -24) && GetHealth(playerList[i]) > 0)
                return false;
        }
        return true;
    }

    public void LoadQuestEncounter(string questName)
    {
        Invoke(questName, 0);
    }

    public bool HasQuest(string questName, int questStep)
    {
        foreach (QuestCard q in quests)
        {
            if (q.cardName == questName && q.questStep == questStep)
                return true;
        }
        return false;
    }

    public void CrazedHermit()
    {
        if(HasQuest("The Abandoned Path", 0))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if(a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("CrazedHermitAbandonedPath");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void CrazedHermitAbandonedPath()
    {
        Action OnComplete = default;
        if(isYourTurn)
        {
            OnComplete = () => {
                foreach(QuestCard q in quests)
                {
                    if (q.cardName == "The Abandoned Path")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Crazed Hermit");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("An old man is standing about, using a rickety cane to stand.  He’s covered in old, torn rags that are covered with dirt and grime.  Any area close to him seems to almost be dying as moves about, the grass turning gray and disappearing and the trees almost shaking with discomfort.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Crazed Hermit");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The hermit runs up to you and starts loudly speaking,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Crazed Hermit");
                QuestManager.Instance.SetSpeaker("Crazed Hermit");
                QuestManager.Instance.SetDialogue("Hello… HELLO… heLLo… heeeellloo my friend.  I, yes I, seem to have lost my horse when traveling down this path, this most beautiful path.  Would you be willing to help an old, normal man, I am a very normal man, get his very special horse back?");
                QuestManager.Instance.PlayAudio("CrazedHermitAbandonedPath", 0.5f, 26.5f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Crazed Hermit");
                QuestManager.Instance.SetSpeaker("Crazed Hermit");
                QuestManager.Instance.SetDialogue("I will pay you good money, such good, very good, lots of money.  Once you get the horse, bring it to my home which is just beyond those trees over there.");
                QuestManager.Instance.PlayAudio("CrazedHermitAbandonedPath", 26.5f, 40f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void DistressedVillager()
    {
        if (HasQuest("Goblin Hunt", 0))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("DistressedVillagerGoblinHunt");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void DistressedVillagerGoblinHunt()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Goblin Hunt")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Distressed Villager");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A young woman is looking around frantically.  She is covered in dirt and seems to be wounded.  You see tears forming around her eyes and a look of desperation consumes her face.  You can tell something has terribly frightened this woman.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Distressed Villager");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The young woman sees you approaching and rushes over to you.  She frantically cries,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Distressed Villager");
                QuestManager.Instance.SetSpeaker("Distressed Villager");
                QuestManager.Instance.SetDialogue("Please, you have to help me!  I saw an army of goblins making camp nearby!  They spotted me spying on them so I ran as fast as I could, but what if they track me down!  Please, adventurer, you have to kill them before they get to me!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void ForestHag()
    {
        if (HasQuest("Double Rainbow", 0))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("ForestHagDoubleRainbow");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void ForestHagDoubleRainbow()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Double Rainbow")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Forest Hag");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("An old lady using a wooden stick to stand is beside her desolate shack.  Her skin is an odd shade of green, her nose is long and pimply, and the rags she uses as clothes are ridden with dirt and grime.  Her voice is coarse as she sings to herself while stirring her pot of... meat?");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Forest Hag");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The hag turns and calls out to you.  As you walk over, she cackles to herself and begins asking you for a favor,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Forest Hag");
                QuestManager.Instance.SetSpeaker("Forest Hag");
                QuestManager.Instance.SetDialogue("One of my little pets escaped from my home, can you be a sweet little dear and capture it for me?  When you do find it, don’t forget to give it a good spanking for being... so so disobedient hehehe!  I won’t mind if you kill it either, I can always find new pets!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Forest Hag");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("She points her wooden stick towards the Overgrown Temple, and suddenly a brick road the color of egg yolk appears.  The hag cackles again and exclaims to follow the road as she returns to stirring her pot of meat.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void FourEyedBoy()
    {
        if (HasQuest("Isekai!", 2))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) =>
            {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("FourEyedBoyIsekai");
                }
                else
                    DefaultTile();
            });
        }
        else
        {
            if (HasQuest("Bringin' the Bath Water", 0))
            {
                MakeChoice("Follow Quest", "Ignore Quest", true, true);
                ChoiceListener((a) =>
                {
                    if (a == 1)
                    {
                        ResetEncounterFails();
                        localPlayer.LoadIntoQuest("FourEyedBoyBringinTheBathWater1");
                    }
                    else
                        DefaultTile();
                });
            }
            else
            {
                if (HasQuest("Bringin' the Bath Water", 2))
                {
                    MakeChoice("Follow Quest", "Ignore Quest", true, true);
                    ChoiceListener((a) => {
                        if (a == 1)
                        {
                            ResetEncounterFails();
                            localPlayer.LoadIntoQuest("FourEyedBoyBringinTheBathWater2");
                        }
                        else
                            DefaultTile();
                    });
                }
                else
                    DefaultTile();
            }
        }
    }

    public void FourEyedBoyIsekai()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Isekai!")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        localPlayer.ReduceChaos(-1 * q.rewardChaos);
                        foreach (Player p in playerList)
                            p.GainGold(q.rewardGold);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A small boy appears before you, his glasses are cracked and he talks to himself.  Something is off about this boy though, his body distorts and the image of himself seems to split.  It happens so quickly you question if it’s just your mind playing tricks...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The boy faces towards you, his eyes are filled with tears.  He asks if you’re here to finish him off.  You shake your head and he states,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("I don’t even know why I’m here!  I was just sitting in my room talking to my discord kitten and suddenly I’m in this strange land!  I hope my little kitten is okay, I have to get back and tell her...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("As he was about to finish his sentence, the sky turns dark and hovering in the clouds is a frightening smile and a pair of red eyes, it laughs as the  boy’s body begins to distort violently.  The boy screams for help but just as fast as it started, the boy’s body freezes and he looks at you.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("Wow you’re an adventurer aren’t you!  Back at the village I would tell everyone I was going to be an adventurer too!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("He keeps talking, seemingly unaware of what happened moments ago.  You leave confused, but in the back of your mind the smile appears again and your hand forcibly checks your pockets, someone or something rewards you for completing your task.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void FourEyedBoyBringinTheBathWater1()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Bringin' the Bath Water")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A small boy appears before you, his glasses are cracked and he talks to himself.  Something is off about this boy though, his body distorts and the image of himself seems to split.  It happens so quickly you question if it’s just your mind playing tricks...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("As you begin to approach the boy, his body distorts again and you hear him mumbling to himself,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("It’s somewhere around here, where is it...  where is it!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Before you can get any closer the boy notices you and his eyes sparkle with opportunity,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("Hey adventurer!  Could you help me out?  I heard there was a spring here with mystical forbidden waters, and if you drink it you will have the power of over 9000 men and no one will be able to resist you!  Doesn’t that sound great?");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("He shows you a poorly drawn map,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 7
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("It’s somewhere around here, but I was never really good at reading, or fighting, or even walking.  Can you help me out?  I’ll reward you!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 8
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You agree and begin looking for the location on the map.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void FourEyedBoyBringinTheBathWater2()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Bringin' the Bath Water")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        localPlayer.ReduceChaos(-1 * q.rewardChaos);
                        foreach (Player p in playerList)
                            p.GainGold(q.rewardGold);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                if (localPlayer.BetrayedBoy.Value)
                    localPlayer.IncreaseChaos(1);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The boy notices you approaching almost immediately.  He shoots up from his sorry looking state, and seems incredibly excited, shaking with anticipation.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                QuestManager.Instance.SetDialogue("D-did you get it?");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Four-Eyed Boy");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("he asks.  You tell him that you got some special water from the spring.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetChoices(2, new string[2]{ "Give him \"Good Stuff\"", "Give him some water"}, new Action[2]{
                    () => {
                        // Gave him good stuff
                    }, () => {
                        // Gave him bad water (betrayed)
                        foreach(Player p in playerList)
                            p.SetValue("BetrayedBoy", true);
                    }
                }, new Func<bool>[2] {
                    () => {
                        // Requires bathwater
                        return localPlayer.HasBathWater.Value;
                    }, () => {
                        // No requirement
                        return true;
                    }
                });
            },
            () => {
                // Chunk 4
                if(localPlayer.BetrayedBoy.Value)
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You hand over the jar of water to the boy.  He inspects it, then removes the lid and inhales deeply.  The boy looks at you and says");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You hand the jar over to the boy and he squeals in excitement.  You begin to ask for your reward when all of a sudden, the boy opens the lid of the jar and begins chugging the water at an ungodly rate.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 5
                if(localPlayer.BetrayedBoy.Value)
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                    QuestManager.Instance.SetDialogue("Something isn’t right, I don’t smell their wonderful scent.  Is this just regular spring water?");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You’re taken aback by the display, but you continue to ask about the reward.  Once he is done, he looks at you and grins,");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 6
                if(localPlayer.BetrayedBoy.Value)
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You look at him quizzically.  Could this boy really smell the difference between spring water and spring water that an elemental had been in?  You tell the boy that this is the special water he ordered so he better pay up.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Four-Eyed Boy");
                    QuestManager.Instance.SetDialogue("I have the strength of over 9000 men and you think you have the power to make demands of me?!");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 7
                if(localPlayer.BetrayedBoy.Value)
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("The boy angrily eyes you, then takes his jar and then throws your reward on the ground.  (+1 Chaos)");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
                else
                {
                    QuestManager.Instance.SetImage("Four-Eyed Boy");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("He sprints full speed towards you, you calmly pull out your weapon and sigh, smacking the boy with the blunt end of your weapon, knocking him out.  You loot the boy’s body and leave.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
            }
        }, OnComplete);
    }

    public void PabloTheNoob()
    {
        if (HasQuest("DisharMEOWny", 0))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) =>
            {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("PabloTheNoobDisharMEOWNy1");
                }
                else
                    DefaultTile();
            });
        }
        else
        {
            if (HasQuest("DisharMEOWny", 2))
            {
                MakeChoice("Follow Quest", "Ignore Quest", true, true);
                ChoiceListener((a) => {
                    if (a == 1)
                    {
                        ResetEncounterFails();
                        localPlayer.LoadIntoQuest("PabloTheNoobDisharMEOWNy2");
                    }
                    else
                        DefaultTile();
                });
            }
            else
                DefaultTile();
        }
    }

    public void PabloTheNoobDisharMEOWNy1()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "DisharMEOWny")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A young man, barely in his teens, stands before you.  His small frame and loose armor makes it seem like he weighs less than a single gold piece.  He’s panting and covered in visible scratches and bruises.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Under his breath you can clearly tell he’s cursing to himself... something about wanting his mom to come get him?");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You ask him if he has seen any notable monsters or Chaos activity around the area.  His eyes perk up almost immediately as he points you towards the Ominous Clearing and exclaims,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Pablo the Noob");
                QuestManager.Instance.SetDialogue("I was attacked by the scariest monster I’ve ever seen over there!  My mom was right, I should never have left home!  I hate touching all this grass, it gets everywhere!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void PabloTheNoobDisharMEOWNy2()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "DisharMEOWny")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You begin chiding Pablo for making you fight such a small feline.  He looks at you confused,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Pablo the Noob");
                QuestManager.Instance.SetDialogue("Small cat... did you...  did you kill Mr. Whiskers?!  That wasn’t the monster, that was my cat!  I was trying to save him from the monster!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Pablo the Noob");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Just as he says that, a loud roar can be heard at the Ominous Clearing, something is very angwy...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void ShiftyPeddler()
    {
        if (HasQuest("Isekai!", 0))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("ShiftyPeddlerIsekai");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void ShiftyPeddlerIsekai()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Isekai!")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A man is leaning against a tree at the side of the forest trail. His long coat and large hat are out of place in the surrounding area, but he seems unbothered by it all.  He whispers to himself and large grins stretch almost forcibly across his face.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The peddler approaches you, his grin going from cheek to cheek in an almost terrifying manner.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Shifty Peddler");
                QuestManager.Instance.SetDialogue("Hello there stranger, enjoying the sights?  It’s all marvelous isn’t it, this strange land?  Almost like it was crafted for us.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("His smile somehow grows even wider and in a blink of an eye, he disappears from your sight and you hear his voice deep within the forest.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Shifty Peddler");
                QuestManager.Instance.SetDialogue("We oughta help each other, after all we are both travelers here heh heh heh...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Your head begins to forcibly turn towards an almost invisible path,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 7
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Shifty Peddler");
                QuestManager.Instance.SetDialogue("Now now, don’t worry.  All I need from you is to have a little fun with someone down that road.  Simple isn’t it?  He he he...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Your mind fills with the information of your task, and the voice in the forest retreats.  Still though... in the back of your mind you see the image of a large grin.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void SuspiciousHorse()
    {
        if (HasQuest("The Abandoned Path", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("SuspiciousHorseAbandonedPath");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void SuspiciousHorseAbandonedPath()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "The Abandoned Path")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Suspicious Horse");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A horse stands in the middle of an open clearing, jumping about and enjoying its freedom.  As it spots you, it turns as still as a statue and begins staring you down.  It’s eyes, red and almost human-like, following your every movement.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Suspicious Horse");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You approach the horse...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetChoices(2, new string[2]{ "Grab the Horse", "Speak to the Horse"}, new Action[2]{
                    () => {
                        // Grabbed the Horse
                        foreach(Player p in playerList)
                            p.SetValue("GrabbedHorse", true);
                    }, () => {
                        // Spoke to the Horse (Animalkin)
                    }
                }, new Func<bool>[2] {
                    () => {
                        // No requirement
                        return true;
                    }, () => {
                        // Requires animalkin
                        return Animalkin(localPlayer);
                    }
                });
            },
            () => {
                // Chunk 3
                if(localPlayer.GrabbedHorse.Value)
                {
                    QuestManager.Instance.SetImage("Suspicious Horse");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You grab the reins of the horse and you start leading it back to the hermit’s home.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
                else
                {
                    QuestManager.Instance.SetImage("Suspicious Horse");
                    QuestManager.Instance.SetSpeaker("Suspicious Horse");
                    QuestManager.Instance.SetDialogue("DON’T BRING ME BACK TO THAT CRAZY MAN, HE WANTS TO FEED ME TO THAT SPIDER!");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetChoices(2, new string[2]{ "Ignore the Horse", "Trust the Horse"}, new Action[2]{
                    () => {
                        // Ignored the Horse
                        foreach(Player p in playerList)
                            p.SetValue("GrabbedHorse", true);
                    }, () => {
                        // Trusted the Horse
                    }
                }, new Func<bool>[2] {
                    () => {
                        // No requirement
                        return true;
                    }, () => {
                        // No requirement
                        return true;
                    }
                });
                }
            },
            () => {
                // Chunk 4
                if(localPlayer.GrabbedHorse.Value)
                {
                    QuestManager.Instance.SetImage("Suspicious Horse");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You tell the horse he’s crazy and take him back anyway.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
                else
                {
                    QuestManager.Instance.SetImage("Suspicious Horse");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You heed the horse’s warning and head back cautiously, leaving the horse behind.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
            }
        }, OnComplete);
    }

    public void VeteranHunter()
    {
        if (HasQuest("Goblin Hunt", 2))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("VeteranHunterGoblinHunt");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void VeteranHunterGoblinHunt()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Goblin Hunt")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("A hooded figure seems to be stomping over something on the ground.  A large black coat and gray mask obstruct any view of the person underneath.  They seem to pay you no mind.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You approach the hunter and ask if they know anything about the goblin horde in the area.  The hunter turns to you and gives a wide grin.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Veteran Hunter");
                QuestManager.Instance.SetDialogue("Oh you want to kill Goblins too?  Good, they all deserve to die!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("His grin widens as he reveals what he was stomping on, a young goblin on the brink of death.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Veteran Hunter");
                QuestManager.Instance.SetDialogue("How about you come over and finish this little one off huh?  I mean, how am I supposed to trust you if you can’t even kill this measly little runt?");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("Veteran Hunter");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The young goblin locks eyes with you as you approach, pleading for its life.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetChoices(2, new string[2]{ "Kill the Goblin", "Kill the Hunter"}, new Action[2]{
                    () => {
                        // Killed the Goblin
                        foreach(Player p in playerList)
                            p.SetValue("KilledGoblin", true);
                    }, () => {
                        // Killed the Hunter
                    }
                }, new Func<bool>[2] {
                    () => {
                        // No requirement
                        return true;
                    }, () => {
                        // No requirement
                        return true;
                    }
                });
            },
            () => {
                // Chunk 7
                if(localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You swing your weapon down, taking whatever miniscule amount of life was left in the young goblin.  The hunter crouches and proceeds to cut the goblin’s finger off.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You strike the Veteran Hunter and as they fall to the ground they shout,");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 8
                if(localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Veteran Hunter");
                    QuestManager.Instance.SetDialogue("Ruthless, I like that.  We’re going to need that ruthlessness when we exterminate them from the face of this world.  They’re a pest!  No, they’re even worse than that...");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Veteran Hunter");
                    QuestManager.Instance.SetDialogue("You’re siding with those scum?  You’re no better than them, all of you deserve to burn in the Chaos!");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 9
                if(localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("The hunter continues his ramblings as he begins walking north, you follow closely behind.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
                else
                {
                    QuestManager.Instance.SetImage("Veteran Hunter");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("The ring on his finger slips as he dies, you pick it up just in case.  As you help the young goblin up, he looks at you confused by your kindness.  Nevertheless, he leans against you and points north, beckoning you to continue heading in that direction.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
            }
        }, OnComplete);
    }

    public void AbandonedOutpost()
    {
        if (HasQuest("Goblin Hunt", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("AbandonedOutpostGoblinHunt");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void AbandonedOutpostGoblinHunt()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Goblin Hunt")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Abandoned Outpost");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("What once seemed to have been a very busy forest outpost, is now empty and void of life.  Everything seems to have been picked up and moved in a hurry, with items scattered about from frantic packing and traveling.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Abandoned Outpost");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("As you take a closer look at the outpost, you notice signs of recent activity.  Footsteps caked in the mud, and the smell of blood lingering in the air.  You also see some signs of a struggle, with the trail of broken branches and small blood splatters leading towards a small campsite.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void AncientSpring()
    {
        if (HasQuest("Bringin' the Bath Water", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("AncientSpringBrininTheBathWater");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void AncientSpringBrininTheBathWater()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                foreach (QuestCard q in quests)
                {
                    if (q.cardName == "Bringin' the Bath Water")
                    {
                        localPlayer.GainXP(q.objectiveXPRewards[q.questStep]);
                        q.questStep++;
                    }
                }
                localPlayer.UpdateQuests(quests);
                EndTurn();
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Ancient Spring");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("This spring seems to be rather untouched by the Chaos of the surrounding region.  The pools combine beautiful smooth stones with a vibrant flora.  An aura of steam looms over the springs, creating a very tranquil and almost spa-like atmosphere.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Ancient Spring");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("In the distance you think you hear what might even be laughter...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Ancient Spring");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("As you approach the hot spring, you can barely make out what seems to be a red-orange glowing light behind a thick wall of steam.  You walk towards the light and it begins to grow, until it finally begins to take form.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Fire Elemental");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You take a couple more steps forward until a female fire elemental is standing before you.  You stand there unsure of what to say, but the elemental speaks first,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Fire Elemental");
                QuestManager.Instance.SetSpeaker("Fire Elemental");
                QuestManager.Instance.SetDialogue("Welcome to the Witch Stream!  Since the Chaos has been destroying the area we need to move streams, but the elementals at Utoob Stream want us to pay money first before we move in,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("Fire Elemental");
                QuestManager.Instance.SetSpeaker("Fire Elemental");
                QuestManager.Instance.SetDialogue("...so we have to charge you 20 Gold for any water you want.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetChoices(2, new string[2]{ "Pay the fee", "Refuse to pay"}, new Action[2]{
                    () => {
                        // Paid the fee
                        localPlayer.LoseGold(20);
                        foreach(Player p in playerList)
                            p.SetValue("HasBathWater", true);
                    }, () => {
                        // Refused to pay
                    }
                }, new Func<bool>[2] {
                    () => {
                        // Requires 20 gold
                        return GetGold(localPlayer) >= 20;
                    }, () => {
                        // No requirement
                        return true;
                    }
                });
            },
            () => {
                // Chunk 7
                if(localPlayer.HasBathWater.Value)
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Fire Elemental");
                    QuestManager.Instance.SetDialogue("Great!  Follow me over here...");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You inform the fire elemental that 20 Gold is a bit too pricey.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 8
                if(localPlayer.HasBathWater.Value)
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("exclaims the fire elemental.  She then leads you over to a hidden pool in the back, where there are 2 other female elementals soaking in the pool.  Your guide elemental then grabs a glass jar and scoops up some of the water from the pool.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Fire Elemental");
                    QuestManager.Instance.SetDialogue("It’s literally only 20 gold, everyone has 20 gold!");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 9
                if(localPlayer.HasBathWater.Value)
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Fire Elemental");
                    QuestManager.Instance.SetDialogue("Here you go, and remember you can find us at Utoob!");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Fire Elemental");
                    QuestManager.Instance.SetDialogue("she replies.  You tell her that you are just gonna take some of the other water.  She frustratedly turns around and leaves you to your devices.  You find some spring water and fill up a jar with it.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
            },
            () => {
                // Chunk 10
                if(localPlayer.HasBathWater.Value)
                {
                    QuestManager.Instance.SetImage("Fire Elemental");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You walk away with a confused expression on your face.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            }
        }, OnComplete);
    }

    public void BanditHideout()
    {
        if (HasQuest("Isekai!", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("BanditHideoutIsekai");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void BanditHideoutIsekai()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                CombatManager.Instance.monsterCard = miniBossDeck["Bandit Weeb Lord"];
                localPlayer.SendCombatNotifications();
                localPlayer.SetValue("ParticipatingInCombat", 1);
                CallEncounterElement(7);
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Bandit Hideout");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The bandit hideout is against a mountain wall, crates and equipment strewn about.  It's a large space and in the middle of it, a battle is occurring.  A boy with glasses is fighting off waves of bandits...");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Bandit Hideout");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("...and by fighting off, he’s actually curled up on a ball and everytime a bandit attacks him, a wave of force sends them flying.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Bandit Hideout");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("As you rush in to save the boy, your vision darkens and a frighteningly wide smile appears before you.  It exclaims,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Shifty Peddler");
                QuestManager.Instance.SetSpeaker("Shifty Peddler");
                QuestManager.Instance.SetDialogue("This is so much fun! So many things to play with!  Let’s see how well you do!,");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("Bandit Hideout");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Your vision returns but things have changed, the bandits are no more and instead, the boy with glasses is staring at a dark version of himself floating in the air.  The boy runs away as the dark version turns to face you and attacks!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void HowlingCave()
    {
        if (HasQuest("Goblin Hunt", 3))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("HowlingCaveGoblinHunt");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void HowlingCaveGoblinHunt()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                if(localPlayer.KilledGoblin.Value)
                {
                    CombatManager.Instance.monsterCard = miniBossDeck["Goblin Horde"];
                    localPlayer.SendCombatNotifications();
                    localPlayer.SetValue("ParticipatingInCombat", 1);
                    CallEncounterElement(7);
                }
                else
                {
                    foreach (QuestCard q in quests)
                    {
                        if (q.cardName == "Goblin Hunt")
                        {
                            localPlayer.GainXP(q.objectiveXPRewards[q.questStep] + 6);
                            localPlayer.ReduceChaos(-1 * q.rewardChaos);
                            foreach (Player p in playerList)
                                p.GainGold(q.rewardGold);
                            q.questStep++;
                        }
                    }
                    localPlayer.UpdateQuests(quests);
                    localPlayer.DrawLootCards(2, localPlayer.UUID.Value, true);
                }
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Howling Cave");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The cave forms like a jagged mouth protruding from a rocky overhang.  Sharp rocks line the entrance of the cave on all sides.  If the appearance wasn’t inviting enough, distant sounds of wailing and screaming seem to echo from the depths.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                if(localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Howling Cave");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You approach alongside the hunter, when suddenly the walls of the cave are lit up and an army of goblins stands before you.  The hunter laughs to himself, ready to kill every single last one of them.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
                else
                {
                    QuestManager.Instance.SetImage("Howling Cave");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You approach alongside the young goblin, when suddenly the walls of the cave are lit up and an army of goblins stands before you.  Before they can attack you, the young goblin raises his hands and says something in their language.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 3
                if(localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Howling Cave");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("He opens his mouth as if to give a speech but is suddenly stopped as an arrow forces its way into his head.  The finger of the young goblin falls out of his pocket, and the goblins stare at it.  After they realize what it is, they launch their attack.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
                else
                {
                    QuestManager.Instance.SetImage("Howling Cave");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("They lower their weapons and begin talking amongst themselves.  A large goblin walks past the horde and comes face to face with you,");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 4
                if(!localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Goblin Prince");
                    QuestManager.Instance.SetSpeaker("Goblin Prince");
                    QuestManager.Instance.SetDialogue("You save...  prince.  You are...  friend of...  goblins.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 5
                if(!localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Goblin Prince");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("The young goblin looks towards you and nods in agreement as he begins walking towards his people.  You give him the ring of the dead hunter and the young goblin prince speaks,");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 6
                if(!localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Goblin Prince");
                    QuestManager.Instance.SetSpeaker("Goblin Prince");
                    QuestManager.Instance.SetDialogue("Bad man... hunt goblin... years.  All we want... peace.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 7
                if(!localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Goblin Prince");
                    QuestManager.Instance.SetSpeaker("Narrator");
                    QuestManager.Instance.SetDialogue("You prepare to leave but are given valuable treasure and parting words as thanks for saving the prince.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
                }
            },
            () => {
                // Chunk 8
                if(!localPlayer.KilledGoblin.Value)
                {
                    QuestManager.Instance.SetImage("Goblin Prince");
                    QuestManager.Instance.SetSpeaker("Goblin Prince");
                    QuestManager.Instance.SetDialogue("May the... peace... be with you.");
                    QuestManager.Instance.PlayAudio("");
                    QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
                }
            }
        }, OnComplete);
    }

    public void OminousClearing()
    {
        if (HasQuest("DisharMEOWny", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) =>
            {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("OminousClearingDisharMEOWNy1");
                }
                else
                    DefaultTile();
            });
        }
        else
        {
            if (HasQuest("DisharMEOWny", 3))
            {
                MakeChoice("Follow Quest", "Ignore Quest", true, true);
                ChoiceListener((a) => {
                    if (a == 1)
                    {
                        ResetEncounterFails();
                        localPlayer.LoadIntoQuest("OminousClearingDisharMEOWNy2");
                    }
                    else
                        DefaultTile();
                });
            }
            else
                DefaultTile();
        }
    }

    public void OminousClearingDisharMEOWNy1()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                CombatManager.Instance.monsterCard = miniBossDeck["Discord Kitten"];
                localPlayer.SendCombatNotifications();
                localPlayer.SetValue("ParticipatingInCombat", 1);
                CallEncounterElement(7);
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Ominous Clearing");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You approach the forest clearing, a seemingly unremarkable patch of land aside from the clearly out-of-place throne in the middle of it.  As you get closer to the throne, a spine-chilling aura emanates from it that fills you with dread.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Ominous Clearing");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You approach the throne, taking notice of the small paw prints along the ground.  As you get closer, a small feline appears on the throne, and eyes you suspiciously.  It begins purring and stretches across the length of the throne, it’s not until you get closer that it leaps at you!");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void OminousClearingDisharMEOWNy2()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                CombatManager.Instance.monsterCard = miniBossDeck["Raging Discord Kitten"];
                localPlayer.SendCombatNotifications();
                localPlayer.SetValue("ParticipatingInCombat", 1);
                CallEncounterElement(7);
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1
                QuestManager.Instance.SetImage("Ominous Clearing");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You return to the area, the sounds of a very ominous purring echoing across the land.  You almost believe it’s nighttime when a large shadow begins to cover the sun, you look up only to come across the largest feline you’ve ever seen.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Ominous Clearing");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Clearly corrupted with Chaos, the feline stares at you with only murderous intent in it’s eyes, you have made it very angwy.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void OvergrownTemple()
    {
        if (HasQuest("Double Rainbow", 1))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("OvergrownTempleDoubleRainbow");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void OvergrownTempleDoubleRainbow()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                CombatManager.Instance.monsterCard = miniBossDeck["Rainbow Slime"];
                localPlayer.SendCombatNotifications();
                localPlayer.SetValue("ParticipatingInCombat", 1);
                CallEncounterElement(7);
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Overgrown Temple");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The temple is in a state of decay, its once magnificent designs falling into ruin as nature reclaims its land.  Leaves and vines wrap around the columns, patches of dirt and soil sprout from the floor, and nothing but the smell of dew fills the air.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Overgrown Temple");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The sound of plopping fills the temple, becoming more and more viscous as you come closer to the main hall.  As you reach the middle ground of the temple, a Rainbow Slime drops from the air.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Overgrown Temple");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Its eyes are filled with anger as it prepares to attack you, but you spot something else in its eyes … fear?  You notice two tiny crimson slippers within the slime's body, and begin to wonder if the slime was the pet or whoever the owner of the slippers were.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("Overgrown Temple");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Before you can ponder any longer, the slime's body glows with magnificent brightness, it’s prepared to fight.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    public void WebbedForest()
    {
        if (HasQuest("The Abandoned Path", 2))
        {
            MakeChoice("Follow Quest", "Ignore Quest", true, true);
            ChoiceListener((a) => {
                if (a == 1)
                {
                    ResetEncounterFails();
                    localPlayer.LoadIntoQuest("WebbedForestAbandonedPath");
                }
                else
                    DefaultTile();
            });
        }
        else
            DefaultTile();
    }

    public void WebbedForestAbandonedPath()
    {
        Action OnComplete = default;
        if (isYourTurn)
        {
            OnComplete = () => {
                CombatManager.Instance.monsterCard = miniBossDeck["Spooky Spider"];
                localPlayer.SendCombatNotifications();
                localPlayer.SetValue("ParticipatingInCombat", 1);
                CallEncounterElement(7);
            };
        }

        QuestManager.Instance.LoadIntoQuest(isYourTurn, new List<Action> {
            () => {
                // Chunk 1 (Intro)
                QuestManager.Instance.SetImage("Webbed Forest");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The trees are littered with cobwebs stretching from trunk to trunk, the same can be said as you walk across the forest ground.  You hear the constant rustling of leaves and bushes and occasionally, you can catch red-eyes staring at you from the shadows in the forest before they disappear.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("Webbed Forest");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You begin to approach the location where the hermit claimed to live, yet all you see are eerie spiderwebs caking the surrounding trees.  You advance further until you hear the faintest of noises behind you.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("Webbed Forest");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("You turn around slowly to see a gigantic, ghastly white spider descending from the canopy above.");
                QuestManager.Instance.PlayAudio("");
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete);
    }

    #region Chaos Counter Functions
    public int XPModifier()
    {
        if(ChaosTier() < 6)
            return 2 * (ChaosTier() - 1);
        return 8;
    }

    public void ReduceChaos(int amount)
    {
        if (chaosCounter > 4 && chaosCounter - amount <= 4)
            return;
        if (chaosCounter > 8 && chaosCounter - amount <= 8)
            return;
        if (chaosCounter > 12 && chaosCounter - amount <= 12)
            return;
        if (chaosCounter > 16 && chaosCounter - amount <= 16)
            return;
        if (chaosCounter > 20 && chaosCounter - amount <= 20)
            return;
        chaosCounter -= amount;
        if (chaosCounter < 1)
            chaosCounter = 1;
        UpdateChaosMarker();
    }

    public void IncreaseChaos(int amount)
    {
        chaosCounter += amount;
        if (chaosCounter > 21)
            chaosCounter = 21;
        UpdateChaosMarker();
    }

    public int ChaosTier()
    {
        return 1 + Mathf.FloorToInt((chaosCounter - 1) / 4);
    }

    public void UpdateChaosMarker()
    {
        if(chaosCounter < 21)
        {
            chaosMarker.GetComponentInChildren<TMP_Text>().text = chaosCounter + "";
            chaosMarker.transform.localPosition = new Vector3(-979 + 85.1052632f * (chaosCounter - 1), 0, 0);
        }
        else
        {
            chaosMarker.GetComponentInChildren<TMP_Text>().text = "x";
            chaosMarker.transform.localPosition = new Vector3(-979 + 85.1052632f * (chaosCounter - 1), 0, 0);
        }
    }

    public int AttackModifier()
    {
        return 2 * (ChaosTier() - 1);
    }

    public int HealthModifier()
    {
        return 8 * (ChaosTier() - 1);
    }

    public int PowerModifier()
    {
        return (ChaosTier() - 1);
    }
    #endregion

    #region Stat Roll
    public void StatRollListener(Action<int> Response)
    {
        StartCoroutine(WaitForStatRoll(Response));
    }

    IEnumerator WaitForStatRoll(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchStatRollResult() != 0);
        Response(FetchStatRollResult());
    }

    public void MakeStatRoll(string statRollType, int statRollValue)
    {
        encounterElements.transform.GetChild(1).GetComponent<UIStatRoll>().MakeStatRoll(statRollType, statRollValue);
    }

    public int FetchStatRollResult()
    {
        return encounterElements.transform.GetChild(1).GetComponent<UIStatRoll>().success;
    }
    #endregion

    #region Choice
    public void ChoiceListener(Action<int> Response)
    {
        StartCoroutine(WaitForChoice(Response));
    }

    IEnumerator WaitForChoice(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchChoiceResult() != 0);
        Response(FetchChoiceResult());
    }

    public void MakeChoice(string choice1, string choice2, bool condition1, bool condition2)
    {
        encounterElements.transform.GetChild(3).GetComponent<UIChoice>().MakeChoice(choice1, choice2, condition1, condition2);
    }

    public int FetchChoiceResult()
    {
        return encounterElements.transform.GetChild(3).GetComponent<UIChoice>().choice;
    }
    #endregion

    #region Forced Discard
    public void ForcedDiscardListener(Action Response)
    {
        StartCoroutine(WaitForForcedDiscard(Response));
    }

    IEnumerator WaitForForcedDiscard(Action Response)
    {
        yield return new WaitUntil(() => FetchForcedDiscardResult());
        Response();
    }

    public void ForceDiscard()
    {
        CallEncounterElement(4);
    }

    public bool FetchForcedDiscardResult()
    {
        return encounterElements.transform.GetChild(4).GetComponent<UIForcedDiscard>().completed;
    }
    #endregion

    #region Discard Many
    public void DiscardManyListener(Action<int> Response)
    {
        StartCoroutine(WaitForDiscardMany(Response));
    }

    IEnumerator WaitForDiscardMany(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchDiscardManyResult() != -1);
        Response(FetchDiscardManyResult());
    }

    public void DiscardManyCards()
    {
        CallEncounterElement(5);
    }

    public int FetchDiscardManyResult()
    {
        return encounterElements.transform.GetChild(5).GetComponent<UIDiscardManyCards>().discarded;
    }
    #endregion

    #region Gamble
    public void GambleListener(Action<int> Response)
    {
        StartCoroutine(WaitForGamble(Response));
    }

    IEnumerator WaitForGamble(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchGambleResult() != 0);
        Response(FetchGambleResult());
    }

    public void MakeGamble()
    {
        encounterElements.transform.GetChild(6).GetComponent<UIGamble>().MakeGamble();
    }

    public int FetchGambleResult()
    {
        return encounterElements.transform.GetChild(6).GetComponent<UIGamble>().success;
    }
    #endregion

    #region Option Requirements
    public bool Angelkin(Player p)
    {
        return p.Race.Value == "Aasimar";
    }
    public bool Animalkin(Player p)
    {
        return p.Race.Value == "Centaur" || Leonin(p);
    }
    public bool Berserk(Player p)
    {
        return p.Trait.Value == "Berserk";
    }
    public bool Dwarf(Player p)
    {
        return p.Race.Value == "Dwarf";
    }
    public bool Elven(Player p)
    {
        return p.Race.Value == "High Elf" || p.Race.Value == "Night Elf";
    }
    public bool FleetFooted(Player p)
    {
        return p.Trait.Value == "Fleet-Footed";
    }
    public bool Highborn(Player p)
    {
        return p.Trait.Value == "Highborn";
    }
    public bool Holy(Player p)
    {
        return p.Trait.Value == "Holy";
    }
    public bool Leonin(Player p)
    {
        return p.Race.Value == "Leonin";
    }
    public bool Looter(Player p)
    {
        return p.Trait.Value == "Looter";
    }
    public bool Mystical(Player p)
    {
        return p.Trait.Value == "Mystical";
    }
    public bool Powerful(Player p)
    {
        return p.Trait.Value == "Powerful";
    }
    public bool HasTorch(Player p)
    {
        return p.Inventory1.Value == "Torch" || p.Inventory2.Value == "Torch" || p.Inventory3.Value == "Torch" || p.Inventory4.Value == "Torch" || p.Inventory5.Value == "Torch";
    }
    public bool IsStartOrEndOfDay()
    {
        return startOrEndOfDay;
    }
    public bool IsInStore()
    {
        return inStore;
    }
    public bool HasAllyInStore()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].UUID.Value != localPlayer.UUID.Value && WentToStore(playerList[i]))
                return true;
        }
        return false;
    }
    public bool HasDeadAlly(Player p)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].UUID.Value != p.UUID.Value && GetHealth(playerList[i]) <= 0)
                return true;
        }
        return false;
    }
    public bool WentToStore(Player p)
    {
        return p.EndOfDayActivity.Value == 0;
    }
    public bool CanUseWeapon(Player p, WeaponCard w)
    {
        return (p.Class.Value + "") switch
        {
            "Warrior" => w.attackType == "Physical / STR",
            "Paladin" => w.attackType == "Physical / STR",
            "Ranger" => w.attackType == "Physical / DEX",
            "Rogue" => w.attackType == "Physical / DEX",
            "Sorcerer" => w.attackType == "Magical / INT",
            "Necromancer" => w.attackType == "Magical / INT",
            _ => false,
        };
    }
    public bool CanUseArmor(Player p, ArmorCard a)
    {
        return (p.Class.Value + "") switch
        {
            "Warrior" => true,
            "Paladin" => true,
            "Ranger" => a.itemType == "Medium Armor" || a.itemType == "Light Armor",
            "Rogue" => a.itemType == "Medium Armor" || a.itemType == "Light Armor",
            "Sorcerer" => a.itemType == "Light Armor",
            "Necromancer" => a.itemType == "Light Armor",
            _ => false,
        };
    }
    public bool IsPhysicalBased(Player p)
    {
        return (p.Class.Value + "") switch
        {
            "Warrior" => true,
            "Paladin" => true,
            "Ranger" => true,
            "Rogue" => true,
            "Sorcerer" => false,
            "Necromancer" => false,
            _ => false,
        };
    }
    public bool IsMagicalBased(Player p)
    {
        return (p.Class.Value + "") switch
        {
            "Warrior" => false,
            "Paladin" => false,
            "Ranger" => false,
            "Rogue" => false,
            "Sorcerer" => true,
            "Necromancer" => true,
            _ => false,
        };
    }
    public string GetPrimaryStat(Player p)
    {
        return (p.Class.Value + "") switch
        {
            "Warrior" => "STR",
            "Paladin" => "STR",
            "Ranger" => "DEX",
            "Rogue" => "DEX",
            "Sorcerer" => "INT",
            "Necromancer" => "INT",
            _ => "",
        };
    }
    #endregion

    #region Calculate Stats
    public int GetMod(int stat)
    {
        return Mathf.FloorToInt((stat - 10) * 0.5f);
    }

    public int GetHealth(Player p)
    {
        return p.Health.Value;
    }
    public int GetMaxHealth(Player p)
    {
        return 2 * GetConstitution(p);
    }
    public int GetAbilityCharges(Player p)
    {
        return p.AbilityCharges.Value;
    }
    public int GetMaxAbilityCharges(Player p)
    {
        return GetMod(GetEnergy(p)) + p.Level.Value;
    }
    public int GetXP(Player p)
    {
        return p.XP.Value;
    }
    public int GetNeededXP(Player p)
    {
        return 5 + 5 * p.Level.Value;
    }

    public int GetStrength(Player p)
    {
        int x = p.Strength.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).strength;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).strength;
        return x;
    }
    public int GetDexterity(Player p)
    {
        int x = p.Dexterity.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).dexterity;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).dexterity;
        return x;
    }
    public int GetIntelligence(Player p)
    {
        int x = p.Intelligence.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).intelligence;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).intelligence;
        return x;
    }
    public int GetSpeed(Player p)
    {
        int x = p.Speed.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).speed;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).speed;
        if (itemReference.ContainsKey(p.Armor.Value + ""))
            x += (itemReference[p.Armor.Value + ""] as ArmorCard).speed;
        return x;
    }
    public int GetConstitution(Player p)
    {
        int x = p.Constitution.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).constitution;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).constitution;
        return x;
    }
    public int GetEnergy(Player p)
    {
        int x = p.Energy.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).energy;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).energy;
        if (itemReference.ContainsKey(p.Armor.Value + ""))
            x += (itemReference[p.Armor.Value + ""] as ArmorCard).energy;
        return x;
    }

    public int GetAttack(Player p)
    {
        int x = 0;
        if (itemReference.ContainsKey(p.Weapon.Value + ""))
            x += (itemReference[p.Weapon.Value + ""] as WeaponCard).damage;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Heaven's Paragon"), p) && GetHealth(p) == GetMaxHealth(p))
            x += 2;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Battlelust"), p) && GetHealth(p) < GetMaxHealth(p) * 0.5f)
            x += GetLevel(p);
        return x;
    }
    public int GetCrit(Player p)
    {
        int x = 12;
        if (itemReference.ContainsKey(p.Weapon.Value + ""))
            x = (itemReference[p.Weapon.Value + ""] as WeaponCard).crit;
        return x;
    }
    public int GetArmor(Player p)
    {
        int x = 0;
        if (itemReference.ContainsKey(p.Weapon.Value + ""))
            x += (itemReference[p.Weapon.Value + ""] as WeaponCard).armor;
        if (itemReference.ContainsKey(p.Armor.Value + ""))
            x += (itemReference[p.Armor.Value + ""] as ArmorCard).armor;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Dwarven Defense"), p))
            x += 1;
        return x;
    }
    public int GetPhysicalPower(Player p)
    {
        if (IsMagicalBased(p))
            return 0;
        int x = p.Level.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).physicalPower;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).physicalPower;
        return x;
    }
    public int GetMagicalPower(Player p)
    {
        if (IsPhysicalBased(p))
            return 0;
        int x = p.Level.Value;
        if (itemReference.ContainsKey(p.Ring1.Value + ""))
            x += (itemReference[p.Ring1.Value + ""] as RingCard).magicalPower;
        if (itemReference.ContainsKey(p.Ring2.Value + ""))
            x += (itemReference[p.Ring2.Value + ""] as RingCard).magicalPower;
        return x;
    }
    public int GetLevel(Player p)
    {
        return p.Level.Value;
    }
    public int GetGold(Player p)
    {
        return p.Gold.Value;
    }
    public int GetLevelUpPoints(Player p)
    {
        return p.LevelUpPoints.Value;
    }

    public int GetRange(Player p)
    {
        int x = 0;
        if (itemReference.ContainsKey(p.Weapon.Value + ""))
            x += (itemReference[p.Weapon.Value + ""] as WeaponCard).range;
        return x;
    }
    public bool InAssistingRange(Player p1, Player p2, int range)
    {
        return gameboard[p1.Position.Value].DistanceToTile(p2.Position.Value) <= range + 1;
    }
    public Color GetPlayerColor(Player p)
    {
        return playerColors[p.Color.Value];
    }
    public string GetPlayerColorString(Player p)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(playerColors[p.Color.Value]);
    }
    #endregion

    public List<Vector3Int> GetTreasureTilePositions()
    {
        List<Vector3Int> tiles = new List<Vector3Int>();

        Vector3Int test = new Vector3Int(3, 15, -18);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(5, 9, -14);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(3, 3, -6);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(8, -4, -4);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(7, 3, -10);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(8, 6, -14);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(16, 9, -25);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(21, 0, -21);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(15, -1, -14);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);
        test = new Vector3Int(16, -8, -8);
        if (gameboard[test].TreasureTokenIsEnabled())
            tiles.Add(test);

        return tiles;
    }

    public void SaveGame()
    {
        SaveFile s = new SaveFile("GameData", playerList, turnOrderPlayerList, GetTreasureTilePositions(), encounterDeck, lootDeck, quests, chapterBoss, chaosCounter, turnMarker);
        DataManager d = new DataManager();
        d.WriteSaveFile(s);
    }

    public void DisconnectFromGame()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsConnectedClient)
            NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("JLMainMenu");
    }

    public bool PlayerDisconnected()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] == null)
                return true;
        }
        return false;
    }

    public void EasterEggCheck(Player p)
    {
        //Debug.Log(p.Name.Value);
        //Debug.Log(ComputeSha256Hash(p.Name.Value + ""));
        if (ComputeSha256Hash(p.Name.Value + "") == "c2f81653c077bd8f56aa6eb34d92646ae5c1c4a2b0f7b42ae59f3e3fe022ba07" && p.Race.Value == "Human" && p.Class.Value == "Paladin" && p.Trait.Value == "Holy")
        {
            p.SetValue("Name", "Tychdrion");
            p.SetValue("Image", "ee2");
            p.SetValue("Armor", "Divine Plate Armor");
            p.SetValue("Weapon", "Divine Greatsword");
            p.SetValue("Ring1", "Ring of Divine Justice");
            p.SetValue("Ring2", "Ring of Divine Justice");
            p.SetValue("Inventory1", "Torch");
            p.SetValue("Inventory2", "Torch");
            p.SetValue("Inventory3", "Energy Potion");
            p.SetValue("Inventory4", "Energy Potion");
            p.SetValue("Level", 5);
            p.SetValue("XP", 30);
            p.SetValue("Strength", 20);
            p.SetValue("Dexterity", 20);
            p.SetValue("Intelligence", 20);
            p.SetValue("Speed", 20);
            p.SetValue("Constitution", 20);
            p.SetValue("Energy", 20);
            p.SetValue("LevelUpPoints", 0);
        }
        if (ComputeSha256Hash(p.Name.Value + "") == "43b443c3317b3448f6c50ed3b9e74212419589c7ab1154492e3a716b17a69a1e" && p.Race.Value == "Centaur")
        {
            p.SetValue("Name", "Suspicious Horse");
            p.SetValue("Image", "ee3");
        }
    }

    static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}