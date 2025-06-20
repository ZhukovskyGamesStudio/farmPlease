using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RealShopDialog : DialogWithData<RealShopData> {
    [SerializeField]
    private Button _goldenClockButton, _goldenScytheButton, _goldenBatteryButton, _goldenCroponomButton;

    public override UniTask Show(Action onClose) {
        UIHud.Instance.ProfileView.Hide();
        return base.Show(onClose);
    }

    protected override UniTask Close() {
        UIHud.Instance.ProfileView.Show();
        return base.Close();
    }

    public override void SetData(RealShopData data) {
        _goldenBatteryButton.interactable = !data.HasGoldenBattery;
        _goldenCroponomButton.interactable = !data.HasGoldenCroponom;

        _goldenClockButton.interactable = !RealShopUtils.IsGoldenClockActive(data);
        _goldenScytheButton.interactable = !RealShopUtils.IsGoldenScytheActive(data);
    }

    public void BuyGoldenClock() {
        SaveLoadManager.CurrentSave.RealShopData.GoldenClockLastBoughtTime = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
        SaveLoadManager.SaveGame();
        UIHud.Instance.ClockView.UpdateGoldenState();
    }

    public void BuyGoldenScythe() {
        SaveLoadManager.CurrentSave.RealShopData.GoldenScytheLastBoughtTime = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
        SaveLoadManager.SaveGame();
        UIHud.Instance.FastPanelScript.UpdateGoldenScytheState();
    }

    public void BuyGoldenBattery() {
        SaveLoadManager.CurrentSave.RealShopData.HasGoldenBattery = true;
        SaveLoadManager.SaveGame();
        UIHud.Instance.BatteryView.UpdateGoldenState();
    }

    public void BuyGoldenCroponom() {
        SaveLoadManager.CurrentSave.RealShopData.HasGoldenCroponom = true;
        SaveLoadManager.SaveGame();
        UIHud.Instance.OpenCroponomButton.UpdateGoldenState();
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