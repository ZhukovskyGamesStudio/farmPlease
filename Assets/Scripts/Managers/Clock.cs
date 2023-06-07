using System;
using DefaultNamespace.Abstract;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class Clock : Singleton<Clock> {
        private const int MAX_ENERGY = 7;
        private readonly TimeSpan _timespanForRefillOneEnergy = new TimeSpan(3,0,0); 

        private GameSaveProfile save => SaveLoadManager.CurrentSave;

        public void TryAddDay() {
            if (!PlayerController.canInteract) {
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
            save.clockEnergy--;

            if (GameModeManager.Instance.InfiniteClockEnergy)
                save.clockEnergy = MAX_ENERGY;

            UIHud.Instance.ClockView.SetAmountWithWasteAnimation(save.clockEnergy);
        }

        public void RefillToMaxEnergy() {
            SetEnergy(MAX_ENERGY);
        }

        public void TryRefillForRealtimePassed() {
            if (save.clockEnergy == MAX_ENERGY) {
                return;
            }
            long now = NowTotalMilliseconds;
            long last = save.lastClockRefilledTimestamp;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(now - last); 
            
            int refillAmount = Mathf.FloorToInt((float)(timeSpan / _timespanForRefillOneEnergy));
            if (refillAmount <= 0) {
                return;
            }

            save.clockEnergy += refillAmount;
            int newEnergy = Mathf.Min(save.clockEnergy, MAX_ENERGY);
            SetEnergy(newEnergy);
            save.lastClockRefilledTimestamp += (long)_timespanForRefillOneEnergy.TotalMilliseconds * refillAmount;
            SaveLoadManager.Instance.SaveGame();
            UIHud.Instance.ClockView.ShowHelloPanel();
        }

        public static long NowTotalMilliseconds => (long) (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

        public void SetEnergy(int newEnergy) {
            save.clockEnergy = newEnergy;
            UIHud.Instance.ClockView.SetAmount(save.clockEnergy);
        }

        public bool HasEnergy => save.clockEnergy > 0;
    }
}