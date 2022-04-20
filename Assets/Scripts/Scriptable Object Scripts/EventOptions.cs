using UnityEngine;
using AdventuresOfOldMultiplayer;
using System.Collections;

[CreateAssetMenu(fileName = "Event Options", menuName = "Event Options")]
public class EventOptions : ScriptableObject
{
    public void ForkInTheRoad(int option)
    {
        Player p = PlayManager.Instance.localPlayer;
        switch (option)
        {
            case 0:
                p.ForkInTheRoadHelper(p.UUID.Value);
                break;
        }
    }

    public void TrappedWolfCub(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Trapped Wolf Cub"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:
                p.CompleteEncounter(false, p.UUID.Value);
                xp += 2;
                p.DrawLootCards(1, p.UUID.Value, true);
                p.GainXP(xp);
                break;
            case 1:
                PlayManager.Instance.MakeStatRoll("DEX", 9);
                PlayManager.Instance.StatRollListener((a) => {
                    if (a == 1)
                        PlayManager.Instance.ReduceChaos(1);
                    p.CompleteEncounter(true, p.UUID.Value);
                    p.GainXP(xp);
                });
                break;
            case 2:
                p.CompleteEncounter(false, p.UUID.Value);
                p.IncreaseChaos(1);
                xp += 2;
                p.DrawLootCards(1, p.UUID.Value, true);
                p.GainXP(xp);
                break;
        }
    }

    public void TrailOfCandy(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Trail of Candy"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:
                p.SetPosition(new Vector3Int(7, 13, -20));
                p.CompleteEncounter(true, p.UUID.Value);
                p.GainXP(xp);
                break;
            case 1:
                PlayManager.Instance.CallEncounterElement(2);
                break;
            case 2:
                p.CompleteEncounter(true, p.UUID.Value);
                p.GainXP(xp);
                break;
        }
    }

    public void MagicalGemstone(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Magical Gemstone"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:
                p.RestoreAbilityCharges(999);
                p.CompleteEncounter(true, p.UUID.Value);
                p.GainXP(xp);
                break;
            case 1:
                PlayManager.Instance.MakeStatRoll("ENG", 10);
                PlayManager.Instance.StatRollListener((a) => {
                    if (a == 1)
                    {
                        p.RestoreAbilityCharges(1);
                        p.RestoreHealth(5);
                    }
                    else
                    {
                        p.LoseAbilityCharges(1);
                        p.TakeDamage(5);
                    }
                    p.CompleteEncounter(true, p.UUID.Value);
                    p.GainXP(xp);
                });
                break;
        }
    }

    // Needs Discard Card implementation still...
    public void EnchantedLake(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Enchanted Lake"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:
                PlayManager.Instance.MakeChoice("+2 Loot Cards", "+4 XP", true, true);
                PlayManager.Instance.ChoiceListener((a) => {
                    if (a == 1)
                    {
                        p.DrawLootCards(2, p.UUID.Value, true);
                        p.CompleteEncounter(false, p.UUID.Value);
                    }
                    else
                    {
                        xp += 4;
                        p.CompleteEncounter(true, p.UUID.Value);
                    }
                    p.GainXP(xp);
                });
                break;
            case 1:
                PlayManager.Instance.MakeChoice("Health +10 Health", "+2 XP", true, true);
                PlayManager.Instance.ChoiceListener((a) => {
                    if (a == 1)
                    {
                        p.RestoreHealth(10);
                    }
                    else
                    {
                        xp += 2; 
                    }
                    p.CompleteEncounter(true, p.UUID.Value);
                    p.GainXP(xp);
                });
                break;
            case 2:
                PlayManager.Instance.MakeChoice("Take 15 Damage", "Discard 1 Card", true, InventoryManager.Instance.HasCardInInventory(p));
                PlayManager.Instance.ChoiceListener((a) => {
                    if (a == 1)
                    {
                        p.TakeDamage(15);
                    }
                    else
                    {
                        // Discard 1 card
                    }
                    p.CompleteEncounter(true, p.UUID.Value);
                    p.GainXP(xp);
                });
                break;
        }
    }

    public void AbandonedStatue(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Abandoned Statue"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:
                p.RestoreHealth(999);
                break;
            case 1:
                p.GainGold(80);
                p.IncreaseChaos(1);
                break;
            case 2:
                xp += 4;
                p.IncreaseChaos(2); 
                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    // Left off here: vvv
    public void TravelingMerchant(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Traveling Merchant"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void PayingTheToll(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Paying the Toll"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void ItsATrap(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["It's a Trap!"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void SomethingShiny(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Something Shiny"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void SpiderNest(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Spider Nest"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void MysteriousMushroom(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Mysterious Mushroom"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void WellFromHell(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Well from Hell!"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void DyingDeer(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Dying Deer"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }

    public void FairySpirit(int option)
    {
        EventCard e = PlayManager.Instance.encounterReference["Fairy Spirit"] as EventCard;
        Player p = PlayManager.Instance.localPlayer;
        int xp = e.xp;
        switch (option)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
        p.CompleteEncounter(true, p.UUID.Value);
        p.GainXP(xp);
    }
}
