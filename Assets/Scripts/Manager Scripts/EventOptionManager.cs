using UnityEngine;

public class EventOptionManager : Singleton<EventOptionManager>
{
    public void CallOption(string objectName, int id)
    {
        Invoke(objectName + "_Option_" + id, 0);
    }

    #region Fork in the Road
    private void ForkInTheRoad_Option_0()
    {
        PlayManager.Instance.localPlayer.ForkInTheRoadHelper(PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Trapped Wolf Cub
    private void TrappedWolfCub_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TrappedWolfCub_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TrappedWolfCub_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Trail of Candy
    private void TrailOfCandy_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TrailOfCandy_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TrailOfCandy_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Magical Gemstone
    private void MagicalGemstone_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void MagicalGemstone_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Enchanted Lake
    private void EnchantedLake_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void EnchantedLake_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void EnchantedLake_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Abandoned Statue
    private void AbandonedStatue_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void AbandonedStatue_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Traveling Merchant
    private void TravelingMerchant_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TravelingMerchant_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void TravelingMerchant_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Paying the Toll
    private void PayingTheToll_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void PayingTheToll_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region It's a Trap!
    private void ItsATrap_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void ItsATrap_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Something Shiny
    private void SomethingShiny_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void SomethingShiny_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void SomethingShiny_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Spider Nest
    private void SpiderNest_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void SpiderNest_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void SpiderNest_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Mysterious Mushroom
    private void MysteriousMushroom_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void MysteriousMushroom_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void MysteriousMushroom_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Well from Hell!
    private void WellFromHell_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void WellFromHell_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Dying Deer
    private void DyingDeer_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void DyingDeer_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void DyingDeer_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion

    #region Fairy Spirit
    private void FairySpirit_Option_0()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void FairySpirit_Option_1()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }

    private void FairySpirit_Option_2()
    {
        // requirement + effect here...
        PlayManager.Instance.localPlayer.CompleteEncounter(true, PlayManager.Instance.localPlayer.UUID.Value);
    }
    #endregion
}
