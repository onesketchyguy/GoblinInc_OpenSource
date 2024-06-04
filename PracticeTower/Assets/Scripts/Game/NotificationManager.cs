using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace LowEngine
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager instance;

        public Font notificationFont;

        Queue<Button> notifications = new Queue<Button>() { };

        List<string> oneNotifications = new List<string>();

        private void Awake()
        {
            instance = this;
        }

        public void ShowNotificationOnce(string notText)
        {
            if (oneNotifications.Contains(notText)) return;
            oneNotifications.Add(notText);

            CancelInvoke("ClearOldestNotification");
            Button notification = Instantiate(CreateButton(notText), transform);
            notification.name = notText;
            notification.onClick.AddListener(() => ClearNotificationOne(notification.gameObject));
            notifications.Enqueue(notification);
            Invoke("ClearOldestNotification", 5);
        }

        public void ShowNotification(string notText)
        {
            CancelInvoke("ClearOldestNotification");

            Button notification = Instantiate(CreateButton(notText), transform);

            notification.onClick.AddListener(() => ClearNotification(notification.gameObject));

            notifications.Enqueue(notification);

            Invoke("ClearOldestNotification", 5);
        }

        void ClearOldestNotification()
        {
            if (notifications.Count > 0)
            {
                Button go = notifications.Dequeue();

                if (go != null)
                {
                    if (oneNotifications.Contains(go.gameObject.name)) oneNotifications.Remove(go.gameObject.name);
                    Destroy(go.gameObject);
                }

                if (notifications.Count > 0) Invoke("ClearOldestNotification", 5);
            }
            else
            {
                CancelInvoke("ClearOldestNotification");
            }
        }

        void ClearNotificationOne(GameObject obj)
        {
            oneNotifications.Remove(obj.name);
            Destroy(obj);
        }

        void ClearNotification(GameObject obj)
        {
            Destroy(obj);
        }

        Button CreateButton(string textToShow)
        {
            Button b = new GameObject($"{textToShow}").AddComponent<Button>();

            Text t = b.gameObject.AddComponent<Text>();

            t.alignment = TextAnchor.UpperLeft;

            t.font = notificationFont;

            t.color = Color.white;

            t.text = textToShow;

            t.resizeTextForBestFit = true;

            Outline o = t.gameObject.AddComponent<Outline>();

            o.effectColor = Color.black;

            b.targetGraphic = t;

            return b;
        }
    }
}