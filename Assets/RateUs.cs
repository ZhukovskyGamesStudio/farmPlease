using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Google.Play.Review;
public class RateUs : MonoBehaviour
{
    [SerializeField] private Button _ratebutton, _closebutton, _alsoclosebutton;
    [SerializeField] private Text _ratetext;

    [SerializeField] private GameObject _rateusdialog;
    //[SerializeField] private OtherScript _otherscript;
    private ReviewManager _reviewmanager;
    private PlayReviewInfo _playReviewInfo;
    
// Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _reviewmanager = new ReviewManager();
        _closebutton.onClick.AddListener(CloseView);
        _alsoclosebutton.onClick.AddListener(CloseView);
        _ratebutton.onClick.AddListener(ReviewDialog);
    }

    private void CloseView()
    {
        if(_rateusdialog != null)
            _rateusdialog.SetActive(false);
    }
    // Update is called once per frame
    private void ReviewDialog()
    {
        StartCoroutine(LaunchReviewFlow());
    }

    private IEnumerator LaunchReviewFlow()
    {
        var requestFlowOperation = _reviewmanager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewmanager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Сброс объекта

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(launchFlowOperation.Error.ToString());
            yield break;
        }
        // Окно оценки успешно показано пользователю
    //    CloseView();
    }
}
