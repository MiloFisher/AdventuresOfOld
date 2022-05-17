using UnityEngine;

public class ConsumableManager : Singleton<ConsumableManager>
{
    public void CallEffect(string objectName)
    {
        Invoke("Use_" + objectName, 0);
    }

    private void Use_Bomb()
    {
        CombatManager.Instance.UseBomb();
        InventoryManager.Instance.Discard();
        if (CombatManager.Instance.InCombat())
            CombatManager.Instance.UsedItem();
    }

    private void Use_EnergyPotion()
    {
        PlayManager.Instance.localPlayer.RestoreAbilityCharges(2);
        InventoryManager.Instance.Discard();
        if (CombatManager.Instance.InCombat())
            CombatManager.Instance.UsedItem();
    }

    private void Use_HealthPotion()
    {
        PlayManager.Instance.localPlayer.RestoreHealth(10);
        InventoryManager.Instance.Discard();
        if (CombatManager.Instance.InCombat())
            CombatManager.Instance.UsedItem();
    }

    private void Use_ScrollOfResurrection()
    {
        PlayManager.Instance.TargetPlayerSelection("Choose Player to Revive", true, true, false, (p) => {
            // Target player is revived and this player discards the card
            p.Resurrect();
            InventoryManager.Instance.Discard();
        }, (p) => {
            // Requirement is that player is dead
            return PlayManager.Instance.GetHealth(p) <= 0;
        }, true, "Cancel", () => {
            InventoryManager.Instance.HideOptions();
        });
    }

    private void Use_ScrollOfTeleportation()
    {
        PlayManager.Instance.TargetPlayerSelection("Choose Destination Player", true, false, false, (p) => {
            // This player teleports to target player and this player discards the card
            PlayManager.Instance.localPlayer.SetPosition(p.Position.Value);
            if(PlayManager.Instance.movePhase)
            {
                PlayManager.Instance.HideAllTiles();
                PlayManager.Instance.MovePhase(p.Position.Value);
            }
            InventoryManager.Instance.Discard();
        }, (p) => {
            // No requirement
            return true;
        }, true, "Cancel", () => {
            InventoryManager.Instance.HideOptions();
        });
    }

    private void Use_Torch()
    {
        PlayManager.Instance.GetEncounter();
        InventoryManager.Instance.Discard();
    }
}
