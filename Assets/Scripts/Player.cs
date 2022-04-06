using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AdventuresOfOld
{
    public class Player : NetworkBehaviour
    {
        public bool isBot;
        public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> UUID = new NetworkVariable<FixedString64Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                if (!isBot)
                    AssignUsernameAndUUID();
                else
                    GenerateUsernameAndUUID();
            }
        }

        void AssignUsernameAndUUID()
        {
            ProfileManager p = GameObject.FindGameObjectWithTag("Profile Manager").GetComponent<ProfileManager>();
            if (NetworkManager.Singleton.IsServer)
            {
                Username.Value = p.username;
                UUID.Value = p.uuid;
            }
            else
            {
                AssignUsernameAndUUIDServerRPC(p.username, p.uuid);
            }
        }

        [ServerRpc]
        void AssignUsernameAndUUIDServerRPC(FixedString64Bytes username, FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            Username.Value = username;
            UUID.Value = uuid;
        }

        void GenerateUsernameAndUUID()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Username.Value = ProfileManager.Instance.GenerateBotUsername();
                UUID.Value = ProfileManager.Instance.GenerateUUID();
            }
        }

        public void Disconnect()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                DisconnectClientRPC();
            }
        }

        [ClientRpc]
        private void DisconnectClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                LobbyManager.Instance.LeaveLobby();
            }
        }
    }
}
