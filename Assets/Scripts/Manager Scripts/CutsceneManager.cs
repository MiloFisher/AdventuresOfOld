using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using System;
using Unity.Netcode;

public class CutsceneManager : Singleton<CutsceneManager>
{
    GameObject[] players;
    Player localPlayer;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        // Gets local player
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player>().IsLocalPlayer)
            {
                localPlayer = p.GetComponent<Player>();
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
                if (!g.GetComponent<Player>().Ready.Value && !g.GetComponent<Player>().isBot)
                    allReady = false;
            }
            return allReady;
        });

        // Once all players are ready, unready them all and start the game
        foreach (GameObject g in players)
        {
            g.GetComponent<Player>().Unready();
        }

        // when all players are ready, have host do this:
        localPlayer.LoadIntoCutscene("Prophecy");
    }

    public void LoadCutsceneEncounter(string cutsceneName)
    {
        Invoke(cutsceneName, 0);
    }

    public void Prophecy()
    {
        Action OnComplete = default;
        if (NetworkManager.Singleton.IsHost)
        {
            OnComplete = () => {
                localPlayer.ChangeScene("Character Creation");
            };
        }

        QuestManager.Instance.LoadIntoQuest(NetworkManager.Singleton.IsHost, new List<Action> {
            () => {
                // Chunk 1
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("The Oracle");
                QuestManager.Instance.SetDialogue("Beware the bleeding eye of the Gray Fox, who leaves at dawn but comes from dusk.");
                QuestManager.Instance.PlayAudio("Prophecy", 0f, 6.8f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 2
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("The Oracle");
                QuestManager.Instance.SetDialogue("Beware the Acolytes who come from Chaos, who will not rest until all is lost.");
                QuestManager.Instance.PlayAudio("Prophecy", 6.8f, 14f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 3
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("The Oracle");
                QuestManager.Instance.SetDialogue("Look for the ones born with the heart of a phoenix, scarred by the sun but made to lead us.");
                QuestManager.Instance.PlayAudio("Prophecy", 14f, 21.6f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 4
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("The Oracle");
                QuestManager.Instance.SetDialogue("Look toward them for salvation, lest the world fall to damnation.");
                QuestManager.Instance.PlayAudio("Prophecy", 21.6f, 28f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 5
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("For the realm of Sol, this was a prophecy of doom, an old sage’s tale, and a bedroom song to the children.  Whether or not those who heard it believed in its omen, life continued on and the land prospered under its brilliant rays of sunlight.");
                QuestManager.Instance.PlayAudio("Prophecy", 28f, 43.4f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 6
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("It wasn’t until a fateful night that the land realized that the warnings of Chaos were true, when the moon reached its highest point in the night sky and glowed crimson.  And it wasn’t until people left their homes to behold the sight, that the crimson moon began to leak its bleeding eye.");
                QuestManager.Instance.PlayAudio("Prophecy", 43.4f, 60.5f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 7
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The land stood silent as the blood seeped deep into their world. Then, all at once, the pillars of Chaos erupted shooting into the starry heavens above.  In the hours since, the various beings of the realm convened to discuss the coming doom.");
                QuestManager.Instance.PlayAudio("Prophecy", 60.5f, 79.5f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 8
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The old prophecy seemed to be coming true, and the only ones who could save the realm and kill these Chaos Acolytes were those born with the heart of a phoenix.  Some argued the saviors were those born during the time when the sun was highest in the sky,");
                QuestManager.Instance.PlayAudio("Prophecy", 79.5f, 93.7f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 9
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("others argued it were those who had the courage like no other, but no one knew for certain.  As dawn broke and the sun arrived, the moon had disappeared but the pillars of Chaos were still engulfing the sky, slowly but surely turning the daylight crimson.");
                QuestManager.Instance.PlayAudio("Prophecy", 93.7f, 111f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 10
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("The realm called forth upon those who believed they were the saviors of prophecy.  Several individuals came forward, all claiming to be the heroes of prophecy.  Without many other options, the realm has placed their trust in your party to defeat the Acolytes who come from Chaos.");
                QuestManager.Instance.PlayAudio("Prophecy", 111f, 128f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.CONTINUE);
            },
            () => {
                // Chunk 11
                QuestManager.Instance.SetImage("");
                QuestManager.Instance.SetSpeaker("Narrator");
                QuestManager.Instance.SetDialogue("Will this be a party of heroes who save the world?  Or will this be a band of misfits who will die horrible deaths?  Your party introduces themselves to each other, and begins their adventure.");
                QuestManager.Instance.PlayAudio("Prophecy", 128f, 140.8f);
                QuestManager.Instance.SetButtonDisplay(ButtonDisplay.FINISH);
            }
        }, OnComplete, localPlayer);
    }
}
