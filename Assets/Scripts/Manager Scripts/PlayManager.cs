using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Dictionary<string, EncounterCard> encounterReference = new Dictionary<string, EncounterCard>();

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

    public int turnMarker;

    public bool isYourTurn;

    public GameObject transitions;

    public GameObject encounterElements;

    public GameObject endOfDayElements;

    public GameObject[] playerPieces;

    public GameObject[] characterPanels;

    public GameObject characterDisplays;
    public GameObject characterDisplayMinimizeButton;
    public float minimizedX = -1600f;
    public float maximizedX = 518f;
    public float characterSheetOpenLength = 0.004f;
    public Player selectedPlayer;
    public bool characterDisplayOpen;

    public Sprite[] portraits;

    public GameObject loadingScreen;

    public EncounterCard testingCard;

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
            LoadGameSetup();
    }

    private void Update()
    {
        if (!loadingScreen.activeInHierarchy)
            DrawPlayerPieces();
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
            p.SetValue("Ring1", InventoryManager.Instance.emptyValue);
            p.SetValue("Ring2", InventoryManager.Instance.emptyValue);
            p.SetValue("Inventory1", InventoryManager.Instance.emptyValue);
            p.SetValue("Inventory2", InventoryManager.Instance.emptyValue);
            p.SetValue("Inventory3", InventoryManager.Instance.emptyValue);
            p.SetValue("Inventory4", InventoryManager.Instance.emptyValue);
            p.SetValue("Inventory5", InventoryManager.Instance.emptyValue);

            // Set player positions to starting tile
            p.SetPosition(new Vector3Int(0, 7, -7));
        }

        // 3) Deal Quest Cards (host only)
        ShuffleDeck(questDeck);
        for (int i = 0; i < Mathf.FloorToInt(playerList.Count / 2); i++)
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

        // *** Testing Only ***
        encounterDeck[0] = testingCard;

        // 6) Miniboss and minion decks already set up

        // Give each player a color (host only)
        string[] colorList = { "red", "blue", "green", "yellow", "purple", "orange" };
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].SetValue("Color", colorList[i]);
        }

        yield return new WaitForSeconds(1);

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

    // (Host Only)
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
            p.UpdateCharacterPanelsClientRPC();
            // Play transition for all players
            p.PlayTransitionClientRPC(0); // Transition 0 is Start of Day
        }

        // Start turn of player who goes first
        turnOrderPlayerList[turnMarker].StartTurnClientRPC();
    }

    public void EndOfDay()
    {
        // Call Choose Activity prompt which is element 0
        CallEndOfDayElement(0);
    }

    public void StartTurn()
    {
        // Set the variable to mark it is this player's turn
        isYourTurn = true;
        if (!transitions.transform.GetChild(0).gameObject.activeInHierarchy)
            CallTransition(1);
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

    public void MovePhase()
    {
        // Activate all tiles within the player's move range
        gameboard[localPlayer.Position.Value].Activate(GetMod(localPlayer.Speed.Value));
    }

    public void EncounterPhase()
    {
        // First Parse tile landed on
        Vector3Int tilePos = gameboard[localPlayer.Position.Value].position;

        // Check NPC tiles first
        if (tilePos == new Vector3Int(13, 0, -13))
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
        // Deactivate the selected tiles and move the player to the target position
        foreach (KeyValuePair<Vector3Int, Tile> t in gameboard)
            t.Value.Deactivate();
        localPlayer.SetPosition(pos);

        // Call Encounter Phase transition
        CallTransition(2);
    }

    public void CallTransition(int id)
    {
        transitions.transform.GetChild(id).gameObject.SetActive(true);
    }

    public void CallEncounterElement(int id)
    {
        encounterElements.transform.GetChild(id).gameObject.SetActive(true);
    }

    public void CallEndOfDayElement(int id)
    {
        endOfDayElements.transform.GetChild(id).gameObject.SetActive(true);
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

    public int GetMod(int stat)
    {
        return Mathf.FloorToInt((stat - 10) * 0.5f);
    }

    public void SetupPlayerPieces()
    {
        int i;
        for (i = 0; i < playerList.Count; i++)
        {
            playerPieces[i].SetActive(true);
            playerPieces[i].GetComponent<Image>().color = ColorLookUp(playerList[i].Color.Value + "");
        }
        for (; i < 6; i++)
        {
            playerPieces[i].SetActive(false);
        }
    }

    public void SetupCharacterPanels()
    {
        // Set up portrait dictionary
        Dictionary<FixedString64Bytes, Sprite> portaitDictionary = new Dictionary<FixedString64Bytes, Sprite>();
        foreach (Sprite s in portraits)
        {
            portaitDictionary.Add(s.name, s);
        }

        int i;
        for (i = 0; i < turnOrderPlayerList.Count; i++)
        {
            characterPanels[i].SetActive(true);
            characterPanels[i].transform.localPosition = new Vector3(characterPanels[i].transform.localPosition.x, 117.5f * (turnOrderPlayerList.Count - 2 * i - 1), 0);
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateCharacterImage(portaitDictionary[turnOrderPlayerList[i].Image.Value]);
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateCharacterName(turnOrderPlayerList[i].Name.Value + "", turnOrderPlayerList[i].Color.Value + "");
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
            characterPanels[i].GetComponent<UICharacterPanel>().UpdateHealthbar(turnOrderPlayerList[i].Health.Value, 2 * turnOrderPlayerList[i].Constitution.Value);
        }
    }

    public Color ColorLookUp(string color)
    {
        switch (color)
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            case "green": return Color.green;
            case "purple": return new Color(0.627f, 0, 1);
            case "yellow": return Color.yellow;
            case "orange": return new Color(1, 0.627f, 0);
            default: return Color.black;
        }
    }

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
        for (int i = 1; i <= 100; i++)
        {
            characterDisplays.transform.localPosition = new Vector3(minimizedX + (maximizedX - minimizedX) * i * 0.01f, 0, 0);
            yield return new WaitForSeconds(characterSheetOpenLength);
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
        for (int i = 99; i >= 0; i--)
        {
            characterDisplays.transform.localPosition = new Vector3(minimizedX + (maximizedX - minimizedX) * i * 0.01f, 0, 0);
            yield return new WaitForSeconds(characterSheetOpenLength);
        }
        characterDisplayMinimizeButton.SetActive(false);
    }

    public int GetStatModFromType(string statRollType)
    {
        switch (statRollType)
        {
            case "STR": return GetMod(localPlayer.Strength.Value);
            case "DEX": return GetMod(localPlayer.Dexterity.Value);
            case "INT": return GetMod(localPlayer.Intelligence.Value);
            case "SPD": return GetMod(localPlayer.Speed.Value);
            case "CON": return GetMod(localPlayer.Constitution.Value);
            case "ENG": return GetMod(localPlayer.Energy.Value);
            default: Debug.LogError("Unknown Stat Roll Type: " + statRollType); return 0;
        }
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
        // Fill later
        localPlayer.DrawEncounterCards(1, localPlayer.UUID.Value, true);

        // EndTurn();
    }

    public void DefaultTile()
    {
        // Either give an encounter if they have 2 fails, otherswise activated prompt to roll for encounter
        if (localPlayer.FailedEncounters.Value == 2)
            GetEncounter();
        else
            CallEncounterElement(0);
    }

    public void TreasureTile()
    {
        DefaultTile(); // Temporary
    }

    public void CrazedHermit()
    {
        DefaultTile(); // Temporary
    }

    public void DistressedVillager()
    {
        DefaultTile(); // Temporary
    }

    public void ForestHag()
    {
        DefaultTile(); // Temporary
    }

    public void FourEyedBoy()
    {
        DefaultTile(); // Temporary
    }

    public void PabloTheNoob()
    {
        DefaultTile(); // Temporary
    }

    public void ShiftyPeddler()
    {
        DefaultTile(); // Temporary
    }

    public void SuspiciousHorse()
    {
        DefaultTile(); // Temporary
    }

    public void VeteranHunter()
    {
        DefaultTile(); // Temporary
    }

    public void AbandonedOutpost()
    {
        DefaultTile(); // Temporary
    }

    public void AncientSpring()
    {
        DefaultTile(); // Temporary
    }

    public void BanditHideout()
    {
        DefaultTile(); // Temporary
    }

    public void HowlingCave()
    {
        DefaultTile(); // Temporary
    }

    public void OminousClearing()
    {
        DefaultTile(); // Temporary
    }

    public void OvergrownTemple()
    {
        DefaultTile(); // Temporary
    }

    public void WebbedForest()
    {
        DefaultTile(); // Temporary
    }

    #region Chaos Counter Functions
    public int XPModifier()
    {
        return 2 * Mathf.FloorToInt((chaosCounter - 1) / 4);
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
    }

    public void IncreaseChaos(int amount)
    {
        chaosCounter += amount;
        if (chaosCounter > 21)
            chaosCounter = 21;
    }

    public int ChaosTier()
    {
        return 1 + Mathf.FloorToInt((chaosCounter - 1) / 4);
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
    #endregion
}