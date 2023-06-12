using System.Collections.Generic;
using DefaultNamespace.Abstract;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour {
    public GridLayoutGroup SeedsGrid;
    public GameObject SeedPrefab;
    public GameObject SeedsPanel;

    public Image BackPackImage;
    public Sprite closed, opened;
    public Sprite closedBuffed, openedBuffed;

    public CropsTable CropsTablePrefab;

    [HideInInspector]
    public bool isOpen;

    private bool _isBuffed;
    private GameObject[] _seeds;

    public void OpenClose() {
        isOpen = !isOpen;
        if (_isBuffed)
            BackPackImage.sprite = isOpen ? openedBuffed : closedBuffed;
        else
            BackPackImage.sprite = isOpen ? opened : closed;
        SeedsPanel.SetActive(isOpen);
    }

    public void SetBuffed(bool isOn) {
        _isBuffed = isOn;
        if (_isBuffed)
            BackPackImage.sprite = isOpen ? openedBuffed : closedBuffed;
        else
            BackPackImage.sprite = isOpen ? opened : closed;
    }

    private void GenerateSeedButtons() {
        List<GameObject> buttonsList = new();

        for (int i = 0; i < CropsTablePrefab.Crops.Length; i++) {
            CropConfig crop = CropsTablePrefab.Crops[i];

            GameObject button = Instantiate(SeedPrefab, SeedsGrid.transform);
            Seed seed = button.GetComponent<Seed>();

            seed.amountText.text = "0";
            seed.crop = crop.type;
            seed.SeedImage.sprite = crop.SeedSprite;
            seed.button.onClick.AddListener(() => InventoryManager.Instance.ChooseSeed(crop.type));
            seed.button.onClick.AddListener(() => OpenClose());
            seed.button.onClick.AddListener(() => PlayerController.Instance.ChangeTool(2));
            seed.gameObject.SetActive(false);
            buttonsList.Add(button);
        }

        _seeds = buttonsList.ToArray();
    }

    public void UpdateGrid(SerializableDictionary<Crop, int> seedsInventory) {
        if (_seeds == null)
            GenerateSeedButtons();

        int itemsToShow = 0;

        for (int i = 0; i < _seeds.Length; i++) {
            int amount = 0;
            if (seedsInventory.ContainsKey(_seeds[i].GetComponent<Seed>().crop))
                amount = seedsInventory[_seeds[i].GetComponent<Seed>().crop];

            if (amount > 0) {
                itemsToShow++;
                _seeds[i].SetActive(true);
                _seeds[i].GetComponent<Seed>().amountText.text = amount.ToString();
            } else {
                _seeds[i].SetActive(false);
            }
        }

        if (itemsToShow < 4)
            SeedsGrid.constraintCount = itemsToShow;
        else
            SeedsGrid.constraintCount = 4;
    }
}