using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AdventuresOfOld
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                //Move();
                AssignUsername();
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

        public void AssignUsername()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Username.Value = GameObject.FindGameObjectWithTag("Profile Manager").GetComponent<ProfileManager>().username;
            }
            else
            {
                AssignUsernameServerRPC(GameObject.FindGameObjectWithTag("Profile Manager").GetComponent<ProfileManager>().username);
            }
        }

        [ServerRpc]
        void AssignUsernameServerRPC(FixedString64Bytes username, ServerRpcParams rpcParams = default)
        {
            Username.Value = username;
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
