using System;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UnityEngine;

public class NoAdsDialog : DialogBase {
    [SerializeField]
    private TextMeshProUGUI _priceText;

    protected override bool IsHideProfile => true;

    public override UniTask Show(Action onClose) {
        _priceText.text = InAppsManager.Instance.InAppsProvider.GetPrice(InApsIds.NoAds);
        return base.Show(onClose);
    }

    public void Buy() {
        InAppsManager.Instance.InAppsProvider.Buy(InApsIds.NoAds, OnBought);
    }

    private void OnBought() {
        SaveLoadManager.CurrentSave.RealShopData.HasNoAds = true;
        ZhukovskyAdsManager.Instance.CancelAdsAndDisableButton();
        SaveLoadManager.SaveGame();
        CloseByButton();
    }
}