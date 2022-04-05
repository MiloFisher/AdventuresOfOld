using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AdventuresOfOld
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> UUID = new NetworkVariable<FixedString64Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                //Move();
                AssignUsernameAndUUID();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
        }

        public void AssignUsernameAndUUID()
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

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            transform.position = Position.Value;
        }
    }
}
