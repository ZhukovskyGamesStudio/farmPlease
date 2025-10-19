using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Dialogs;
using Managers;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

public class BoostersManager : Singleton<BoostersManager> {
    public bool IsUmlimitedEnergyActive { get; private set; }
    public bool IsDoubleXpActive { get; private set; }

    [SerializeField]
    private WatchAdRewardView _unlimitedEnergyAdReward, _doubleXpAdReward;

    public bool IsNextRewardBig => _nextRewardRnd >= 90;

    private int _nextRewardRnd;

    protected override void OnFirstInit() {
        base.OnFirstInit();
        GenerateRewardRnd();
    }

    private void GenerateRewardRnd() {
        _nextRewardRnd = Random.Range(0, 100);
    }

    public void ShowWatchForUnlimitedEnergyAd() {
        DialogsManager.Instance.ShowDialogWithData(typeof(WatchAdDialog), new WatchAdDialog.Data {
            AdId = AdsIds.RewardedUnlimitedEnergy,
            Header = ZG_Localization.LocalizationManager.Instance.GetText("unlim_energy_ad_header"),
            RewardViewPrefab = _unlimitedEnergyAdReward,
            OnClaim = () => { ActivateUnlimitedEnergy(ConfigsManager.Instance.CostsConfig.UnlimitedEnergySeconds); },
            Reward = new Reward() {
                Items = new List<RewardItem>() {
                    new RewardItem() {
                        Type = nameof(IsUmlimitedEnergyActive),
                        Amount = ConfigsManager.Instance.CostsConfig.UnlimitedEnergySeconds
                    }
                }
            }
        });
    }

    public void ShowWatchForDoubleXpAd() {
        DialogsManager.Instance.ShowDialogWithData(typeof(WatchAdDialog), new WatchAdDialog.Data {
            AdId = AdsIds.RewardedDoubleXp,
            Header = ZG_Localization.LocalizationManager.Instance.GetText("xp_ad_header"),
            RewardViewPrefab = _doubleXpAdReward,
            OnClaim = () => { ActivateDoubleXp(ConfigsManager.Instance.CostsConfig.UnlimitedDoubleXpSeconds); },
            Reward = new Reward() {
                Items = new List<RewardItem>() {
                    new RewardItem() {
                        Type = nameof(IsDoubleXpActive),
                        Amount = ConfigsManager.Instance.CostsConfig.UnlimitedDoubleXpSeconds
                    }
                }
            }
        });
    }

    public void ActivateUnlimitedEnergy(int seconds) {
        UIHud.Instance.BatteryView.SetUnlimitedEnergyActive(true);
        UIHud.Instance.BatteryView.UnlimitedBoosterTimer.SetTime(TimeSpan.FromSeconds(seconds));
        IsUmlimitedEnergyActive = true;
        WaitUnlimitedEnergyEnd(seconds).Forget();
    }

    public void ActivateDoubleXp(int seconds) {
        UIHud.Instance.DoubleXpBooster.SetActive(true);
        UIHud.Instance.DoubleXpTimer.SetTime(TimeSpan.FromSeconds(seconds));
        IsDoubleXpActive = true;
        WaitDoubleXpEnd(seconds).Forget();
    }

    private async UniTask WaitDoubleXpEnd(int seconds) {
        UIHud.Instance.DoubleXpTimer.gameObject.SetActive(true);
        while (seconds > 0) {
            UIHud.Instance.DoubleXpTimer.SetTime(TimeSpan.FromSeconds(seconds));
            await UniTask.WaitForSeconds(1);
            seconds--;
        }

        IsDoubleXpActive = false;
        UIHud.Instance.DoubleXpBooster.SetActive(false);
    }

    private async UniTask WaitUnlimitedEnergyEnd(int seconds) {
        while (seconds > 0) {
            UIHud.Instance.BatteryView.UnlimitedBoosterTimer.SetTime(TimeSpan.FromSeconds(seconds));
            await UniTask.WaitForSeconds(1);
            seconds--;
        }

        IsUmlimitedEnergyActive = false;
        UIHud.Instance.BatteryView.SetUnlimitedEnergyActive(false);
    }

    public void OnClickFlyingDecor(GameObject decor) {
        if (_nextRewardRnd <= 50) {
            InventoryManager.Instance.AddXp(1);
        } else if (_nextRewardRnd <= 80) {
            DropCoin(decor.transform.position, 1);
            InventoryManager.Instance.AddCoins(1);
        } else if (_nextRewardRnd <= 90) {
            InventoryManager.Instance.AddSeed(Crop.Tomato, 1);
            UIHud.Instance.BackpackAttention.ShowAttention();
        } else if (_nextRewardRnd <= 95) {
            if (SaveLoadManager.CurrentSave.CurrentLevel >= ConfigsManager.Instance.CostsConfig.LevelToUnlimitedEnergyBooster &&
                !IsUmlimitedEnergyActive) {
                ShowWatchForUnlimitedEnergyAd();
            } else {
                DropCoin(decor.transform.position, 3);
                InventoryManager.Instance.AddXp(3);
            }
        } else if (_nextRewardRnd <= 100) {
            if (SaveLoadManager.CurrentSave.CurrentLevel >= ConfigsManager.Instance.CostsConfig.LevelToDoubleXpBooster && !IsDoubleXpActive) {
                ShowWatchForDoubleXpAd();
            } else {
                DropCoin(decor.transform.position, 2);
                InventoryManager.Instance.AddCoins(2);
            }
        }

        GenerateRewardRnd();
    }

    private void DropCoin(Vector3 pos, int amount) {
        for (int i = 0; i < amount; i++) {
            var obj = Instantiate(CropsTable.Instance.FlyingCoinFxPrefab);
            obj.Init(pos);
        }
    }
}

[Serializable]
public class BoostersData {
    public string UmlimitedEnergyLastActiveTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime UmlimitedEnergyLastActiveDateTime => DateTime.Parse(UmlimitedEnergyLastActiveTime, DateTimeFormatInfo.InvariantInfo);

    public string DoubleXpLastActiveTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime DoubleXpLastActiveDateTime => DateTime.Parse(DoubleXpLastActiveTime, DateTimeFormatInfo.InvariantInfo);
}