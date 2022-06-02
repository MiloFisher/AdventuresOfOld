using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AdventuresOfOldMultiplayer;
using Unity.Collections;

public class DataManager
{
    public SaveFile GetSaveFile(string filename, bool includeGameData = false)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + filename))
            return default;

        SaveFile s = new SaveFile(filename);
        var file = File.OpenText(Application.persistentDataPath + "/" + filename);

        // Read in players from file
        int playerCount = int.Parse(file.ReadLine());
        for(int i = 0; i < playerCount; i++)
        {
            PlayerData p = new PlayerData();

            p.isBot = bool.Parse(file.ReadLine());
            p.Username = file.ReadLine();
            p.UUID = file.ReadLine();
            p.Image = file.ReadLine();
            p.Name = file.ReadLine();
            p.Race = file.ReadLine();
            p.Class = file.ReadLine();
            p.Trait = file.ReadLine();
            p.Gold = int.Parse(file.ReadLine());
            p.Level = int.Parse(file.ReadLine());
            p.XP = int.Parse(file.ReadLine());
            p.Strength = int.Parse(file.ReadLine());
            p.Dexterity = int.Parse(file.ReadLine());
            p.Intelligence = int.Parse(file.ReadLine());
            p.Speed = int.Parse(file.ReadLine());
            p.Constitution = int.Parse(file.ReadLine());
            p.Energy = int.Parse(file.ReadLine());
            p.Health = int.Parse(file.ReadLine());
            p.AbilityCharges = int.Parse(file.ReadLine());
            p.Armor = file.ReadLine();
            p.Weapon = file.ReadLine();
            p.Ring1 = file.ReadLine();
            p.Ring2 = file.ReadLine();
            p.Inventory1 = file.ReadLine();
            p.Inventory2 = file.ReadLine();
            p.Inventory3 = file.ReadLine();
            p.Inventory4 = file.ReadLine();
            p.Inventory5 = file.ReadLine();
            p.LevelUpPoints = int.Parse(file.ReadLine());
            p.FailedEncounters = int.Parse(file.ReadLine());
            int posX = int.Parse(file.ReadLine());
            int posY = int.Parse(file.ReadLine());
            int posZ = int.Parse(file.ReadLine());
            p.Position = new Vector3Int(posX, posY, posZ);
            p.TurnPhase = int.Parse(file.ReadLine());
            p.Color = int.Parse(file.ReadLine());
            p.Ready = bool.Parse(file.ReadLine());
            p.EndOfDayActivity = int.Parse(file.ReadLine());
            p.ParticipatingInCombat = int.Parse(file.ReadLine());
            p.HasBathWater = bool.Parse(file.ReadLine());
            p.BetrayedBoy = bool.Parse(file.ReadLine());
            p.GrabbedHorse = bool.Parse(file.ReadLine());
            p.KilledGoblin = bool.Parse(file.ReadLine());
            p.RequestedTaunt = bool.Parse(file.ReadLine());
            p.IronWill = bool.Parse(file.ReadLine());
            p.HasYetToAttack = bool.Parse(file.ReadLine());
            p.JusticarsVow = bool.Parse(file.ReadLine());
            p.SuccessfullyAttackedMonster = bool.Parse(file.ReadLine());

            s.playerList.Add(p);
        }

        // Return here if not including game data
        if (!includeGameData)
        {
            file.Close();
            return s;
        } 

        // Read turn order list from file
        string[] turnOrder = file.ReadLine().Split(',');
        foreach(string element in turnOrder)
            s.turnOrderPlayerList.Add(element);

        // Read treasure tiles from file
        string[] xPositions = file.ReadLine().Split(',');
        string[] yPositions = file.ReadLine().Split(',');
        string[] zPositions = file.ReadLine().Split(',');
        if(xPositions[0] != "x")
        {
            for(int i = 0; i < xPositions.Length; i++)
                s.treasureTiles.Add(new Vector3Int(int.Parse(xPositions[i]), int.Parse(yPositions[i]), int.Parse(zPositions[i])));
        }

        // Read encounter deck from file
        string[] encounterDeck = file.ReadLine().Split(',');
        foreach (string element in encounterDeck)
            s.encounterDeck.Add(PlayManager.Instance.encounterReference[element]);

        // Read loot deck from file
        string[] lootDeck = file.ReadLine().Split(',');
        foreach (string element in lootDeck)
            s.lootDeck.Add(PlayManager.Instance.itemReference[element]);

        // Read quests from file
        string[] quests = file.ReadLine().Split(',');
        string[] questSteps = file.ReadLine().Split(',');
        for (int i = 0; i < quests.Length; i++)
        {
            QuestCard q = PlayManager.Instance.questReference[quests[i]];
            q.questStep = int.Parse(questSteps[i]);
            s.quests.Add(q);
        }

        // Read chapter boss from file
        string bossName = file.ReadLine();
        s.chapterBoss = PlayManager.Instance.chapterBossDeck.Find((a) => a.cardName == bossName);

        // Read chaos counter from file
        s.chaosCounter = int.Parse(file.ReadLine());

        // Read turn marker from file
        s.turnMarker = int.Parse(file.ReadLine());

        file.Close();
        return s;
    }

    public void WriteSaveFile(SaveFile s)
    {
        var file = File.CreateText(Application.persistentDataPath + "/" + s.filename);

        // Write players to file
        file.WriteLine(s.playerList.Count);
        foreach(PlayerData p in s.playerList)
        {
            file.WriteLine(p.isBot);
            file.WriteLine(p.Username);
            file.WriteLine(p.UUID);
            file.WriteLine(p.Image);
            file.WriteLine(p.Name);
            file.WriteLine(p.Race);
            file.WriteLine(p.Class);
            file.WriteLine(p.Trait);
            file.WriteLine(p.Gold);
            file.WriteLine(p.Level);
            file.WriteLine(p.XP);
            file.WriteLine(p.Strength);
            file.WriteLine(p.Dexterity);
            file.WriteLine(p.Intelligence);
            file.WriteLine(p.Speed);
            file.WriteLine(p.Constitution);
            file.WriteLine(p.Energy);
            file.WriteLine(p.Health);
            file.WriteLine(p.AbilityCharges);
            file.WriteLine(p.Armor);
            file.WriteLine(p.Weapon);
            file.WriteLine(p.Ring1);
            file.WriteLine(p.Ring2);
            file.WriteLine(p.Inventory1);
            file.WriteLine(p.Inventory2);
            file.WriteLine(p.Inventory3);
            file.WriteLine(p.Inventory4);
            file.WriteLine(p.Inventory5);
            file.WriteLine(p.LevelUpPoints);
            file.WriteLine(p.FailedEncounters);
            file.WriteLine(p.Position.x);
            file.WriteLine(p.Position.y);
            file.WriteLine(p.Position.z);
            file.WriteLine(p.TurnPhase);
            file.WriteLine(p.Color);
            file.WriteLine(p.Ready);
            file.WriteLine(p.EndOfDayActivity);
            file.WriteLine(p.ParticipatingInCombat);
            file.WriteLine(p.HasBathWater);
            file.WriteLine(p.BetrayedBoy);
            file.WriteLine(p.GrabbedHorse);
            file.WriteLine(p.KilledGoblin);
            file.WriteLine(p.RequestedTaunt);
            file.WriteLine(p.IronWill);
            file.WriteLine(p.HasYetToAttack);
            file.WriteLine(p.JusticarsVow);
            file.WriteLine(p.SuccessfullyAttackedMonster);
        }

        // Write turn order list to file
        string turnOrder = s.turnOrderPlayerList[0] + "";
        for (int i = 1; i < s.turnOrderPlayerList.Count; i++)
            turnOrder += "," + s.turnOrderPlayerList[i];
        file.WriteLine(turnOrder);

        // Write treasure tiles to file
        if (s.treasureTiles.Count > 0)
        {
            string xPos = s.treasureTiles[0].x + "";
            for (int i = 1; i < s.treasureTiles.Count; i++)
                xPos += "," + s.treasureTiles[i].x;
            file.WriteLine(xPos);
            string yPos = s.treasureTiles[0].y + "";
            for (int i = 1; i < s.treasureTiles.Count; i++)
                yPos += "," + s.treasureTiles[i].y;
            file.WriteLine(yPos);
            string zPos = s.treasureTiles[0].z + "";
            for (int i = 1; i < s.treasureTiles.Count; i++)
                zPos += "," + s.treasureTiles[i].z;
            file.WriteLine(zPos);
        }
        else
        {
            file.WriteLine("x");
            file.WriteLine("x");
            file.WriteLine("x");
        }

        // Write encounter deck to file
        string encounterDeck = s.encounterDeck[0].cardName;
        for (int i = 1; i < s.encounterDeck.Count; i++)
            encounterDeck += "," + s.encounterDeck[i].cardName;
        file.WriteLine(encounterDeck);

        // Write loot deck to file
        string lootDeck = s.lootDeck[0].cardName;
        for (int i = 1; i < s.lootDeck.Count; i++)
            lootDeck += "," + s.lootDeck[i].cardName;
        file.WriteLine(lootDeck);

        // Write quests to file
        string quests = s.quests[0].cardName;
        for (int i = 1; i < s.quests.Count; i++)
            quests += "," + s.quests[i].cardName;
        file.WriteLine(quests);
        string questsSteps = s.quests[0].questStep + "";
        for (int i = 1; i < s.quests.Count; i++)
            questsSteps += "," + s.quests[i].questStep;
        file.WriteLine(questsSteps);

        // Write chapter boss to file
        file.WriteLine(s.chapterBoss.cardName);

        // Write chaos counter to file
        file.WriteLine(s.chaosCounter);

        // Write turn marker to file
        file.WriteLine(s.turnMarker);

        file.Close();

        //Debug.Log("Saved Game to: " + Application.persistentDataPath);
    }
}

