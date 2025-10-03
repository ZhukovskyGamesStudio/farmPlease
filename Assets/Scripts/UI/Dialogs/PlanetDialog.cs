using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlanetDialog : Dialogs.DialogWithData<PlanetDialog.Data> {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _rocketAppear, _rocketFly, _rocketLand;

    [SerializeField]
    private GameObject _continueButton, _dunesWhite;

    [SerializeField]
    private Button _dunesButton;

    private bool _isWaitForClick;

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
        _animation.Play(_rocketAppear.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _animation.Play(_rocketFly.name);
    }

    public async UniTask ContinueCutscene() {
        _animation.Play(_rocketLand.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _dunesButton.interactable = true;
        _dunesWhite.gameObject.SetActive(true);
    }

    public void ClickCutscene() {
        _isWaitForClick = false;
    }

    public void ClickDunes() {
        Close();
    }
}