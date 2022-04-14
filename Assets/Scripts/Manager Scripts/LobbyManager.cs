using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

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

        private bool inLobby;

        public int playersInLobby;
        public string lobbyType;

        void Start()
        {
            hostingInProgress.SetActive(false);
            joiningInProgress.SetActive(false);
        }

        void Update()
        {
            if(inLobby && !NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost)
            {
                LeaveLobby();
            }
            if (inLobby)
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
                        else if(player.GetComponent<Player>().isBot)
                        {
                            playerList[x].text = "[Bot] ";
                        }
                        else
                        {
                            playerList[x].text = "[Client] ";
                        }
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
                    if(x != 0)
                        removeButtonList[x-1].SetActive(false);
                }

                if (playersInLobby > 6)
                {
                    for(int i = 6; i < playersInLobby; i++)
                    {
                        RemovePlayer(i);
                    }
                }

                startGameButton.SetActive(playersInLobby >= 3 && NetworkManager.Singleton.IsHost);
                addBotButton.SetActive(playersInLobby < 6 && NetworkManager.Singleton.IsHost);
            }
        }

        public void RemovePlayer(int id)
        {
            GameObject[] players = GetOrderedPlayers();
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
                Instantiate(botPlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<NetworkObject>().Spawn();
        }

        public async void HostLobby(string _lobbyType)
        {
            lobbyType = _lobbyType; // "New Game" or "Load Game"

            hostingInProgress.SetActive(true);

            if (RelayManager.Instance.IsRelayEnabled)
                await RelayManager.Instance.SetupRelay();

            StartCoroutine(HostingProcedure());
        }

        IEnumerator HostingProcedure()
        {
            for (int i = 0; i < 10; i++)
            {
                NetworkManager.Singleton.StartHost();
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
            catch(Exception e)
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
                p.GetComponent<Player>().ChangeScene("Core Game"); //p.GetComponent<Player>().ChangeScene("Character Creation");
            else if (lobbyType == "Load Game")
                p.GetComponent<Player>().ChangeScene("Core Game");
            else
                Debug.LogError("Unknown LobbyType: \"" + lobbyType + "\"");
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
    }
}
