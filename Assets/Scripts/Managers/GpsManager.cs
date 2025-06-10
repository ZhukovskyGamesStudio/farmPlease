using System;
using Abstract;

using NotificationSamples;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

namespace Managers
{
    public class GpsManager : PreloadableSingleton<GpsManager> {

#pragma warning disable CS0414 // Field is assigned but its value is never used
        [HideInInspector] private static readonly string TutorialLeaderboard = "CgkI1N701sUbEAIQAQ";
#pragma warning restore CS0414 // Field is assigned but its value is never used

        public static bool IsAuthenticated;

        [SerializeField] private GameNotificationsManager NotificationsManager;

        protected override void OnFirstInit() {
            IsAuthenticated = false;
        }

        public void Initialize() {
#if UNITY_ANDROID

            if (IsAuthenticated) {
                Debug.Instance.Log("Google Play Services УЖЕ подключены");
            } else {
                PlayGamesPlatform.DebugLogEnabled = true;
                PlayGamesPlatform.Activate();
                /*Social.localUser.Authenticate(success => {
                    IsAuthenticated = success;
                    Settings.Instance.SettingsPanel.GpgsUpdated(IsAuthenticated);
                    if (!success)
                        Debug.Instance.Log(
                            "Google Play Services не подключены. Глобальная статистика и рекорды могут не работать");
                });*/
            }
#endif
        }

        public void InitializeNotifications() {
            return;
#if UNITY_ANDROID
            GameNotificationChannel channel = new("someId", "New day notifications",
                "Notify you about starting new ingame day");
            NotificationsManager.Initialize(channel);
            NotificationsManager.DismissAllNotifications();
#endif
        }

        public void NewDayNotification(bool isOn = true) {
            if (!NotificationsManager.Initialized) {
                Debug.Instance.Log("Notification Manager is not initialized");
                return;
            }

            NotificationsManager.CancelNotification(0);
            if (isOn) {
                DateTime time = DateTime.Today + Settings.Instance.GetDayPoint();
                if (DateTime.Now > time)
                    time = time.AddDays(1);
                //DebugManager.instance.Log("Cancelled and Sheduled new day notification at " + time.ToString());

                CreateNotification(0, "Наступил новый день", "Используйте энергию с умом!", time, true);
            } else {
                Debug.Instance.Log("Cancelled new day notification ");
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
            return IsAuthenticated;
        }

        public static void ReportScore(int score, string what) {
#if UNITY_ANDROID
            if (IsAuthenticated)
                switch (what) {
                    case "tutorialLeaderboard":
                        //Social.ReportScore(score, TutorialLeaderboard, success => { });
                        break;
                }
#endif
        }

        public void ShowLeaderBoard() {
#if UNITY_ANDROID
            //Social.ShowLeaderboardUI();
#endif
        }

        public static void ExitFromGps() {
/*#if UNITY_ANDROID
        PlayGamesPlatform.Instance.SignOut();
#endif*/
        }
    }
}