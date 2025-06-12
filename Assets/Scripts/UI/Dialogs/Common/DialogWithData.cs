using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class DialogWithData<T> : DialogBase {
    public abstract void SetData(T data);
}

public abstract class DialogBase : MonoBehaviour {
   
    private DialogAnimationController _animationController;
    private DialogAnimationController AnimationController => _animationController ??= GetComponent<DialogAnimationController>();
    protected event Action OnClose;

    public virtual async UniTask Show(Action onClose) {
        gameObject.SetActive(true);
        OnClose = onClose;
        if (AnimationController) {
            await AnimationController.Show();
        }
    }

    public void CloseByButton() {
        Close().Forget();
    }

    protected virtual async UniTask Close() {
        if (AnimationController) {
            await AnimationController.Hide();
        }
        OnClose?.Invoke();
        Destroy(gameObject);
    }
}