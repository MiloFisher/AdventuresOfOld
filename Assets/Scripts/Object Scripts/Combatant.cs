using System;
using System.Collections.Generic;
using AdventuresOfOldMultiplayer;
using UnityEngine;

public enum CombatantType { PLAYER, MONSTER, MINION };

[Serializable]
public class Combatant
{
    public CombatantType combatantType;
    public Player player;
    public MonsterCard monster;
    public Combatant minion;
    public List<Effect> statusEffects;

    private int currentHealth = 0;
    private int minionMaxHealth = 0;
    private int minionAttack = 0;
    private int minionPower = 0;
    public bool hasHatched;
    public int startHealth;

    public Combatant(CombatantType combatantType, Player player)
    {
        this.combatantType = combatantType;
        this.player = player;
        minion = default;
        monster = default;
        statusEffects = new List<Effect>();
    }

    public Combatant(CombatantType combatantType, MonsterCard monster)
    {
        this.combatantType = combatantType;
        this.monster = monster;
        minion = default;
        player = default;
        statusEffects = new List<Effect>();
        currentHealth = monster.health + PlayManager.Instance.HealthModifier();
    }

    public Combatant(CombatantType combatantType, Player player, MonsterCard monster, int startHealth = default)
    {
        this.combatantType = combatantType;
        this.player = player;
        this.monster = monster;
        this.startHealth = startHealth;
        statusEffects = new List<Effect>();
        monster.Skill(this);
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
        else if (combatantType == CombatantType.MONSTER)
            return monster.health + PlayManager.Instance.HealthModifier();
        else
            return minionMaxHealth;
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

        int armorUp = HasArmorUp();
        if(armorUp > -1)
            armor += armorUp;

        if (IsCursed())
        {
            armor = HalfRoundedUp(armor);
        }

        int weakened = IsWeakened();
        if (weakened > -1)
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
        else if (combatantType == CombatantType.MONSTER)
            attack = monster.attack + PlayManager.Instance.AttackModifier();
        else
            attack = minionAttack;

        int attackUp = HasAttackUp();
        if (attackUp > -1)
            attack += attackUp;

        if (IsUnhatchedSpiderEgg())
            attack = 0;

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
        int power;

        if (combatantType == CombatantType.PLAYER)
            power = PlayManager.Instance.GetPhysicalPower(player);
        else if (combatantType == CombatantType.MONSTER)
            power = monster.physicalPower + PlayManager.Instance.PowerModifier();
        else
            power = minionPower;

        int flamingShot = HasFlamingShot();
        if (flamingShot > -1)
            power += flamingShot;

        int bonusPower = HasBonusPower();
        if (bonusPower > -1)
            power += bonusPower;

        int powerUp = HasPowerUp();
        if (powerUp > -1)
            power += powerUp;

        int powerDown = HasPowerDown();
        if (powerDown > -1)
        {
            power -= powerDown;
            if (power < 0)
                power = 0;
        }

        return power;
    }

    public int GetMagicalPower()
    {
        int power;

        if (combatantType == CombatantType.PLAYER)
            power = PlayManager.Instance.GetMagicalPower(player);
        else if (combatantType == CombatantType.MONSTER)
            power = monster.magicalPower + PlayManager.Instance.PowerModifier();
        else
            power = 0;

        int flamingShot = HasFlamingShot();
        if (flamingShot > -1)
            power += flamingShot;

        int bonusPower = HasBonusPower();
        if (bonusPower > -1)
            power += bonusPower;

        int powerUp = HasPowerUp();
        if (powerUp > -1)
            power += powerUp;

        int powerDown = HasPowerDown();
        if (powerDown > -1)
        {
            power -= powerDown;
            if (power < 0)
                power = 0;
        }

        return power;
    }

