using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdventuresOfOldMultiplayer;
using Unity.Collections;

public class CombatManager : Singleton<CombatManager>
{
    public List<Combatant> turnOrderCombatantList;
    public GameObject combatFadeOverlay;
    public GameObject combatBackground;
    public GameObject combatMainLayout;
    public GameObject enemyCard;
    public GameObject[] playerCards;
    public int combatTurnMarker;
    public MonsterCard monsterCard;
    public Combatant monster;
    
    public float fadeLength = 0.01f;
    public float fadedWaitTime = 0.5f;

    // *** Testing Variables ***
    //public int PLAYER_AMOUNT = 6;
    //public float START_RADIAN;

    public float radius;
    public float originX;
    public float originY;

    private bool ready;
    private bool combatantListSet;

    private void Update()
    {
        if(ready)
            AllignPlayerCards();
    }

    public void LoadIntoCombat()
    {
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
        }
    }

    private void AllignPlayerCards()
    {
        int playerAmount = Mathf.Clamp(turnOrderCombatantList.Count - 1, 1, 6);
        //playerAmount = PLAYER_AMOUNT;
        float theta = Mathf.PI / 5f;
        float startingDegree = 0;
        //startingDegree = START_RADIAN;
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
                playerCards[index].GetComponent<UIPlayerCard>().ActivateTurnMarker(combatTurnMarker == i);
                playerCards[index].GetComponent<UIPlayerCard>().SetVisuals(turnOrderCombatantList[i].player);
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

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void ResetCombat()
    {
        combatantListSet = false;
        ready = false;
        combatMainLayout.SetActive(false);
        combatBackground.SetActive(false);
    }
}
