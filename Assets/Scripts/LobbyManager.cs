using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

namespace AdventuresOfOld {
    public class LobbyManager : MonoBehaviour
    {
        public MenuManager menuManager;
        public UNetTransport transport;
        public TMP_InputField joinAddress;
        public TMP_Text hostAddress;

        public GameObject hostingInProgress;
        public GameObject joiningInProgress;

        public TMP_Text[] playerList;

        private bool inLobby;

        // Start is called before the first frame update
        void Start()
        {
            hostingInProgress.SetActive(false);
            joiningInProgress.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(inLobby && !NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost)
            {
                LeaveLobby();
            }
            if (inLobby)
            {
                int x = 0;
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if(x == 0)
                    {
                        playerList[x].text = "[Host] ";
                    }
                    else
                    {
                        playerList[x].text = "[Client] ";
                    }
                    playerList[x++].text += player.GetComponent<Player>().Username.Value + "";
                }
                for (; x < 6; x++)
                {
                    playerList[x].text = "";
                }
            }
        }

        public async void HostLobby()
        {
            //transport.ConnectAddress = GetLocalIPv4();
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
            //transport.ConnectAddress = joinAddress.text;
            StartCoroutine(JoiningProcedure());
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinAddress.text))
                await RelayManager.Instance.JoinRelay(joinAddress.text);
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
            //hostAddress.text = transport.ConnectAddress;
            hostAddress.text = RelayManager.Instance.JoinCode;
            menuManager.SwapScene(5);
            inLobby = true;
        }

        public void LeaveLobby()
        {
            NetworkManager.Singleton.Shutdown();
            inLobby = false;
            menuManager.SwapScene(0);
        }

        public string GetLocalIPv4()
        {
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return "No IP Found.";
        }
    }
}
