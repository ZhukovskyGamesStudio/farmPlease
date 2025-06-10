using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterChangeFx : MonoBehaviour {
    [SerializeField]
    private Animation _fxAnimation;

    [SerializeField]
    private string _animationName;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private TextMeshProUGUI _amountText;

    public void Init(Sprite icon, int amount) {
        _image.sprite = icon;
        _amountText.text = (amount > 0 ? "+" : "") + amount;
        if (gameObject.activeInHierarchy) {
            PlayAnimAndDestroy().Forget();
        } else {
            Destroy(gameObject);
        }
    }

    private async UniTaskVoid PlayAnimAndDestroy() {
        _fxAnimation.Play(_animationName);
        await UniTask.WaitWhile(() => _fxAnimation.isPlaying);
        Destroy(gameObject);
    }
}