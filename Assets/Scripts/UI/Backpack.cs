using System.Collections.Generic;
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

    private bool isBuffed;
    private PlayerController PlayerController;
    private GameObject[] seeds;

    private void Start() {
        PlayerController = PlayerController.Instance;
    }

    public void OpenClose() {
        isOpen = !isOpen;
        if (isBuffed)
            BackPackImage.sprite = isOpen ? openedBuffed : closedBuffed;
        else
            BackPackImage.sprite = isOpen ? opened : closed;
        SeedsPanel.SetActive(isOpen);
    }

    public void SetBuffed(bool isOn) {
        isBuffed = isOn;
        if (isBuffed)
            BackPackImage.sprite = isOpen ? openedBuffed : closedBuffed;
        else
            BackPackImage.sprite = isOpen ? opened : closed;
    }

    private void GenerateSeedButtons() {
        List<GameObject> buttonsList = new();

        for (int i = 0; i < CropsTablePrefab.Crops.Length; i++) {
            CropSO crop = CropsTablePrefab.Crops[i];

            GameObject button = Instantiate(SeedPrefab, SeedsGrid.transform);
            Seed seed = button.GetComponent<Seed>();

            seed.amountText.text = "0";
            seed.CropType = crop.type;
            seed.SeedImage.sprite = crop.SeedSprite;
            seed.button.onClick.AddListener(() => InventoryManager.instance.ChooseSeed(crop.type));
            seed.button.onClick.AddListener(() => OpenClose());
            seed.button.onClick.AddListener(() => PlayerController.ChangeTool(2));
            seed.gameObject.SetActive(false);
            buttonsList.Add(button);
        }

        seeds = buttonsList.ToArray();
    }

    public void UpdateGrid(Dictionary<CropsType, int> seedsInventory) {
        if (seeds == null)
            GenerateSeedButtons();

        int itemsToShow = 0;

        for (int i = 0; i < seeds.Length; i++) {
            int amount = 0;
            if (seedsInventory.ContainsKey(seeds[i].GetComponent<Seed>().CropType))
                amount = seedsInventory[seeds[i].GetComponent<Seed>().CropType];

            if (amount > 0) {
                itemsToShow++;
                seeds[i].SetActive(true);
                seeds[i].GetComponent<Seed>().amountText.text = amount.ToString();
            } else {
                seeds[i].SetActive(false);
            }
        }

        if (itemsToShow < 4)
            SeedsGrid.constraintCount = itemsToShow;
        else
            SeedsGrid.constraintCount = 4;
    }
}