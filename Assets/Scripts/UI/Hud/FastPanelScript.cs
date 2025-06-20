using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class FastPanelScript : MonoBehaviour {
        public Button[] toolButtons;
        public Sprite HoeNormalSprite, WatercanNormalSprite, SeedNormalSprite, ScytheNormalSprite, CalendarNormalSprite;
        public Image HoeImage, WatercanImage, SeedImage, ScytheImage;
        public TextMeshProUGUI SeedText;
        public Image[] SlotsImages;
        public Sprite[] SlotsNormal, SlotsPressed;

        [SerializeField]
        private List<FastPanelSlotView> _fastPanelSlots;

        [SerializeField]
        private FastPanelSlotView _normalScythe, _goldenScythe;

        private Backpack _backpack;
        private Image _calendarImage;
        private Crop _curCropSeed;

        public void Init() {
            _calendarImage = UIHud.Instance.TimePanel.CalendarImage;
            _backpack = UIHud.Instance.Backpack;
        }

        public void UpdateGoldenScytheState() {
            bool isGolden = RealShopUtils.IsGoldenScytheActive(SaveLoadManager.CurrentSave.RealShopData);
            _goldenScythe.gameObject.SetActive(isGolden);
            _normalScythe.gameObject.SetActive(!isGolden);

            _fastPanelSlots[3] = isGolden ? _goldenScythe : _normalScythe;
        }

        //Переделать в единый метод для всех инструментов
        public void UpdateToolsImages() {
            HoeImage.sprite = HoeNormalSprite;
            WatercanImage.sprite = WatercanNormalSprite;

            ScytheImage.sprite = ScytheNormalSprite;
            _calendarImage.sprite = CalendarNormalSprite;

            foreach (ToolBuff type in InventoryManager.ToolsActivated.Keys) {
                ToolConfig tool = ToolsTable.ToolByType(type);
                Image toChange = null;
                switch (tool.toolUIType) {
                    case ToolUIType.Hoe:
                        toChange = HoeImage;
                        break;

                    case ToolUIType.Watercan:
                        toChange = WatercanImage;
                        break;

                    case ToolUIType.Seed:
                        break;

                    case ToolUIType.Scythe:
                        toChange = ScytheImage;
                        break;

                    case ToolUIType.Calendar:
                        toChange = _calendarImage;
                        break;
                }

                if (tool.toolUIType == ToolUIType.Seed) _backpack.SetBuffed(InventoryManager.Instance.IsToolWorking(type));

                if (InventoryManager.Instance.IsToolWorking(type) && toChange != null) {
                    toChange.sprite = tool.buffedIcon;
                }
            }
        }

        public void UpdateHover(int index) {
            foreach (var VARIABLE in _fastPanelSlots) {
                VARIABLE.SetIsPressed(false);
            }

            _fastPanelSlots[index].SetIsPressed(true);
        }

        public void ChangeSeedFastPanel(Crop crop, int amount) {
            SeedText.text = amount.ToString();
            SeedImage.sprite = CropsTable.CropByType(crop).SeedSprite;
            _curCropSeed = crop;
        }

        public void UpdateSeedFastPanel(Crop crop, int amount) {
            if (_curCropSeed == crop)
                SeedText.text = amount.ToString();
        }

        public void ChangeTool(int index) {
            if ((Tool)index == Tool.SeedBag && PlayerController.Instance.CurTool == Tool.SeedBag) {
                UIHud.Instance.Backpack.OpenFromSeedClick();
                return;
            }

            PlayerController.Instance.ChangeTool(index);
        }
    }
}