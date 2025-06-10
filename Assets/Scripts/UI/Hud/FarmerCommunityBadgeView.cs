using UnityEngine;

public class FarmerCommunityBadgeView : MonoBehaviour {
    public void OnBadgeClick() {
        gameObject.SetActive(false);
        FarmerCommunityManager.Instance.GoToNextFarmer();
    }
}