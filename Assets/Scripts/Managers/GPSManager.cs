using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using NotificationSamples;
using UnityEngine;

public class GPSManager : PreloadedManager {
    public static GPSManager instance;

    [HideInInspector]
    private static readonly string tutorialLeaderboard = "CgkI1N701sUbEAIQAQ";

    public static bool isAuthenticated;

    [SerializeField]
    private GameNotificationsManager NotificationsManager;

    public override IEnumerator Init() {
        if (instance == null) {
            isAuthenticated = false;
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        yield break;
    }

    public void Initialize() {
#if UNITY_ANDROID

        if (isAuthenticated) {
            DebugManager.instance.Log("Google Play Services УЖЕ подключены");
        } else {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate(success => {
                isAuthenticated = success;
                SettingsManager.instance.SettingsPanel.GPGSUpdated(isAuthenticated);
                if (!success)
                    DebugManager.instance.Log(
                        "Google Play Services не подключены. Глобальная статистика и рекорды могут не работать");
            });
        }
#endif
    }

    public void InitializeNotifications() {
#if UNITY_ANDROID
        GameNotificationChannel channel = new("someId", "New day notifications",
            "Notify you about starting new ingame day");
        NotificationsManager.Initialize(channel);
        NotificationsManager.DismissAllNotifications();
#endif
    }

    public void NewDayNotification(bool isOn = true) {
        if (!NotificationsManager.Initialized) {
            DebugManager.instance.Log("Notification Manager is not initialized");
            return;
        }

        NotificationsManager.CancelNotification(0);
        if (isOn) {
            DateTime time = DateTime.Today + SettingsManager.instance.GetDayPoint();
            if (DateTime.Now > time)
                time = time.AddDays(1);
            //DebugManager.instance.Log("Cancelled and Sheduled new day notification at " + time.ToString());

            CreateNotification(0, "Наступил новый день", "Используйте энергию с умом!", time, true);
        } else {
            DebugManager.instance.Log("Cancelled new day notification ");
        }
    }

    public void CreateNotification() {
        CreateNotification(-1, "Test notification", "I'm here", DateTime.Now.AddSeconds(15), false);
    }

    public void CreateNotification(int id, string title, string body, DateTime time, bool isReshedule) {
        IGameNotification notification = NotificationsManager.CreateNotification();
        if (notification != null) {
            notification.Id = id;
            notification.Title = title;
            notification.Body = body;
            notification.DeliveryTime = time;
            notification.LargeIcon = "tomato";
            PendingNotification toDisplay = NotificationsManager.ScheduleNotification(notification);
            toDisplay.Reschedule = isReshedule;
        }
    }

    public static bool IsInitialized() {
        return isAuthenticated;
    }

    public static void ReportScore(int score, string what) {
#if UNITY_ANDROID
        if (isAuthenticated)
            switch (what) {
                case "tutorialLeaderboard":
                    Social.ReportScore(score, tutorialLeaderboard, success => { });
                    break;
            }
#endif
    }

    public void ShowLeaderBoard() {
#if UNITY_ANDROID
        Social.ShowLeaderboardUI();
#endif
    }

    public static void ExitFromGPS() {
/*#if UNITY_ANDROID
        PlayGamesPlatform.Instance.SignOut();
#endif*/
    }
}