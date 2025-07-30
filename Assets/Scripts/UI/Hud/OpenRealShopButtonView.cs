using Managers;
using UnityEngine;

public class OpenRealShopButtonView : MonoBehaviour {
    public void Open() {
        DialogsManager.Instance.ShowDialogWithData(typeof(RealShopDialog), SaveLoadManager.CurrentSave.RealShopData);
    }
}