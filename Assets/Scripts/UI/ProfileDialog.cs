using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileDialog : MonoBehaviour {

    [SerializeField]
    private TMP_InputField _nicknameInput;
    
    [SerializeField]
    private TextMeshProUGUI _cropsCollectedText, _coinsText;

    [SerializeField]
    private Image _currentLevelIcon, _nextLevelIcon;

    [SerializeField]
    private AnimatableProgressbar _xpProgressBar;

    [SerializeField]
    private List<RewardItemView> _rewardItemViews;
    
    [SerializeField]
    private Sprite _coinRewardIcon;

    
    public void Show() {
        _nicknameInput.SetTextWithoutNotify(SaveLoadManager.CurrentSave.Nickname);
        _cropsCollectedText.text = SaveLoadManager.CurrentSave.CropPoints.ToString();
        _coinsText.text = SaveLoadManager.CurrentSave.Coins.ToString();
        
        _currentLevelIcon.sprite = ConfigsManager.Instance.LevelsIcon[SaveLoadManager.CurrentSave.CurrentLevel];
        
        //TODO add view for last ingame level
        _nextLevelIcon.sprite = ConfigsManager.Instance.LevelsIcon[SaveLoadManager.CurrentSave.CurrentLevel+1];
        SetLevelProgress();

        SetRewards();
        
      
        gameObject.SetActive(true);
    }

    private void SetLevelProgress() {
        int curLevelMin = XpUtils.XpByLevel(SaveLoadManager.CurrentSave.CurrentLevel);
        int nextLevel = XpUtils.XpByLevel(SaveLoadManager.CurrentSave.CurrentLevel + 1);
        _xpProgressBar.SetAmount(SaveLoadManager.CurrentSave.Xp - curLevelMin, nextLevel - curLevelMin);
    }

    private void SetRewards() {
        var nextLevel = SaveLoadManager.CurrentSave.CurrentLevel + 1;
        var reward = ConfigsManager.Instance.LevelConfigs[nextLevel].Reward;
        RewardUtils.SetRewardsView(reward, _rewardItemViews, _coinRewardIcon);
    }
    
    public void OnNicknameChanged(string input) {
        if (string.IsNullOrWhiteSpace(input) || input.Length <=3) {
            _nicknameInput.SetTextWithoutNotify(SaveLoadManager.CurrentSave.Nickname);
            return;
        }
        SaveLoadManager.CurrentSave.Nickname = input;
        SaveLoadManager.SaveGame();
    }
    
    public void Close() {
        gameObject.SetActive(false);
    }
}