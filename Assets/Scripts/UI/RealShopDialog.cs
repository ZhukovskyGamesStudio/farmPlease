using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using MadPixel.InApps;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class RealShopDialog : DialogWithData<RealShopData> {
    [SerializeField]
    private RealShopOfferView _goldenClockButton, _goldenScytheButton, _goldenBatteryButton, _goldenCroponomButton;

    [SerializeField]
    private RealShopAcceptorView _acceptorView;

    private IInAppsProvider Provider => InAppsManager.Instance.InAppsProvider;

    protected override bool IsHideProfile => true;

    public override void SetData(RealShopData data) {
        _goldenBatteryButton.SetData(!data.HasGoldenBattery, () => Provider.Buy(InApsIds.Battery, BuyGoldenBattery));
        _goldenCroponomButton.SetData(!data.HasGoldenCroponom, () => Provider.Buy(InApsIds.Croponom, BuyGoldenCroponom));

        bool isClockActive = RealShopUtils.IsGoldenClockActive(data);
        _goldenClockButton.SetData(!isClockActive, () => Provider.Buy(InApsIds.Clock, BuyGoldenClock));
        if (isClockActive) {
            _goldenClockButton.StartTimer(RealShopUtils.ClockTimeLeft(data));
        }

        bool isScytheActive = RealShopUtils.IsGoldenScytheActive(data);
        _goldenScytheButton.SetData(!isScytheActive, () => Provider.Buy(InApsIds.Scythe, BuyGoldenScythe));
        if (isScytheActive) {
            _goldenScytheButton.StartTimer(RealShopUtils.ScytheTimeLeft(data));
        }

        _goldenBatteryButton.SetPrice(Provider.GetPrice(InApsIds.Battery));
        _goldenScytheButton.SetPrice(Provider.GetPrice(InApsIds.Scythe));
        _goldenClockButton.SetPrice(Provider.GetPrice(InApsIds.Clock));
        _goldenCroponomButton.SetPrice(Provider.GetPrice(InApsIds.Croponom));
    }

    public void BuyGoldenClock() {
        SaveLoadManager.CurrentSave.RealShopData.GoldenClockLastBoughtTime = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
        SaveLoadManager.SaveGame();
        Clock.Instance.RefillToMaxEnergy();
        UIHud.Instance.ClockView.UpdateGoldenState();
        _goldenClockButton.ShowOpenAnimation();
        _acceptorView.ShowBuyAnimation();
        _goldenClockButton.StartTimer(RealShopUtils.ClockTimeLeft(SaveLoadManager.CurrentSave.RealShopData));
        //CloseByButton();
    }

    public void BuyGoldenScythe() {
        SaveLoadManager.CurrentSave.RealShopData.GoldenScytheLastBoughtTime = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
        SaveLoadManager.SaveGame();
        UIHud.Instance.FastPanelScript.UpdateGoldenScytheState();
        _goldenScytheButton.ShowOpenAnimation();
        _acceptorView.ShowBuyAnimation();
        _goldenScytheButton.StartTimer(RealShopUtils.ScytheTimeLeft(SaveLoadManager.CurrentSave.RealShopData));
        //CloseByButton();
    }

    public void BuyGoldenBattery() {
        SaveLoadManager.CurrentSave.RealShopData.HasGoldenBattery = true;
        SaveLoadManager.SaveGame();
        UIHud.Instance.BatteryView.UpdateGoldenState();
        _goldenBatteryButton.ShowOpenAnimation();
        _acceptorView.ShowBuyAnimation();
        //loseByButton();
    }

    public void BuyGoldenCroponom() {
        SaveLoadManager.CurrentSave.RealShopData.HasGoldenCroponom = true;
        SaveLoadManager.SaveGame();
        UIHud.Instance.OpenCroponomButton.UpdateGoldenState();
        _goldenCroponomButton.ShowOpenAnimation();
        _acceptorView.ShowBuyAnimation();
        //CloseByButton();
    }
}

[Serializable]
public class RealShopData {
    public bool HasNoAds;
    public bool HasGoldenBattery;
    public bool HasGoldenCroponom;

    public string GoldenClockLastBoughtTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime GoldenClockLastBoughtDatetime => DateTime.Parse(GoldenClockLastBoughtTime, DateTimeFormatInfo.InvariantInfo);

    public string GoldenScytheLastBoughtTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime GoldenScytheLastBoughtDatetime => DateTime.Parse(GoldenScytheLastBoughtTime, DateTimeFormatInfo.InvariantInfo);
}