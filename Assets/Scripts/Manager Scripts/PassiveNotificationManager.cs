using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassiveNotificationManager : Singleton<PassiveNotificationManager>
{
    public TMP_Text notificationDisplay;

    private class PassiveNotification
    {
        public string message;
        public float maxLifetime;
        public float currentLifetime;

        public PassiveNotification(string message, float maxLifetime)
        {
            this.message = message;
            this.maxLifetime = maxLifetime;
            currentLifetime = 0;
        }
    }

    private List<PassiveNotification> notifications = new List<PassiveNotification>();

    private void Update()
    {
        notificationDisplay.text = "";
        for (int i = 0; i < notifications.Count; i++)
        {
            float t = Time.deltaTime;
            PassiveNotification n = notifications[i];
            n.currentLifetime += t;
            if (n.currentLifetime >= n.maxLifetime)
            {
                notifications.RemoveAt(i);
                i--;
            }
            else
            {
                notificationDisplay.text += n.message + "\n";
            }
        }
    }

    public void AddNotification(string notification)
    {
        notifications.Add(new PassiveNotification(notification, 3f));
    }
}
