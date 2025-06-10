using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SeedShopDialog : DialogWithData<SeedShopData> {
    [SerializeField]
    private Button _closeButton;

    public Button CloseButton => _closeButton;

    public GameObject ChangeSeedsButtonActive;

    [SerializeField]
    private TextMeshProUGUI _changeSeedsCost;

    public CanvasGroup FirstBagCanvas => _firstOffer.CanvasGroup;
    public CanvasGroup SecondBagCanvas => _secondOffer.CanvasGroup;

    [SerializeField]
    private SeedOffer _firstOffer, _secondOffer, _ambarOffer;

    [SerializeField]
    private Animation _cartAnimation;

    [SerializeField]
    private Animation _mainAnimation;

    [SerializeField]
    private Animation _buyingBagAnimation;

    private SeedShopData _data;

    public override void SetData(SeedShopData data) {
        _data = data;
        SetSeedShopWithData(_data);
    }

    public override void Show(Action onClose) {
        base.Show(onClose);
        if (_data.NeedShowChange) {
            _mainAnimation.Play("HideUIInstant");
            StartCoroutine(ShowChangeEndAnimation());
            SaveLoadManager.CurrentSave.SeedShopData.NeedShowChange = false;
        }
    }

    public void SetSeedShopWithData(SeedShopData save) {
        SetSeedsShop(save.FirstOffer, save.SecondOffer);

        if (save.AmbarCrop == Crop.None) {
            _ambarOffer.gameObject.SetActive(false);
        } else {
            SetAmbarCrop(save.AmbarCrop);
        }

        ChangeSeedsButtonActive.SetActive(save.ChangeButtonActive);
        _changeSeedsCost.text = ConfigsManager.Instance.CostsConfig.SeedsShopChangeCost.ToString();
    }

    public void SetSeedsShop(Crop first, Crop second) {
        SetOffer(_firstOffer, first);
        SetOffer(_secondOffer, second);
    }

    private void SetOffer(SeedOffer offer, Crop crop) {
        CropConfig cropConfig = CropsTable.CropByType(crop);
        offer.SetData(cropConfig, delegate {
            Audio.Instance.PlaySound(Sounds.Button);
            return InventoryManager.Instance.TryBuySeed(cropConfig.type, cropConfig.cost, cropConfig.buyAmount);
        }, CloseHints, _buyingBagAnimation.transform);
    }

    public void SetAmbarCrop(Crop type) {
        SaveLoadManager.CurrentSave.AmbarCrop = type;
        if (type == Crop.Weed) {
            _ambarOffer.gameObject.SetActive(false);
            return;
        }

        SetOffer(_ambarOffer, type);
        _ambarOffer.gameObject.SetActive(true);
    }

    private void ChangeSeeds() {
        List<Crop> possibleCrops = new();
        foreach (CropConfig key in CropsTable.Instance.Crops) {
            if (!UnlockableUtils.HasUnlockable(key.type)) {
                continue;
            }

            if (key.CanBeBought) {
                possibleCrops.Add(key.type);
            } else if (InventoryManager.IsCropsBoughtD.ContainsKey(key.type) && InventoryManager.IsCropsBoughtD[key.type]) {
                possibleCrops.Add(key.type);
            }
        }

        if (possibleCrops.Count == 1) {
            SetSeedsShop(possibleCrops[0], possibleCrops[0]);
            return;
        }

        Crop firstCrop = possibleCrops[Random.Range(0, possibleCrops.Count)];
        possibleCrops.Remove(firstCrop);
        Crop secondCrop = possibleCrops[Random.Range(0, possibleCrops.Count)];
        SetSeedsShop(firstCrop, secondCrop);
    }

    private IEnumerator ShowUI() {
        _mainAnimation.Play("ShowUI");
        yield return new WaitWhile(() => _mainAnimation.isPlaying);
    }

    private IEnumerator HideUI() {
        _mainAnimation.Play("HideUI");
        yield return new WaitWhile(() => _mainAnimation.isPlaying);
    }

    private IEnumerator ShowChangeAnimation() {
        CloseHints();
        PlayerController.CanInteract = false;
        StartCoroutine(HideUI());
        _cartAnimation.Play("CartChangeStart");
        yield return new WaitWhile(() => _cartAnimation.isPlaying);
        ChangeSeeds();
        yield return ShowChangeEndAnimation();
    }

    private IEnumerator ShowChangeEndAnimation() {
        PlayerController.CanInteract = false;
        _cartAnimation.Play("CartChangeEnd");
        yield return new WaitWhile(() => _cartAnimation.isPlaying);
        _cartAnimation.Play("CartShowUI");
        StartCoroutine(ShowUI());
        PlayerController.CanInteract = true;
    }

    public void ChangeSeedsByButton() {
        if (InventoryManager.Instance.EnoughMoney(ConfigsManager.Instance.CostsConfig.SeedsShopChangeCost)) {
            InventoryManager.Instance.AddCoins(-1 * ConfigsManager.Instance.CostsConfig.SeedsShopChangeCost);
            StartCoroutine(ShowChangeAnimation());
            ChangeSeedsButtonActive.SetActive(false);
            SaveLoadManager.CurrentSave.SeedShopData.ChangeButtonActive = false;
        }
    }

    public void SetBuyingBag(bool isActive) {
        _buyingBagAnimation.Play(isActive ? "Show" : "Hide");
    }

    public void ShowHint(int hintIndex) {
        CloseHints();
        switch (hintIndex) {
            case 0:
                _firstOffer.ShowHint();
                break;
            case 1:
                _secondOffer.ShowHint();
                break;
            case 2:
                _ambarOffer.ShowHint();
                break;
        }
    }

    public void CloseHints() {
        _firstOffer.CloseHint();
        _secondOffer.CloseHint();
        _ambarOffer.CloseHint();
    }

    public override void Close() {
        CloseHints();
        base.Close();
    }
}