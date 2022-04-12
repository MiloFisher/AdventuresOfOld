using UnityEngine;

public class MonsterAbilityManager : Singleton<MonsterAbilityManager>
{
    public void CallSkill(string objectName)
    {
        Invoke(objectName + "_Skill", 0);
    }

    public void CallPassive(string objectName)
    {
        Invoke(objectName + "_Passive", 0);
    }

    // Encounter Monsters

    #region Ugly Slime
    private void UglySlime_Skill()
    {
        // effect goes here...
    }

    private void UglySlime_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Slime Congregation
    private void SlimeCongregation_Skill()
    {
        // effect goes here...
    }

    private void SlimeCongregation_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Spider Egg
    private void SpiderEgg_Skill()
    {
        // effect goes here...
    }

    private void SpiderEgg_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Giant Spider
    private void GiantSpider_Skill()
    {
        // effect goes here...
    }

    private void GiantSpider_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Crazy Wolf
    private void CrazyWolf_Skill()
    {
        // effect goes here...
    }

    private void CrazyWolf_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Ravenous Wolfpack
    private void RavenousWolfpack_Skill()
    {
        // effect goes here...
    }

    private void RavenousWolfpack_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Giant Rat
    private void GiantRat_Skill()
    {
        // effect goes here...
    }

    private void GiantRat_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Giant Fiery Rat
    private void GiantFieryRat_Skill()
    {
        // effect goes here...
    }

    private void GiantFieryRat_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Shy Unfriendly Bandits
    private void ShyUnfriendlyBandits_Skill()
    {
        // effect goes here...
    }

    private void ShyUnfriendlyBandits_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Beefy Chad Bandits
    private void BeefyChadBandits_Skill()
    {
        // effect goes here...
    }

    private void BeefyChadBandits_Passive()
    {
        // effect goes here...
    }
    #endregion

    // Mini-boss Monsters

    #region Discord Kitten
    private void DiscordKitten_Skill()
    {
        // effect goes here...
    }

    private void DiscordKitten_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Raging Discord Kitten
    private void RagingDiscordKitten_Skill()
    {
        // effect goes here...
    }

    private void RagingDiscordKitten_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Rainbow Slime
    private void RainbowSlime_Skill()
    {
        // effect goes here...
    }

    private void RainbowSlime_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Bandit Weeb Lord
    private void BanditWeebLord_Skill()
    {
        // effect goes here...
    }

    private void BanditWeebLord_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Spooky Spider
    private void SpookySpider_Skill()
    {
        // effect goes here...
    }

    private void SpookySpider_Passive()
    {
        // effect goes here...
    }
    #endregion

    #region Goblin Horde
    private void GoblinHorde_Skill()
    {
        // effect goes here...
    }

    private void GoblinHorde_Passive()
    {
        // effect goes here...
    }
    #endregion

    // Boss Monsters

    #region Corrupted Tree Spirit
    private void CorruptedTreeSpirit_Skill()
    {
        // effect goes here...
    }

    private void CorruptedTreeSpirit_Passive()
    {
        // effect goes here...
    }
    #endregion

    // Minion Monsters

    #region Undead Minion
    private void UndeadMinion_Skill()
    {
        // effect goes here...
    }

    private void UndeadMinion_Passive()
    {
        // effect goes here...
    }
    #endregion
}
