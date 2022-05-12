using UnityEngine;
using System.Collections.Generic;

public class MonsterAbilityManager : Singleton<MonsterAbilityManager>
{
    private Combatant target;

    public void CallSkill(string objectName, Combatant c)
    {
        target = c;
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
        CombatManager.Instance.MonsterEndTurn();
    }

    private void UglySlime_Passive()
    {
        CombatManager.Instance.OnPlayerDealDamage = (t) => {
            CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 2, 1));
        };
    }
    #endregion

    #region Slime Congregation
    private void SlimeCongregation_Skill()
    {
        CombatManager.Instance.MakeStatRoll("INT", 9);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn, new List<Effect>{new Effect("Eaten", 1)});
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void SlimeCongregation_Passive()
    {
        CombatManager.Instance.OnPlayerDealDamage = (t) => {
            CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 2, 2));
        };
    }
    #endregion

    #region Spider Egg
    private void SpiderEgg_Skill()
    {
        if(CombatManager.Instance.monster.IsUnhatchedSpiderEgg())
        {
            Combatant c = CombatManager.Instance.monster;
            c.Hatch();
            CombatManager.Instance.InflictEffect(c, new Effect("Attack Up", -1, 6));
            CombatManager.Instance.InflictEffect(c, new Effect("Power Up", -1, 1));
        }
        CombatManager.Instance.MonsterEndTurn();
    }

    private void SpiderEgg_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.waitUntil = true;
            CombatManager.Instance.MakeStatRoll("CON", 7);
            CombatManager.Instance.StatRollListener((a) => {
                if(a != 1)
                {
                    CombatManager.Instance.InflictEffect(t, new Effect("Poisoned", -1, 1));
                }
                CombatManager.Instance.waitUntil = false;
            });
        };
    }
    #endregion

    #region Giant Spider
    private void GiantSpider_Skill()
    {
        CombatManager.Instance.MakeStatRoll("DEX", 8);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn, new List<Effect> { new Effect("Enwebbed", 1) });
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void GiantSpider_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.waitUntil = true;
            CombatManager.Instance.MakeStatRoll("CON", 8);
            CombatManager.Instance.StatRollListener((a) => {
                if (a != 1)
                {
                    CombatManager.Instance.InflictEffect(t, new Effect("Poisoned", -1, 2));
                }
                CombatManager.Instance.waitUntil = false;
            });
        };
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
        CombatManager.Instance.OnPlayerBeingAttacked = OnAttacked.PAY_10_GOLD;
    }
    #endregion

    #region Beefy Chad Bandits
    private void BeefyChadBandits_Skill()
    {
        // effect goes here...
    }

    private void BeefyChadBandits_Passive()
    {
        CombatManager.Instance.OnPlayerBeingAttacked = OnAttacked.PAY_20_GOLD;
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
