using UnityEngine;

public class MonsterAbilityManager : Singleton<MonsterAbilityManager>
{
    public void CallSkill(string cardName)
    {
        Invoke(cardName + "_Skill", 0);
    }

    public void CallPassive(string cardName)
    {
        Invoke(cardName + "_Passive", 0);
    }

    #region Beefy Chad Bandits
    private void BeefyChadBandits_Skill()
    {
        // Sigma Steal!
        // effect goes here...
    }

    private void BeefyChadBandits_Passive()
    {
        // Acquire Currency
        // effect goes here...
    }
    #endregion
}
