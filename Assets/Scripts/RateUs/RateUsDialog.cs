using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Google.Play.Review;

public class RateUsDialog : DialogBase {
    [SerializeField]
    private Button _ratebutton, _alsoclosebutton;

    [SerializeField]
    private Text _ratetext;

    [SerializeField]
    private GameObject _rateusdialog;

    //[SerializeField] private OtherScript _otherscript;
    private ReviewManager _reviewmanager;

    private PlayReviewInfo _playReviewInfo;

    public override UniTask Show(Action onClose) {
        _reviewmanager = new ReviewManager();
        return base.Show(onClose);
    }

    public void RateGood() {
        StartCoroutine(LaunchReviewFlow());
    }

    public void RateBad() {
        CloseByButton();
    }

    private void CloseView() {
        if (_rateusdialog != null)
            _rateusdialog.SetActive(false);
    }

    private IEnumerator LaunchReviewFlow() {
        var requestFlowOperation = _reviewmanager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
            Debug.LogError(requestFlowOperation.Error.ToString());
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewmanager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Сброс объекта

        if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
            Debug.LogError(launchFlowOperation.Error.ToString());
            yield break;
        }
        // Окно оценки успешно показано пользователю
        //    CloseView();
    }
}