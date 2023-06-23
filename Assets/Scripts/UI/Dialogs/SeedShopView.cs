using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class SeedShopView : MonoBehaviour {
    [SerializeField] private Button _closeButton;
    public Button CloseButton => _closeButton;
    public SeedOffer SeedOfferPrefab;

    public RectTransform[] slotPosition;
    public int ChangeCost;

    public CropsTable CropsTablePrefab;

    public GameObject ChangeSeedsButton;

    public GameObject ambarSeed;
    public Button ambarBuyButton;
    public Image ambarSprite;
    private Dictionary<Crop, SeedOffer> _buttonsD;

    private SeedOffer[] _seedButtons;
    public  List<SeedOffer> CurrentButtons { get; private set; } 

    private Crop Ambar => SaveLoadManager.CurrentSave.AmbarCrop;

    private void GenerateButtons() {
        _seedButtons = new SeedOffer[CropsTablePrefab.Crops.Length];
        _buttonsD = new Dictionary<Crop, SeedOffer>();

        for (int i = 0; i < _seedButtons.Length; i++) {
            CropConfig crop = CropsTablePrefab.Crops[i];

            _seedButtons[i] = Instantiate(SeedOfferPrefab, transform);

            SeedOffer seedOffer = _seedButtons[i].GetComponent<SeedOffer>();
            seedOffer.costText.text = crop.cost.ToString();
            seedOffer.explainText.text = crop.explainText;
            seedOffer.OfferImage.sprite = crop.SeedSprite;
            seedOffer.BuyButton.onClick.AddListener(() =>
                InventoryManager.Instance.BuySeed(crop.type, crop.cost, crop.buyAmount));
            seedOffer.BuyButton.onClick.AddListener(() => Audio.Instance.PlaySound(Sounds.Button));

            if (crop.Rarity == 1)
                seedOffer.RareEdge.SetActive(true);
            else if (crop.Rarity == 2)
                seedOffer.LegendaryEdge.SetActive(true);

            _seedButtons[i].gameObject.SetActive(false);
            _buttonsD.Add(crop.type, _seedButtons[i]);
        }

        ambarSeed.SetActive(false);
    }

    public bool[] GetButtonsData() {
        bool[] buttons = new bool[_seedButtons.Length];
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = _seedButtons[i].gameObject.activeSelf;
        return buttons;
    }

    public void SetSeedShopWithData(GameSaveProfile save) {
        bool[] buttons = save.SeedShopButtonData;
        bool isChangeButtonActive = save.SeedShopChangeButton;
        GenerateButtons();

        int poscounter = 0;
        for (int i = 0; i < buttons.Length; i++) {
            _seedButtons[i].gameObject.SetActive(buttons[i]);
            if (buttons[i]) {
                _seedButtons[i].transform.position = slotPosition[poscounter].position;
                poscounter++;
            }
        }

        if (save.AmbarCrop == Crop.None)
            ambarSeed.SetActive(false);
        else
            SetAmbarCrop(save.AmbarCrop);

        ChangeSeedsButton.SetActive(isChangeButtonActive);
    }

    public void SetSeedsShop(Crop first, Crop second) {
        int index = 0;
        CurrentButtons = new List<SeedOffer>();
        foreach (var VARIABLE in _buttonsD.Keys) {
            bool isActive = VARIABLE == first || VARIABLE == second;
            _buttonsD[VARIABLE].gameObject.SetActive(isActive);
            if (isActive) {
                CurrentButtons.Add(_buttonsD[VARIABLE]);
                _buttonsD[VARIABLE].transform.position = slotPosition[index].position;
                index++;
            }
        }
    }

    public void SetAmbarCrop(Crop type) {
        SaveLoadManager.CurrentSave.AmbarCrop = type;
        if (type == Crop.Weed) {
            ambarSeed.SetActive(false);
            return;
        }

        ambarSeed.SetActive(true);
        CropConfig crop = CropsTable.CropByType(type);
        ambarSprite.sprite = crop.SeedSprite;
        ambarBuyButton.onClick.AddListener(
            () => InventoryManager.Instance.BuySeed(crop.type, crop.cost, crop.buyAmount));
    }

    private void ChangeSeeds() {
        if (_seedButtons == null)
            GenerateButtons();
        List<SeedOffer> buttons = new();
        CurrentButtons = new List<SeedOffer>();
        foreach (Crop key in _buttonsD.Keys)
            //Здесь должна быть двойная проверка: если всегда доступен ИЛИ уже куплен
            if (CropsTable.ContainCrop(key)) {
                if (CropsTable.CropByType(key).CanBeBought) {
                    buttons.Add(_buttonsD[key]);
                    _buttonsD[key].gameObject.SetActive(false);
                }

                if (InventoryManager.Instance.IsCropsBoughtD.ContainsKey(key))
                    if (InventoryManager.Instance.IsCropsBoughtD[key]) {
                        buttons.Add(_buttonsD[key]);
                        _buttonsD[key].gameObject.SetActive(false);
                    }
            }

        for (int i = 0; i < 2; i++) {
            int x = Random.Range(0, buttons.Count);
            SeedOffer button = buttons[x];
            CurrentButtons.Add(button);
            buttons.Remove(button);

            button.gameObject.SetActive(true);
            button.transform.position = slotPosition[i].position;
        }
    }

    public void ChangeSeedsByButton() {
        if (SaveLoadManager.CurrentSave.Coins >= ChangeCost) {
            InventoryManager.Instance.AddCoins(-1 * ChangeCost);
            ChangeSeeds();
            ChangeSeedsButton.SetActive(false);
        }
    }

    public void ChangeSeedsNewDay() {
        ChangeSeeds();
        ChangeSeedsButton.SetActive(true);
    }
}