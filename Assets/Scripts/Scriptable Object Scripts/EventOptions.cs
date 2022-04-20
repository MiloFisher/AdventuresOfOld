using UnityEngine;
using AdventuresOfOldMultiplayer;
using System.Collections;

[CreateAssetMenu(fileName = "Event Options", menuName = "Event Options")]
public class EventOptions : ScriptableObject
{
    #region Fork in the Road
    public void ForkInTheRoad_Option_0()
    {
        PlayManager.Instance.localPlayer.ForkInTheRoadHelper(PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Trapped Wolf Cub
    public void TrappedWolfCub_Option_0()
    {
        Player p = PlayManager.Instance.localPlayer;
        p.CompleteEncounter(false, p.UUID.Value);
        p.GainXP(2);
        p.DrawLootCards(1, p.UUID.Value, true);
    }

    public void TrappedWolfCub_Option_1()
    {
        PlayManager.Instance.MakeStatRoll("DEX", 9);
        PlayManager.Instance.StatRollListener((a) => {
            if (a == 1)
                PlayManager.Instance.ReduceChaos(1);
            PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
        });
    }

    public void TrappedWolfCub_Option_2()
    {
        Player p = PlayManager.Instance.localPlayer;
        p.CompleteEncounter(false, p.UUID.Value);
        PlayManager.Instance.IncreaseChaos(1);
        p.GainXP(2);
        p.DrawLootCards(1, p.UUID.Value, true);
    }
    #endregion

    #region Trail of Candy
    public void TrailOfCandy_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void TrailOfCandy_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void TrailOfCandy_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Magical Gemstone
    public void MagicalGemstone_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void MagicalGemstone_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Enchanted Lake
    public void EnchantedLake_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void EnchantedLake_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void EnchantedLake_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Abandoned Statue
    public void AbandonedStatue_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void AbandonedStatue_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void AbandonedStatue_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Traveling Merchant
    public void TravelingMerchant_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void TravelingMerchant_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void TravelingMerchant_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Paying the Toll
    public void PayingTheToll_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void PayingTheToll_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region It's a Trap!
    public void ItsATrap_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void ItsATrap_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Something Shiny
    public void SomethingShiny_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void SomethingShiny_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void SomethingShiny_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Spider Nest
    public void SpiderNest_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void SpiderNest_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void SpiderNest_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Mysterious Mushroom
    public void MysteriousMushroom_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void MysteriousMushroom_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void MysteriousMushroom_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Well from Hell!
    public void WellFromHell_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void WellFromHell_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Dying Deer
    public void DyingDeer_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void DyingDeer_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void DyingDeer_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Fairy Spirit
    public void FairySpirit_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void FairySpirit_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    public void FairySpirit_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion
}
