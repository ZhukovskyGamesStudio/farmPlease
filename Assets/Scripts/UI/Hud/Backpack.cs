using System;
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
        [SerializeField]
        private BackpackItem _backpackItemPrefab;
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
        private Dictionary<string, BackpackItem> _backpackItemsViews;
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
        
        private void CloseBySelectedItem() {
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
            _backpackItemsViews = new();

            for (int i = 0; i < CropsTablePrefab.Crops.Length; i++) {
                CropConfig crop = CropsTablePrefab.Crops[i];

                BackpackItem backpackItem = Instantiate(_backpackItemPrefab, SeedsGrid.transform);

                backpackItem.InitButton(0, crop.SeedSprite, () => {
                    InventoryManager.Instance.ChooseSeed(crop.type);
                    CloseBySelectedItem();
                    PlayerController.Instance.ChangeTool(2);
                }, () => SaveLoadManager.CurrentSave.Seeds.SafeGet(crop.type, 0),
                    ItemColorType.Seed);
                backpackItem.gameObject.SetActive(false);
                _backpackItemsViews.Add(crop.type.ToString(), backpackItem);
                if (crop.type == Crop.Tomato) {
                    TomatoButton = backpackItem.GetComponent<Button>();
                }
            }
        }

        private void GenerateToolsButtons() {
            var cnfg = ToolsTable.Instance.ToolsSO;
            for (int i = 0; i < cnfg.Length; i++) {
                ToolConfig tool = cnfg[i];

                BackpackItem backpackItem = Instantiate(_backpackItemPrefab, SeedsGrid.transform);

                backpackItem.InitButton(0, tool.gridIcon, () => {
                    ShowConfirmDialog(delegate {
                        InventoryManager.Instance.ActivateTool(tool.buff);
                        CloseBySelectedItem();
                    });
                }, () => SaveLoadManager.CurrentSave.ToolBuffsStored.SafeGet(tool.buff, 0),
                    ItemColorType.Tool);
                backpackItem.gameObject.SetActive(false);
                _backpackItemsViews.Add(tool.buff.ToString(), backpackItem);
            }
        }

        public void UpdateGrid(SerializableDictionary<Crop, int> seedsInventory) {
            if (_backpackItemsViews == null) {
                GenerateSeedButtons();
                GenerateToolsButtons();
            }
              

            int itemsToShow = 0;

            foreach (BackpackItem item in _backpackItemsViews.Values) {
                int newAmount = item.SyncAmount();
                if (newAmount > 0) {
                    itemsToShow++;
                }
            }

            AdjustGridSize(itemsToShow);
        }

        private void AdjustGridSize(int itemsToShow) {
            SeedsGrid.constraintCount = itemsToShow < 4 ? itemsToShow : 4;
        }

        private void ShowConfirmDialog(Action confirmedCallback) {
            confirmedCallback?.Invoke();
        }
    }
}