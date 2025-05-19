using UI;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    public void OnClick() {
        UIHud.Instance.ProfileDialog.Show();
    }

    public void SetProgress(int from, int to) {
        
    }
}