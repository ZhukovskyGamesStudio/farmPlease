using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lofelt.NiceVibrations;
using ZG_Localization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NewLevelDialog : Dialogs.DialogWithData<NewLevelDialog.Data> {
    [SerializeField]
    private Image _previousLevelIcon, _nextLevelIcon;

    [SerializeField]
    private TextMeshProUGUI _previousLevelName, _nextLevelName;

    [SerializeField]
    private AnimationClip _previousIdleClip, _clickOnLevelClip, _levelChangeClip, _nextIdleClip, _levelDisappearClip;

    [SerializeField]
    private Animation _levelAnimation, _unlockAnimation;

    [SerializeField]
    private AnimationClip _unlockAppearClip;

    [SerializeField]
    private RewardItemView _unlockView;

    private int _clicksNeeded, _clicksMade;
    private Data _data;
    [Serializable]
    public class Data {
        public int newLevel;
        public RewardWithUnlockable RewardWithUnlockable;
    }

    public override void SetData(Data data) {
        _data = data;
        int newLevel = data.newLevel;
        _previousLevelIcon.sprite = ConfigsManager.Instance.LevelsConfig.LevelsIcon[newLevel - 1];
        _nextLevelIcon.sprite = ConfigsManager.Instance.LevelsConfig.LevelsIcon[newLevel];

        _previousLevelName.text = LocalizationUtils.L(ConfigsManager.Instance.LevelsConfig.LevelConfigs[newLevel - 1].LevelNameLoc);
        _nextLevelName.text = LocalizationUtils.L(ConfigsManager.Instance.LevelsConfig.LevelConfigs[newLevel].LevelNameLoc);

        //_clicksNeeded = 3 + newLevel;
        _clicksNeeded = 3;
        RewardUtils.SetUnlockView(data.RewardWithUnlockable, _unlockView);
    }

    public override async UniTask Show(Action onClose, Action<bool> onHideUI) {
        var showAnim = base.Show(onClose, onHideUI);
        var forAnimation = Instantiate(UIHud.Instance.ProfileView.LevelIcon, transform, worldPositionStays: true);
        forAnimation.sprite = _previousLevelIcon.sprite;
        forAnimation.raycastTarget = false;
        var c = forAnimation.gameObject.AddComponent<CanvasGroup>();
        c.alpha = 1;
        c.ignoreParentGroups = true;
        _previousLevelIcon.gameObject.SetActive(false);
        float flyTime = 0.5f;

        forAnimation.GetComponent<RectTransform>().DOSizeDelta(_previousLevelIcon.GetComponent<RectTransform>().sizeDelta, flyTime);
        forAnimation.transform.DOMoveX(_previousLevelIcon.transform.position.x, flyTime).SetEase(Ease.InQuad);
        forAnimation.transform.DOMoveY(_previousLevelIcon.transform.position.y, flyTime);
        forAnimation.transform.DOScale(_previousLevelIcon.transform.localScale, flyTime);
        var flyAnim = UniTask.WaitForSeconds(flyTime);

        await UniTask.WaitForSeconds(flyTime / 2);
        UIHud.Instance.ProfileView.Hide();

        await UniTask.WhenAll(showAnim, flyAnim);
        Destroy(forAnimation);
        _previousLevelIcon.gameObject.SetActive(true);
        _levelAnimation.Play(_previousIdleClip.name);
    }

    protected override UniTask Close() {
        UIHud.Instance.ProfileView.Show();
        VibrationsUtils.Vibrate(HapticPatterns.PresetType.LightImpact);
        RewardUtils.ClaimUnlockOnly(_data.RewardWithUnlockable);
        return base.Close();
    }

    public void Click() {
        if (_clicksMade >= _clicksNeeded) {
            return;
        }
        VibrationsUtils.Vibrate(HapticPatterns.PresetType.LightImpact);
        _clicksMade++;

        if (_clicksMade >= _clicksNeeded) {
            VibrationsUtils.Vibrate(HapticPatterns.PresetType.Success);
            ChangeLevel();
        } else {
            _levelAnimation.Stop();
            _levelAnimation.Play(_clickOnLevelClip.name);
            _levelAnimation.PlayQueued(_previousIdleClip.name);
        }
    }

    private void ChangeLevel() {
        _levelAnimation.Stop();
        _levelAnimation.Play(_clickOnLevelClip.name);
        _levelAnimation.PlayQueued(_levelChangeClip.name);
        _levelAnimation.PlayQueued(_nextIdleClip.name);
    }

    public void ChangeToUnlock() {
        VibrationsUtils.Vibrate(HapticPatterns.PresetType.LightImpact);
        UnlockAnimaion().Forget();
    }

    private async UniTask UnlockAnimaion() {
        _levelAnimation.Play(_levelDisappearClip.name);
        await UniTask.WaitWhile(() => _levelAnimation.isPlaying);
        _unlockAnimation.Play(_unlockAppearClip.name);
        await UniTask.WaitWhile(() => _unlockAnimation.isPlaying);
    }
}