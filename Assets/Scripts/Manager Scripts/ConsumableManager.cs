using UnityEngine;

public class ConsumableManager : Singleton<ConsumableManager>
{
    public void CallEffect(string objectName)
    {
        Invoke("Use_" + objectName, 0);
    }

    private void Use_HealthPotion()
    {
        // effect goes here...
    }
}
