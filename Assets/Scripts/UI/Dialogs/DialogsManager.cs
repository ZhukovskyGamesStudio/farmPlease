using System;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    [SerializeField]
    private RewardDialog _rewardDialogPrefab;

    public static DialogsManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void ShowRewardDialog(Reward reward) {
        RewardDialog dialog = Instantiate(_rewardDialogPrefab, transform);
        dialog.Show(reward);
    }
}