using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    [SerializeField]
    private List<DialogBase> _dialogs = new List<DialogBase>();

    private Queue<DialogBase> _dialogQueue = new Queue<DialogBase>();
    public static DialogsManager Instance { get; private set; }
    private DialogBase _shownDialog;

    private void Awake() {
        Instance = this;
    }

    public void ShowDialog(Type dialogType) {
        if (_shownDialog != null && _shownDialog.GetComponent(dialogType) != null) {
            return;
        }

        if (_dialogQueue.Any(d => d.GetComponent(dialogType) != null)) {
            return;
        }

        DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
        DialogBase dialogInstance = Instantiate(prefab, transform);
        DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
        dialogBase.gameObject.SetActive(false);
        AddToQueue(dialogBase);
    }

    public void ShowDialogWithData<T>(Type dialogType, T data) {
        if (_shownDialog != null && _shownDialog.GetComponent(dialogType) != null) {
            return;
        }

        if (_dialogQueue.Any(d => d.GetComponent(dialogType) != null)) {
            return;
        }

        DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
        DialogBase dialogInstance = Instantiate(prefab, transform);
        DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
        if (dialogBase is DialogWithData<T> dialogWithData) {
            dialogWithData.SetData(data);
        }
        dialogBase.gameObject.SetActive(false);
        AddToQueue(dialogBase);
    }

    private void AddToQueue(DialogBase dialog) {
        _dialogQueue.Enqueue(dialog);
        TryShowFromQueue();
    }

    private void TryShowFromQueue() {
        if (_shownDialog != null || _dialogQueue.Count == 0) {
            return;
        }

        _shownDialog = _dialogQueue.Dequeue();
        _shownDialog.Show(OnClosedDialog);
    }

    private void OnClosedDialog() {
        _shownDialog = null;
        TryShowFromQueue();
    }
}