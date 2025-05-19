using System;
using Abstract;
using ScriptableObjects;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class Settings : Singleton<Settings> {

        public SettingsPanel SettingsPanel;
        public SettingsData SettingsData => SaveLoadManager.CurrentSave.SettingsData;

        [SerializeField]
        private CheatCodeConfigList CheatCodeConfigList;

        public void InitSettingsView() {
            GpsManager.Instance.InitializeNotifications();

            SettingsPanel.Initialize(CheatCodeConfigList);
            SettingsPanel.UpdateSettingsPanel(SettingsData);

            RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, false);
            RealTImeManager.SkipOne = SettingsData.SkipOne;
            //DebugManager.instance.Log("New day notify is " + settingsProfile.sendNotifications);
            GpsManager.Instance.NewDayNotification(SettingsData.SendNotifications);

            Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
        }

         

        public void ChangeSettings(SettingsData data) {
            SaveLoadManager.CurrentSave.SettingsData = new SettingsData(data);
            SaveLoadManager.SaveGame();
        }

        public void NotificationChanged() {
            GpsManager.Instance.NewDayNotification(SettingsData.SendNotifications);
        }

        public void DayMomentChanged() {
            throw new NotImplementedException();
        }

         

        public TimeSpan GetDayPoint() {
            switch (SettingsData.NewDayPoint) {
                case 0:
                    return new TimeSpan(0, 0, 0);

                case 1:
                    return new TimeSpan(6, 0, 0);

                case 2:
                    return new TimeSpan(12, 0, 0);

                case 3:
                    return new TimeSpan(18, 0, 0);
            }

            UnityEngine.Debug.Log("error");
            return new TimeSpan(12, 0, 0);
        }
    }
}