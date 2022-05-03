using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;
using Unity.Collections;
using System;
using Unity.Netcode;

public enum OnAttacked { NONE, PAY_10_GOLD, PAY_20_GOLD };

public class CombatManager : Singleton<CombatManager>
{
    public List<Combatant> turnOrderCombatantList;
    public GameObject combatFadeOverlay;
    public GameObject combatBackground;
    public GameObject combatMainLayout;
    public GameObject statRoll;
    public GameObject defensiveOptions;
    public GameObject enemyCard;
    public GameObject[] playerCards;
    public int combatTurnMarker;
    public MonsterCard monsterCard;
    public Combatant monster;
    public GameObject attackIconPrefab;
    
    public float fadeLength = 0.01f;
    public float fadedWaitTime = 0.5f;
    public float attackFadeLength = 0.004f;
    public float attackFlashLength = 0.5f;

    public float radius;
    public float originX;
    public float originY;
    public bool isYourTurn;
    public bool isMonsterTurn;
    public int[] monsterTargets;

    public Action<Combatant> OnPlayerDealDamage;
    public OnAttacked OnPlayerBeingAttacked;

    private bool ready;
    private bool combatantListSet;

    private void Update()
    {
        if(ready)
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
        PlayManager.Instance.localPlayer.UpdateTurnMarker(combatTurnMarker);

        // Cycle effects
        CycleEffects(c);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void EndBotTurn()
    {
        // Increment turn marker for all players
        combatTurnMarker++;
        PlayManager.Instance.localPlayer.UpdateTurnMarker(combatTurnMarker);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void StartCombatantsTurn()
    {
        if (combatTurnMarker >= turnOrderCombatantList.Count)
            combatTurnMarker = 0;
        if (turnOrderCombatantList[combatTurnMarker].combatantType == CombatantType.PLAYER)
            PlayManager.Instance.localPlayer.StartNextCombatantTurn(combatTurnMarker);
        else
            PlayManager.Instance.localPlayer.StartMonsterTurn(GetMonsterTargets());
    }

    public void TakeTurn()
    {
        // Player takes a turn here...
        Combatant c = GetCombatantFromPlayer(PlayManager.Instance.localPlayer);
        int poisoned = c.IsPoisoned();
        if (poisoned > -1)
        {
            StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(c), poisoned, () => {
                c.TakeDamage(poisoned, true);
            }));

            // Visualize player taking damage for all other players
            PlayManager.Instance.localPlayer.VisualizeTakeDamageForOthers(poisoned);
        }
        if (c.IsEnwebbed())
            EndTurn();
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
        PlayManager.Instance.localPlayer.UpdateTurnMarker(combatTurnMarker);

        // Start next combatant turn
        StartCombatantsTurn();
    }

    public void AttackPlayer(Combatant c, List<Effect> debuffs = default)
    {
        EnableDefensiveOptions();
        DefensiveOptionsListener((a) => {
            if(a == 1)
            {
                // Avoided getting attacked
                MonsterEndTurn();
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
                    },
                    () => {
                        // OnComplete
                        MonsterEndTurn();
                    }));
                }
                else
                {
                    // Attack animation + effect + end monster turn
                    StartCoroutine(AnimatePlayerAttacked(c,
                    () => {
                        // OnAttack
                        c.TakeDamage(monster.GetAttack());
                        foreach (Effect e in debuffs)
                            InflictEffect(c, e);
                    },
                    () => {
                        // OnComplete
                        MonsterEndTurn();
                    }));
                }

                // Visualize player attack for all other players
                PlayManager.Instance.localPlayer.VisualizeAttackForOthers();
            }
        });
    }

    public void InflictEffect(Combatant c, Effect e)
    {
        c.player.GainStatusEffect(e.name, e.duration, e.potency);
    }

    public void CleanseEffect(Combatant c, string effectName)
    {
        c.player.RemoveStatusEffect(effectName);
    }

    public void CycleEffects(Combatant c)
    {
        c.player.CycleStatusEffects();
    }

    // Called from Player.cs
    public void GainStatusEffect(Player p, Effect e)
    {
        GetCombatantFromPlayer(p).GainStatusEffect(e);
    }
    // Called from Player.cs
    public void RemoveStatusEffect(Player p, string effectName)
    {
        GetCombatantFromPlayer(p).RemoveStatusEffect(effectName);
    }
    // Called from Player.cs
    public void CycleStatusEffects(Player p)
    {
        GetCombatantFromPlayer(p).CycleStatusEffects();
    }

    public void VisualizePlayerAttacked(Player p)
    {
        StartCoroutine(AnimatePlayerAttacked(GetCombatantFromPlayer(p)));
    }

    public void VisualizePlayerTakeDamage(Player p, int amount)
    {
        StartCoroutine(AnimatePlayerTakeDamage(GetPlayerCardFromCombatant(GetCombatantFromPlayer(p)), amount));
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
        int playerAmount = Mathf.Clamp(turnOrderCombatantList.Count - 1, 1, 6);
        float theta = Mathf.PI / 5f;
        float startingDegree = 0;
        float start = (6 - playerAmount) / 2f;
        int i;
        for(i = 0; i < playerAmount; i++)
        {
            playerCards[i].SetActive(true);
            playerCards[i].transform.localPosition = new Vector3(radius * Mathf.Cos(startingDegree - theta*(i+start)) + originX, radius * Mathf.Sin(startingDegree - theta * (i+start)) + originY, 0);
        }
        for(; i < 6; i++)
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
                enemyCard.GetComponent<UIEncounterCard>().ActivateTurnMarker(combatTurnMarker == i);
                enemyCard.GetComponent<UIEncounterCard>().SetVisuals(turnOrderCombatantList[i].monster.cardName);
                enemyCard.GetComponent<UIEncounterCard>().UpdateHealthBar(turnOrderCombatantList[i]);
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
        combatMainLayout.SetActive(true);
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

    IEnumerator AnimatePlayerAttacked(Combatant target, Action OnAttack = default, Action OnComplete = default)
    {
        // Create attack icon at midpoint between enemy and target player
        GameObject attackIcon = Instantiate(attackIconPrefab, transform);
        UIPlayerCard playerCard = GetPlayerCardFromCombatant(target);
        attackIcon.transform.localPosition = enemyCard.transform.localPosition + (playerCard.transform.localPosition - enemyCard.transform.localPosition) / 2;

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

    public bool IsCombatant(Player p)
    {
        for(int i = 0; i < turnOrderCombatantList.Count; i++)
        {
            if (turnOrderCombatantList[i].combatantType == CombatantType.PLAYER && turnOrderCombatantList[i].player.UUID.Value == p.UUID.Value)
                return true;
        }
        return false;
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
        OnPlayerDealDamage = default;
        OnPlayerBeingAttacked = default;
        isMonsterTurn = false;
        isYourTurn = false;
        combatantListSet = false;
        ready = false;
        combatMainLayout.SetActive(false);
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
}
