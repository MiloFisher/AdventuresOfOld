using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace AdventuresOfOld {
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
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            // If Bot
            if(players[id].GetComponent<Player>().IsOwnedByServer)
            {
                Destroy(players[id]);
            }
            //If player
            else
            {
                players[id].GetComponent<Player>().Disconnect();
            }
        }

        public void AddBot()
        {
            if(playersInLobby < 6)
                Instantiate(botPlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<NetworkObject>().Spawn();
        }

        public async void HostLobby()
        {
            StartCoroutine(HostingProcedure());

            if (RelayManager.Instance.IsRelayEnabled)
                await RelayManager.Instance.SetupRelay();

            NetworkManager.Singleton.StartHost();
        }

        IEnumerator HostingProcedure()
        {
            hostingInProgress.SetActive(true);
            for (int i = 0; i < 20; i++)
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
            StartCoroutine(JoiningProcedure());

            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCode.text))
                await RelayManager.Instance.JoinRelay(joinCode.text);

            NetworkManager.Singleton.StartClient();
        }

        IEnumerator JoiningProcedure()
        {
            joiningInProgress.SetActive(true);
            for (int i = 0; i < 20; i++)
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
            MenuManager.Instance.SwapScene(0);
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
