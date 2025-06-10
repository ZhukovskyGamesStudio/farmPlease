using System;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private Animation _animation;
        
        public void OpenClose() {
            if (IsLockOpenCloseByFtue) {
                return;
            }
            UIHud.Instance.BackpackAttention.Hide();
            isOpen = !isOpen;
            UpdateSprite();
            SeedsPanel.SetActive(isOpen);
        }

        public void OpenFromSeedClick() {
            isOpen = true;
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
            _backpackItemsViews = new Dictionary<string, BackpackItem>();

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
                     tool.buff == ToolBuff.WeekBattery ? ItemColorType.Energy : ItemColorType.Tool);
                backpackItem.gameObject.SetActive(false);
                _backpackItemsViews.Add(tool.buff.ToString(), backpackItem);
            }
        }
        
        private void GenerateBuildingsButtons() {
            var cnfg = BuildingsTable.Instance.Buildings;
            for (int i = 0; i < cnfg.Length; i++) {
                BuildingConfig building = cnfg[i];

                BackpackItem backpackItem = Instantiate(_backpackItemPrefab, SeedsGrid.transform);

                void StartBuilding() {
                    PlayerController.Instance.StartStopBuilding();
                    PlayerController.Instance.InitializeBuilding(building.type, true);
                    CloseBySelectedItem();
                }

                backpackItem.InitButton(0, building.gridIcon, () => {
                    ShowConfirmDialog(StartBuilding);
                }, () => SaveLoadManager.CurrentSave.BuildingsStored.Contains(building.type) ? 1 : 0, ItemColorType.Building);
                backpackItem.gameObject.SetActive(false);
                _backpackItemsViews.Add(building.type.ToString(), backpackItem);
            }
        }

        public void UpdateGrid() {
            if (_backpackItemsViews == null) {
                GenerateSeedButtons();
                GenerateToolsButtons();
                GenerateBuildingsButtons();
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

        public void ShowAddedAnimation() {
            _animation.Play("BackpackAddedAnimation");
        }
    }
}