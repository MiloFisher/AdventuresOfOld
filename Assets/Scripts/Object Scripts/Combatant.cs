using System;
using System.Collections.Generic;
using AdventuresOfOldMultiplayer;

public enum CombatantType { PLAYER, MONSTER };

[Serializable]
public class Combatant
{
    public CombatantType combatantType;
    public Player player;
    public MonsterCard monster;
    public List<Effect> statusEffects;

    private int currentHealth;

    public Combatant(CombatantType combatantType, Player player)
    {
        this.combatantType = combatantType;
        this.player = player;
        statusEffects = new List<Effect>();
        monster = default;
    }

    public Combatant(CombatantType combatantType, MonsterCard monster)
    {
        this.combatantType = combatantType;
        this.monster = monster;
        player = default;
        statusEffects = new List<Effect>();
        currentHealth = monster.health + PlayManager.Instance.HealthModifier();
    }

    public string GetName()
    {
        if (combatantType == CombatantType.PLAYER)
            return player.Name.Value + "";
        else
            return monster.cardName;
    }

    public string GetColor()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetPlayerColorString(player);
        else
            return "white";
    }

    public int GetHealth()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetHealth(player);
        else
            return currentHealth;
    }

    public int GetMaxHealth()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetMaxHealth(player);
        else
            return monster.health + PlayManager.Instance.HealthModifier();
    }

    public bool IsAlive()
    {
        return GetHealth() > 0;
    }

    public int GetArmor()
    {
        int armor;

        if (combatantType == CombatantType.PLAYER)
            armor = PlayManager.Instance.GetArmor(player);
        else
            armor = 0;

        int weakened = IsWeakened();
        if(weakened > -1)
        {
            armor -= weakened;
            if (armor < 0)
                armor = 0;
        }

        return armor;
    }

    public int GetAttack()
    {
        int attack;

        if (combatantType == CombatantType.PLAYER)
            attack = PlayManager.Instance.GetAttack(player);
        else
            attack = monster.attack + PlayManager.Instance.AttackModifier();

        if (IsEaten())
            attack = HalfRoundedUp(attack);

        return attack;
    }

    public int GetSpeed()
    {
        if(combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetSpeed(player);
        else
            return monster.speed;
    }

    public int GetPhysicalPower()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetPhysicalPower(player);
        else
            return monster.physicalPower + PlayManager.Instance.PowerModifier();
    }

    public int GetMagicalPower()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetMagicalPower(player);
        else
            return monster.magicalPower + PlayManager.Instance.PowerModifier();
    }

    public void TakeDamage(int amount, bool isTrue = false)
    {
        if (combatantType == CombatantType.PLAYER)
            player.TakeDamage(amount, GetArmor(), isTrue);
        else
        {
            currentHealth -= amount;
            if (currentHealth < 0)
                currentHealth = 0;
            PlayManager.Instance.localPlayer.UpdateMonsterHealth(currentHealth);
        }
    }

    public void RestoreHealth(int amount)
    {
        if (IsPlagued())
            return;

        if (combatantType == CombatantType.PLAYER)
            player.RestoreHealth(amount);
        else
        {
            currentHealth += amount;
            if (currentHealth > GetMaxHealth())
                currentHealth = GetMaxHealth();
            player.UpdateMonsterHealth(currentHealth);
        }
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = value;
    }

    public void GainStatusEffect(Effect e)
    {
        bool alreadyContains = false;
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if(statusEffects[i].name == e.name)
            {
                alreadyContains = true;
                if (e.potency > statusEffects[i].potency || (e.potency == statusEffects[i].potency && e.duration > statusEffects[i].duration))
                    statusEffects[i] = e;
                break;
            }
        }
        if(!alreadyContains)
            statusEffects.Add(e);

        if(combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
    }

    public void RemoveStatusEffect(string effectName)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].name == effectName)
            {
                statusEffects.RemoveAt(i);
                break;
            }
        }

        if (combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
    }

    public void CycleStatusEffects()
    {
        // To make a status effect not expire, create it with negative duration
        for (int i = 0; i < statusEffects.Count; i++)
        {
            statusEffects[i].duration--;
            if (statusEffects[i].duration == 0)
            {
                statusEffects.RemoveAt(i);
                i--;
            }
        }

        if (combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
    }

    private int HalfRoundedUp(int x)
    {
        return (int)MathF.Ceiling(x / 2f);
    }

    private int HalfRoundedDown(int x)
    {
        return (int)MathF.Floor(x / 2f);
    }

    private int HasEffect(string effectName)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].name == effectName)
                return statusEffects[i].potency;
        }
        return -1;
    }

    public bool IsEaten()
    {
        return HasEffect("Eaten") > -1;
    }

    public bool IsEnwebbed()
    {
        return HasEffect("Enwebbed") > -1;
    }

    public bool IsPlagued()
    {
        return HasEffect("Plagued") > -1;
    }

    public int IsPoisoned()
    {
        return HasEffect("Poisoned");
    }

    public int IsWeakened()
    {
        return HasEffect("Weakened");
    }

    // Need to implement functionality still vvv

    public int IsBleeding()
    {
        return HasEffect("Bleeding");
    }

    public int IsBurning()
    {
        return HasEffect("Burning");
    }

    public bool IsDazed()
    {
        return HasEffect("Dazed") > -1;
    }
}
