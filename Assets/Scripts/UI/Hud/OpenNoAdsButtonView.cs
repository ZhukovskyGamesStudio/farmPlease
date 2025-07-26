using UnityEngine;

public class OpenNoAdsButtonView : MonoBehaviour {
    public void ShowDialog() {
        DialogsManager.Instance.ShowDialog(typeof(NoAdsDialog));
    }
}