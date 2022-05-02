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

    public int GetArmor()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetArmor(player);
        else
            return 0;
    }

    public int GetAttack()
    {
        if (combatantType == CombatantType.PLAYER)
            return PlayManager.Instance.GetAttack(player);
        else
            return monster.attack + PlayManager.Instance.AttackModifier();
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

    public void TakeDamage(int amount)
    {
        if (combatantType == CombatantType.PLAYER)
            player.TakeDamage(amount);
        else
        {
            currentHealth -= amount;
            player.UpdateMonsterHealth(currentHealth);
        }
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = value;
    }
}
