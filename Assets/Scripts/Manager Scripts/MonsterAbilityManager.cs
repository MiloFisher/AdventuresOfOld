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
        CombatManager.Instance.MakeStatRoll("STR", 9);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void CrazyWolf_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.HealMonster(2);
        };
    }
    #endregion

    #region Ravenous Wolfpack
    private void RavenousWolfpack_Skill()
    {
        CombatManager.Instance.MakeStatRoll("STR", 10);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void RavenousWolfpack_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.HealMonster(3);
        };
    }
    #endregion

    #region Giant Rat
    private void GiantRat_Skill()
    {
        CombatManager.Instance.MakeStatRoll("DEX", 9);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void GiantRat_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.waitUntil = true;
            CombatManager.Instance.MakeStatRoll("CON", 8);
            CombatManager.Instance.StatRollListener((a) => {
                if (a != 1)
                {
                    if(CombatManager.Instance.IsThisCombatantsTurn(t))
                        CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 2));
                    else
                        CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 1));
                }
                CombatManager.Instance.waitUntil = false;
            });
        };
    }
    #endregion

    #region Giant Fiery Rat
    private void GiantFieryRat_Skill()
    {
        CombatManager.Instance.MakeStatRoll("DEX", 10);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void GiantFieryRat_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.waitUntil = true;
            CombatManager.Instance.MakeStatRoll("CON", 11);
            CombatManager.Instance.StatRollListener((a) => {
                if (a != 1)
                {
                    if (CombatManager.Instance.IsThisCombatantsTurn(t))
                        CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 2));
                    else
                        CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 1));
                }
                CombatManager.Instance.waitUntil = false;
            });
        };
    }
    #endregion

    #region Shy Unfriendly Bandits
    private void ShyUnfriendlyBandits_Skill()
    {
        CombatManager.Instance.MakeStatRoll("INT", 10);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void ShyUnfriendlyBandits_Passive()
    {
        CombatManager.Instance.OnPlayerBeingAttacked = OnAttacked.PAY_10_GOLD;
    }
    #endregion

    #region Beefy Chad Bandits
    private void BeefyChadBandits_Skill()
    {
        CombatManager.Instance.MakeStatRoll("INT", 11);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
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
        CombatManager.Instance.MakeStatRoll("STR", 3);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.InstantKill(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void DiscordKitten_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            if (CombatManager.Instance.IsThisCombatantsTurn(t))
                CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 2, 1));
            else
                CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 1, 1));
        };
    }
    #endregion

    #region Raging Discord Kitten
    private void RagingDiscordKitten_Skill()
    {
        CombatManager.Instance.MakeStatRoll("STR", 11);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void RagingDiscordKitten_Passive()
    {
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            if (CombatManager.Instance.IsThisCombatantsTurn(t))
                CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 2));
            else
                CombatManager.Instance.InflictEffect(t, new Effect("Plagued", 1));
        };
    }
    #endregion

    #region Rainbow Slime
    private void RainbowSlime_Skill()
    {
        CombatManager.Instance.MakeStatRoll("INT", 9);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn, new List<Effect> { new Effect("Eaten", 1) });
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void RainbowSlime_Passive()
    {
        CombatManager.Instance.OnPlayerDealDamage = (t) => {
            CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 2, 3));
        };
    }
    #endregion

    #region Bandit Weeb Lord
    private void BanditWeebLord_Skill()
    {
        CombatManager.Instance.MakeStatRoll("INT", 10);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void BanditWeebLord_Passive()
    {
        if(PlayManager.Instance.isYourTurn)
        {
            CombatManager.Instance.InflictEffect(CombatManager.Instance.monster, new Effect("Power Fantasy", -1));
        }
    }
    #endregion

    #region Spooky Spider
    private void SpookySpider_Skill()
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

    private void SpookySpider_Passive()
    {
        if (PlayManager.Instance.isYourTurn && !PlayManager.Instance.localPlayer.GrabbedHorse.Value)
        {
            CombatManager.Instance.InflictEffect(CombatManager.Instance.monster, new Effect("Power Down", -1, 2));
        }
        CombatManager.Instance.OnPlayerTakeDamage = (t) => {
            CombatManager.Instance.waitUntil = true;
            CombatManager.Instance.MakeStatRoll("CON", 8);
            CombatManager.Instance.StatRollListener((a) => {
                if (a != 1)
                {
                    CombatManager.Instance.InflictEffect(t, new Effect("Poisoned", -1, 3));
                }
                CombatManager.Instance.waitUntil = false;
            });
        };
    }
    #endregion

    #region Goblin Horde
    private void GoblinHorde_Skill()
    {
        CombatManager.Instance.MakeStatRoll("STR", 8);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn);
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void GoblinHorde_Passive()
    {
        CombatManager.Instance.fleeingPrevented = true;
    }
    #endregion

    // Boss Monsters

    #region Corrupted Tree Spirit
    private void CorruptedTreeSpirit_Skill()
    {
        CombatManager.Instance.MakeStatRoll(PlayManager.Instance.GetPrimaryStat(PlayManager.Instance.localPlayer), 12);
        CombatManager.Instance.StatRollListener((a) => {
            if (a == -1)
            {
                CombatManager.Instance.AttackPlayer(target, CombatManager.Instance.MonsterEndTurn, new List<Effect> { new Effect("Plagued", 1), new Effect("Poisoned", -1, 3) });
            }
            else
                CombatManager.Instance.MonsterEndTurn();
        });
    }

    private void CorruptedTreeSpirit_Passive()
    {
        CombatManager.Instance.OnPlayerSpendAbilityCharge = (t) => {
            if (CombatManager.Instance.IsThisCombatantsTurn(t))
                CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 2, 3));
            else
                CombatManager.Instance.InflictEffect(t, new Effect("Weakened", 1, 3));
        };
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
