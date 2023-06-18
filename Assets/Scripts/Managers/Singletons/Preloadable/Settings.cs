using System;
using System.Collections;
using Abstract;
using ScriptableObjects;
using UI;
using UnityEngine;

namespace Managers
{
    public class Settings : PreloadableSingleton<Settings> {

        public SettingsPanel SettingsPanel;
        public SettingsProfile SettingsProfile;
        [SerializeField]
        private CheatCodeConfigList CheatCodeConfigList;

        /**********/

        protected override void OnFirstInit() {
            SettingsProfile = new SettingsProfile();
            SettingsProfile.Load();
            LoadSettings();
        }

        public void LoadSettings() {
            GpsManager.Instance.InitializeNotifications();

            SettingsPanel.Initialize(CheatCodeConfigList);
            SettingsPanel.UpdateSettingsPanel(SettingsProfile);

            RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, false);
            RealTImeManager.SkipOne = SettingsProfile.SkipOne;
            if (Time.Instance != null)
                Time.Instance.ChangeDayPoint(GetDayPoint());
            //DebugManager.instance.Log("New day notify is " + settingsProfile.sendNotifications);
            GpsManager.Instance.NewDayNotification(SettingsProfile.SendNotifications);

            Audio.Instance.ChangeVolume(SettingsProfile.MasterVolume, SettingsProfile.MusicVolume,
                SettingsProfile.EffectsVolume);
        }

        /**********/

        public void ChangeSettings(SettingsProfile profile) {
            SettingsProfile = new SettingsProfile(profile);
            SettingsProfile.Save();
        }

        public void NotificationChanged() {
            GpsManager.Instance.NewDayNotification(SettingsProfile.SendNotifications);
        }

        public void DayMomentChanged() {
            SettingsProfile.SkipOne = true;
            SettingsProfile.Save();

            RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, true);
            if (Time.Instance)
                Time.Instance.ChangeDayPoint(GetDayPoint());
            GpsManager.Instance.NewDayNotification(SettingsProfile.SendNotifications);
        }

        /**********/

        public TimeSpan GetDayPoint() {
            switch (SettingsProfile.NewDayPoint) {
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

    public class SettingsProfile {
        public float EffectsVolume;
        public float MasterVolume;
        public float MusicVolume;
        public int NewDayPoint;
        public bool SendNotifications;
        public bool SkipOne;

        public SettingsProfile() {
        }

        public SettingsProfile(SettingsProfile profile) {
            MasterVolume = profile.MasterVolume;
            MusicVolume = profile.MusicVolume;
            EffectsVolume = profile.EffectsVolume;
            SendNotifications = profile.SendNotifications;
            NewDayPoint = profile.NewDayPoint;
            SkipOne = profile.SkipOne;
        }

        public void Save() {
            PlayerPrefs.SetFloat("masterVolume", MasterVolume);
            PlayerPrefs.SetFloat("musicVolume", MusicVolume);
            PlayerPrefs.SetFloat("effectsVolume", EffectsVolume);
            PlayerPrefs.SetInt("sendNotifications", SendNotifications ? 1 : 0);
            PlayerPrefs.SetInt("newDayPoint", NewDayPoint);
            PlayerPrefs.SetInt("skipOne", SkipOne ? 1 : 0);
        }

        public void Load() {
            MasterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
            MusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            EffectsVolume = PlayerPrefs.GetFloat("effectsVolume", 0.5f);
            SendNotifications = PlayerPrefs.GetInt("sendNotifications", 1) == 1;
            NewDayPoint = PlayerPrefs.GetInt("newDayPoint", 2);
            SkipOne = PlayerPrefs.GetInt("skipOne", 0) == 1;
        }

        public void Clear() {
            PlayerPrefs.DeleteKey("masterVolume");
            PlayerPrefs.DeleteKey("musicVolume");
            PlayerPrefs.DeleteKey("effectsVolume");
            PlayerPrefs.DeleteKey("sendNotifications");
            PlayerPrefs.DeleteKey("newDayPoint");
            PlayerPrefs.DeleteKey("skipOne");
        }
    }
}