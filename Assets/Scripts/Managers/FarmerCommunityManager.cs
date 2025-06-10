using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UI;
using UnityEngine;

public class FarmerCommunityManager : MonoBehaviour {
    public static FarmerCommunityManager Instance { get; private set; }
    private FarmerCommunityApi _api = new FarmerCommunityApi();
    private GameSaveProfile _nextLoadedFarm;
    private bool _isLoading;

    public bool IsNextFarmLoaded => _nextLoadedFarm != null && !_isLoading;
    public bool IsUnlocked => UnlockableUtils.HasUnlockable(Unlockable.FarmerCommunity.ToString());

    private void Awake() {
        Instance = this;
    }

    public async UniTask PreloadNextFarm() {
        if (_isLoading) {
            return;
        }

        _isLoading = true;
        List<string> excludedFarmers = new() { SaveLoadManager.CurrentSave.UserId };
        if (_nextLoadedFarm != null) {
            excludedFarmers.Add(_nextLoadedFarm.UserId);
        }

        _nextLoadedFarm = await _api.GetRandomPlayerAsync(excludedFarmers);
        _isLoading = false;
        UIHud.Instance.FarmerCommunityBadgeView.gameObject.SetActive(IsNextFarmLoaded && IsUnlocked);
    }

    public async UniTask GoToNextFarmer() {
        UIHud.Instance.HomeFarmUi.SetActive(false);
        UIHud.Instance.OtherFarmUI.SetData(_nextLoadedFarm);
        SmartTilemap.Instance.GenerateTilesWithData(_nextLoadedFarm.TilesData);

        await PreloadNextFarm();

        UIHud.Instance.OtherFarmUI.SetNextFarmLoaded();
    }

    public void GoToHomeFarm() {
        UIHud.Instance.HomeFarmUi.SetActive(true);
        UIHud.Instance.OtherFarmUI.Close();
        SmartTilemap.Instance.GenerateTilesWithData(SaveLoadManager.CurrentSave.TilesData);
        UIHud.Instance.FarmerCommunityBadgeView.gameObject.SetActive(IsNextFarmLoaded && IsUnlocked);
    }
}