public class PlayerData
{
    public bool isBot;
    public FixedString64Bytes Username;
    public FixedString64Bytes UUID;
    public FixedString64Bytes Image;
    public FixedString64Bytes Name;
    public FixedString64Bytes Race;
    public FixedString64Bytes Class;
    public FixedString64Bytes Trait;
    public int Gold;
    public int Level;
    public int XP;
    public int Strength;
    public int Dexterity;
    public int Intelligence;
    public int Speed;
    public int Constitution;
    public int Energy;
    public int Health;
    public int AbilityCharges;
    public FixedString64Bytes Armor;
    public FixedString64Bytes Weapon;
    public FixedString64Bytes Ring1;
    public FixedString64Bytes Ring2;
    public FixedString64Bytes Inventory1;
    public FixedString64Bytes Inventory2;
    public FixedString64Bytes Inventory3;
    public FixedString64Bytes Inventory4;
    public FixedString64Bytes Inventory5;

    // Additional Character Data
    public int LevelUpPoints;
    public int FailedEncounters;

    // Gameplay Data
    public Vector3Int Position;
    public int TurnPhase;
    public int Color;
    public bool Ready;
    public int EndOfDayActivity;
    public int ParticipatingInCombat;

    // Quest data
    public bool HasBathWater;
    public bool BetrayedBoy;
    public bool GrabbedHorse;
    public bool KilledGoblin;

