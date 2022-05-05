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
    public GameObject attackRoll;
    public GameObject defensiveOptions;
    public GameObject combatOptions;
    public UIEncounterCard enemyCard;
    public GameObject[] playerCards;
    public int combatTurnMarker;
    public MonsterCard monsterCard;
    public Combatant monster;
    public GameObject attackIconPrefab;
    
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
    public int[] monsterTargets;
    public bool fleeingPrevented;
    public CombatLayoutStyle combatLayoutStyle;
    public int attackerId;

    public Action<Combatant> OnPlayerDealDamage;
    public OnAttacked OnPlayerBeingAttacked;

    private bool ready;
    private bool combatantListSet;
    private bool changingStyle;

    private void Update()
    {
        if(ready && !changingStyle && CombatOverCheck() == -1)
            AllignPlayerCards();
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
            PlayManager.Instance.localPlayer.SetTurnOrderCombatantList(arr);
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

                    // If the current turn player is a combatant, and they are alive, give them their rewards
                    if(IsCombatant(currentTurnPlayer) && GetCombatantFromPlayer(currentTurnPlayer).IsAlive())
                    {
                        switch(monsterCard.type)
                        {
                            case MonsterType.BASIC:
                                currentTurnPlayer.DrawLootCards(1, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(3);
                                currentTurnPlayer.GainGold(10);
                                break;
                            case MonsterType.ELITE:
                                currentTurnPlayer.DrawLootCards(2, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(6);
                                currentTurnPlayer.GainGold(20);
                                break;
                            case MonsterType.MINIBOSS:
                                currentTurnPlayer.DrawLootCards(1, currentTurnPlayer.UUID.Value, true);
                                currentTurnPlayer.GainXP(6);
                                currentTurnPlayer.GainGold(20);
                                break;
                            case MonsterType.BOSS:
                                // Win the game
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
                                chosenOne.DrawLootCards(1, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(3, true);
                                chosenOne.GainGold(10);
                                break;
                            case MonsterType.ELITE:
                                chosenOne.DrawLootCards(2, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(6, true);
                                chosenOne.GainGold(20);
                                break;
                            case MonsterType.MINIBOSS:
                                chosenOne.DrawLootCards(1, chosenOne.UUID.Value, false);
                                chosenOne.GainXP(6, true);
                                chosenOne.GainGold(20);
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
                                p.DrawLootCards(1, p.UUID.Value, false);
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
                StartCoroutine(AnimateMonsterTakeDamage(enemyCard, burning, () => {
                    monster.TakeDamage(burning);
                }));

                // Visualize player taking damage for all other players
                PlayManager.Instance.localPlayer.VisualizeMonsterTakeDamageForOthers(burning);

                // Check if still alive with incoming poison damage
                if (monster.GetHealth() <= burning)
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
            StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(c), poisoned, () => {
                c.TakeDamage(poisoned, true);
            }));

            // Visualize player taking damage for all other players
            PlayManager.Instance.localPlayer.VisualizeTakeDamageForOthers(poisoned);

            // Check if still alive with incoming poison damage
            if (c.GetHealth() <= poisoned)
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
                        else
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
                    });
                }));
                PlayManager.Instance.localPlayer.TransitionOthersToStyle(CombatLayoutStyle.ATTACKING, attackerId);
            }
            else if (a == 2)
            {
                // Successfully fled
                RemoveFromCombat(c, isYourTurn);
            }
            else
            {
                // Attack Ability
            }
        });
    }

    public void AttackMonster(Combatant c, int damage, List<Effect> debuffs = default)
    {
        // Attack animation + effect + end monster turn
        StartCoroutine(AnimateMonsterAttacked(c, damage,() => {
            // OnAttack
            monster.TakeDamage(damage);
            if(debuffs != default)
            {
                foreach (Effect e in debuffs)
                    InflictEffect(c, e);
            }
            OnPlayerDealDamage(c);
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
            if ((turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == c.player.UUID.Value) || (turnOrderCombatantList[i].combatantType == CombatantType.MONSTER && turnOrderCombatantList[i].monster.cardName == c.monster.cardName))
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
        PlayManager.Instance.localPlayer.SetTurnOrderCombatantList(arr);

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
        EnableDefensiveOptions();
        DefensiveOptionsListener((a) => {
            if(a == 1)
            {
                // Avoided getting attacked
                OnComplete();
            }
            else
            {
                // Got attacked
                if(c.GetArmor() >= monster.GetAttack())
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
                    }, OnComplete));
                }

                // Visualize player attack for all other players
                PlayManager.Instance.localPlayer.VisualizeAttackForOthers();
            }
        });
    }

    public void InflictEffect(Combatant c, Effect e)
    {
        if (c.combatantType == CombatantType.PLAYER)
            c.player.GainStatusEffect(e.name, e.duration, e.potency);
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
                    playerCards[index].GetComponent<UIPlayerCard>().ActivateCrosshair(IsTargetedByMonster(turnOrderCombatantList[i].player) && turnOrderCombatantList[combatTurnMarker].monster != null && !isMonsterTurn);
                    playerCards[index].GetComponent<UIPlayerCard>().ActivateTurnMarker(combatTurnMarker == i);
                    playerCards[index].GetComponent<UIPlayerCard>().SetVisuals(turnOrderCombatantList[i]);
                    index--;
                }
                else
                {
                    enemyCard.ActivateTurnMarker(combatTurnMarker == i);
                    enemyCard.SetVisuals(turnOrderCombatantList[i].monster.cardName);
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
                    playerCards[index].GetComponent<UIPlayerCard>().ActivateCrosshair(IsTargetedByMonster(turnOrderCombatantList[i].player) && turnOrderCombatantList[combatTurnMarker].monster != null && !isMonsterTurn && CombatOverCheck() == -1);
                    playerCards[index].GetComponent<UIPlayerCard>().ActivateTurnMarker(combatTurnMarker == i);
                    playerCards[index].GetComponent<UIPlayerCard>().SetVisuals(turnOrderCombatantList[i]);
                    index--;
                }
                else
                {
                    enemyCard.ActivateTurnMarker(combatTurnMarker == i);
                    enemyCard.SetVisuals(turnOrderCombatantList[i].monster.cardName);
                    enemyCard.UpdateHealthBar(turnOrderCombatantList[i]);
                    enemyCard.SetDisplayPosition(new Vector3(470, 0, 0));
                }
            }
        }
    }

    IEnumerator FadeOverlay()
    {
        combatFadeOverlay.SetActive(true);

        // First fade in overlay
        for(int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        // Activate main layout while screen is obstructed by overlay
        combatLayout.SetActive(true);
        combatBackground.SetActive(true);

        yield return new WaitUntil(() => combatantListSet);
        combatBackground.GetComponent<Image>().sprite = monsterCard.background;

        // Hold faded in overlay
        yield return new WaitForSeconds(fadedWaitTime);
        ready = true;

        // Then fade out overlay
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
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
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        // Deactivate main layout while screen is obstructed by overlay
        combatLayout.SetActive(false);
        combatBackground.SetActive(false);

        // Hold faded in overlay
        yield return new WaitForSeconds(fadedWaitTime);

        // Then fade out overlay
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(combatFadeOverlay.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod);
        }

        combatFadeOverlay.SetActive(false);

        // Call OnComplete
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
            yield return new WaitForSeconds(layoutChangeLength * Global.animTimeMod);
        }

        // Update current style
        combatLayoutStyle = targetLayoutStyle;
        changingStyle = false;

        // Call OnComplete
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
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod);
        }

        // Animate take damage
        StartCoroutine(AnimatePlayerTakeDamage(playerCard, monster.GetAttack() - target.GetArmor(), OnAttack));
        yield return new WaitForSeconds(attackFlashLength);

        // Fade out attack icon
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod);
        }

        // Destroy attackIcon
        Destroy(attackIcon);

        // Call OnComplete function
        OnComplete();
    }

    IEnumerator AnimatePlayerTakeDamage(UIPlayerCard playerCard, int damage, Action OnAttack = default)
    {
        // Call OnAttack function
        OnAttack();

        playerCard.ActivateDamaged(true);
        playerCard.DisplayDamageNumber(damage);
        yield return new WaitForSeconds(attackFlashLength);
        playerCard.ActivateDamaged(false);
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
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod);
        }

        // Animate take damage
        StartCoroutine(AnimateMonsterTakeDamage(enemyCard, damage, OnAttack));
        yield return new WaitForSeconds(attackFlashLength);

        // Fade out attack icon
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(attackIcon.GetComponent<Image>(), i * Global.animRate);
            yield return new WaitForSeconds(attackFadeLength * Global.animTimeMod);
        }

        // Destroy attackIcon
        Destroy(attackIcon);

        // Call OnComplete function
        OnComplete();
    }

    IEnumerator AnimateMonsterTakeDamage(UIEncounterCard card, int damage, Action OnAttack = default)
    {
        // Call OnAttack function
        OnAttack();

        card.ActivateDamaged(true);
        card.DisplayDamageNumber(damage);
        yield return new WaitForSeconds(attackFlashLength);
        card.ActivateDamaged(false);
    }

    public void SetTurnOrderCombatantList(FixedString64Bytes[] arr)
    {
        turnOrderCombatantList = new List<Combatant>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (PlayManager.Instance.encounterReference.ContainsKey(arr[i] + ""))
            {
                monsterCard = PlayManager.Instance.encounterReference[arr[i] + ""] as MonsterCard;
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
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value != p.UUID.Value && PlayManager.Instance.MeetsTauntRequirement(turnOrderCombatantList[i].player))
                taunters.Add(turnOrderCombatantList[i]);
        }
        return taunters;
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void ResetCombat()
    {
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

    #region Defensive Options
    public void DefensiveOptionsListener(Action<int> Response)
    {
        StartCoroutine(WaitForDefensiveOptions(Response));
    }

    IEnumerator WaitForDefensiveOptions(Action<int> Response)
    {
        yield return new WaitUntil(() => FetchDefensiveOptionsResult() != 0);
        Response(FetchDefensiveOptionsResult());
    }

    public void EnableDefensiveOptions()
    {
        defensiveOptions.SetActive(true);
    }

    private int FetchDefensiveOptionsResult()
    {
        return defensiveOptions.GetComponent<UIDefensiveOptions>().resolution;
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
}
