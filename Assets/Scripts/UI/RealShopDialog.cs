using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RealShopDialog : DialogWithData<RealShopData> {
    [SerializeField]
    private RealShopOfferView _goldenClockButton, _goldenScytheButton, _goldenBatteryButton, _goldenCroponomButton;

    [SerializeField]
    private RealShopAcceptorView _acceptorView;
    
    public override UniTask Show(Action onClose) {
        UIHud.Instance.ProfileView.Hide();
        return base.Show(onClose);
    }

    protected override UniTask Close() {
        UIHud.Instance.ProfileView.Show();
        return base.Close();
    }

    public override void SetData(RealShopData data) {
        _goldenBatteryButton.SetData(!data.HasGoldenBattery, BuyGoldenBattery);
        _goldenCroponomButton.SetData(!data.HasGoldenCroponom, BuyGoldenCroponom);

        bool isClockActive = RealShopUtils.IsGoldenClockActive(data);
        _goldenClockButton.SetData(!isClockActive, BuyGoldenClock);
        if (isClockActive) {
            _goldenClockButton.StartTimer(RealShopUtils.ClockTimeLeft(data));
        }

        bool isScytheActive = RealShopUtils.IsGoldenScytheActive(data);
        _goldenScytheButton.SetData(!isScytheActive, BuyGoldenScythe);
        if (isScytheActive) {
            _goldenScytheButton.StartTimer(RealShopUtils.ScytheTimeLeft(data));
        }
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
    public bool HasGoldenBattery;
    public bool HasGoldenCroponom;

    public string GoldenClockLastBoughtTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime GoldenClockLastBoughtDatetime => DateTime.Parse(GoldenClockLastBoughtTime, DateTimeFormatInfo.InvariantInfo);

    public string GoldenScytheLastBoughtTime = DateTime.MinValue.ToString(DateTimeFormatInfo.InvariantInfo);
    public DateTime GoldenScytheLastBoughtDatetime => DateTime.Parse(GoldenScytheLastBoughtTime, DateTimeFormatInfo.InvariantInfo);
}