    // Ability data
    public bool RequestedTaunt;
    public bool IronWill;
    public bool HasYetToAttack;
    public bool JusticarsVow;
    public bool SuccessfullyAttackedMonster;

    public PlayerData() { }

    public PlayerData(Player p)
    {
        isBot = p.isBot;
        Username = p.Username.Value;
        UUID = p.UUID.Value;
        Image = p.Image.Value;
        Name = p.Name.Value;
        Race = p.Race.Value;
        Class = p.Class.Value;
        Trait = p.Trait.Value;
        Gold = p.Gold.Value;
        Level = p.Level.Value;
        XP = p.XP.Value;
        Strength = p.Strength.Value;
        Dexterity = p.Dexterity.Value;
        Intelligence = p.Intelligence.Value;
        Speed = p.Speed.Value;
        Constitution = p.Constitution.Value;
        Energy = p.Energy.Value;
        Health = p.Health.Value;
        AbilityCharges = p.AbilityCharges.Value;
        Armor = p.Armor.Value;
        Weapon = p.Weapon.Value;
        Ring1 = p.Ring1.Value;
        Ring2 = p.Ring2.Value;
        Inventory1 = p.Inventory1.Value;
        Inventory2 = p.Inventory2.Value;
        Inventory3 = p.Inventory3.Value;
        Inventory4 = p.Inventory4.Value;
        Inventory5 = p.Inventory5.Value;
        LevelUpPoints = p.LevelUpPoints.Value;
        FailedEncounters = p.FailedEncounters.Value;
        Position = p.Position.Value;
        TurnPhase = p.TurnPhase.Value;
        Color = p.Color.Value;
        Ready = p.Ready.Value;
        EndOfDayActivity = p.EndOfDayActivity.Value;
        ParticipatingInCombat = p.ParticipatingInCombat.Value;
        HasBathWater = p.HasBathWater.Value;
        BetrayedBoy = p.BetrayedBoy.Value;
        GrabbedHorse = p.GrabbedHorse.Value;
        KilledGoblin = p.KilledGoblin.Value;
        RequestedTaunt = p.RequestedTaunt.Value;
        IronWill = p.IronWill.Value;
        HasYetToAttack = p.HasYetToAttack.Value;
        JusticarsVow = p.JusticarsVow.Value;
        SuccessfullyAttackedMonster = p.SuccessfullyAttackedMonster.Value;
    }
}

