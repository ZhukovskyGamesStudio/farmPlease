using System.Collections;
using Managers;
using Tables;
using UI;
using UnityEngine;

public class ToolShopDialog : DialogWithData<ToolShopData> {
    [SerializeField]
    private GameObject ChangeButton;

    [SerializeField]
    private ToolOffer _toolOffer1, _toolOffer2;

    [SerializeField]
    private GameObject _exitButton, _toolBox;

    [SerializeField]
    private Animation _landingPlatformAnimation;

    [SerializeField]
    private Animation _tabletAnimation, _bagAnimation;

    private bool _waitingToolMovedToBag;

    private ToolBuff _toolBuff1, _toolBuff2;

    private bool _fistActive, _secondActive;

    [SerializeField]
    private Transform _noToolsText;

    private void BuyTool1(ToolBuff buff) {
        BuyTool(_toolOffer1, buff);
        Audio.Instance.PlaySound(Sounds.Button);
    }

    private void BuyTool2(ToolBuff buff) {
        BuyTool(_toolOffer2, buff);
        Audio.Instance.PlaySound(Sounds.Button);
    }

    public override void SetData(ToolShopData data) {
        _toolOffer1.SetData(ToolsTable.ToolByType(data.FirstOffer), data.FirstOfferActive, BuyTool1);
        _toolBuff1 = data.FirstOffer;
        _fistActive = data.FirstOfferActive;

        _toolOffer2.SetData(ToolsTable.ToolByType(data.SecondOffer), data.SecondOfferActive, BuyTool2);
        _toolBuff2 = data.SecondOffer;
        _secondActive = data.SecondOfferActive;

        ChangeButton.SetActive(data.ChangeButtonActive);
        UpdateNoToolsMessage();
    }

    public void ChangeToolsButton() {
        if (InventoryManager.Instance.EnoughMoney(ConfigsManager.Instance.CostsConfig.ToolsShopChangeCost)) {
            InventoryManager.Instance.AddCoins(-1 * ConfigsManager.Instance.CostsConfig.ToolsShopChangeCost);
            ToolsUtils.ChangeTools();
            SaveLoadManager.CurrentSave.ToolShopData.ChangeButtonActive = false;
            SaveLoadManager.SaveGame();
            SetData(SaveLoadManager.CurrentSave.ToolShopData);
        }
    }

    public void BuyTool(ToolOffer offer, ToolBuff tool) {
        var config = ToolsTable.ToolByType(tool);
        if (InventoryManager.Instance.EnoughMoney(config.cost)) {
            InventoryManager.Instance.BuyTool(config.buff, config.cost, config.buyAmount);
            if (_toolBuff1 == tool) {
                _fistActive = false;
                SaveLoadManager.CurrentSave.ToolShopData.FirstOfferActive = false;
            } else {
                _secondActive = false;
                SaveLoadManager.CurrentSave.ToolShopData.SecondOfferActive = false;
            }

            offer.gameObject.SetActive(false);
            StartCoroutine(Buying());
            SaveLoadManager.SaveGame();
            UpdateNoToolsMessage();
        }
    }

    private void UpdateNoToolsMessage() {
        _noToolsText.gameObject.SetActive(!_fistActive && !_secondActive);
    }

    public void OnMovedToBag() {
        _waitingToolMovedToBag = false;
        _toolBox.SetActive(false);
    }

    private IEnumerator Buying() {
        _toolBox.SetActive(true);
        _exitButton.SetActive(false);
        _waitingToolMovedToBag = true;
        _landingPlatformAnimation.Play("LandingPlatformPrepare");
        _tabletAnimation.Play("TabletHide");
        yield return new WaitWhile(() => _landingPlatformAnimation.isPlaying);
        _landingPlatformAnimation.Play("LandingPlatformLand");
        yield return new WaitWhile(() => _landingPlatformAnimation.isPlaying);
        _bagAnimation.Play("Show");
        yield return new WaitWhile(() => _waitingToolMovedToBag);
        _bagAnimation.Play("Hide");
        yield return new WaitWhile(() => _bagAnimation.isPlaying);
        _tabletAnimation.Play("TabletShow");
        _landingPlatformAnimation.Play("LandingPlatformIdle");
        _exitButton.SetActive(true);
    }
}