using System;
using System.Collections.Generic;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class Clock : Singleton<Clock> {
        public const int MAX_ENERGY = 7;
        private const string REFILLED_ENERGY_TEXT = "Здравствуй фермер, заряд часов восстановлен."; //TODO придумать сюда текст получше

        private readonly TimeSpan _timespanForRefillOneEnergy = new TimeSpan(3, 0, 0);

        private GameSaveProfile Save => SaveLoadManager.CurrentSave;

        private bool _isAlreadyClicked;

        public void TryAddDay() {
            if (!PlayerController.CanInteract) {
                return;
            }

            if (!HasEnergy && !GameModeManager.Instance.InfiniteClockEnergy) {
                ShowNoEnergyAnimation();
                return;
            }

            if (SaveLoadManager.CurrentSave.Energy > 0 && !_isAlreadyClicked) {
                UIHud.Instance.BatteryView.ShowHasEnergyAnimation();
                _isAlreadyClicked = true;
                return;
            }

            _isAlreadyClicked = false;
            LoseOneEnergy();
            TimeManager.Instance.AddDay();
        }

        private static void ShowNoEnergyAnimation() {
            UIHud.Instance.ClockView.ShowZeroTimeAnimation();
            Audio.Instance.PlaySound(Sounds.ZeroEnergy);

            
            if (SaveLoadManager.CurrentSave.ToolBuffsStored.SafeGet(ToolBuff.WeekBattery,0) > 0) {
                UIHud.Instance.BackpackAttention.ShowAttention();
            } else {
                DialogsManager.Instance.ShowDialogWithData(typeof(WatchAdDialog), new Reward() {
                    Items = new List<RewardItem>() {
                        new RewardItem() {
                            Type = ToolBuff.WeekBattery.ToString(),
                            Amount = 1
                        }
                    }
                });
            }
        }

        private void LoseOneEnergy() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                Save.LastClockRefilledTimestamp = NowTotalMilliseconds;
            }

            Save.ClockEnergy--;

            if (GameModeManager.Instance.InfiniteClockEnergy)
                Save.ClockEnergy = MAX_ENERGY;

            UIHud.Instance.ClockView.SetAmountWithWasteAnimation(Save.ClockEnergy);
            UIHud.Instance.ClockView.SetInteractable(false);

            SaveLoadManager.SaveGame();
        }

        public static void GenerateEnergy() {
            SaveLoadManager.CurrentSave.ClockEnergy = MAX_ENERGY;
        }

        public void RefillToMaxEnergy() {
            SetEnergy(MAX_ENERGY);
            UIHud.Instance.ClockView.SetFullAmount(Save.ClockEnergy);
        }

        public void TryRefillForRealtimePassed() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                return;
            }

            long now = NowTotalMilliseconds;
            long last = Save.LastClockRefilledTimestamp;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(now - last);

            int refillAmount = Mathf.FloorToInt((float)(timeSpan / _timespanForRefillOneEnergy));
            if (refillAmount <= 0) {
                return;
            }

            Save.ClockEnergy += refillAmount;
            int newEnergy = Mathf.Min(Save.ClockEnergy, MAX_ENERGY);
            SetEnergy(newEnergy);
            Save.LastClockRefilledTimestamp += (long)_timespanForRefillOneEnergy.TotalMilliseconds * refillAmount;
            SaveLoadManager.SaveGame();
            UIHud.Instance.HelloPanel.Show(REFILLED_ENERGY_TEXT);
        }

        public static long NowTotalMilliseconds => (long)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

        public void SetEnergy(int newEnergy) {
            Save.ClockEnergy = newEnergy;
            UIHud.Instance.ClockView.SetAmount(Save.ClockEnergy);
        }

        public void AddEnergy(int amount) {
            Save.ClockEnergy += amount;
            if (Save.ClockEnergy > MAX_ENERGY) {
                Save.ClockEnergy = MAX_ENERGY;
            }

            SaveLoadManager.SaveGame();
            UIHud.Instance.ClockView.SetAmount(Save.ClockEnergy);
        }

        public bool HasEnergy => Save.ClockEnergy > 0;
    }
}