public class SaveFile
{
    public string filename;
    public List<PlayerData> playerList;
    public List<FixedString64Bytes> turnOrderPlayerList;
    public List<Vector3Int> treasureTiles;
    public List<EncounterCard> encounterDeck;
    public List<LootCard> lootDeck;
    public List<QuestCard> quests;
    public MonsterCard chapterBoss;
    public int chaosCounter;
    public int turnMarker;

    public SaveFile(string filename)
    {
        this.filename = filename;
        playerList = new List<PlayerData>();
        turnOrderPlayerList = new List<FixedString64Bytes>();
        treasureTiles = new List<Vector3Int>();
        encounterDeck = new List<EncounterCard>();
        lootDeck = new List<LootCard>();
        quests = new List<QuestCard>();
        chapterBoss = default;
        chaosCounter = 0;
        turnMarker = 0;
    }

    public SaveFile(string filename, List<Player> playerList, List<Player> turnOrderPlayerList, List<Vector3Int> treasureTiles, List<EncounterCard> encounterDeck, List<LootCard> lootDeck, List<QuestCard> quests, MonsterCard chapterBoss, int chaosCounter, int turnMarker)
    {
        this.filename = filename;
        this.playerList = new List<PlayerData>();
        foreach (Player p in playerList)
            this.playerList.Add(new PlayerData(p));
        this.turnOrderPlayerList = new List<FixedString64Bytes>();
        foreach (Player p in turnOrderPlayerList)
            this.turnOrderPlayerList.Add(p.UUID.Value);
        this.treasureTiles = treasureTiles;
        this.encounterDeck = encounterDeck;
        this.lootDeck = lootDeck;
        this.quests = quests;
        this.chapterBoss = chapterBoss;
        this.chaosCounter = chaosCounter;
        this.turnMarker = turnMarker;
    }
}
