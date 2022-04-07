using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdventuresOfOld
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
        public NetworkVariable<Vector3Int> Position = new NetworkVariable<Vector3Int>();


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
            if (IsOwner)
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

        public void SetValue(string valueName, string value)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                switch (valueName)
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
                    default: Debug.LogError("Unknown Value: \"" + valueName + "\""); break;
                }
            }
            else
            {
                SetStringValueServerRPC(valueName, value);
            }
        }

        [ServerRpc]
        void SetStringValueServerRPC(FixedString64Bytes valueName, FixedString64Bytes value, ServerRpcParams rpcParams = default)
        {
            switch (valueName+"")
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
                default: Debug.LogError("Unknown Value: \"" + valueName + "\""); break;
            }
        }

        public void SetValue(string valueName, int value)
        {
            if (NetworkManager.Singleton.IsServer)
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
            else
            {
                SetIntValueServerRPC(valueName, value);
            }
        }

        [ServerRpc]
        void SetIntValueServerRPC(FixedString64Bytes valueName, int value, ServerRpcParams rpcParams = default)
        {
            switch (valueName + "")
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
    }
}
