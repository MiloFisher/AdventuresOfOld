using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

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
        public NetworkVariable<int> FailedEncounters = new NetworkVariable<int>();

        // Gameplay Data
        public NetworkVariable<Vector3Int> Position = new NetworkVariable<Vector3Int>();
        public NetworkVariable<int> TurnPhase = new NetworkVariable<int>();
        public NetworkVariable<int> Color = new NetworkVariable<int>();
        public NetworkVariable<bool> Ready = new NetworkVariable<bool>();
        public NetworkVariable<int> EndOfDayActivity = new NetworkVariable<int>();
        public NetworkVariable<int> ParticipatingInCombat = new NetworkVariable<int>();

        // Quest data
        public NetworkVariable<bool> HasBathWater = new NetworkVariable<bool>();
        public NetworkVariable<bool> BetrayedBoy = new NetworkVariable<bool>();
        public NetworkVariable<bool> GrabbedHorse = new NetworkVariable<bool>();
        public NetworkVariable<bool> KilledGoblin = new NetworkVariable<bool>();

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
                case "FailedEncounters": FailedEncounters.Value = value; break;
                case "EndOfDayActivity": EndOfDayActivity.Value = value; break;
                case "ParticipatingInCombat": ParticipatingInCombat.Value = value; break;
                case "Color": Color.Value = value; break;
                default: Debug.LogError("Unknown Value: \"" + valueName + "\""); break;
            }
        }

        public void SetValue(string valueName, bool value)
        {
            if (NetworkManager.Singleton.IsServer)
                SetBoolValue(valueName, value);
            else
                SetBoolValueServerRPC(valueName, value);
        }

        [ServerRpc]
        private void SetBoolValueServerRPC(FixedString64Bytes valueName, bool value, ServerRpcParams rpcParams = default)
        {
            SetBoolValue(valueName + "", value);
        }

        private void SetBoolValue(string valueName, bool value)
        {
            switch (valueName)
            {
                case "HasBathWater": HasBathWater.Value = value; break;
                case "BetrayedBoy": BetrayedBoy.Value = value; break;
                case "GrabbedHorse": GrabbedHorse.Value = value; break;
                case "KilledGoblin": KilledGoblin.Value = value; break;
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

        public void Unready()
        {
            if (NetworkManager.Singleton.IsServer)
                Ready.Value = false;
            else
                UnreadyServerRPC();
        }
        [ServerRpc]
        private void UnreadyServerRPC(ServerRpcParams rpcParams = default)
        {
            Ready.Value = false;
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
                PlayManager.Instance.UpdateChaosMarker();
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

        public void UpdateTurnMarker(int marker)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.SetTurnMarkerClientRPC(marker);
            }
            else
                UpdateTurnMarkerServerRPC(marker);
        }
        [ServerRpc]
        private void UpdateTurnMarkerServerRPC(int marker, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.SetTurnMarkerClientRPC(marker);
        }

        [ClientRpc]
        public void SetTurnMarkerClientRPC(int marker, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.turnMarker = marker;
            }
        }

        public void StartNextPlayerTurn(int turnMarker)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                PlayManager.Instance.turnOrderPlayerList[turnMarker].StartTurnClientRPC();
            }
            else
                StartNextPlayerTurnServerRPC(turnMarker);
        }
        [ServerRpc]
        private void StartNextPlayerTurnServerRPC(int turnMarker, ServerRpcParams rpcParams = default)
        {
            PlayManager.Instance.turnOrderPlayerList[turnMarker].StartTurnClientRPC();
        }

        [ClientRpc]
        public void StartTurnClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if(isBot)
                    PlayManager.Instance.StartBotTurn();
                else
                    PlayManager.Instance.StartTurn();
            }
        }

        [ClientRpc]
        public void SetupPlayerPiecesClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.SetupPlayerPieces();
            }
        }

        [ClientRpc]
        public void SetupCharacterPanelsClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.SetupCharacterPanels();
            }
        }

        public void EndDayForPlayers()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.isBot)
                        p.BotEndOfDayClientRPC(); // Call end of day for bots
                    else
                        p.PlayTransitionClientRPC(3); // Transition 3 is End of Day
                }
            }
            else
                EndDayForPlayersServerRPC();
        }
        [ServerRpc]
        private void EndDayForPlayersServerRPC(ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.isBot)
                    p.BotEndOfDayClientRPC(); // Call end of day for bots
                else
                    p.PlayTransitionClientRPC(3); // Transition 3 is End of Day
            }
        }

        [ClientRpc]
        public void BotEndOfDayClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if (isBot)
                {
                    // Put actual logic here later
                    EndOfDayActivity.Value = 0;
                    ReadyUp();
                }
            }
        }

        [ClientRpc]
        public void CloseLoadingScreenClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.CloseLoadingScreen();
            }
        }

        public void DrawLootCards(int amount, FixedString64Bytes uuid, bool endTurnAfter)
        {
            if (Health.Value <= 0)
                return;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach(Player p in PlayManager.Instance.playerList)
                {
                    if(p.UUID.Value == uuid)
                    {
                        for (int i = 0; i < amount; i++)
                            p.AddLootCardsToDrawClientRPC(PlayManager.Instance.DrawFromLootDeck());
                        p.DrawLootCardsClientRPC(amount, endTurnAfter);
                    }
                }
            }
            else
                DrawLootCardsServerRPC(amount, uuid, endTurnAfter);
        }
        [ServerRpc]
        private void DrawLootCardsServerRPC(int amount, FixedString64Bytes uuid, bool endTurnAfter, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value == uuid)
                {
                    for (int i = 0; i < amount; i++)
                        p.AddLootCardsToDrawClientRPC(PlayManager.Instance.DrawFromLootDeck());
                    p.DrawLootCardsClientRPC(amount, endTurnAfter);
                }
            }
        }
        [ClientRpc]
        private void AddLootCardsToDrawClientRPC(FixedString64Bytes cardName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                LootManager.Instance.AddLootCardToDraw(cardName+"");
            }
        }
        [ClientRpc]
        private void DrawLootCardsClientRPC(int amount, bool endTurnAfter, bool forStore = default, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if(isBot)
                {
                    if (forStore)
                        ReadyUp();
                }
                else
                    LootManager.Instance.DrawCard(amount, endTurnAfter, forStore);
            }
        }

        public void DrawEncounterCards(int amount, FixedString64Bytes uuid, bool animateOpening)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                List<string> cards = new List<string>();
                for(int i = 0; i < amount; i++)
                    cards.Add(PlayManager.Instance.DrawFromEncounterDeck());
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    for (int i = 0; i < amount; i++)
                        p.AddEncounterCardsToDrawClientRPC(cards[i]);
                    p.DrawEncounterCardsClientRPC(amount, animateOpening, p.UUID.Value == uuid, uuid);
                }
            }
            else
                DrawEncounterCardsServerRPC(amount, uuid, animateOpening);
        }
        [ServerRpc]
        private void DrawEncounterCardsServerRPC(int amount, FixedString64Bytes uuid, bool animateOpening, ServerRpcParams rpcParams = default)
        {
            List<string> cards = new List<string>();
            for (int i = 0; i < amount; i++)
                cards.Add(PlayManager.Instance.DrawFromEncounterDeck());
            foreach (Player p in PlayManager.Instance.playerList)
            {
                for (int i = 0; i < amount; i++)
                    p.AddEncounterCardsToDrawClientRPC(cards[i]);
                p.DrawEncounterCardsClientRPC(amount, animateOpening, p.UUID.Value == uuid, uuid);
            }
        }
        [ClientRpc]
        private void AddEncounterCardsToDrawClientRPC(FixedString64Bytes cardName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                EncounterManager.Instance.AddEncounterCardToDraw(cardName + "");
            }
        }
        [ClientRpc]
        private void DrawEncounterCardsClientRPC(int amount, bool animateOpening, bool isYourTurn, FixedString64Bytes uuid, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                EncounterManager.Instance.DrawCard(amount, animateOpening, isYourTurn, uuid + "");
            }
        }

        public void CompleteEncounter(bool endTurnAfter, FixedString64Bytes uuid)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        p.CompleteEncounterClientRPC(endTurnAfter);
                    else
                        p.CompleteEncounterClientRPC(false);
                }
            }
            else
                CompleteEncounterServerRPC(endTurnAfter, uuid);
        }
        [ServerRpc]
        private void CompleteEncounterServerRPC(bool endTurnAfter, FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value == uuid)
                    p.CompleteEncounterClientRPC(endTurnAfter);
                else
                    p.CompleteEncounterClientRPC(false);
            }
        }
        [ClientRpc]
        private void CompleteEncounterClientRPC(bool endTurnAfter, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                EncounterManager.Instance.CompleteEncounter(endTurnAfter);
            }
        }

        public void ForkInTheRoadHelper(FixedString64Bytes uuid)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        p.ForkInTheRoadHelperClientRPC(true);
                    else
                        p.ForkInTheRoadHelperClientRPC(false);
                }
            }
            else
                ForkInTheRoadHelperServerRPC(uuid);
        }
        [ServerRpc]
        private void ForkInTheRoadHelperServerRPC(FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value == uuid)
                    p.ForkInTheRoadHelperClientRPC(true);
                else
                    p.ForkInTheRoadHelperClientRPC(false);
            }
        }
        [ClientRpc]
        private void ForkInTheRoadHelperClientRPC(bool drawMoreCards, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                EncounterManager.Instance.ForkInTheRoadHelper(drawMoreCards);
            }
        }

        public void GainXP(int amount, bool halved = false)
        {
            if (Level.Value == 5 || Health.Value <= 0)
                return;
            int total = PlayManager.Instance.XPModifier() + amount;
            if (halved)
                total = (int)MathF.Ceiling(total / 2f);
            if (NetworkManager.Singleton.IsServer)
            {
                XP.Value += total;
                LevelUpCheckClientRPC();
            }
            else
                GainXPServerRPC(total);
        }
        [ServerRpc]
        private void GainXPServerRPC(int total, ServerRpcParams rpcParams = default)
        {
            XP.Value += total;
            LevelUpCheckClientRPC();
        }
        [ClientRpc]
        private void LevelUpCheckClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if (XP.Value > PlayManager.Instance.GetNeededXP(this))
                {
                    LevelUp(PlayManager.Instance.GetNeededXP(this));
                    PlayManager.Instance.LevelUpNotification();
                }
            }
        }

        public void LevelUp(int neededXP)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                XP.Value -= neededXP;
                Level.Value++;
                AbilityCharges.Value++;
                LevelUpPoints.Value += 3;
                if (Level.Value == 5)
                    XP.Value = neededXP;
            }
            else
                LevelUpServerRPC(neededXP);
        }
        [ServerRpc]
        private void LevelUpServerRPC(int neededXP, ServerRpcParams rpcParams = default)
        {
            XP.Value -= neededXP;
            Level.Value++;
            AbilityCharges.Value++;
            LevelUpPoints.Value += 3;
            if (Level.Value == 5)
                XP.Value = neededXP;
        }

        public void GainGold(int amount)
        {
            if (NetworkManager.Singleton.IsServer)
                Gold.Value += amount;
            else
                GainGoldServerRPC(amount);
        }
        [ServerRpc]
        private void GainGoldServerRPC(int amount, ServerRpcParams rpcParams = default)
        {
            Gold.Value += amount;
        }

        public void LoseGold(int amount)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Gold.Value -= amount;
                if (Gold.Value < 0)
                    Gold.Value = 0;
            }
            else
                LoseGoldServerRPC(amount);
        }
        [ServerRpc]
        private void LoseGoldServerRPC(int amount, ServerRpcParams rpcParams = default)
        {
            Gold.Value -= amount;
            if (Gold.Value < 0)
                Gold.Value = 0;
        }

        public void TakeDamage(int amount, int armor, bool isTrue = false)
        {
            int damage = isTrue ? amount : amount - armor;
            if (NetworkManager.Singleton.IsServer)
            {
                if (damage > 0)
                    Health.Value -= damage;
                if (Health.Value <= 0)
                {
                    Health.Value = 0;
                    XP.Value = 0;
                }
            }
            else
                TakeDamageServerRPC(damage);
        }
        [ServerRpc]
        private void TakeDamageServerRPC(int damage, ServerRpcParams rpcParams = default)
        {
            if (damage > 0)
                Health.Value -= damage;
            if (Health.Value <= 0)
            {
                Health.Value = 0;
                XP.Value = 0;
            }
        }

        public void RestoreAbilityCharges(int amount)
        {
            if (Health.Value <= 0)
                return;
            int cap = PlayManager.Instance.GetMaxAbilityCharges(this);
            if (NetworkManager.Singleton.IsServer)
            {
                AbilityCharges.Value += amount;
                if (AbilityCharges.Value > cap)
                    AbilityCharges.Value = cap;
            }
            else
                RestoreAbilityChargesServerRPC(amount, cap);
        }
        [ServerRpc]
        private void RestoreAbilityChargesServerRPC(int amount, int cap, ServerRpcParams rpcParams = default)
        {
            AbilityCharges.Value += amount;
            if (AbilityCharges.Value > cap)
                AbilityCharges.Value = cap;
        }

        public void RestoreHealth(int amount)
        {
            if (Health.Value <= 0)
                return;
            int cap = PlayManager.Instance.GetMaxHealth(this);

            if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Heaven's Paragon"), this))
                amount += 2;

            if (NetworkManager.Singleton.IsServer)
            {
                Health.Value += amount;
                if (Health.Value > cap)
                    Health.Value = cap;
            }
            else
                RestoreRestoreHealthServerRPC(amount, cap);
        }
        [ServerRpc]
        private void RestoreRestoreHealthServerRPC(int amount, int cap, ServerRpcParams rpcParams = default)
        {
            Health.Value += amount;
            if (Health.Value > cap)
                Health.Value = cap;
        }

        public void LoseAbilityCharges(int amount)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                AbilityCharges.Value -= amount;
                if (AbilityCharges.Value < 0)
                    AbilityCharges.Value = 0;
            }
            else
                LoseAbilityChargesServerRPC(amount);
        }
        [ServerRpc]
        private void LoseAbilityChargesServerRPC(int amount, ServerRpcParams rpcParams = default)
        {
            AbilityCharges.Value -= amount;
            if (AbilityCharges.Value < 0)
                AbilityCharges.Value = 0;
        }

        public void IncreaseChaos(int amount)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.IncreaseChaosClientRPC(amount);
            }
            else
                IncreaseChaosServerRPC(amount);
        }
        [ServerRpc]
        private void IncreaseChaosServerRPC(int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.IncreaseChaosClientRPC(amount);
        }
        [ClientRpc]
        private void IncreaseChaosClientRPC(int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.IncreaseChaos(amount);
            }
        }

        public void ReduceChaos(int amount)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.ReduceChaosClientRPC(amount);
            }
            else
                ReduceChaosServerRPC(amount);
        }
        [ServerRpc]
        private void ReduceChaosServerRPC(int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.ReduceChaosClientRPC(amount);
        }
        [ClientRpc]
        private void ReduceChaosClientRPC(int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.ReduceChaos(amount);
            }
        }

        public void UpdateQuests(List<QuestCard> q)
        {
            FixedString64Bytes[] quests = new FixedString64Bytes[q.Count];
            int[] steps = new int[q.Count];
            for(int i = 0; i < q.Count; i++)
            {
                quests[i] = q[i].cardName;
                steps[i] = q[i].questStep;
            }
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.UpdateQuestsClientRPC(quests, steps);
            }
            else
                UpdateQuestsServerRPC(quests, steps);
        }
        [ServerRpc]
        private void UpdateQuestsServerRPC(FixedString64Bytes[] quests, int[] steps, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.UpdateQuestsClientRPC(quests, steps);
        }
        [ClientRpc]
        private void UpdateQuestsClientRPC(FixedString64Bytes[] quests, int[] steps, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.UpdateQuests(quests, steps);
            }
        }

        public void UpdateStats(int temporaryStrength, int temporaryDexterity, int temporaryIntelligence, int temporarySpeed, int temporaryConstitution, int temporaryEnergy)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Strength.Value += temporaryStrength;
                Dexterity.Value += temporaryDexterity;
                Intelligence.Value += temporaryIntelligence;
                Speed.Value += temporarySpeed;
                Constitution.Value += temporaryConstitution;
                Health.Value += temporaryConstitution * 2;
                if(Energy.Value % 2 == 0)
                    AbilityCharges.Value += Mathf.FloorToInt(temporaryEnergy / 2f);
                else
                    AbilityCharges.Value += Mathf.CeilToInt(temporaryEnergy / 2f);
                Energy.Value += temporaryEnergy;
                

                LevelUpPoints.Value -= temporaryStrength + temporaryDexterity + temporaryIntelligence + temporarySpeed + temporaryConstitution + temporaryEnergy;
            }
            else
                UpdateStatsServerRPC(temporaryStrength, temporaryDexterity, temporaryIntelligence, temporarySpeed, temporaryConstitution, temporaryEnergy);
        }
        [ServerRpc]
        public void UpdateStatsServerRPC(int temporaryStrength, int temporaryDexterity, int temporaryIntelligence, int temporarySpeed, int temporaryConstitution, int temporaryEnergy, ServerRpcParams rpcParams = default)
        {
            Strength.Value += temporaryStrength;
            Dexterity.Value += temporaryDexterity;
            Intelligence.Value += temporaryIntelligence;
            Speed.Value += temporarySpeed;
            Constitution.Value += temporaryConstitution;
            Health.Value += temporaryConstitution * 2;
            if (Energy.Value % 2 == 0)
                AbilityCharges.Value += Mathf.FloorToInt(temporaryEnergy / 2f);
            else
                AbilityCharges.Value += Mathf.CeilToInt(temporaryEnergy / 2f);
            Energy.Value += temporaryEnergy;

            LevelUpPoints.Value -= temporaryStrength + temporaryDexterity + temporaryIntelligence + temporarySpeed + temporaryConstitution + temporaryEnergy;
        }

        public void SendCombatNotifications()
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if(p.UUID.Value != uuid)
                        p.SendCombatNotificationsClientRPC();
                }
            }
            else
                SendCombatNotificationsServerRPC(uuid);
        }
        [ServerRpc]
        private void SendCombatNotificationsServerRPC(FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.SendCombatNotificationsClientRPC();
            }
        }
        [ClientRpc]
        private void SendCombatNotificationsClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if(!isBot)
                    PlayManager.Instance.CombatNotification();
                else
                {
                    SetValue("ParticipatingInCombat", 0);
                    ReadyUp();
                }
            }
        }

        public void ContinueToCombat()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.ContinueToCombatClientRPC();
            }
            else
                ContinueToCombatServerRPC();
        }
        [ServerRpc]
        private void ContinueToCombatServerRPC(ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.ContinueToCombatClientRPC();
        }
        [ClientRpc]
        private void ContinueToCombatClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.ContinueToCombat();
            }
        }

        public void UpdateMonsterHealth(int value)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.UpdateMonsterHealthClientRPC(value);
            }
            else
                UpdateMonsterHealthServerRPC(value);
        }
        [ServerRpc]
        private void UpdateMonsterHealthServerRPC(int value, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.UpdateMonsterHealthClientRPC(value);
        }
        [ClientRpc]
        private void UpdateMonsterHealthClientRPC(int value, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.monster.SetCurrentHealth(value);
            }
        }

        public void SetTurnOrderCombatantList(FixedString64Bytes[] arr, bool keepMonster)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.SetTurnOrderCombatantListClientRPC(arr, keepMonster);
            }
            else
                SetTurnOrderCombatantListServerRPC(arr, keepMonster);
        }
        [ServerRpc]
        private void SetTurnOrderCombatantListServerRPC(FixedString64Bytes[] arr, bool keepMonster, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.SetTurnOrderCombatantListClientRPC(arr, keepMonster);
        }
        [ClientRpc]
        public void SetTurnOrderCombatantListClientRPC(FixedString64Bytes[] arr, bool keepMonster, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.SetTurnOrderCombatantList(arr, keepMonster);
            }
        }

        public void UpdateCombatTurnMarker(int marker)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.UpdateCombatTurnMarkerClientRPC(marker);
            }
            else
                UpdateCombatTurnMarkerServerRPC(marker);
        }
        [ServerRpc]
        private void UpdateCombatTurnMarkerServerRPC(int marker, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.UpdateCombatTurnMarkerClientRPC(marker);
        }
        [ClientRpc]
        public void UpdateCombatTurnMarkerClientRPC(int marker, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.combatTurnMarker = marker;
            }
        }

        public void StartNextCombatantTurn(int turnMarker)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                CombatManager.Instance.turnOrderCombatantList[turnMarker].player.StartNextCombatantTurnClientRPC();
            }
            else
                StartNextCombatantTurnServerRPC(turnMarker);
        }
        [ServerRpc]
        private void StartNextCombatantTurnServerRPC(int turnMarker, ServerRpcParams rpcParams = default)
        {
            CombatManager.Instance.turnOrderCombatantList[turnMarker].player.StartNextCombatantTurnClientRPC();
        }
        [ClientRpc]
        public void StartNextCombatantTurnClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if (isBot)
                    CombatManager.Instance.StartBotTurn();
                else
                    CombatManager.Instance.StartTurn();
            }
        }

        public void StartMonsterTurn(int[] targets)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.StartMonsterTurnClientRPC(targets);
            }
            else
                StartMonsterTurnServerRPC(targets);
        }
        [ServerRpc]
        private void StartMonsterTurnServerRPC(int[] targets, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.StartMonsterTurnClientRPC(targets);
        }
        [ClientRpc]
        public void StartMonsterTurnClientRPC(int[] targets, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.StartMonsterTurn(targets);
            }
        }

        public void VisualizeAttackForOthers()
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeAttackForOthersClientRPC(uuid);
                }
            }
            else
                VisualizeAttackForOthersServerRPC(uuid);
        }
        [ServerRpc]
        private void VisualizeAttackForOthersServerRPC(FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeAttackForOthersClientRPC(uuid);
            }
        }
        [ClientRpc]
        public void VisualizeAttackForOthersClientRPC(FixedString64Bytes uuid, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if(p.UUID.Value == uuid)
                        CombatManager.Instance.VisualizePlayerAttacked(p);
                }
            }
        }

        public void VisualizeTakeDamageForOthers(int amount)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeTakeDamageForOthersClientRPC(uuid, amount);
                }
            }
            else
                VisualizeTakeDamageForOthersServerRPC(uuid, amount);
        }
        [ServerRpc]
        private void VisualizeTakeDamageForOthersServerRPC(FixedString64Bytes uuid, int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeTakeDamageForOthersClientRPC(uuid, amount);
            }
        }
        [ClientRpc]
        public void VisualizeTakeDamageForOthersClientRPC(FixedString64Bytes uuid, int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        CombatManager.Instance.VisualizePlayerTakeDamage(p, amount);
                }
            }
        }

        public void GainStatusEffect(string name, int duration, int potency)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.GainStatusEffectClientRPC(uuid, name, duration, potency);
            }
            else
                GainStatusEffectServerRPC(uuid, name, duration, potency);
        }
        [ServerRpc]
        private void GainStatusEffectServerRPC(FixedString64Bytes uuid, FixedString64Bytes name, int duration, int potency, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.GainStatusEffectClientRPC(uuid, name, duration, potency);
        }
        [ClientRpc]
        public void GainStatusEffectClientRPC(FixedString64Bytes uuid, FixedString64Bytes name, int duration, int potency, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                Effect e = new Effect(name + "", duration, potency);
                foreach(Player p in PlayManager.Instance.playerList)
                {
                    if(p.UUID.Value == uuid)
                        CombatManager.Instance.GainStatusEffect(CombatManager.Instance.GetCombatantFromPlayer(p), e);
                }
                
            }
        }

        public void RemoveStatusEffect(string effectName)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.RemoveStatusEffectClientRPC(uuid, effectName);
            }
            else
                RemoveStatusEffectServerRPC(uuid, effectName);
        }
        [ServerRpc]
        private void RemoveStatusEffectServerRPC(FixedString64Bytes uuid, FixedString64Bytes effectName, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.RemoveStatusEffectClientRPC(uuid, effectName);
        }
        [ClientRpc]
        public void RemoveStatusEffectClientRPC(FixedString64Bytes uuid, FixedString64Bytes effectName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        CombatManager.Instance.RemoveStatusEffect(CombatManager.Instance.GetCombatantFromPlayer(p), effectName + "");
                }

            }
        }

        public void CycleStatusEffects()
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.CycleStatusEffectsClientRPC(uuid);
            }
            else
                CycleStatusEffectsServerRPC(uuid);
        }
        [ServerRpc]
        private void CycleStatusEffectsServerRPC(FixedString64Bytes uuid, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.CycleStatusEffectsClientRPC(uuid);
        }
        [ClientRpc]
        public void CycleStatusEffectsClientRPC(FixedString64Bytes uuid, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        CombatManager.Instance.CycleStatusEffects(CombatManager.Instance.GetCombatantFromPlayer(p));
                }

            }
        }

        public void MonsterGainStatusEffect(string name, int duration, int potency)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.MonsterGainStatusEffectClientRPC(name, duration, potency);
            }
            else
                MonsterGainStatusEffectServerRPC(name, duration, potency);
        }
        [ServerRpc]
        private void MonsterGainStatusEffectServerRPC(FixedString64Bytes name, int duration, int potency, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.MonsterGainStatusEffectClientRPC(name, duration, potency);
        }
        [ClientRpc]
        public void MonsterGainStatusEffectClientRPC(FixedString64Bytes name, int duration, int potency, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.GainStatusEffect(CombatManager.Instance.monster, new Effect(name + "", duration, potency));
            }
        }

        public void MonsterRemoveStatusEffect(string effectName)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.MonsterRemoveStatusEffectClientRPC(effectName);
            }
            else
                MonsterRemoveStatusEffectServerRPC(effectName);
        }
        [ServerRpc]
        private void MonsterRemoveStatusEffectServerRPC(FixedString64Bytes effectName, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.MonsterRemoveStatusEffectClientRPC(effectName);
        }
        [ClientRpc]
        public void MonsterRemoveStatusEffectClientRPC(FixedString64Bytes effectName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.RemoveStatusEffect(CombatManager.Instance.monster, effectName + "");
            }
        }

        public void MonsterCycleStatusEffects()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.MonsterCycleStatusEffectsClientRPC();
            }
            else
                MonsterCycleStatusEffectsServerRPC();
        }
        [ServerRpc]
        private void MonsterCycleStatusEffectsServerRPC(ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.MonsterCycleStatusEffectsClientRPC();
        }
        [ClientRpc]
        public void MonsterCycleStatusEffectsClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.CycleStatusEffects(CombatManager.Instance.monster);
            }
        }

        public void VisualizeMonsterAttackForOthers(int amount)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeMonsterAttackForOthersClientRPC(uuid, amount);
                }
            }
            else
                VisualizeMonsterAttackForOthersServerRPC(uuid, amount);
        }
        [ServerRpc]
        private void VisualizeMonsterAttackForOthersServerRPC(FixedString64Bytes uuid, int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeMonsterAttackForOthersClientRPC(uuid, amount);
            }
        }
        [ClientRpc]
        public void VisualizeMonsterAttackForOthersClientRPC(FixedString64Bytes uuid, int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        CombatManager.Instance.VisualizeMonsterAttacked(p, amount);
                }
            }
        }

        public void VisualizeMonsterTakeDamageForOthers(int amount)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeMonsterTakeDamageForOthersClientRPC(amount);
                }
            }
            else
                VisualizeMonsterTakeDamageForOthersServerRPC(uuid, amount);
        }
        [ServerRpc]
        private void VisualizeMonsterTakeDamageForOthersServerRPC(FixedString64Bytes uuid, int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeMonsterTakeDamageForOthersClientRPC(amount);
            }
        }
        [ClientRpc]
        public void VisualizeMonsterTakeDamageForOthersClientRPC(int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.VisualizeMonsterTakeDamage(amount);
            }
        }

        public void TransitionOthersToStyle(CombatLayoutStyle s, int attackerId = -1)
        {
            FixedString64Bytes uuid = UUID.Value;
            FixedString64Bytes style = "";
            switch (s)
            {
                case CombatLayoutStyle.DEFAULT: style = "DEFAULT"; break;
                case CombatLayoutStyle.ATTACKING: style = "ATTACKING"; break;
            }
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.TransitionOthersToStyleClientRPC(style, attackerId);
                }
            }
            else
                TransitionOthersToStyleServerRPC(uuid, style, attackerId);
        }
        [ServerRpc]
        private void TransitionOthersToStyleServerRPC(FixedString64Bytes uuid, FixedString64Bytes style, int attackerId, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.TransitionOthersToStyleClientRPC(style, attackerId);
            }
        }
        [ClientRpc]
        public void TransitionOthersToStyleClientRPC(FixedString64Bytes style, int attackerId, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatLayoutStyle s = CombatLayoutStyle.DEFAULT;
                switch(style + "")
                {
                    case "DEFAULT": s = CombatLayoutStyle.DEFAULT; break;
                    case "ATTACKING": s = CombatLayoutStyle.ATTACKING; break;
                }
                CombatManager.Instance.TransitionToStyle(s, attackerId);
            }
        }

        public void SendCombatCompleteNotifications(int result)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.SendCombatCompleteNotificationsClientRPC(result);
                }
            }
            else
                SendCombatCompleteNotificationsServerRPC(uuid, result);
        }
        [ServerRpc]
        private void SendCombatCompleteNotificationsServerRPC(FixedString64Bytes uuid, int result, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.SendCombatCompleteNotificationsClientRPC(result);
            }
        }
        [ClientRpc]
        private void SendCombatCompleteNotificationsClientRPC(int result, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                if (!isBot)
                    CombatManager.Instance.CombatCompleteNotification(result);
                else
                {

                }
            }
        }

        public void VisualizeHealForOthers(int amount)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeHealForOthersClientRPC(uuid, amount);
                }
            }
            else
                VisualizeHealForOthersServerRPC(uuid, amount);
        }
        [ServerRpc]
        private void VisualizeHealForOthersServerRPC(FixedString64Bytes uuid, int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeHealForOthersClientRPC(uuid, amount);
            }
        }
        [ClientRpc]
        public void VisualizeHealForOthersClientRPC(FixedString64Bytes uuid, int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == uuid)
                        CombatManager.Instance.VisualizePlayerHeal(p, amount);
                }
            }
        }

        public void VisualizeMonsterHealForOthers(int amount)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.VisualizeMonsterHealForOthersClientRPC(amount);
                }
            }
            else
                VisualizeMonsterHealForOthersServerRPC(uuid, amount);
        }
        [ServerRpc]
        private void VisualizeMonsterHealForOthersServerRPC(FixedString64Bytes uuid, int amount, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.VisualizeMonsterHealForOthersClientRPC(amount);
            }
        }
        [ClientRpc]
        public void VisualizeMonsterHealForOthersClientRPC(int amount, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                CombatManager.Instance.VisualizeMonsterHeal(amount);
            }
        }

        [ClientRpc]
        public void TakeShortRestClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                PlayManager.Instance.TakeShortRest(this);
            }
        }

        [ClientRpc]
        public void TakeLongRestClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                PlayManager.Instance.TakeLongRest(this);
            }
        }

        [ClientRpc]
        public void GoToShrineClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                PlayManager.Instance.GoToShrine(this);
            }
        }

        public void SetupStore()
        {
            int storeSize = 5;
            if (NetworkManager.Singleton.IsServer)
            {
                List<string> cards = new List<string>();
                for (int i = 0; i < storeSize; i++)
                    cards.Add(PlayManager.Instance.DrawFromLootDeck());
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if(p.EndOfDayActivity.Value == 0)
                    {
                        for (int i = 0; i < storeSize; i++)
                            p.AddLootCardsToDrawClientRPC(cards[i]);
                        p.DrawLootCardsClientRPC(storeSize, false, true);
                    }
                }
            }
            else
                SetupStoreServerRPC(storeSize);
        }
        [ServerRpc]
        private void SetupStoreServerRPC(int storeSize, ServerRpcParams rpcParams = default)
        {
            List<string> cards = new List<string>();
            for (int i = 0; i < storeSize; i++)
                cards.Add(PlayManager.Instance.DrawFromLootDeck());
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.EndOfDayActivity.Value == 0)
                {
                    for (int i = 0; i < storeSize; i++)
                        p.AddLootCardsToDrawClientRPC(cards[i]);
                    p.DrawLootCardsClientRPC(storeSize, false, true);
                }
            }
        }

        public void RemoveStoreCardForOthers(int slot)
        {
            FixedString64Bytes uuid = UUID.Value;
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value != uuid)
                        p.RemoveStoreCardForOthersClientRPC(slot);
                }
            }
            else
                RemoveStoreCardForOthersServerRPC(uuid, slot);
        }
        [ServerRpc]
        private void RemoveStoreCardForOthersServerRPC(FixedString64Bytes uuid, int slot, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
            {
                if (p.UUID.Value != uuid)
                    p.RemoveStoreCardForOthersClientRPC(slot);
            }
        }
        [ClientRpc]
        public void RemoveStoreCardForOthersClientRPC(int slot, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                LootManager.Instance.RemoveStoreCard(slot);
            }
        }

        public void Resurrect(int health = 1)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Health.Value = health;
            }
            else
                ResurrectServerRPC(health);
        }
        [ServerRpc]
        private void ResurrectServerRPC(int health, ServerRpcParams rpcParams = default)
        {
            Health.Value = health;
        }

        public void SetNextDialogueChunk()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.SetNextDialogueChunkClientRPC();
            }
            else
                SetNextDialogueChunkServerRPC();
        }
        [ServerRpc]
        private void SetNextDialogueChunkServerRPC(ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.SetNextDialogueChunkClientRPC();
        }
        [ClientRpc]
        public void SetNextDialogueChunkClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                QuestManager.Instance.SetNextChunk();
            }
        }

        public void EndDialogue()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.EndDialogueClientRPC();
            }
            else
                EndDialogueServerRPC();
        }
        [ServerRpc]
        private void EndDialogueServerRPC(ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.EndDialogueClientRPC();
        }
        [ClientRpc]
        public void EndDialogueClientRPC(ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                QuestManager.Instance.EndDialogue();
            }
        }

        public void LoadIntoQuest(string questName)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.LoadIntoQuestClientRPC(questName);
            }
            else
                LoadIntoQuestServerRPC(questName);
        }
        [ServerRpc]
        private void LoadIntoQuestServerRPC(FixedString64Bytes questName, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.LoadIntoQuestClientRPC(questName);
        }
        [ClientRpc]
        public void LoadIntoQuestClientRPC(FixedString64Bytes questName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.LoadQuestEncounter(questName + "");
            }
        }

        public void GameOver(int state)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                PlayManager.Instance.GameOver(state);
            }
            else
                GameOverServerRPC(state);
        }
        [ServerRpc]
        private void GameOverServerRPC(int state, ServerRpcParams rpcParams = default)
        {
            PlayManager.Instance.GameOver(state);
        }

        public void SetBoss(string cardName)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (Player p in PlayManager.Instance.playerList)
                    p.SetBossClientRPC(cardName);
            }
            else
                SetBossServerRPC(cardName);
        }
        [ServerRpc]
        private void SetBossServerRPC(FixedString64Bytes cardName, ServerRpcParams rpcParams = default)
        {
            foreach (Player p in PlayManager.Instance.playerList)
                p.SetBossClientRPC(cardName);
        }
        [ClientRpc]
        public void SetBossClientRPC(FixedString64Bytes cardName, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && !isBot)
            {
                PlayManager.Instance.SetBoss(cardName + "");
            }
        }
        #endregion
    }
}
