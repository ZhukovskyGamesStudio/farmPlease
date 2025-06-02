using System;
using UnityEngine;

public abstract class DialogWithData<T> : DialogBase {
    public abstract void SetData(T data);
}

public abstract class DialogBase : MonoBehaviour {
    protected event Action OnClose;

    public virtual void Show(Action onClose) {
        gameObject.SetActive(true);
        OnClose = onClose;
    }

    public virtual void Close() {
        OnClose?.Invoke();
        Destroy(gameObject);
    }
}