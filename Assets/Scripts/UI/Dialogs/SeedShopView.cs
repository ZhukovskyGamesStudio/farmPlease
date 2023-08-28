using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SeedShopView : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    public Button CloseButton => _closeButton;
    public int ChangeCost;

    public CropsTable CropsTablePrefab;

    public GameObject ChangeSeedsButton;

    public GameObject ambarSeed;
    public Button ambarBuyButton;
    public Image ambarSprite;

    public TextMeshProUGUI _coinsCounter;
    public CanvasGroup FirstBagCanvas => _firstOffer.CanvasGroup;
    public CanvasGroup SecondBagCanvas => _secondOffer.CanvasGroup;

    [SerializeField] private SeedOffer _firstOffer, _secondOffer;

    [SerializeField]
    private Animation _cartAnimation;
    [SerializeField]
    private Animation _mainAnimation;
    private Crop Ambar => SaveLoadManager.CurrentSave.AmbarCrop;

    [SerializeField] private Animation _buyingBagAnimation;
    private bool _isShowChangeNeeded;
    private void OnEnable()
    {
        if (_isShowChangeNeeded)
        {
            _mainAnimation.Play("HideUIInstant");
            StartCoroutine(ShowChangeEndAnimation());
            _isShowChangeNeeded = false;
        }
    }

    public void GetButtonsData(out Crop first, out Crop second)
    {
        first = _firstOffer.CurrentCrop;
        second = _secondOffer.CurrentCrop;
    }

    public void SetSeedShopWithData(GameSaveProfile save)
    {
        bool isChangeButtonActive = save.SeedShopChangeButton;

        SetSeedsShop(save.ShopFirstOffer, save.ShopSecondOffer);

        if (save.AmbarCrop == Crop.None)
            ambarSeed.SetActive(false);
        else
            SetAmbarCrop(save.AmbarCrop);

        ChangeSeedsButton.SetActive(isChangeButtonActive);
    }

    public void SetSeedsShop(Crop first, Crop second)
    {
        SetOffer(_firstOffer, first);
        SetOffer(_secondOffer, second);
    }

    private void SetOffer(SeedOffer offer, Crop first)
    {
        CropConfig cropConfig = CropsTable.CropByType(first);
        offer.SetData(cropConfig, delegate
        {
            InventoryManager.Instance.BuySeed(cropConfig.type, cropConfig.cost, cropConfig.buyAmount);
            _coinsCounter.text = SaveLoadManager.CurrentSave.Coins.ToString();
            Audio.Instance.PlaySound(Sounds.Button);
        }, CloseHints);
    }

    public void SetAmbarCrop(Crop type)
    {
        SaveLoadManager.CurrentSave.AmbarCrop = type;
        if (type == Crop.Weed)
        {
            ambarSeed.SetActive(false);
            return;
        }

        ambarSeed.SetActive(true);
        CropConfig crop = CropsTable.CropByType(type);
        ambarSprite.sprite = crop.SeedSprite;
        ambarBuyButton.onClick.AddListener(
            () => InventoryManager.Instance.BuySeed(crop.type, crop.cost, crop.buyAmount));
    }

    private void ChangeSeeds()
    {
        List<Crop> possibleCrops = new();
        foreach (CropConfig key in CropsTable.Instance.Crops)
        {
            if (key.CanBeBought)
            {
                possibleCrops.Add(key.type);
            }
            else if (InventoryManager.Instance.IsCropsBoughtD.ContainsKey(key.type) &&
                     InventoryManager.Instance.IsCropsBoughtD[key.type])
            {
                possibleCrops.Add(key.type);
            }
        }


        Crop firstCrop = possibleCrops[Random.Range(0, possibleCrops.Count)];
        possibleCrops.Remove(firstCrop);
        Crop secondCrop = possibleCrops[Random.Range(0, possibleCrops.Count)];
        SetSeedsShop(firstCrop, secondCrop);
    }

    private IEnumerator ShowUI()
    {
        _mainAnimation.Play("ShowUI");
        yield return new WaitWhile(() => _mainAnimation.isPlaying);
    }
    private IEnumerator HideUI()
    {
        _mainAnimation.Play("HideUI");
        yield return new WaitWhile(() => _mainAnimation.isPlaying);
    }
    private IEnumerator ShowChangeAnimation()
    {
        PlayerController.CanInteract = false;
        StartCoroutine(HideUI());
        _cartAnimation.Play("CartChangeStart");
        yield return new WaitWhile(() => _cartAnimation.isPlaying);
        ChangeSeeds();
        yield return ShowChangeEndAnimation();
    }

    private IEnumerator ShowChangeEndAnimation()
    {
        PlayerController.CanInteract = false;
        _cartAnimation.Play("CartChangeEnd");
        yield return new WaitWhile(() => _cartAnimation.isPlaying);
        _cartAnimation.Play("CartShowUI");
        StartCoroutine(ShowUI());
        PlayerController.CanInteract = true;
    }

    public void ChangeSeedsByButton()
    {
        if (SaveLoadManager.CurrentSave.Coins >= ChangeCost)
        {
            InventoryManager.Instance.AddCoins(-1 * ChangeCost);
            StartCoroutine(ShowChangeAnimation());
            ChangeSeedsButton.SetActive(false);
        }
    }

    public void ChangeSeedsNewDay()
    {
        _isShowChangeNeeded = true;
        ChangeSeeds();
        ChangeSeedsButton.SetActive(true);
    }

    public void SetBuyingBag(bool isActive)
    {
        _buyingBagAnimation.Play(isActive ? "Show" : "Hide");
    }

    public void ShowHint(int hintIndex)
    {
        CloseHints();
        if (hintIndex == 0)
        {
            _firstOffer.ShowHint();
        }

        if (hintIndex == 1)
        {
            _secondOffer.ShowHint();
        }
    }

    public void CloseHints()
    {
        _firstOffer.CloseHint();
        _secondOffer.CloseHint();
    }
}