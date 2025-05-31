using System;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    [SerializeField]
    private RewardDialog _rewardDialogPrefab;
    [SerializeField]
    private ProfileDialog _profileDialogPrefab;
    public static DialogsManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void ShowRewardDialog(Reward reward, Action onClaim) {
        RewardDialog dialog = Instantiate(_rewardDialogPrefab, transform);
        dialog.Show(reward, onClaim);
    }
    
    public void ShowProfileDialog( Action onClose) {
        ProfileDialog dialog = Instantiate(_profileDialogPrefab, transform);
        dialog.Show(onClose);
    }
}