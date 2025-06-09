using UnityEngine;

public class FarmerCommunityBadgeView : MonoBehaviour {
    public void OnBadgeClick() {
        FarmerCommunityManager.Instance.GoToNextFarmer();
    }
}