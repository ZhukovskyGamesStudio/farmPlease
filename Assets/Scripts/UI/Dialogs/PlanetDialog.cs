using System;
using Cysharp.Threading.Tasks;
using Lofelt.NiceVibrations;
using ScriptableObjects;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PlanetDialog : Dialogs.DialogWithData<PlanetDialog.Data> {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _rocketAppear, _rocketFly, _rocketLand;

    [SerializeField]
    private GameObject _continueButton, _dunesWhite, _finger;

    [SerializeField]
    private Button _dunesButton;

    [SerializeField]
    private KnowledgeCanSpeak _knowledgeCanSpeak;

    private bool _isWaitForClick;
    public bool IsShowing { get; private set; }

    protected override bool IsHideProfile => true;

    public class Data {
        public bool IsRocketCutscene;
    }

    private Data _data;

    public override void SetData(Data data) {
        _data = data;
    }

    public override async UniTask Show(Action onClose, Action<bool> onHideUI) {
        await base.Show(onClose, onHideUI);
        if (_data.IsRocketCutscene) {
            await ShowRocketCutscene();
        }
    }

    public async UniTask ShowRocketCutscene() {
        _dunesButton.interactable = false;
        IsShowing = true;
        _animation.Play(_rocketAppear.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _animation.Play(_rocketFly.name);
        IsShowing = false;
    }

    private bool _isWaitingForStepEnd;
    private float _autoSkipAfterSeconds = 3.5f;

    public async UniTask ShowRocketSpeakCutscene(string hintText, bool isHidingAfter = false, bool isShadow = true) {
        _knowledgeCanSpeak.gameObject.SetActive(true);
        _isWaitingForStepEnd = true;
        _knowledgeCanSpeak.ShowSpeak(hintText, () => { _isWaitingForStepEnd = false; }, isHidingAfter, isShadow);
        var delay = UniTask.Delay(TimeSpan.FromSeconds(_autoSkipAfterSeconds));
        var waitForTap = UniTask.WaitWhile(() => _isWaitingForStepEnd);
        await UniTask.WhenAny(delay, waitForTap);
        if (_isWaitingForStepEnd) {
            _knowledgeCanSpeak.HideSpeak();
        }

        await UniTask.WaitWhile(() => _isWaitingForStepEnd);
    }

    public void PlayImpactVibration() {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }

    public async UniTask ContinueCutscene() {
        _animation.Play(_rocketLand.name);
        await UniTask.WaitWhile(() => _animation != null && _animation.isPlaying);
        _dunesButton.interactable = true;
        _dunesWhite.gameObject.SetActive(true);
        _finger.gameObject.SetActive(true);
    }

    public void ClickDunes() {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        Close();
    }
}