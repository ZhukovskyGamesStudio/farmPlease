using System;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class Clock : Singleton<Clock> {
        public const int MAX_ENERGY = 7;
        private const string REFILLED_ENERGIES_TEXT = "здравствуй фермер,\n заряды часов восстановлены.";
        private const string REFILLED_ENERGY_TEXT = "заряд\nчасов\nвосстановлен.";

        private TimeSpan TimespanForRefillOneEnergy => TimeSpan.FromMinutes(ConfigsManager.Instance.CostsConfig.MunitesForOneChargeRefill);

        private GameSaveProfile Save => SaveLoadManager.CurrentSave;

        private bool _isAlreadyClicked;
        private Coroutine _realtimeClockCoroutine;

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

        public bool IsAdIconActive() {
            return SaveLoadManager.CurrentSave.ToolBuffsStored.SafeGet(ToolBuff.WeekBattery, 0) == 0 &&
                   SaveLoadManager.CurrentSave.ClockEnergy == 0 && KnowledgeUtils.HasKnowledge(Knowledge.NoEnergy);
        }

        private static void ShowNoEnergyAnimation() {
            UIHud.Instance.ClockView.ShowZeroTimeAnimation();
            Audio.Instance.PlaySound(Sounds.ZeroEnergy);

            if (SaveLoadManager.CurrentSave.ToolBuffsStored.SafeGet(ToolBuff.WeekBattery, 0) > 0) {
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

        private IEnumerator ClockRealtimeCoroutine(float timeLeft) {
            float cur = 0;
            while (cur < timeLeft) {
                if(KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                    cur += Time.deltaTime;
                }

                UIHud.Instance.ClockView.SetClockArrowRotation(-360 * (cur / timeLeft));
                yield return new WaitForEndOfFrame();
            }

            _realtimeClockCoroutine = null;

            if (Save.ClockEnergy < MAX_ENERGY && !GameModeManager.Instance.InfiniteClockEnergy) {
                TryRefillForRealtimePassed();
            }
        }

        private void LoseOneEnergy() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                Save.LastClockRefilledTimestamp = NowTotalMilliseconds;
            }

            if (GameModeManager.Instance.InfiniteClockEnergy || RealShopUtils.IsGoldenClockActive(SaveLoadManager.CurrentSave.RealShopData)) {
                Save.ClockEnergy = MAX_ENERGY;
            } else {
                if (_realtimeClockCoroutine == null) {
                    _realtimeClockCoroutine = StartCoroutine(ClockRealtimeCoroutine((float)TimespanForRefillOneEnergy.TotalSeconds));
                }

                Save.ClockEnergy--;
            }

            UIHud.Instance.ClockView.SetAmountWithWasteAnimation(Save.ClockEnergy);
            UIHud.Instance.ClockView.SetInteractable(false);

            SaveLoadManager.SaveGame();
        }

        public static void GenerateEnergy() {
            SaveLoadManager.CurrentSave.ClockEnergy = MAX_ENERGY;
        }

        public void RefillToMaxEnergy() {
            if (_realtimeClockCoroutine != null) {
                StopCoroutine(_realtimeClockCoroutine);
                _realtimeClockCoroutine = null;
            }

            SetEnergy(MAX_ENERGY);
            UIHud.Instance.ClockView.SetFullAmount(Save.ClockEnergy);
        }

        public void TryRefillForRealtimePassed() {
            if (Save.ClockEnergy == MAX_ENERGY) {
                return;
            }

            if (_realtimeClockCoroutine != null) {
                StopCoroutine(_realtimeClockCoroutine);
            }

            long now = NowTotalMilliseconds;
            long last = Save.LastClockRefilledTimestamp;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(now - last);

            int refillAmount = Mathf.FloorToInt((float)(timeSpan / TimespanForRefillOneEnergy));
            if (refillAmount <= 0) {
                _realtimeClockCoroutine =
                    StartCoroutine(ClockRealtimeCoroutine((float)(TimespanForRefillOneEnergy.TotalSeconds - timeSpan.TotalSeconds)));
                return;
            }

            Save.ClockEnergy += refillAmount;
            int newEnergy = Mathf.Min(Save.ClockEnergy, MAX_ENERGY);
            SetEnergy(newEnergy);
            Save.LastClockRefilledTimestamp += (long)TimespanForRefillOneEnergy.TotalMilliseconds * refillAmount;
            SaveLoadManager.SaveGame();
            DialogsManager.Instance.ShowDialogWithData(typeof(EnergyRefillDialog),
                refillAmount > 1 ? REFILLED_ENERGIES_TEXT : REFILLED_ENERGY_TEXT);
            if (Save.ClockEnergy != MAX_ENERGY) {
                _realtimeClockCoroutine = StartCoroutine(ClockRealtimeCoroutine((float)TimespanForRefillOneEnergy.TotalSeconds));
            }
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