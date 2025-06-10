using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class OtherFarmUI : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _otherFarmNameText;

    [SerializeField]
    private ProfileView _profileView;

    [SerializeField]
    private CountersView _countersView;

    [SerializeField]
    private Image _levelIcon;
    [SerializeField]
    private FarmerCommunityBadgeView _farmerCommunityBadgeView;

    public void SetData(GameSaveProfile otherFarm) {
        _otherFarmNameText.text = otherFarm.Nickname;
        gameObject.SetActive(true);
        _profileView.SetData(otherFarm);
        _countersView.SetData(otherFarm);
        _levelIcon.sprite = ConfigsManager.Instance.LevelsIcon[otherFarm.CurrentLevel];
        
        _farmerCommunityBadgeView.gameObject.SetActive(false);
    }

    public void SetNextFarmLoaded() {
        _farmerCommunityBadgeView.gameObject.SetActive(FarmerCommunityManager.Instance.IsNextFarmLoaded);
    }

    public void OnExit() {
        FarmerCommunityManager.Instance.GoToHomeFarm();
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}