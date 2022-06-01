using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class CharacterCreationManager : MonoBehaviour
{
    Player localPlayer;
    [SerializeField] private GameObject UIMan;
    private GameObject canvas;
    private GameObject title;
    private GameObject[] players;
    private bool readiedUp;

    private string emptyValue = "empty";

    //Creating Race Class for each default race
    //order: str, dex, int, spd, con, eng
    List<Race> default_races = new List<Race>() {
        new Race("Human", new Stats(10,10,10,18,10,12), "", ""),
        new Race("High Elf", new Stats(7,11,12,18,8,14), "", ""),
        new Race("Night Elf", new Stats(9,12,9,18,10,12), "", ""),
        new Race("Dwarf", new Stats(12,10,8,16,12,12), "", ""),
        new Race("Centaur", new Stats(8,13,9,20,8,12), "", ""),
        new Race("Leonin", new Stats(12,12,6,18,10,12), "", ""),
        new Race("Aasimar", new Stats(11,8,11,18,8,14), "", ""),
        //Admin class
        //new Race("Dragon Amongst Men", new Stats(99,99,99,99,99,99), "Do not refuse a toast just to drink a forfeit.", "Peerless")
    };


    void setupLocalCharacter() {
        players = GameObject.FindGameObjectsWithTag("Player");

        // Gets local player
        foreach(GameObject p in players)
        {
            if (p.GetComponent<Player>().IsLocalPlayer)
            {
                localPlayer = p.GetComponent<Player>();
            }
        }

        CreatedChar player = UIMan.GetComponent<CharManUI>().getChar();
        // Set Variable
        localPlayer.SetValue("Name", player.getName());
        localPlayer.SetValue("Race", player.getChosen_race().get_name());
        localPlayer.SetValue("Class", player.getChosen_class().getName());
        localPlayer.SetValue("Trait", player.getChosen_trait().getName());
        if (player.getChosen_trait().getName() == "Highborn") {
            localPlayer.SetValue("Gold", 100);
        }
        else {
            localPlayer.SetValue("Gold", 10);
        }
        localPlayer.SetValue("Level", 1);
        localPlayer.SetValue("XP", 0);
        localPlayer.SetValue("Strength", player.getStr());
        localPlayer.SetValue("Dexterity", player.getDex());
        localPlayer.SetValue("Intelligence", player.getInte());
        localPlayer.SetValue("Speed", player.getSpd());
        localPlayer.SetValue("Constitution", player.getCon());
        localPlayer.SetValue("Energy", player.getEng());
        localPlayer.SetValue("Image", player.getImage());
        // Set gear to empty
        localPlayer.SetValue("Weapon", player.getWeapon());
        localPlayer.SetValue("Armor", player.getArmor());
        localPlayer.SetValue("Ring1", player.getRing1());
        localPlayer.SetValue("Ring2", player.getRing2());
        localPlayer.SetValue("Inventory1", emptyValue);
        localPlayer.SetValue("Inventory2", emptyValue);
        localPlayer.SetValue("Inventory3", emptyValue);
        localPlayer.SetValue("Inventory4", emptyValue);
        localPlayer.SetValue("Inventory5", emptyValue);

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
                    p.SetValue("Image", "portrait_human");
                    // Set gear to empty
                    p.SetValue("Weapon", emptyValue);
                    p.SetValue("Armor", emptyValue);
                    p.SetValue("Ring1", emptyValue);
                    p.SetValue("Ring2", emptyValue);
                    p.SetValue("Inventory1", emptyValue);
                    p.SetValue("Inventory2", emptyValue);
                    p.SetValue("Inventory3", emptyValue);
                    p.SetValue("Inventory4", emptyValue);
                    p.SetValue("Inventory5", emptyValue);
                    // Ready up bots to load into game
                    p.ReadyUp();
                }
            }
        }

        // Ready up local player to load into game
        localPlayer.ReadyUp();

        // If player is host, Wait for other players before changing scene
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(WaitForPlayers());
        }
    }

    IEnumerator WaitForPlayers()
    {
        // Wait until all players are ready
        yield return new WaitUntil(() => {
            bool allReady = true;
            foreach (GameObject g in players)
            {
                if (!g.GetComponent<Player>().Ready.Value)
                    allReady = false;
            }
            return allReady;
        });

        // Once all players are ready, unready them all and start the game
        foreach (GameObject g in players)
        {
            g.GetComponent<Player>().Unready();
        }
        localPlayer.ChangeScene("Core Game");
    }

    // Start is called before the first frame update
    void Start()
    {   
        //Setting up beginning screen
        title = GameObject.Find("Title_TXT");
        title.GetComponent<TextMeshProUGUI>().SetText("Race Selection");
        UIMan.GetComponent<CharManUI>().gamestart = false;


        //Scrollable container to spawn the buttons in
        canvas = GameObject.Find("RaceTextContainer");

        foreach (Race race in default_races) {
            UIMan.GetComponent<CharManUI>().RaceButtonBuilder(race, canvas.transform);
        }

    }

    void Update() {
        if (UIMan.GetComponent<CharManUI>().gamestart && !readiedUp) {
            setupLocalCharacter();
            readiedUp = true;
        }
    }
}
