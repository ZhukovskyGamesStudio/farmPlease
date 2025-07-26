using System;
using Abstract;
using Managers;
using UI;
using UnityEngine.SceneManagement;

public class ZhukovskyAdsManager : PreloadableSingleton<ZhukovskyAdsManager> {
    public float GameInterAdCooldown;
    public IAdsProvider AdsProvider { get; private set; }
    public InterAdRunner InterAdRunner { get; private set; }

    protected override void OnFirstInit() {
        base.OnFirstInit();
#if MADPIXEL
        AdsProvider = new MadPixelAdsProvider();
#elif YG_PLATFORM
        AdsProvider = new YGAdsProvider();
#else
		AdsProvider = new AdsProviderMock();
#endif

        InterAdRunner = new InterAdRunner(GameInterAdCooldown, AdsProvider);
        if (LevelsUtils.IsIntersUnlocked) {
            InterAdRunner.IsInterAdRunEnabled = true;
        }

        if (SaveLoadManager.CurrentSave.RealShopData.HasNoAds) {
            CancelAdsAndDisableButton();
        }

        SceneManager.sceneLoaded += (_, _) => SetAdsButtonVisible();
    }

    public void EnableIntersAndBanners() {
        InterAdRunner.IsInterAdRunEnabled = true;
        AdsProvider.SetBanners(true);
        SetAdsButtonVisible();
        SaveLoadManager.SaveGame();
    }

    private void SetAdsButtonVisible() {
        if (!UIHud.Instance) {
            return;
        }

        bool isActive = LevelsUtils.IsIntersUnlocked && !SaveLoadManager.CurrentSave.RealShopData.HasNoAds;
        UIHud.Instance.OpenNoAdsButtonView.gameObject.SetActive(isActive);
    }

    public void CancelAdsAndDisableButton() {
        AdsProvider.CancelAds();
        AdsProvider.SetBanners(false);
        SetAdsButtonVisible();
    }

    private void Update() {
        InterAdRunner.Update();
    }
}