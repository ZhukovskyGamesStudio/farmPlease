using System;
using System.Collections.Generic;
using Abstract;
using Dialogs;
using Managers;
using Unity.Notifications;
using Unity.Notifications.Android;
using Random = UnityEngine.Random;

public class NotificationsManager : PreloadableSingleton<NotificationsManager> {
    protected override void OnFirstInit() {
        base.OnFirstInit();
        NotificationCenter.Initialize(new NotificationCenterArgs() {
            AndroidChannelId = "energy_channel",
            AndroidChannelDescription = "Alerts when energy is full",
            AndroidChannelName = "Energy Notifications"
        });
        AndroidNotificationCenter.Initialize();
    }

    public void TryShowAskDialog() {
        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.NotRequested &&
            !SaveLoadManager.CurrentSave.WasAskedNotifications) {
            DialogsManager.Instance.ShowDialog(typeof(NotificationsAskDialog));
            SaveLoadManager.CurrentSave.WasAskedNotifications = true;
            SaveLoadManager.SaveGame();
        }
    }

    public void TryRequestPermission() {
        // Проверяем, поддерживает ли устройство запрос разрешения
        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.NotRequested) {
            NotificationCenter.RequestPermission();
        }
    }

    private void CancelNotifications() {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            int seconds = Clock.Instance.SecondsToRefillMaxEnergy();
            ScheduleEnergyRestoredNotification(seconds);
        } else {
            CancelNotifications();
        }
    }

    private void ScheduleEnergyRestoredNotification(int secondsToFull) {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel {
            Id = "energy_channel",
            Name = "Energy Notifications",
            Importance = Importance.Default,
            Description = "Alerts when energy is full"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        List<string> randoms = new List<string>() {
            "Твои грядки🌱 скучают по тяпке!",
            "🤖Роботы ждут твоих команд!",
            "🚀Ещё немного до следующего уровня!🚀"
        };
        var rnd = randoms[Random.Range(0, randoms.Count)];

        var notification = new AndroidNotification {
            Title = ZG_Localization.LocalizationManager.Instance.GetText("energy_notification"),
            Text = ZG_Localization.LocalizationManager.Instance.GetText(rnd),
            FireTime = DateTime.Now.AddSeconds(secondsToFull),
        };

        AndroidNotificationCenter.SendNotification(notification, "energy_channel");
#elif UNITY_IOS
//TODO this is untested
        var timeTrigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = new System.TimeSpan(0, 0, secondsToFull),
            Repeats = false
        };

        var notification = new iOSNotification
        {
            Identifier = "_energyRestored",
            Title = "Энергия восстановлена!",
            Body = "Твоя энергия снова полна — возвращайся на ферму 🌾",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }
}