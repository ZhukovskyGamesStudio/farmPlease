using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedShopScript : MonoBehaviour {
    public GameObject SeedOfferPrefab;

    public RectTransform[] slotPosition;
    public int ChangeCost;

    public CropsTable CropsTablePrefab;

    public GameObject ChangeSeedsButton;

    [Header("Ambar seed")]
    public CropsType AmbarType;

    public GameObject ambarSeed;
    public Button ambarBuyButton;
    public Image ambarSprite;
    private Dictionary<CropsType, GameObject> buttonsD;

    private GameObject[] SeedButtons;

    private void GenerateButtons() {
        SeedButtons = new GameObject[CropsTablePrefab.Crops.Length];
        buttonsD = new Dictionary<CropsType, GameObject>();

        for (int i = 0; i < SeedButtons.Length; i++) {
            CropSO crop = CropsTablePrefab.Crops[i];

            SeedButtons[i] = Instantiate(SeedOfferPrefab, transform);

            SeedOffer seedOffer = SeedButtons[i].GetComponent<SeedOffer>();
            seedOffer.costText.text = crop.cost.ToString();
            seedOffer.explainText.text = crop.explainText;
            seedOffer.OfferImage.sprite = crop.SeedSprite;
            seedOffer.BuyButton.onClick.AddListener(() =>
                InventoryManager.instance.BuySeed(crop.type, crop.cost, crop.buyAmount));
            seedOffer.BuyButton.onClick.AddListener(() => Audio.Instance.PlaySound(Sounds.Button));

            if (crop.Rarity == 1)
                seedOffer.RareEdge.SetActive(true);
            else if (crop.Rarity == 2)
                seedOffer.LegendaryEdge.SetActive(true);

            SeedButtons[i].SetActive(false);
            buttonsD.Add(crop.type, SeedButtons[i]);
        }

        ambarSeed.SetActive(false);
        AmbarType = CropsType.Weed;
    }

    public bool[] GetButtonsData() {
        bool[] buttons = new bool[SeedButtons.Length];
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = SeedButtons[i].activeSelf;
        return buttons;
    }

    public int GetAmbarSeedData() {
        if (AmbarType == CropsType.Weed)
            return -1;
        return (int) AmbarType;
    }

    public void SetSeedShopWithData(bool[] buttons, bool isChangeButtonActive, int ambarCropType) {
        GenerateButtons();

        int poscounter = 0;
        for (int i = 0; i < buttons.Length; i++) {
            SeedButtons[i].SetActive(buttons[i]);
            if (buttons[i]) {
                SeedButtons[i].transform.position = slotPosition[poscounter].position;
                poscounter++;
            }
        }

        if (ambarCropType == -1)
            ambarSeed.SetActive(false);
        else
            SetAmbarCrop((CropsType) ambarCropType);

        ChangeSeedsButton.SetActive(isChangeButtonActive);
    }

    public void SetAmbarCrop(CropsType type) {
        AmbarType = type;
        if (type == CropsType.Weed) {
            ambarSeed.SetActive(false);
            return;
        }

        AmbarType = type;
        ambarSeed.SetActive(true);
        CropSO crop = CropsTable.CropByType(type);
        ambarSprite.sprite = crop.SeedSprite;
        ambarBuyButton.onClick.AddListener(
            () => InventoryManager.instance.BuySeed(crop.type, crop.cost, crop.buyAmount));
    }

    private void ChangeSeeds() {
        if (SeedButtons == null)
            GenerateButtons();
        List<GameObject> buttons = new();
        foreach (CropsType key in buttonsD.Keys)
            //Здесь должна быть двойная проверка: если всегда доступен ИЛИ уже куплен
            if (CropsTable.ContainCrop(key)) {
                if (CropsTable.CropByType(key).CanBeBought) {
                    buttons.Add(buttonsD[key]);
                    buttonsD[key].SetActive(false);
                }

                if (InventoryManager.instance.isCropsBoughtD.ContainsKey(key))
                    if (InventoryManager.instance.isCropsBoughtD[key]) {
                        buttons.Add(buttonsD[key]);
                        buttonsD[key].SetActive(false);
                    }
            }

        for (int i = 0; i < 2; i++) {
            int x = Random.Range(0, buttons.Count);
            GameObject button = buttons[x];
            buttons.Remove(button);

            button.SetActive(true);
            button.transform.position = slotPosition[i].position;
        }
    }

    public void ChangeSeedsByButton() {
        if (InventoryManager.instance.coins >= ChangeCost) {
            InventoryManager.instance.AddCoins(-1 * ChangeCost);
            ChangeSeeds();
            ChangeSeedsButton.SetActive(false);
        }
    }

    public void ChangeSeedsNewDay() {
        ChangeSeeds();
        ChangeSeedsButton.SetActive(true);
    }
}