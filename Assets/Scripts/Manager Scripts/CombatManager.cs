using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;
using Unity.Collections;
using System;
using Unity.Netcode;

public enum OnAttacked { NONE, PAY_10_GOLD, PAY_20_GOLD };
public enum CombatLayoutStyle { DEFAULT, ATTACKING };

public class CombatManager : Singleton<CombatManager>
{
    public List<Combatant> turnOrderCombatantList;
    public GameObject combatFadeOverlay;
    public GameObject combatBackground;
    public GameObject combatLayout;
    public GameObject statRoll;
    public GameObject choice;
    public GameObject attackRoll;
    public GameObject defensiveOptions;
    public GameObject combatOptions;
    public UIEncounterCard enemyCard;
    public GameObject[] playerCards;
    public int combatTurnMarker;
    public MonsterCard monsterCard;
    public Combatant monster;
    public GameObject attackIconPrefab;
    public UICombatCharacterPanel[] characterPanels;
    
    public float fadeLength = 0.01f;
    public float fadedWaitTime = 0.5f;
    public float attackFadeLength = 0.004f;
    public float attackFlashLength = 0.5f;
    public float layoutChangeLength = 0.006f;

    public float radius;
    public float originX;
    public float originY;
    public bool isYourTurn;
    public bool isMonsterTurn;
    public int[] monsterTargets = new int[0];
    public bool fleeingPrevented;
    public CombatLayoutStyle combatLayoutStyle;
    public int attackerId;
    public bool waitUntil;
    public bool usedHoly;
    public bool receivedLoneWolf;

    public Action<Combatant> OnPlayerDealDamage;
    public Action<Combatant> OnPlayerTakeDamage;
    public Action<Combatant> OnPlayerSpendAbilityCharge;
    public OnAttacked OnPlayerBeingAttacked;

    private bool ready;
    private bool combatantListSet;
    private bool changingStyle;
    private bool usedItemThisTurn;
    private bool canUseAttackAbilities;

    private void Update()
    {
        if(ready && !changingStyle)
        {
            AllignPlayerCards();
            UpdateCharacterPanels();
        }
    }