    public void TakeDamage(int amount, bool isTrue = false)
    {
        if (combatantType == CombatantType.PLAYER)
        {
            if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Berserk"), player))
                CombatManager.Instance.InflictEffect(this, new Effect("Attack Up", -1, 2, true));

            player.TakeDamage(amount, GetArmor(), isTrue);
        }
        else if (combatantType == CombatantType.MONSTER)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                if(HasPowerFantasy())
                {
                    CombatManager.Instance.CleanseEffect(this, "Power Fantasy");
                    CombatManager.Instance.InflictEffect(this, new Effect("Power Up", -1, 1));
                    currentHealth = 1;
                }
                else
                    currentHealth = 0;
            }
            PlayManager.Instance.localPlayer.UpdateMonsterHealth(currentHealth);
        }
        else
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
                currentHealth = 0;
            CombatManager.Instance.UpdateMinion(player, GetHealth(), GetMaxHealth(), GetAttack(), GetPhysicalPower());
        }
    }

    public void RestoreHealth(int amount)
    {
        if (IsPlagued())
            return;

        if (combatantType == CombatantType.PLAYER)
        {
            if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Angelic Wrath"), player))
                CombatManager.Instance.InflictEffect(this, new Effect("Attack Up", -1, 2, true));

            player.RestoreHealth(amount);
        }
        else
        {
            currentHealth += amount;
            if (currentHealth > GetMaxHealth())
                currentHealth = GetMaxHealth();
            PlayManager.Instance.localPlayer.UpdateMonsterHealth(currentHealth);
        }
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = value;
    }

    public void SetMinionMaxHealth(int value)
    {
        minionMaxHealth = value;
    }

    public void SetMinionAttack(int value)
    {
        minionAttack = value;
    }

    public void SetMinionPower(int value)
    {
        minionPower = value;
    }

    public void GainStatusEffect(Effect e)
    {
        bool alreadyContains = false;
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if(statusEffects[i].name == e.name && !e.canStack)
            {
                alreadyContains = true;
                if (e.potency > statusEffects[i].potency || (e.potency == statusEffects[i].potency && e.duration > statusEffects[i].duration && e.counter >= statusEffects[i].counter))
                    statusEffects[i] = e;
                break;
            }
        }
        if(!alreadyContains)
            statusEffects.Add(e);

        if (combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
        else if (combatantType == CombatantType.MONSTER)
            CombatManager.Instance.enemyCard.DrawStatusEffects(statusEffects);
        else
            CombatManager.Instance.GetPlayerCardFromCombatant(CombatManager.Instance.GetCombatantFromPlayer(player)).DrawStatusEffects(statusEffects, true);
    }

    public void RemoveStatusEffect(string effectName)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].name == effectName)
            {
                statusEffects.RemoveAt(i);
                i--;
            }
        }

        if (combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
        else if (combatantType == CombatantType.MONSTER)
            CombatManager.Instance.enemyCard.DrawStatusEffects(statusEffects);
        else
            CombatManager.Instance.GetPlayerCardFromCombatant(CombatManager.Instance.GetCombatantFromPlayer(player)).DrawStatusEffects(statusEffects, true);
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
        else if (combatantType == CombatantType.MONSTER)
            CombatManager.Instance.enemyCard.DrawStatusEffects(statusEffects);
        else
            CombatManager.Instance.GetPlayerCardFromCombatant(CombatManager.Instance.GetCombatantFromPlayer(player)).DrawStatusEffects(statusEffects, true);
    }

    public void UseStatusEffect(string effectName)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].name == effectName)
            {
                statusEffects[i].counter--;
                if(statusEffects[i].counter <= 0)
                {
                    statusEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        if (combatantType == CombatantType.PLAYER)
            CombatManager.Instance.GetPlayerCardFromCombatant(this).DrawStatusEffects(statusEffects);
        else if (combatantType == CombatantType.MONSTER)
            CombatManager.Instance.enemyCard.DrawStatusEffects(statusEffects);
        else
            CombatManager.Instance.GetPlayerCardFromCombatant(CombatManager.Instance.GetCombatantFromPlayer(player)).DrawStatusEffects(statusEffects, true);
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
        int total = 0;
        bool canHaveMultipleStacks = false;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].name == effectName)
            {
                if (statusEffects[i].canStack)
                {
                    canHaveMultipleStacks = true;
                    total += statusEffects[i].potency;
                }
                else
                    return statusEffects[i].potency;
            }
        }
        if (canHaveMultipleStacks)
            return total;
        return -1;
    }

    public bool IsUnhatchedSpiderEgg()
    {
        return combatantType == CombatantType.MONSTER && monster.cardName == "Spider Egg" && hasHatched == false;
    }

    public void Hatch()
    {
        hasHatched = true;
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

    // Buffs

    public int HasPowerUp()
    {
        return HasEffect("Power Up");
    }

    public int HasAttackUp()
    {
        return HasEffect("Attack Up");
    }

    public int HasArmorUp()
    {
        return HasEffect("Armor Up");
    }

    // Extra

    public int HasPowerDown()
    {
        return HasEffect("Power Down");
    }

    public bool HasPowerFantasy()
    {
        return HasEffect("Power Fantasy") > -1;
    }

    public bool HasVanish()
    {
        return HasEffect("Vanish") > -1;
    }

    public int HasFlamingShot()
    {
        return HasEffect("Flaming Shot");
    }

    public int HasBonusPower()
    {
        return HasEffect("Bonus Power");
    }

    public bool IsCursed()
    {
        return HasEffect("Cursed") > -1;
    }
}
