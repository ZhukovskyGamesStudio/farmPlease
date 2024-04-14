using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(PlayAnimAndDestroy());
    }

    private IEnumerator PlayAnimAndDestroy() {
        _fxAnimation.Play(_animationName);
        yield return new WaitWhile(() => _fxAnimation.isPlaying);
        Destroy(gameObject);
    }
}