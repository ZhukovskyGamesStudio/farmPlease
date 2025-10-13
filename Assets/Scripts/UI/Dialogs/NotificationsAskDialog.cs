using System;
using Cysharp.Threading.Tasks;
using Dialogs;

public class NotificationsAskDialog : DialogBase {
    protected override bool IsHideProfile => true;

    public void Allow() {
        NotificationsManager.Instance.TryRequestPermission();
        CloseByButton();
    }

    public void Deny() {
        CloseByButton();
    }
}