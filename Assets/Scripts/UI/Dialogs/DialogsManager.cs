using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    [SerializeField]
    private List<DialogBase> _dialogs = new List<DialogBase>();

    public static DialogsManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void ShowDialog(Type dialogType, Action onClose) {
        DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
        DialogBase dialogInstance = Instantiate(prefab, transform);
        DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
        dialogBase.Show(onClose);
    }

    public void ShowDialogWithData<T>(Type dialogType, T data, Action onClose) {
        DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
        DialogBase dialogInstance = Instantiate(prefab, transform);
        DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
        if (dialogBase is DialogWithData<T> dialogWithData) {
            dialogWithData.SetData(data);
        }

        dialogBase.Show(onClose);
    }
}