    public void LoadIntoCombat()
    {
        ResetCombat();
        StartCoroutine(FadeOverlay());

        // If it is your turn, set turn order combatant list for all players
        if(PlayManager.Instance.isYourTurn)
        {
            turnOrderCombatantList = new List<Combatant>();
            foreach(Player p in PlayManager.Instance.playerList)
            {
                if (p.ParticipatingInCombat.Value == 1)
                    turnOrderCombatantList.Add(new Combatant(CombatantType.PLAYER, p));
            }
            monster = new Combatant(CombatantType.MONSTER, monsterCard);
            turnOrderCombatantList.Add(monster);

            PlayManager.Instance.ShuffleDeck(turnOrderCombatantList);
            turnOrderCombatantList.Sort((a, b) => b.GetSpeed() - a.GetSpeed());

            FixedString64Bytes[] arr = new FixedString64Bytes[turnOrderCombatantList.Count];
            for (int i = 0; i < turnOrderCombatantList.Count; i++)
            {
                if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER)
                    arr[i] = turnOrderCombatantList[i].player.UUID.Value;
                else
                    arr[i] = turnOrderCombatantList[i].monster.cardName;
            }
            combatTurnMarker = 0;
            PlayManager.Instance.localPlayer.SetTurnOrderCombatantList(arr, false);
            PlayManager.Instance.localPlayer.UpdateCombatTurnMarker(combatTurnMarker);
            StartCombatantsTurn();
        }
    }

    public void EndCombat(int result)
    {
        // Results:
        // 1 = Monster Died
        // 2 = Players died or fled
        // 3 = Both parties died

        Action OnComplete = default;
        
        switch (result)
        {
            case 1:
                // Monster defeated case
                OnComplete = () => {
                    Player currentTurnPlayer = PlayManager.Instance.turnOrderPlayerList[PlayManager.Instance.turnMarker];
                    List<Player> otherPlayersInCombat = new List<Player>();
                    foreach(Combatant c in turnOrderCombatantList)
                    {
                        if (c.combatantType == CombatantType.PLAYER && c.player.UUID.Value != currentTurnPlayer.UUID.Value)
                            otherPlayersInCombat.Add(c.player);
                    }

                    int cardsDrawn = 0;
                    // If the current turn player is a combatant, and they are alive, give them their rewards
                    if (IsCombatant(currentTurnPlayer) && GetCombatantFromPlayer(currentTurnPlayer).IsAlive())
                    {
                        switch (monsterCard.type)
                        {
                            case MonsterType.BASIC:
                                cardsDrawn = 1;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), currentTurnPlayer))
                                    cardsDrawn += 1;
                                currentTurnPlayer.DrawLootCards(cardsDrawn, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(3);
                                currentTurnPlayer.GainGold(10);
                                break;
                            case MonsterType.ELITE:
                                cardsDrawn = 2;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), currentTurnPlayer))
                                    cardsDrawn += 1;
                                currentTurnPlayer.DrawLootCards(cardsDrawn, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(6);
                                currentTurnPlayer.GainGold(20);
                                break;
                            case MonsterType.MINIBOSS:
                                cardsDrawn = 1;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), currentTurnPlayer))
                                    cardsDrawn += 1;
                                currentTurnPlayer.DrawLootCards(cardsDrawn, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(6);
                                currentTurnPlayer.GainGold(20);

                                // Quest Progress Check
                                if(monster.GetName() == "Bandit Weeb Lord")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Isekai!")
                                            q.questStep++;
                                    }
                                }
                                else if(monster.GetName() == "Goblin Horde")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Goblin Hunt")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Discord Kitten")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "DisharMEOWny")
                                            q.questStep++;
                                    }
                                }
                                else if (monster.GetName() == "Raging Discord Kitten")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "DisharMEOWny")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Rainbow Slime")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Double Rainbow")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Spooky Spider")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "The Abandoned Path")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                PlayManager.Instance.localPlayer.UpdateQuests(PlayManager.Instance.quests);
                                break;
                            case MonsterType.BOSS:
                                // Win the game
                                currentTurnPlayer.GameOver(1);
                                break;
                        }
                        currentTurnPlayer.CompleteEncounter(false, currentTurnPlayer.UUID.Value);
                    }
                    // Otherwise pick another combatant to be graced
                    else
                    {
                        PlayManager.Instance.ShuffleDeck(otherPlayersInCombat);
                        Player chosenOne = otherPlayersInCombat[0];
                        otherPlayersInCombat.RemoveAt(0);
                        switch (monsterCard.type)
                        {
                            case MonsterType.BASIC:
                                cardsDrawn = 1;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), chosenOne))
                                    cardsDrawn += 1;
                                chosenOne.DrawLootCards(cardsDrawn, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(3, true);
                                chosenOne.GainGold(10);
                                break;
                            case MonsterType.ELITE:
                                cardsDrawn = 2;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), chosenOne))
                                    cardsDrawn += 1;
                                chosenOne.DrawLootCards(cardsDrawn, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(6, true);
                                chosenOne.GainGold(20);
                                break;
                            case MonsterType.MINIBOSS:
                                cardsDrawn = 1;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), chosenOne))
                                    cardsDrawn += 1;
                                chosenOne.DrawLootCards(cardsDrawn, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(6, true);
                                chosenOne.GainGold(20);

                                // Quest Progress Check
                                if (monster.GetName() == "Bandit Weeb Lord")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Isekai!")
                                            q.questStep++;
                                    }
                                }
                                else if (monster.GetName() == "Goblin Horde")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Goblin Hunt")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Discord Kitten")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "DisharMEOWny")
                                            q.questStep++;
                                    }
                                }
                                else if (monster.GetName() == "Raging Discord Kitten")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "DisharMEOWny")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Rainbow Slime")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "Double Rainbow")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                else if (monster.GetName() == "Spooky Spider")
                                {
                                    foreach (QuestCard q in PlayManager.Instance.quests)
                                    {
                                        if (q.cardName == "The Abandoned Path")
                                        {
                                            PlayManager.Instance.localPlayer.ReduceChaos(-1 * q.rewardChaos);
                                            foreach (Player p in PlayManager.Instance.playerList)
                                                p.GainGold(q.rewardGold);
                                            q.questStep++;
                                        }
                                    }
                                }
                                PlayManager.Instance.localPlayer.UpdateQuests(PlayManager.Instance.quests);
                                break;
                            case MonsterType.BOSS:
                                // Win the game
                                break;
                        }
                        currentTurnPlayer.CompleteEncounter(true, currentTurnPlayer.UUID.Value);
                    }

                    // Handle other players in combat
                    switch (monsterCard.type)
                    {
                        case MonsterType.BASIC:
                            foreach(Player p in otherPlayersInCombat)
                                p.GainXP(3, true);
                            break;
                        case MonsterType.ELITE:
                            foreach (Player p in otherPlayersInCombat)
                                p.GainXP(6, true);
                            break;
                        case MonsterType.MINIBOSS:
                            foreach (Player p in otherPlayersInCombat)
                            {
                                cardsDrawn = 1;
                                if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Looter"), p))
                                    cardsDrawn += 1;
                                p.DrawLootCards(cardsDrawn, p.UUID.Value, false);
                                p.GainXP(6, true);
                            }
                            break;
                        case MonsterType.BOSS:
                            // Win the game
                            break;
                    }
                };
                break;
            case 2:
                // Players defeated case
                OnComplete = () =>
                {
                    Player currentTurnPlayer = PlayManager.Instance.turnOrderPlayerList[PlayManager.Instance.turnMarker];
                    currentTurnPlayer.CompleteEncounter(true, currentTurnPlayer.UUID.Value);
                };
                break;
            case 3:
                // Laugh because i'm not handling this case lmao
                Debug.Log("Yeah right, I totally believe that every player and the monster ALL died at the same time!");
                break;
        }

        CombatCompleteNotification(result, OnComplete);
        PlayManager.Instance.localPlayer.SendCombatCompleteNotifications(result);
    }

    public void CombatCompleteNotification(int result, Action OnComplete = default)
    {
        string description;
        if (result == 1)
        {
            description = "<color=#267833>VICTORY</color>";
        }
        else
        {
            description = "<color=#902E2E>DEFEAT</color>";
        }
        PlayManager.Instance.SendNotification(4, description, () => {
            StartCoroutine(LeaveCombat(OnComplete));
        });
    }

    public void StartTurn()
    {
        isYourTurn = true;
        if (!PlayManager.Instance.transitions.transform.GetChild(4).gameObject.activeInHierarchy && !combatFadeOverlay.activeInHierarchy)
            PlayManager.Instance.CallTransition(5);
        usedItemThisTurn = false;
    }

    public void StartBotTurn()
    {
        EndBotTurn();
    }

    public void StartMonsterTurn(int[] targets)
    {
        monsterTargets = targets;
        isMonsterTurn = true;
        if (!PlayManager.Instance.transitions.transform.GetChild(4).gameObject.activeInHierarchy && !combatFadeOverlay.activeInHierarchy)
            PlayManager.Instance.CallTransition(6);
    }

    public void EndTurn()
    {
        Combatant c = GetCombatantFromPlayer(PlayManager.Instance.localPlayer);

        // Deactivate variable marking your turn
        isYourTurn = false;

        // Increment turn marker for all players
        combatTurnMarker++;
        PlayManager.Instance.localPlayer.UpdateCombatTurnMarker(combatTurnMarker);

        // Cycle effects
        CycleEffects(c);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void EndBotTurn()
    {
        // Increment turn marker for all players
        combatTurnMarker++;
        PlayManager.Instance.localPlayer.UpdateCombatTurnMarker(combatTurnMarker);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void StartCombatantsTurn()
    {
        if(CombatOverCheck() > -1)
        {
            EndCombat(CombatOverCheck());
            return;
        }

        if (combatTurnMarker >= turnOrderCombatantList.Count)
            combatTurnMarker = 0;
        if (turnOrderCombatantList[combatTurnMarker].combatantType == CombatantType.PLAYER)
            PlayManager.Instance.localPlayer.StartNextCombatantTurn(combatTurnMarker);
        else
        {
            // First handle on start of turn monster debuffs
            int burning = monster.IsBurning();
            if (burning > -1)
            {
                bool goingToDie = monster.GetHealth() <= burning;

                StartCoroutine(AnimateMonsterTakeDamage(enemyCard, burning, () => {
                    monster.TakeDamage(burning);
                }));

                // Visualize player taking damage for all other players
                PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(burning);

                // Check if still alive with incoming poison damage
                if (goingToDie)
                {
                    EndCombat(1);
                    return;
                }
            }

            PlayManager.Instance.localPlayer.StartMonsterTurn(GetMonsterTargets());
        }
    }

    public void TakeTurn()
    {
        Combatant c = GetCombatantFromPlayer(PlayManager.Instance.localPlayer);

        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Lone Wolf")) && turnOrderCombatantList.Count == 2 && !receivedLoneWolf)
        {
            receivedLoneWolf = true;
            InflictEffect(c, new Effect("Power Up", -1, 1, true));
        }

        // Check if still alive
        if (!c.IsAlive())
        {
            EndTurn();
            return;
        }

        // First handle on start of turn debuffs
        int poisoned = c.IsPoisoned();
        if (poisoned > -1)
        {
            bool goingToDie = c.GetHealth() <= poisoned && !c.player.IronWill.Value;

            StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(c), poisoned, () => {
                c.TakeDamage(poisoned, true);
            }));

            // Visualize player taking damage for all other players
            PlayManager.Instance.localPlayer.VisualizeTakeDamageForOthers(poisoned);

            // Check if still alive with incoming poison damage
            if (goingToDie)
            {
                EndTurn();
                return;
            }
        }
        if (c.IsEnwebbed())
        {
            EndTurn();
            return;
        }

        // Start turn
        EnableCombatOptions();
        CombatOptionsListener((a) => {
            if(a == 1)
            {
                // Basic Attack
                attackerId = GetIdFromCombatant(c);
                // Make sure to sync this between all players if we decide to have all players see this
                StartCoroutine(TransitionCombatLayoutStyle(CombatLayoutStyle.ATTACKING, () => {
                    int playerPower = 0;
                    int monsterPower = 0;
                    if (PlayManager.Instance.IsPhysicalBased(c.player))
                    {
                        playerPower = c.GetPhysicalPower();
                        monsterPower = monster.GetPhysicalPower();
                    }
                    else
                    {
                        playerPower = c.GetMagicalPower();
                        monsterPower = monster.GetMagicalPower();
                    }
                    MakeAttackRoll(playerPower, monsterPower);
                    AttackRollListener((a, crit) => {
                        if(a == 1)
                        {
                            // player attacks monster
                            int damage = c.GetAttack();
                            if (crit)
                                damage *= 2;
                            AttackMonster(c, damage);
                        }
                        else if(a == -1)
                        {
                            // monster attacks player
                            AttackPlayer(c, () => {
                                // OnComplete
                                StartCoroutine(TransitionCombatLayoutStyle(CombatLayoutStyle.DEFAULT, () => {
                                    EndTurn();
                                }));
                                PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.DEFAULT);
                            });
                        }
                        else if (a == 99)
                        {
                            // Combat Over
                            EndCombat(CombatOverCheck());
                        }
                    });
                }));
                PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.ATTACKING, attackerId);
            }
            else if (a == 2)
            {
                // Successfully fled
                RemoveFromCombat(c, isYourTurn);
            }
            else if (a == 3)
            {
                // Attack Ability
                Skill s = combatOptions.GetComponent<UICombatOptions>().skillUsed;
                Debug.Log("Used Skill: " + s.skillName);

                // *** Add in fancy ability stuff here ***

                attackerId = GetIdFromCombatant(c);
                // Make sure to sync this between all players if we decide to have all players see this
                StartCoroutine(TransitionCombatLayoutStyle(CombatLayoutStyle.ATTACKING, () => {
                    int playerPower = 0;
                    int monsterPower = 0;
                    if (PlayManager.Instance.IsPhysicalBased(c.player))
                    {
                        playerPower = c.GetPhysicalPower();
                        monsterPower = monster.GetPhysicalPower();
                    }
                    else
                    {
                        playerPower = c.GetMagicalPower();
                        monsterPower = monster.GetMagicalPower();
                    }
                    MakeAttackRoll(playerPower, monsterPower);
                    AttackRollListener((a, crit) => {
                        if (a == 1)
                        {
                            // player attacks monster
                            int damage = c.GetAttack();

                            switch(s.skillName)
                            {
                                case "Sap Energy":
                                    AttackMonster(c, 0, default, () => {
                                        c.player.RestoreAbilityCharges(2);
                                    });
                                    break;
                                case "Balanced Strike":
                                    AttackMonster(c, crit ? (damage + PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(c.player))) * 2 : damage + PlayManager.Instance.GetMod(PlayManager.Instance.GetStrength(c.player)));
                                    break;
                                case "Crushing Blow":
                                    AttackMonster(c, crit ? damage * 2 : damage, new List<Effect> { new Effect("Dazed", -1) });
                                    break;
                                default:
                                    AttackMonster(c, crit ? damage * 2 : damage);
                                    break;
                            }
                        }
                        else if (a == -1)
                        {
                            // monster attacks player
                            switch (s.skillName)
                            {
                                case "Balanced Strike":
                                    InflictEffect(c, new Effect("Armor Up", 1, 3, true));
                                    break;
                            }
                            AttackPlayer(c, () => {
                                // OnComplete
                                StartCoroutine(TransitionCombatLayoutStyle(CombatLayoutStyle.DEFAULT, () => {
                                    EndTurn();
                                }));
                                PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.DEFAULT);
                            });
                        }
                        else if (a == 99)
                        {
                            // Combat Over
                            EndCombat(CombatOverCheck());
                        }
                    });
                }));
                PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.ATTACKING, attackerId);
            }
            else if (a == 99)
            {
                // Combat Over
                EndCombat(CombatOverCheck());
            }
        });
    }

    public void AttackMonster(Combatant c, int damage, List<Effect> debuffs = default, Action OnAttack = default)
    {
        // Attack animation + effect + end monster turn
        StartCoroutine(AnimateMonsterAttacked(c, damage,() => {
            // OnAttack
            int bleeding = monster.IsBleeding();
            if (bleeding > -1)
            {
                damage += bleeding;
                UseStatusEffect(monster, "Bleeding");
            }
            monster.TakeDamage(damage);
            if(debuffs != default)
            {
                foreach (Effect e in debuffs)
                    InflictEffect(monster, e);
            }
            if(OnPlayerDealDamage != default)
                OnPlayerDealDamage(c);
            if (OnAttack != default)
                OnAttack();
        }, () => {
            // OnComplete
            StartCoroutine(TransitionCombatLayoutStyle(CombatLayoutStyle.DEFAULT, () => {
                EndTurn();
            }));
            PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.DEFAULT);
        }));

        // Visualize monster attack for all other players
        PlayManager.Instance.localPlayer.VisualizeMonsterAttackForOthers(damage);
    }

    public void RemoveFromCombat(Combatant c, bool isTheirTurn)
    {
        // Remove from turn order combatant list
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == c.combatantType && ((turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == c.player.UUID.Value) || (turnOrderCombatantList[i].combatantType == CombatantType.MONSTER && turnOrderCombatantList[i].monster.cardName == c.monster.cardName)))
            {
                turnOrderCombatantList.RemoveAt(i);
                break;
            }
        }
        // Create array from turn order combatant list
        FixedString64Bytes[] arr = new FixedString64Bytes[turnOrderCombatantList.Count];
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER)
                arr[i] = turnOrderCombatantList[i].player.UUID.Value;
            else
                arr[i] = turnOrderCombatantList[i].monster.cardName;
        }
        // Update turn order combatant list for all players
        PlayManager.Instance.localPlayer.SetTurnOrderCombatantList(arr, true);

        // If still players in combat
        if (PlayersStillInCombat())
        {
            if(isTheirTurn)
            {
                // Deactivate variable marking your turn
                isYourTurn = false;

                // Start next combatant turn
                StartCombatantsTurn();
            }
        }
        // Otherwise End Combat
        else
        {
            EndCombat(2);
        }
    }

    public void MonsterTakeTurn()
    {
        // If targeted by monster
        if (IsTargetedByMonster(PlayManager.Instance.localPlayer))
        {
            monsterCard.Skill(GetCombatantFromPlayer(PlayManager.Instance.localPlayer));
        }
        // Else if no one was targeted
        else if (monsterTargets.Length == 0)
        {
            // Only host executes skill
            if (NetworkManager.Singleton.IsServer)
                monsterCard.Skill(GetCombatantFromPlayer(PlayManager.Instance.localPlayer));
        }
    }

    public void MonsterEndTurn()
    {
        PlayManager.Instance.localPlayer.ReadyUp();

        // If you are the first targeted player or no one was targeted, wait to continue
        if (IsFirstTargetedByMonster(PlayManager.Instance.localPlayer))
            StartCoroutine(WaitToEndMonsterTurn());
    }

    IEnumerator WaitToEndMonsterTurn()
    {
        yield return new WaitUntil(() =>
        {
            bool allReady = true;
            for(int i = 0; i < monsterTargets.Length; i++)
            {
                if (!turnOrderCombatantList[monsterTargets[i]].player.Ready.Value)
                    allReady = false;
            }
            return allReady;
        });

        // Unready all players and continue them
        foreach (Player p in PlayManager.Instance.playerList)
            p.Unready();

        // Increment turn marker for all players
        combatTurnMarker++;
        PlayManager.Instance.localPlayer.UpdateCombatTurnMarker(combatTurnMarker);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void AttackPlayer(Combatant c, Action OnComplete, List<Effect> debuffs = default)
    {
        if(monster.IsDazed())
        {
            if (OnComplete != default)
                OnComplete();
            CleanseEffect(monster, "Dazed");
            return;
        }
        EnableDefensiveOptions();
        DefensiveOptionsListener((a, p) => {
            if(a == 1)
            {
                // Avoided getting attacked
                if (OnComplete != default)
                    OnComplete();
            }
            else if(a == -1)
            {
                if (p != default)
                {
                    c = GetCombatantFromPlayer(p);
                    RegularAttacked(c, OnComplete, debuffs);
                }
                else
                {
                    Skill revenge = AbilityManager.Instance.GetSkill("Revenge");
                    if (AbilityManager.Instance.HasAbilityUnlocked(revenge) && PlayManager.Instance.GetAbilityCharges(c.player) >= revenge.cost && isYourTurn)
                    {
                        MakeChoice("Use Revenge", "Take Damage", true, true);
                        ChoiceListener((a) =>
                        {
                            if (a == 1)
                            {
                                revenge.UseSkill();
                                InflictEffect(c, new Effect("Power Up", 2, 1, true));
                                // Got attacked
                                if (c.GetArmor() >= monster.GetAttack())
                                {
                                    // No damage will be taken, therefore avoid debuffs
                                    StartCoroutine(AnimatePlayerAttacked(c,
                                    () => {
                                        // OnAttack
                                        c.TakeDamage(monster.GetAttack());
                                       
                                        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, c.GetAttack(), () => {
                                            monster.TakeDamage(c.GetAttack());
                                        }));

                                        // Visualize player taking damage for all other players
                                        PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(c.GetAttack());
                                    }, OnComplete));
                                }
                                else
                                {
                                    // Attack animation + effect + end monster turn
                                    StartCoroutine(AnimatePlayerAttacked(c,
                                    () => {
                                        // OnAttack
                                        c.TakeDamage(monster.GetAttack());

                                        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, c.GetAttack(), () => {
                                            monster.TakeDamage(c.GetAttack());
                                        }));

                                        // Visualize player taking damage for all other players
                                        PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(c.GetAttack());

                                        if (debuffs != default)
                                        {
                                            foreach (Effect e in debuffs)
                                                InflictEffect(c, e);
                                        }
                                    }, () => {
                                        if (c.IsAlive())
                                            StartCoroutine(OnTakeDamage(c, OnComplete));
                                        else if (OnComplete != default)
                                            OnComplete();
                                    }));
                                }

                                // Visualize player attack for all other players
                                PlayManager.Instance.localPlayer.VisualizeAttackForOthers(c.player);
                            }
                            else
                                RegularAttacked(c, OnComplete, debuffs);
                        });
                    }
                    else
                        RegularAttacked(c, OnComplete, debuffs);
                }
            }
            else if(a == 99)
            {
                // Combat Over
                EndCombat(CombatOverCheck());
            }
        });
    }

    private void RegularAttacked(Combatant c, Action OnComplete, List<Effect> debuffs = default)
    {
        // Got attacked
        if (c.GetArmor() >= monster.GetAttack())
        {
            // No damage will be taken, therefore avoid debuffs
            StartCoroutine(AnimatePlayerAttacked(c,
            () => {
                // OnAttack
                c.TakeDamage(monster.GetAttack());
            }, OnComplete));
        }
        else
        {
            // Attack animation + effect + end monster turn
            StartCoroutine(AnimatePlayerAttacked(c,
            () => {
                // OnAttack
                c.TakeDamage(monster.GetAttack());
                if (debuffs != default)
                {
                    foreach (Effect e in debuffs)
                        InflictEffect(c, e);
                }
            }, () => {
                if (c.IsAlive())
                    StartCoroutine(OnTakeDamage(c, OnComplete));
                else if (OnComplete != default)
                    OnComplete();
            }));
        }

        // Visualize player attack for all other players
        PlayManager.Instance.localPlayer.VisualizeAttackForOthers(c.player);
    }

    public void InstantKill(Combatant c, Action OnComplete = default)
    {
        int instantKillDamage = 999;
        if(c.combatantType == CombatantType.PLAYER)
        {
            StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(c), instantKillDamage, () => {
                c.TakeDamage(instantKillDamage, true);
            }, OnComplete));
            PlayManager.Instance.localPlayer.VisualizeTakeDamageForOthers(instantKillDamage);
        }
        else
        {
            StartCoroutine(AnimateMonsterTakeDamage(enemyCard, instantKillDamage, () => {
                c.TakeDamage(instantKillDamage, true);
            }, OnComplete));
            PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(instantKillDamage);
        }
    }

    IEnumerator OnTakeDamage(Combatant c, Action OnComplete)
    {
        if(OnPlayerTakeDamage != default)
            OnPlayerTakeDamage(c);

        yield return new WaitUntil(() => !waitUntil);

        if (OnComplete != default)
            OnComplete();
    }

    public void InflictEffect(Combatant c, Effect e)
    {
        if (c.combatantType == CombatantType.PLAYER)
            c.player.GainStatusEffect(e.name, e.duration, e.potency, e.canStack, e.counter);
        else
            PlayManager.Instance.localPlayer.MonsterGainStatusEffect(e.name, e.duration, e.potency);
    }

    public void CleanseEffect(Combatant c, string effectName)
    {
        if (c.combatantType == CombatantType.PLAYER)
            c.player.RemoveStatusEffect(effectName);
        else
            PlayManager.Instance.localPlayer.MonsterRemoveStatusEffect(effectName);
    }

    public void CycleEffects(Combatant c)
    {
        if (c.combatantType == CombatantType.PLAYER)
            c.player.CycleStatusEffects();
        else
            PlayManager.Instance.localPlayer.MonsterCycleStatusEffects();
    }

    public void UseEffect(Combatant c, string effectName)
    {
        if (c.combatantType == CombatantType.PLAYER)
            c.player.UseStatusEffect(effectName);
        else
            PlayManager.Instance.localPlayer.MonsterUseStatusEffect(effectName);
    }

    public void CleanseAllEffectsFromPlayer(Player p)
    {
        Combatant c = GetCombatantFromPlayer(p);
        CleanseEffect(c, "Eaten");
        CleanseEffect(c, "Enwebbed");
        CleanseEffect(c, "Plagued");
        CleanseEffect(c, "Poisoned");
        CleanseEffect(c, "Weakened");
    }

    public void HealMonster(int amount)
    {
        if (monster.IsPlagued())
            return;
        StartCoroutine(AnimateMonsterHeal(enemyCard, amount, () => {
            monster.RestoreHealth(amount);
            CleanseEffect(monster, "Poisoned");
        }));
        PlayManager.Instance.localPlayer.VisualizeMonsterHealForOthers(amount);
    }

    public void HealPlayer(Player p, int amount)
    {
        Combatant c = GetCombatantFromPlayer(p);
        if (c.IsPlagued())
            return;
        if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Heaven's Paragon"), p))
            amount += 2;
        StartCoroutine(AnimatePlayerHeal(GetPlayerCardFromCombatant(c), amount, () => {
            c.RestoreHealth(amount);
            CleanseEffect(c, "Poisoned");
        }));
        PlayManager.Instance.localPlayer.VisualizeHealForOthers(amount);
    }

    // Called from Player.cs
    public void GainStatusEffect(Combatant c, Effect e)
    {
        c.GainStatusEffect(e);
    }
    // Called from Player.cs
    public void RemoveStatusEffect(Combatant c, string effectName)
    {
        c.RemoveStatusEffect(effectName);
    }
    // Called from Player.cs
    public void CycleStatusEffects(Combatant c)
    {
        c.CycleStatusEffects();
    }
    // Called from Player.cs
    public void UseStatusEffect(Combatant c, string effectName)
    {
        c.UseStatusEffect(effectName);
    }

    public void VisualizePlayerAttacked(Player p)
    {
        StartCoroutine(AnimatePlayerAttacked(GetCombatantFromPlayer(p)));
    }

    public void VisualizePlayerTakeDamage(Player p, int amount)
    {
        StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(GetCombatantFromPlayer(p)), amount));
    }

    public void VisualizeMonsterAttacked(Player p, int amount)
    {
        StartCoroutine(AnimateMonsterAttacked(GetCombatantFromPlayer(p), amount));
    }

    public void VisualizeMonsterTakeDamage(int amount)
    {
        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, amount));
    }

    public void VisualizePlayerHeal(Player p, int amount)
    {
        StartCoroutine(AnimatePlayerHeal(GetPlayerCardFromCombatant(GetCombatantFromPlayer(p)), amount));
    }

    public void VisualizeMonsterHeal(int amount)
    {
        StartCoroutine(AnimateMonsterHeal(enemyCard, amount));
    }

    private int[] GetMonsterTargets()
    {
        // Returns indices of targeted players
        switch(monsterCard.target)
        {
            case Target.ALL:
                List<int> targets = new List<int>();
                for (int i = 0; i < turnOrderCombatantList.Count; i++)
                {
                    if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].IsAlive())
                        targets.Add(i);
                }
                return targets.ToArray();
            case Target.LOWEST_HEALTH:
                return new int[] { GetLowestHealthPlayerIndex() };
            case Target.HIGHEST_HEALTH:
                return new int[] { GetHighestHealthPlayerIndex() };
        };
        return new int[0];
    }

    private int GetLowestHealthPlayerIndex()
    {
        List<Combatant> playersInCombat = new List<Combatant>();
        foreach(Combatant c in turnOrderCombatantList)
        {
            if (c.combatantType == CombatantType.PLAYER && c.IsAlive())
                playersInCombat.Add(c);
        }
        PlayManager.Instance.ShuffleDeck(playersInCombat);
        playersInCombat.Sort((a, b) => a.GetHealth() - b.GetHealth());
        for(int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == playersInCombat[0].player.UUID.Value)
                return i;
        }
        return -1;
    }

    private int GetHighestHealthPlayerIndex()
    {
        List<Combatant> playersInCombat = new List<Combatant>();
        foreach (Combatant c in turnOrderCombatantList)
        {
            if (c.combatantType == CombatantType.PLAYER && c.IsAlive())
                playersInCombat.Add(c);
        }
        PlayManager.Instance.ShuffleDeck(playersInCombat);
        playersInCombat.Sort((a, b) => b.GetHealth() - a.GetHealth());
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == playersInCombat[0].player.UUID.Value)
                return i;
        }
        return -1;
    }

    private void AllignPlayerCards()
    {
        int playerAmount = Mathf.Clamp(turnOrderCombatantList.Count - 1, 0, 6);
        if(combatLayoutStyle == CombatLayoutStyle.DEFAULT)
        {
            float theta = Mathf.PI / 5f;
            float startingDegree = 0;
            float start = (6 - playerAmount) / 2f;
            int i;
            for (i = 0; i < playerAmount; i++)
            {
                playerCards[i].SetActive(true);
                playerCards[i].transform.localPosition = new Vector3(radius * Mathf.Cos(startingDegree - theta * (i + start)) + originX, radius * Mathf.Sin(startingDegree - theta * (i + start)) + originY, 0);
                playerCards[i].transform.localScale = new Vector3(1.1f, 1.1f, 1);
            }
            for (; i < 6; i++)
            {
                playerCards[i].SetActive(false);
            }
            int index = playerAmount - 1;
            for (i = 0; i < turnOrderCombatantList.Count; i++)
            {
                if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER)
                {
                    if(index >= 0)
                    {
                        playerCards[index].GetComponent<UIPlayerCard>().ActivateCrosshair(IsTargetedByMonster(turnOrderCombatantList[i].player) && turnOrderCombatantList[combatTurnMarker % turnOrderCombatantList.Count].monster != null && !isMonsterTurn && CombatOverCheck() == -1);
                        playerCards[index].GetComponent<UIPlayerCard>().ActivateTurnMarker(combatTurnMarker == i);
                        playerCards[index].GetComponent<UIPlayerCard>().SetVisuals(turnOrderCombatantList[i]);
                        index--;
                    }
                }
                else
                {
                    enemyCard.ActivateTurnMarker(combatTurnMarker == i);
                    enemyCard.SetMonsterVisuals(turnOrderCombatantList[i]);
                    enemyCard.UpdateHealthBar(turnOrderCombatantList[i]);
                    enemyCard.SetDisplayPosition(new Vector3(0, 0, 0));
                }
            }
        }
        else if(combatLayoutStyle == CombatLayoutStyle.ATTACKING)
        {
            int i;
            int pos = 0;
            for (i = 0; i < playerAmount; i++)
            {
                playerCards[i].SetActive(true);
                if (i == attackerId)
                {
                    playerCards[i].transform.localPosition = new Vector3(-900, 330, 0);
                    playerCards[i].transform.localScale = new Vector3(1.5f, 1.5f, 1);
                }
                else
                {
                    playerCards[i].transform.localPosition = new Vector3(200 * (2 * pos + 1 - (playerAmount - 1)), -480, 0);
                    playerCards[i].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                    pos++;
                }
            }
            for (; i < 6; i++)
            {
                playerCards[i].SetActive(false);
            }
            int index = playerAmount - 1;
            for (i = 0; i < turnOrderCombatantList.Count; i++)
            {
                if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER)
                {
                    if (index >= 0)
                    {
                        playerCards[index].GetComponent<UIPlayerCard>().ActivateCrosshair(IsTargetedByMonster(turnOrderCombatantList[i].player) && turnOrderCombatantList[combatTurnMarker].monster != null && !isMonsterTurn && CombatOverCheck() == -1);
                        playerCards[index].GetComponent<UIPlayerCard>().ActivateTurnMarker(combatTurnMarker == i);
                        playerCards[index].GetComponent<UIPlayerCard>().SetVisuals(turnOrderCombatantList[i]);
                        index--;
                    }
                }
                else
                {
                    enemyCard.ActivateTurnMarker(combatTurnMarker == i);
                    enemyCard.SetMonsterVisuals(turnOrderCombatantList[i]);
                    enemyCard.UpdateHealthBar(turnOrderCombatantList[i]);
                    enemyCard.SetDisplayPosition(new Vector3(470, 0, 0));
                }
            }
        }
    }

    private void UpdateCharacterPanels()
    {
        for(int i = 0; i < characterPanels.Length; i++)
        {
            characterPanels[i].UpdatePanel(turnOrderCombatantList[(combatTurnMarker + i) % turnOrderCombatantList.Count]);
        }
    }

    IEnumerator FadeOverlay()
    {
        combatFadeOverlay.SetActive(true);

        // First fade in overlay
        for(int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Activate main layout while screen is obstructed by overlay
        combatLayout.SetActive(true);
        combatBackground.SetActive(true);

        yield return new WaitUntil(() => combatantListSet);
        combatBackground.GetComponent<Image>().sprite = monsterCard.background;

        // Hold faded in overlay
        yield return new WaitForSeconds(fadedWaitTime * Global.animSpeed);
        ready = true;

        // Then fade out overlay
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        combatFadeOverlay.SetActive(false);

        PlayManager.Instance.CallTransition(4);

        // Setup Monster Passive
        monsterCard.Passive();
    }

    IEnumerator LeaveCombat(Action OnComplete = default)
    {
        combatFadeOverlay.SetActive(true);

        // First fade in overlay
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Deactivate main layout while screen is obstructed by overlay
        combatLayout.SetActive(false);
        combatBackground.SetActive(false);

        // Hold faded in overlay
        yield return new WaitForSeconds(fadedWaitTime * Global.animSpeed);
        ready = false;

        // Then fade out overlay
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        combatFadeOverlay.SetActive(false);

        // Call OnComplete
        if (OnComplete != default)
            OnComplete();
    }

    public void TransitionToStyle(CombatLayoutStyle style, int attackerId)
    {
        if (attackerId > -1)
            this.attackerId = attackerId;
        StartCoroutine(TransitionCombatLayoutStyle(style));
    }

    IEnumerator TransitionCombatLayoutStyle(CombatLayoutStyle targetLayoutStyle, Action OnComplete = default)
    {
        changingStyle = true;

        int playerAmount = Mathf.Clamp(turnOrderCombatantList.Count - 1, 0, 6);
        float theta = Mathf.PI / 5f;
        float startingDegree = 0;
        float start = (6 - playerAmount) / 2f;

        Vector3[] playerStartingPositions = new Vector3[playerCards.Length];
        Vector3[] playerEndingPositions = new Vector3[playerCards.Length];
        Vector3[] playerStartingScales = new Vector3[playerCards.Length];
        Vector3[] playerEndingScales = new Vector3[playerCards.Length];
        Vector3 monsterStartingPosition = Vector3.zero;
        Vector3 monsterEndingPosition = Vector3.zero;

        // If current style is default
        if(combatLayoutStyle == CombatLayoutStyle.DEFAULT)
        {
            for (int i = 0; i < playerAmount; i++)
            {
                playerStartingPositions[i] = new Vector3(radius * Mathf.Cos(startingDegree - theta * (i + start)) + originX, radius * Mathf.Sin(startingDegree - theta * (i + start)) + originY, 0);
                playerStartingScales[i] = new Vector3(1.1f, 1.1f, 1);
            }
            monsterStartingPosition = new Vector3(0, 0, 0);
        }
        // If current style is attacking
        else if(combatLayoutStyle == CombatLayoutStyle.ATTACKING)
        {
            int pos = 0;
            for (int i = 0; i < playerAmount; i++)
            {
                if (i == attackerId)
                {
                    playerStartingPositions[i] = new Vector3(-900, 330, 0);
                    playerStartingScales[i] = new Vector3(1.5f, 1.5f, 1);
                }
                else
                {
                    playerStartingPositions[i] = new Vector3(200 * (2 * pos + 1 - (playerAmount - 1)), -480, 0);
                    playerStartingScales[i] = new Vector3(1.1f, 1.1f, 1);
                    pos++;
                }
            }
            monsterStartingPosition = new Vector3(470, 0, 0);
        }

        // If target style is default
        if (targetLayoutStyle == CombatLayoutStyle.DEFAULT)
        {
            for (int i = 0; i < playerAmount; i++)
            {
                playerEndingPositions[i] = new Vector3(radius * Mathf.Cos(startingDegree - theta * (i + start)) + originX, radius * Mathf.Sin(startingDegree - theta * (i + start)) + originY, 0);
                playerEndingScales[i] = new Vector3(1.1f, 1.1f, 1);
            }
            monsterEndingPosition = new Vector3(0, 0, 0);
        }
        // If target style is attacking
        else if (targetLayoutStyle == CombatLayoutStyle.ATTACKING)
        {
            int pos = 0;
            for (int i = 0; i < playerAmount; i++)
            {
                if (i == attackerId)
                {
                    playerEndingPositions[i] = new Vector3(-900, 330, 0);
                    playerEndingScales[i] = new Vector3(1.5f, 1.5f, 1);
                }
                else
                {
                    playerEndingPositions[i] = new Vector3(200 * (2 * pos + 1 - (playerAmount - 1)), -480, 0);
                    playerEndingScales[i] = new Vector3(1.1f, 1.1f, 1);
                    pos++;
                }
            }
            monsterEndingPosition = new Vector3(470, 0, 0);
        }

        // Now tween between starting an ending positions
        for(int i = 1; i <= Global.animSteps; i++)
        {
            for (int j = 0; j < playerAmount; j++)
            {
                playerCards[j].transform.localPosition = playerStartingPositions[j] + i * Global.animRate * (playerEndingPositions[j] - playerStartingPositions[j]);
                playerCards[j].transform.localScale = playerStartingScales[j] + i * Global.animRate * (playerEndingScales[j] - playerStartingScales[j]);
            }
            enemyCard.SetDisplayPosition(monsterStartingPosition + i * Global.animRate * (monsterEndingPosition - monsterStartingPosition));
            yield return new WaitForSeconds(layoutChangeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Update current style
        combatLayoutStyle = targetLayoutStyle;
        changingStyle = false;

        // Call OnComplete
        if (OnComplete != default)
            OnComplete();
    }

    IEnumerator AnimatePlayerAttacked(Combatant target, Action OnAttack = default, Action OnComplete = default)
    {
        // Create attack icon at midpoint between enemy and target player
        GameObject attackIcon = Instantiate(attackIconPrefab, transform);
        UIPlayerCard playerCard = GetPlayerCardFromCombatant(target);
        attackIcon.transform.localPosition = enemyCard.GetDisplayPositionScaled() + (playerCard.transform.localPosition - enemyCard.GetDisplayPositionScaled()) / 2;

        // Fade in attack icon
        for(int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Animate take damage
        StartCoroutine(AnimatePlayerTakeDamage(playerCard, monster.GetAttack() - target.GetArmor(), OnAttack));
        yield return new WaitForSeconds(attackFlashLength);

        // Fade out attack icon
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Destroy attackIcon
        Destroy(attackIcon);

        // Call OnComplete function
        if(OnComplete != default)
        {
            OnCharacterIsAttacked(target);
            OnComplete();
        }
    }

    IEnumerator AnimatePlayerTakeDamage(UIPlayerCard playerCard, int damage, Action OnAttack = default, Action OnComplete = default)
    {
        // Call OnAttack function
        if (OnAttack != default)
            OnAttack();

        playerCard.ActivateDamaged(true);
        playerCard.DisplayDamageNumber(damage);
        yield return new WaitForSeconds(attackFlashLength * Global.animSpeed);
        playerCard.ActivateDamaged(false);

        // Call OnComplete function
        if(OnComplete != default)
            OnComplete();
    }

    IEnumerator AnimateMonsterAttacked(Combatant attacker, int damage, Action OnAttack = default, Action OnComplete = default)
    {
        // Create attack icon at midpoint between enemy and attacking player
        GameObject attackIcon = Instantiate(attackIconPrefab, transform);
        UIPlayerCard playerCard = GetPlayerCardFromCombatant(attacker);
        attackIcon.transform.localPosition = enemyCard.GetDisplayPositionScaled() + (playerCard.transform.localPosition - enemyCard.GetDisplayPositionScaled()) / 2;

        // Fade in attack icon
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Animate take damage
        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, damage, OnAttack));
        yield return new WaitForSeconds(attackFlashLength * Global.animSpeed);

        // Fade out attack icon
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod * Global.animSpeed);
        }

        // Destroy attackIcon
        Destroy(attackIcon);

        // Call OnComplete function
        if (OnComplete != default)
            OnComplete();
    }

    IEnumerator AnimateMonsterTakeDamage(UIEncounterCard card, int damage, Action OnAttack = default, Action OnComplete = default)
    {
        // Call OnAttack function
        if (OnAttack != default)
            OnAttack();

        card.ActivateDamaged(true);
        card.DisplayDamageNumber(damage);
        yield return new WaitForSeconds(attackFlashLength * Global.animSpeed);
        card.ActivateDamaged(false);

        // Call OnComplete function
        if (OnComplete != default)
            OnComplete();
    }

    IEnumerator AnimatePlayerHeal(UIPlayerCard playerCard, int heal, Action OnHeal = default)
    {
        // Call OnAttack function
        if (OnHeal != default)
            OnHeal();

        playerCard.ActivateHealed(true);
        playerCard.DisplayHealNumber(heal);
        yield return new WaitForSeconds(attackFlashLength * Global.animSpeed);
        playerCard.ActivateHealed(false);
    }

    IEnumerator AnimateMonsterHeal(UIEncounterCard card, int heal, Action OnHeal = default)
    {
        // Call OnAttack function
        if (OnHeal != default)
            OnHeal();

        card.ActivateHealed(true);
        card.DisplayHealNumber(heal);
        yield return new WaitForSeconds(attackFlashLength * Global.animSpeed);
        card.ActivateHealed(false);
    }

    public void SetTurnOrderCombatantList(FixedString64Bytes[] arr, bool keepUnits)
    {
        if(keepUnits)
        {
            for (int i = 0; i < turnOrderCombatantList.Count; i++)
            {
                bool existsInList = false;
                for (int j = 0; j < arr.Length; j++)
                {
                    if ((turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && arr[j] + "" == turnOrderCombatantList[i].player.UUID.Value) || (turnOrderCombatantList[i].combatantType == CombatantType.MONSTER && arr[j] + "" == turnOrderCombatantList[i].monster.cardName))
                    {
                        existsInList = true;
                    }
                }
                if (!existsInList)
                {
                    turnOrderCombatantList.RemoveAt(i);
                    i--;
                }
            }
            return;
        }
        turnOrderCombatantList = new List<Combatant>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (PlayManager.Instance.encounterReference.ContainsKey(arr[i] + ""))
            {
                monsterCard = PlayManager.Instance.encounterReference[arr[i] + ""] as MonsterCard;
                monster = new Combatant(CombatantType.MONSTER, monsterCard);
                turnOrderCombatantList.Add(monster);
            }
            else if (PlayManager.Instance.miniBossDeck.ContainsKey(arr[i] + ""))
            {
                monsterCard = PlayManager.Instance.miniBossDeck[arr[i] + ""];
                monster = new Combatant(CombatantType.MONSTER, monsterCard);
                turnOrderCombatantList.Add(monster);
            }
            else if (PlayManager.Instance.chapterBoss.cardName == arr[i] + "")
            {
                monsterCard = PlayManager.Instance.chapterBoss;
                monster = new Combatant(CombatantType.MONSTER, monsterCard);
                turnOrderCombatantList.Add(monster);
            }
            else
            {
                foreach (Player p in PlayManager.Instance.playerList)
                {
                    if (p.UUID.Value == arr[i])
                    {
                        turnOrderCombatantList.Add(new Combatant(CombatantType.PLAYER, p));
                    }
                }
            }
        }
        combatantListSet = true;
    }

    public int CombatOverCheck()
    {
        bool playersGone = !PlayersStillInCombat();
        bool monsterDead = !monster.IsAlive();

        // Players died or fled and monster died
        if (playersGone && monsterDead)
            return 3;
        // Players died or fled
        if (playersGone)
            return 2;
        // Monster died
        if (monsterDead)
            return 1;
        // Game isn't over
        return -1;
    }

    public bool PlayersStillInCombat()
    {
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].IsAlive())
                return true;
        }
        return false;
    }

    public bool IsCombatant(Player p)
    {
        for(int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == p.UUID.Value)
                return true;
        }
        return false;
    }

    public int GetIdFromCombatant(Combatant c)
    {
        int pos = 0;
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER)
            {
                if (turnOrderCombatantList[i].player.UUID.Value == c.player.UUID.Value)
                    return pos;
                pos++;
            }
        }
        return -1;
    }

    public Combatant GetCombatantFromPlayer(Player p)
    {
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == p.UUID.Value)
                return turnOrderCombatantList[i];
        }
        return null;
    }

    public UIPlayerCard GetPlayerCardFromCombatant(Combatant c)
    {
        for(int i = 0; i < playerCards.Length; i++)
        {
            if (playerCards[i].GetComponent<UIPlayerCard>().combatant.player.UUID.Value == c.player.UUID.Value)
                return playerCards[i].GetComponent<UIPlayerCard>();
        }
        return null;
    }

    public bool IsTargetedByMonster(Player p)
    {
        for(int i = 0; i < monsterTargets.Length; i++)
        {
            if (turnOrderCombatantList[monsterTargets[i]].player.UUID.Value == p.UUID.Value)
                return true;
        }
        return false;
    }

    public bool IsFirstTargetedByMonster(Player p)
    {
        if (monsterTargets.Length == 0)
            return true;
        return turnOrderCombatantList[monsterTargets[0]].player.UUID.Value == p.UUID.Value;
    }

    public List<Combatant> GetAlliesWhoCanTaunt(Player p)
    {
        List<Combatant> taunters = new List<Combatant>();
        for (int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value != p.UUID.Value && AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Taunt"), turnOrderCombatantList[i].player))
                taunters.Add(turnOrderCombatantList[i]);
        }
        return taunters;
    }

    public bool IsThisCombatantsTurn(Combatant c)
    {
        int i = combatTurnMarker;
        if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && c.combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == c.player.UUID.Value)
            return true;
        else if (turnOrderCombatantList[i].combatantType == CombatantType.MONSTER && c.combatantType == CombatantType.MONSTER && turnOrderCombatantList[i].monster.cardName == c.monster.cardName)
            return true;
        return false;
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void ResetCombat()
    {
        monsterTargets = new int[0];
        OnPlayerDealDamage = default;
        OnPlayerTakeDamage = default;
        OnPlayerSpendAbilityCharge = default;
        OnPlayerBeingAttacked = default;

        receivedLoneWolf = false;
        usedHoly = false;
        canUseAttackAbilities = false;
        usedItemThisTurn = false;
        waitUntil = false;
        changingStyle = false;
        fleeingPrevented = false;
        OnPlayerDealDamage = default;
        OnPlayerBeingAttacked = default;
        isMonsterTurn = false;
        isYourTurn = false;
        combatantListSet = false;
        ready = false;
        combatLayout.SetActive(false);
        combatBackground.SetActive(false);
        foreach(GameObject g in playerCards)
        {
            g.GetComponent<UIPlayerCard>().DrawStatusEffects(new List<Effect>());
        }
        PlayManager.Instance.localPlayer.SetValue("IronWill", AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Iron Will")));
    }

    #region Stat Roll
    public void StatRollListener(Action<int> Response)
    {
        StartCoroutine(WaitForStatRoll(Response));
    }

    IEnumerator WaitForStatRoll(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchStatRollResult() != 0);
        Response(FetchStatRollResult());
    }

    public void MakeStatRoll(string statRollType, int statRollValue)
    {
        statRoll.GetComponent<UIStatRoll>().MakeStatRoll(statRollType, statRollValue);
    }

    private int FetchStatRollResult()
    {
        return statRoll.GetComponent<UIStatRoll>().success;
    }
    #endregion

    #region Choice
    public void ChoiceListener(Action<int> Response)
    {
        StartCoroutine(WaitForChoice(Response));
    }

    IEnumerator WaitForChoice(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchChoiceResult() != 0);
        Response(FetchChoiceResult());
    }

    public void MakeChoice(string choice1, string choice2, bool condition1, bool condition2)
    {
        choice.GetComponent<UIChoice>().MakeChoice(choice1, choice2, condition1, condition2);
    }

    public int FetchChoiceResult()
    {
        return choice.GetComponent<UIChoice>().choice;
    }
    #endregion

    #region Defensive Options
    public void DefensiveOptionsListener(Action<int, Player> Response)
    {
        StartCoroutine(WaitForDefensiveOptions(Response));
    }

    IEnumerator WaitForDefensiveOptions(Action<int, Player> Response)
    {
        yield return new WaitUntil(() => FetchDefensiveOptionsResult() != 0);
        Response(FetchDefensiveOptionsResult(), FetchDefensiveOptionsTargetPlayer());
    }

    public void EnableDefensiveOptions()
    {
        defensiveOptions.SetActive(true);
    }

    private int FetchDefensiveOptionsResult()
    {
        return defensiveOptions.GetComponent<UIDefensiveOptions>().resolution;
    }

    private Player FetchDefensiveOptionsTargetPlayer()
    {
        return defensiveOptions.GetComponent<UIDefensiveOptions>().targetPlayer;
    }
    #endregion

    #region Combat Options
    public void CombatOptionsListener(Action<int> Response)
    {
        StartCoroutine(WaitForCombatOptions(Response));
    }

    IEnumerator WaitForCombatOptions(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchCombatOptionsResult() != 0);
        Response(FetchCombatOptionsResult());
    }

    public void EnableCombatOptions()
    {
        combatOptions.SetActive(true);
    }

    private int FetchCombatOptionsResult()
    {
        return combatOptions.GetComponent<UICombatOptions>().resolution;
    }
    #endregion

    #region Attack Roll
    public void AttackRollListener(Action<int, bool> Response)
    {
        StartCoroutine(WaitForAttackRoll(Response));
    }

    IEnumerator WaitForAttackRoll(Action<int, bool> Response)
    {
        yield return new WaitUntil(() => FetchAttackRollResult() != 0);
        Response(FetchAttackRollResult(), FetchAttackRollCritResult());
    }

    public void MakeAttackRoll(int playerPower, int monsterPower, string abilityName = "Attack Roll")
    {
        attackRoll.GetComponent<UIAttackRoll>().MakeAttackRoll(playerPower, monsterPower, abilityName);
    }

    private int FetchAttackRollResult()
    {
        return attackRoll.GetComponent<UIAttackRoll>().success;
    }

    private bool FetchAttackRollCritResult()
    {
        return attackRoll.GetComponent<UIAttackRoll>().crit;
    }
    #endregion

    public bool InCombat()
    {
        return combatLayout.activeInHierarchy;
    }

    public bool UsedItemThisTurn()
    {
        return usedItemThisTurn;
    }

    public void UsedItem()
    {
        usedItemThisTurn = true;
    }

    public void SetCanUseAttackAbilities(bool active)
    {
        canUseAttackAbilities = active;
    }

    public bool CanUseAttackAbilities()
    {
        return canUseAttackAbilities;
    }

    public void UsedAttackAbility()
    {
        canUseAttackAbilities = false;
    }

    public void UseBomb()
    {
        int damage = 5;
        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, damage, () => {
            monster.TakeDamage(5, true);
        }));
        PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(damage);
    }

    public void UseHoly(Player p)
    {
        usedHoly = true;
        HealPlayer(p, PlayManager.Instance.GetLevel(p) * 2);
    }

    public void OnCharacterIsAttacked(Combatant c)
    {
        if(c.combatantType == CombatantType.PLAYER)
        {
            if(AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Battle Roar"),c.player))
            {
                InflictEffect(c, new Effect("Attack Up", isYourTurn ? 2 : 1, 4, true));
                InflictEffect(c, new Effect("Armor Up", isYourTurn ? 2 : 1, 2, true));
            }
        }
    }

    public bool PlayerRequestedTaunt()
    {
        bool requested = false;
        foreach (Player p in PlayManager.Instance.playerList)
        {
            if (p.RequestedTaunt.Value)
                requested = true;
        }
        return requested;
    }
}
