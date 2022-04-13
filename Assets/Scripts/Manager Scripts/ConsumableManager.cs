using UnityEngine;

public class ConsumableManager : Singleton<ConsumableManager>
{
    public void CallEffect(string objectName)
    {
        Invoke("Use_" + objectName, 0);
    }

    private void Use_Bomb()
    {
        // effect goes here...
    }

    private void Use_EnergyPotion()
    {
        // effect goes here...
    }

    private void Use_HealthPotion()
    {
        // effect goes here...
    }

    private void Use_ScrollOfResurrection()
    {
        // effect goes here...
    }

    private void Use_ScrollOfTeleportation()
    {
        // effect goes here...
    }

    private void Use_Torch()
    {
        // effect goes here...
    }
}
