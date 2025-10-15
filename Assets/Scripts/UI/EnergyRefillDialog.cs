using TMPro;
using UnityEngine;

public class EnergyRefillDialog : Dialogs.DialogWithData<string> {
    [SerializeField]
    private TextMeshProUGUI _hintText;

    public override void SetData(string data) {
        _hintText.text = data;
    }

    public void ClickToClose() {
        CloseByButton();
        NotificationsManager.Instance.TryShowAskDialog();
    }
}