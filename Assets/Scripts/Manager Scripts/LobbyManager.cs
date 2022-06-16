using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using Unity.Collections;

namespace AdventuresOfOldMultiplayer
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        public TMP_InputField joinCode;
        public TMP_Text lobbyCode;

        public GameObject hostingInProgress;
        public GameObject joiningInProgress;
        public GameObject startGameButton;
        public GameObject addBotButton;

        public TMP_Text[] playerList;
        public GameObject[] removeButtonList;

        public GameObject botPlayerPrefab;
        public GameObject networkManagerPrefab;

        private bool inLobby;

        public int playersInLobby;
        public string lobbyType;
        public SaveFile save;
        public GameObject noSavedDataFoundMessage;
        public bool botsEnabled;

        private void Awake()
        {
            if (GameObject.FindGameObjectWithTag("Network Manager") == null)
                Instantiate(networkManagerPrefab);
        }

        void Start()
        {
            hostingInProgress.SetActive(false);
            joiningInProgress.SetActive(false);

            PlayerPrefs.SetInt("IntroEnabled", 1);
            Application.targetFrameRate = Global.frameCap;
        }

        void Update()
        {
            if (inLobby && !NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost)
            {
                LeaveLobby();
            }
            if (inLobby)
            {
                if (lobbyType == "Load Game")
                {
                    int x = 0;
                    GameObject[] players = GetOrderedPlayers();
                    playersInLobby = players.Length;
                    foreach (PlayerData p in save.playerList)
                    {
                        bool joined = PlayerJoined(p.UUID);
                        if (x == 0)
                        {
                            playerList[x].text = "[Host] ";
                        }
                        else if (p.isBot)
                        {
                            playerList[x].text = "[Bot] ";
                        }
                        else
                        {
                            playerList[x].text = "[Client] ";
                        }
                        playerList[x].color = joined ? Color.black : Color.white;
                        playerList[x].text += p.Username + "";

                        if (x != 0)
                        {
                            removeButtonList[x - 1].SetActive(NetworkManager.Singleton.IsHost && joined);
                        }
                        x++;
                    }

                    for (; x < 6; x++)
                    {
                        playerList[x].text = "";
                        if (x != 0)
                            removeButtonList[x - 1].SetActive(false);
                    }

                    if (NetworkManager.Singleton.IsHost)
                    {
                        if(playersInLobby > 6)
                        {
                            for (int i = 6; i < playersInLobby; i++)
                            {
                                RemovePlayer(i);
                            }
                        }

                        for (int i = 0; i < players.Length; i++)
                        {
                            if(!PlayerIsInSave(players[i]))
                            {
                                RemovePlayer(i);
                            }
                        }
                    }

                    startGameButton.SetActive(playersInLobby == save.playerList.Count && NetworkManager.Singleton.IsHost);
                    bool allAdded = true;
                    foreach (PlayerData p in save.playerList)
                    {
                        if (p.isBot)
                        {
                            bool exists = false;
                            foreach (GameObject g in players)
                            {
                                if (g.GetComponent<Player>().UUID.Value == p.UUID)
                                    exists = true;
                            }
                            if (!exists)
                                allAdded = false;
                        }
                    }
                    addBotButton.SetActive(!allAdded && NetworkManager.Singleton.IsHost && botsEnabled);
                }
                else // if (lobbyType == "New Game")
                {
                    int x = 0;
                    GameObject[] players = GetOrderedPlayers();
                    playersInLobby = players.Length;
                    foreach (GameObject player in players)
                    {
                        if (x < 6)
                        {
                            if (x == 0)
                            {
                                playerList[x].text = "[Host] ";
                            }
                            else if (player.GetComponent<Player>().isBot)
                            {
                                playerList[x].text = "[Bot] ";
                            }
                            else
                            {
                                playerList[x].text = "[Client] ";
                            }
                            playerList[x].color = Color.black;
                            playerList[x++].text += player.GetComponent<Player>().Username.Value + "";
                        }

                        if (x != 0 && x < 6)
                        {
                            removeButtonList[x - 1].SetActive(NetworkManager.Singleton.IsHost);
                        }
                    }

                    for (; x < 6; x++)
                    {
                        playerList[x].text = "";
                        if (x != 0)
                            removeButtonList[x - 1].SetActive(false);
                    }

                    if (playersInLobby > 6 && NetworkManager.Singleton.IsHost)
                    {
                        for (int i = 6; i < playersInLobby; i++)
                        {
                            RemovePlayer(i);
                        }
                    }

                    startGameButton.SetActive(playersInLobby >= 3 && NetworkManager.Singleton.IsHost);
                    addBotButton.SetActive(playersInLobby < 6 && NetworkManager.Singleton.IsHost && botsEnabled);
                }
            }
        }

        public bool PlayerIsInSave(GameObject player)
        {
            for(int i = 0; i < save.playerList.Count; i++)
            {
                if (player.GetComponent<Player>().UUID.Value == save.playerList[i].UUID || player.GetComponent<Player>().isBot)
                    return true;
            }
            return false;
        }

        public bool PlayerJoined(FixedString64Bytes UUID)
        {
            GameObject[] players = GetOrderedPlayers();
            for(int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<Player>().UUID.Value == UUID)
                    return true;
            }
            return false;
        }

        public void RemovePlayer(int id)
        {
            GameObject[] players = GetOrderedPlayers();
            if (lobbyType == "Load Game")
            {
                for(int i = 0; i < players.Length; i++)
                {
                    if(players[i].GetComponent<Player>().UUID.Value == save.playerList[id].UUID)
                    {
                        id = i;
                        break;
                    }
                }
            }
            // If Bot
            if(players[id].GetComponent<Player>().IsOwnedByServer)
            {
                Destroy(players[id]);
            }
            //If player
            else
            {
                players[id].GetComponent<Player>().DisconnectClientRPC();
            }
        }

        public void AddBot()
        {
            if(playersInLobby < 6)
            {
                if(lobbyType == "New Game")
                    Instantiate(botPlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<NetworkObject>().Spawn();
                else if(lobbyType == "Load Game")
                {
                    GameObject[] players = GetOrderedPlayers();
                    bool added = false;
                    foreach(PlayerData p in save.playerList)
                    {
                        if(p.isBot && !added)
                        {
                            bool exists = false;
                            foreach(GameObject g in players)
                            {
                                if (g.GetComponent<Player>().UUID.Value == p.UUID)
                                    exists = true;
                            }
                            if(!exists)
                            {
                                GameObject bot = Instantiate(botPlayerPrefab, Vector3.zero, Quaternion.identity);
                                bot.GetComponent<NetworkObject>().Spawn();
                                bot.GetComponent<Player>().SetValue("Username", p.Username + "");
                                bot.GetComponent<Player>().SetValue("UUID", p.UUID + "");
                                added = true;
                            }
                        }
                    }
                }
            }
        }

        IEnumerator ShowNoSavedDataMessage()
        {
            noSavedDataFoundMessage.SetActive(true);
            yield return new WaitForSeconds(2);
            noSavedDataFoundMessage.SetActive(false);
        }

        public async void HostLobby(string _lobbyType)
        {
            lobbyType = _lobbyType; // "New Game" or "Load Game"

            if(lobbyType == "Load Game")
            {
                DataManager d = new DataManager();
                save = d.GetSaveFile("GameData");

                if (save == default)
                {
                    if (!noSavedDataFoundMessage.activeInHierarchy)
                        StartCoroutine(ShowNoSavedDataMessage());
                    return;
                }
            }

            hostingInProgress.SetActive(true);

            bool joinedRelay = true;
            try
            {
                if (RelayManager.Instance.IsRelayEnabled)
                    await RelayManager.Instance.SetupRelay();
            }
            catch (Exception)
            {
                joinedRelay = false;
                hostingInProgress.SetActive(false);
                inLobby = false;
            }

            if (joinedRelay)
            {
                StartCoroutine(HostingProcedure());
                NetworkManager.Singleton.StartHost();
            }
        }

        IEnumerator HostingProcedure()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(.5f);
                if (NetworkManager.Singleton.IsHost)
                {
                    EnterLobby();
                    break;
                }
            }
            hostingInProgress.SetActive(false);
            if (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                inLobby = false;
            }
        }

        public async void JoinLobby()
        {
            joiningInProgress.SetActive(true);

            bool joinedRelay = true;
            try
            {
                if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCode.text))
                {
                    await RelayManager.Instance.JoinRelay(joinCode.text);
                }  
            }
            catch(Exception)
            {
                joinedRelay = false;
                joiningInProgress.SetActive(false);
                inLobby = false;
            }

            if (joinedRelay)
            {
                StartCoroutine(JoiningProcedure());
                NetworkManager.Singleton.StartClient();
            }  
        }

        IEnumerator JoiningProcedure()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(.5f);
                if (NetworkManager.Singleton.IsConnectedClient)
                {
                    EnterLobby();
                    break;
                }
            }
            joiningInProgress.SetActive(false);
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.Shutdown();
                inLobby = false;
            }
        }

        public void EnterLobby()
        {
            lobbyCode.text = RelayManager.Instance.JoinCode;
            MenuManager.Instance.SwapScene(5);
            inLobby = true;
        }

        public void LeaveLobby()
        {
            if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsConnectedClient)
                NetworkManager.Singleton.Shutdown();
            inLobby = false;
            MenuManager.Instance.SwapScene(1);
        }

        public GameObject[] GetOrderedPlayers()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Array.Sort(players, (a,b) => (int)a.GetComponent<Player>().NetworkObjectId - (int)b.GetComponent<Player>().NetworkObjectId);
            return players;
        }

        public void StartGame()
        {
            GameObject p = GetOrderedPlayers()[0];
            PlayerPrefs.SetString("gameType", lobbyType);
            if (lobbyType == "New Game")
                p.GetComponent<Player>().ChangeScene("Prophecy");
            else if (lobbyType == "Load Game")
                p.GetComponent<Player>().ChangeScene("Core Game");
            else
                Debug.LogError("Unknown LobbyType: \"" + lobbyType + "\"");
        }

        public void EnableIntroCutscene(bool active)
        {
            PlayerPrefs.SetInt("IntroEnabled", active ? 1 : 0);
        }

        //public string GetLocalIPv4()
        //{
        //    foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork) {
        //            return ip.ToString();
        //        }
        //    }
        //    return "No IP Found.";
        //}

        public void TestingFunction_LoadFailMenu()
        {
            GetOrderedPlayers()[0].GetComponent<Player>().ChangeScene("JLFailureMenu");
        }

        public void TestingFunction_LoadSuccessMenu()
        {
            GetOrderedPlayers()[0].GetComponent<Player>().ChangeScene("JLSuccessMenu");
        }
    }
}
