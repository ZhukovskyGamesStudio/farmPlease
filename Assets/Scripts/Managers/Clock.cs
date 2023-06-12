using System;
using DefaultNamespace.Abstract;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class Clock : Singleton<Clock> {
        private const int MAX_ENERGY = 7;
        private const string REFILLED_ENERGY_TEXT = "Здравствуй фермер, заряд часов восстановлен."; //TODO придумать сюда текст получше

        private readonly TimeSpan _timespanForRefillOneEnergy = new TimeSpan(3, 0, 0);

        private GameSaveProfile Save => SaveLoadManager.CurrentSave;

        public void TryAddDay() {
            if (!PlayerController.CanInteract) {
                return;
            }

            if (!HasEnergy && !GameModeManager.Instance.InfiniteClockEnergy) {
                ShowNoEnergyAnimation();
                return;
            }

            LoseOneEnergy();
            Time.Instance.AddDay();
        }

        private static void ShowNoEnergyAnimation() {
            UIHud.Instance.ClockView.ShowZeroTimeAnimation();
            Audio.Instance.PlaySound(Sounds.ZeroEnergy);
        }

        private void LoseOneEnergy() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                Save.LastClockRefilledTimestamp = NowTotalMilliseconds;
            }
            Save.ClockEnergy--;

            if (GameModeManager.Instance.InfiniteClockEnergy)
                Save.ClockEnergy = MAX_ENERGY;
            
           

            UIHud.Instance.ClockView.SetAmountWithWasteAnimation(Save.ClockEnergy);
            SaveLoadManager.Instance.SaveGame();
        }

        public void RefillToMaxEnergy() {
            SetEnergy(MAX_ENERGY);
        }

        public void TryRefillForRealtimePassed() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                return;
            }

            long now = NowTotalMilliseconds;
            long last = Save.LastClockRefilledTimestamp;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(now - last);

            int refillAmount = Mathf.FloorToInt((float) (timeSpan / _timespanForRefillOneEnergy));
            if (refillAmount <= 0) {
                return;
            }

            Save.ClockEnergy += refillAmount;
            int newEnergy = Mathf.Min(Save.ClockEnergy, MAX_ENERGY);
            SetEnergy(newEnergy);
            Save.LastClockRefilledTimestamp += (long) _timespanForRefillOneEnergy.TotalMilliseconds * refillAmount;
            SaveLoadManager.Instance.SaveGame();
            UIHud.Instance.HelloPanel.Show(REFILLED_ENERGY_TEXT);
        }

        public static long NowTotalMilliseconds => (long) (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

        public void SetEnergy(int newEnergy) {
            Save.ClockEnergy = newEnergy;
            UIHud.Instance.ClockView.SetAmount(Save.ClockEnergy);
        }

        public bool HasEnergy => Save.ClockEnergy > 0;
    }
}