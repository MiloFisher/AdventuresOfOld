using UnityEngine.SceneManagement;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AdventuresOfOldMultiplayer
{
    public class Player : NetworkBehaviour
    {
        // User Data
        public bool isBot;
        public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> UUID = new NetworkVariable<FixedString64Bytes>();

        // Character Data (if you update or add to these variables, make sure to update the 4x SetValue functions)
        public NetworkVariable<FixedString64Bytes> Image = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Name = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Race = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Class = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Trait = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<int> Gold = new NetworkVariable<int>();
        public NetworkVariable<int> Level = new NetworkVariable<int>();
        public NetworkVariable<int> XP = new NetworkVariable<int>();
        public NetworkVariable<int> Strength = new NetworkVariable<int>();
        public NetworkVariable<int> Dexterity = new NetworkVariable<int>();
        public NetworkVariable<int> Intelligence = new NetworkVariable<int>();
        public NetworkVariable<int> Speed = new NetworkVariable<int>();
        public NetworkVariable<int> Constitution = new NetworkVariable<int>();
        public NetworkVariable<int> Energy = new NetworkVariable<int>();
        public NetworkVariable<int> Health = new NetworkVariable<int>();
        public NetworkVariable<int> AbilityCharges = new NetworkVariable<int>();
        // Max Health = 2x CON
        // Max Ability Charges = ENG Mod + Level
        // Physical Power = Level or 0 + item bonus
        // Magical Power = Level or 0 + item bonus
        // Armor = Value from Armor Item + item bonus
        // Damage = Value from Weapon Item
        public NetworkVariable<FixedString64Bytes> Armor = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Weapon = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Ring1 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Ring2 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Inventory1 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Inventory2 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Inventory3 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Inventory4 = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<FixedString64Bytes> Inventory5 = new NetworkVariable<FixedString64Bytes>();

        // Additional Character Data
        public NetworkVariable<int> LevelUpPoints = new NetworkVariable<int>();

        // Gameplay Data
        public NetworkVariable<Vector3Int> Position = new NetworkVariable<Vector3Int>();
        public NetworkVariable<int> TurnPhase = new NetworkVariable<int>();
        public NetworkVariable<FixedString64Bytes> Color = new NetworkVariable<FixedString64Bytes>();
        public NetworkVariable<bool> Ready = new NetworkVariable<bool>();

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

        #region Main Menu
        void AssignUsernameAndUUID()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Username.Value = ProfileManager.Instance.username;
                UUID.Value = ProfileManager.Instance.uuid;
            }
            else
            {
                AssignUsernameAndUUIDServerRPC(ProfileManager.Instance.username, ProfileManager.Instance.uuid);
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

        [ClientRpc]
        public void DisconnectClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                LobbyManager.Instance.LeaveLobby();
            }
        }

        public void ChangeScene(FixedString64Bytes scene)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.SceneManager.LoadScene(scene + "", LoadSceneMode.Single);
            }
        }
        #endregion

        #region Character
        public void SetValue(string valueName, string value)
        {
            if (NetworkManager.Singleton.IsServer)
                SetStringValue(valueName, value);
            else
                SetStringValueServerRPC(valueName, value);
        }

        [ServerRpc]
        private void SetStringValueServerRPC(FixedString64Bytes valueName, FixedString64Bytes value, ServerRpcParams rpcParams = default)
        {
            SetStringValue(valueName + "", value + "");
        }

        private void SetStringValue(string valueName, string value)
        {
            switch (valueName + "")
            {
                case "Image": Image.Value = value; break;
                case "Name": Name.Value = value; break;
                case "Race": Race.Value = value; break;
                case "Class": Class.Value = value; break;
                case "Trait": Trait.Value = value; break;
                case "Armor": Armor.Value = value; break;
                case "Weapon": Weapon.Value = value; break;
                case "Ring1": Ring1.Value = value; break;
                case "Ring2": Ring2.Value = value; break;
                case "Inventory1": Inventory1.Value = value; break;
                case "Inventory2": Inventory2.Value = value; break;
                case "Inventory3": Inventory3.Value = value; break;
                case "Inventory4": Inventory4.Value = value; break;
                case "Inventory5": Inventory5.Value = value; break;
                case "Color": Color.Value = value; break;
                default: Debug.LogError("Unknown Value: \"" + valueName + "\""); break;
            }
        }

        public void SetValue(string valueName, int value)
        {
            if (NetworkManager.Singleton.IsServer)
                SetIntValue(valueName, value);
            else
                SetIntValueServerRPC(valueName, value);
        }

        [ServerRpc]
        private void SetIntValueServerRPC(FixedString64Bytes valueName, int value, ServerRpcParams rpcParams = default)
        {
            SetIntValue(valueName + "", value);
        }

        private void SetIntValue(string valueName, int value)
        {
            switch (valueName)
            {
                case "Gold": Gold.Value = value; break;
                case "Level": Level.Value = value; break;
                case "XP": XP.Value = value; break;
                case "Strength": Strength.Value = value; break;
                case "Dexterity": Dexterity.Value = value; break;
                case "Intelligence": Intelligence.Value = value; break;
                case "Speed": Speed.Value = value; break;
                case "Constitution": Constitution.Value = value; break;
                case "Energy": Energy.Value = value; break;
                case "Health": Health.Value = value; break;
                case "AbilityCharges": AbilityCharges.Value = value; break;
                case "LevelUpPoints": LevelUpPoints.Value = value; break;
                default: Debug.LogError("Unknown Value: \"" + valueName + "\""); break;
            }
        }
        #endregion

        #region Gameplay
        public void SetPosition(Vector3Int pos)
        {
            if (NetworkManager.Singleton.IsServer)
                Position.Value = pos;
            else
                SetPositionServerRPC(pos);
        }
        [ServerRpc]
        private void SetPositionServerRPC(Vector3Int pos, ServerRpcParams rpcParams = default)
        {
            Position.Value = pos;
        }

        public void SetTurnPhase(int phase)
        {
            if (NetworkManager.Singleton.IsServer)
                TurnPhase.Value = phase;
            else
                SetTurnPhaseServerRPC(phase);
        }
        [ServerRpc]
        private void SetTurnPhaseServerRPC(int phase, ServerRpcParams rpcParams = default)
        {
            TurnPhase.Value = phase;
        }

        public void ReadyUp()
        {
            if (NetworkManager.Singleton.IsServer)
                Ready.Value = true;
            else
                ReadyUpServerRPC();
        }
        [ServerRpc]
        private void ReadyUpServerRPC(ServerRpcParams rpcParams = default)
        {
            Ready.Value = true;
        }

        [ClientRpc]
        public void PlayTransitionClientRPC(int id, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.CallTransition(id);
            }
        }

        [ClientRpc]
        public void SetChaosCounterClientRPC(int chaosCounter, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.chaosCounter = chaosCounter;
            }
        }

        [ClientRpc]
        public void EnableTreasureTokenClientRPC(Vector3Int pos, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.gameboard[pos].EnableTreasureToken();
            }
        }

        [ClientRpc]
        public void SetTurnOrderPlayerListClientRPC(FixedString64Bytes[] arr, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.SetTurnOrderPlayerList(arr);
            }
        }
        #endregion
    }
}
