using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI
{
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

        [SerializeField] private Button _openButton;

        public Button OpenButton => _openButton;
        private bool _isBuffed;
        private List<GameObject> _seeds;
        public Button TomatoButton { get; private set; }
        [HideInInspector]
        public bool IsLockOpenCloseByFtue;
        
        public void OpenClose() {
            if (IsLockOpenCloseByFtue) {
                return;
            }
            isOpen = !isOpen;
            UpdateSprite();
            SeedsPanel.SetActive(isOpen);
        }
        
        private void CloseBySelectedSeed() {
            isOpen = false;
            UpdateSprite();
            SeedsPanel.SetActive(isOpen);
        }

        private void UpdateSprite() {
            if (_isBuffed)
                BackPackImage.sprite = isOpen ? openedBuffed : closedBuffed;
            else
                BackPackImage.sprite = isOpen ? opened : closed;
        }

        public void SetBuffed(bool isOn) {
            _isBuffed = isOn;
            UpdateSprite();
        }

        private void GenerateSeedButtons() {
            _seeds = new();

            for (int i = 0; i < CropsTablePrefab.Crops.Length; i++) {
                CropConfig crop = CropsTablePrefab.Crops[i];

                GameObject button = Instantiate(SeedPrefab, SeedsGrid.transform);
                SeedView seedView = button.GetComponent<SeedView>();

                seedView.amountText.text = "0";
                seedView.crop = crop.type;
                seedView.SeedImage.sprite = crop.SeedSprite;
                seedView.button.onClick.AddListener(() => InventoryManager.Instance.ChooseSeed(crop.type));
                seedView.button.onClick.AddListener(() => CloseBySelectedSeed());
                seedView.button.onClick.AddListener(() => PlayerController.Instance.ChangeTool(2));
                seedView.gameObject.SetActive(false);
                _seeds.Add(button);
                if (crop.type == Crop.Tomato) {
                    TomatoButton = button.GetComponent<Button>();
                }
            }
        }

        public void UpdateGrid(SerializableDictionary<Crop, int> seedsInventory) {
            if (_seeds == null)
                GenerateSeedButtons();

            int itemsToShow = 0;

            for (int i = 0; i < _seeds.Count; i++) {
                int amount = 0;
                if (seedsInventory.ContainsKey(_seeds[i].GetComponent<SeedView>().crop))
                    amount = seedsInventory[_seeds[i].GetComponent<SeedView>().crop];

                if (amount > 0) {
                    itemsToShow++;
                    _seeds[i].SetActive(true);
                    _seeds[i].GetComponent<SeedView>().amountText.text = amount.ToString();
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
}