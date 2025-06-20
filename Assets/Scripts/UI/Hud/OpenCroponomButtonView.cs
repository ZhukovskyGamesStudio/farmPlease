using System;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class OpenCroponomButtonView : MonoBehaviour {
    [SerializeField]
    private GameObject _cropsTag, _toolsTag, _weatherTag, _buildingsTag;

    [SerializeField]
    private Image _buttonImage;

    [SerializeField]
    private Sprite _normalSprite, _goldenSprite;

    [SerializeField]
    private CounterChangeFx _changeUpFx;
    [SerializeField]
    private Transform _fxContainer;
    [SerializeField]
    private Sprite _xpIcon;
    private void Start() {
        UpdateTags();
    }

    public void UpdateGoldenState() {
        _buttonImage.sprite = SaveLoadManager.CurrentSave.RealShopData.HasGoldenCroponom
            ? _goldenSprite
            : _normalSprite;
    }

    public void OnClick() { }

    public void ShowAttention() { }

    public void HideAttention() { }

    public void UpdateTags() {
        _cropsTag.gameObject.SetActive(true);
        _toolsTag.gameObject.SetActive(UnlockableUtils.HasUnlockable(ToolBuff.WeekBattery));
        _weatherTag.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.Weather));
        _buildingsTag.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.FoodMarket));
    }

    public void SpawnAdditionalXp(int amount) {
        CounterChangeFx fx = Instantiate(_changeUpFx, _fxContainer.position, Quaternion.identity,_fxContainer);
        fx.Init(_xpIcon, amount);
    }
}