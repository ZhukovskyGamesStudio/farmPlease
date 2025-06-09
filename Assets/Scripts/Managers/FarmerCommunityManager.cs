using Cysharp.Threading.Tasks;
using Managers;
using UI;
using UnityEngine;

public class FarmerCommunityManager : MonoBehaviour {
    public static FarmerCommunityManager Instance { get; private set; }
    private FarmerCommunityApi _api = new FarmerCommunityApi();
    private GameSaveProfile _nextLoadedFarm;
    private bool _isLoading;

    private void Awake() {
        Instance = this;
    }

    public async UniTask GoToNextFarmer() {
        if (_isLoading) {
            return;
        }

        _isLoading = true;

        _nextLoadedFarm = await _api.GetRandomPlayerAsync(SaveLoadManager.CurrentSave.UserId);
        UIHud.Instance.HomeFarmUi.SetActive(false);
        UIHud.Instance.OtherFarmUI.SetData(_nextLoadedFarm);
        if (_nextLoadedFarm != null) {
            SmartTilemap.Instance.GenerateTilesWithData(_nextLoadedFarm.TilesData);
        }

        _isLoading = false;
    }

    public void GoToHomeFarm() {
        UIHud.Instance.HomeFarmUi.SetActive(true);
        UIHud.Instance.OtherFarmUI.Close();
        SmartTilemap.Instance.GenerateTilesWithData(SaveLoadManager.CurrentSave.TilesData);
    }
}