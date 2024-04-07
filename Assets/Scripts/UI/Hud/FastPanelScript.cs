using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FastPanelScript : MonoBehaviour {
        public Button[] toolButtons;
        public Sprite HoeNormalSprite, WatercanNormalSprite, SeedNormalSprite, ScytheNormalSprite, CalendarNormalSprite;
        public Image HoeImage, WatercanImage, SeedImage, ScytheImage;
        public Text SeedText;
        public Image[] SlotsImages;
        public Sprite[] SlotsNormal, SlotsPressed;
        private Backpack _backpack;
        private Image _calendarImage;

        private Crop _curCropSeed;

        public void Init() {
            _calendarImage = UIHud.Instance.TimePanel.CalendarImage;
            _backpack = UIHud.Instance.Backpack;
        }

        //Переделать в единый метод для всех инструментов
        public void UpdateToolsImages() {
            HoeImage.sprite = HoeNormalSprite;
            WatercanImage.sprite = WatercanNormalSprite;

            ScytheImage.sprite = ScytheNormalSprite;
            _calendarImage.sprite = CalendarNormalSprite;

            foreach (ToolBuff type in InventoryManager.Instance.ToolsActivated.Keys) {
                ToolConfig tool = ToolsTable.ToolByType(type);
                Image toChange = HoeImage;
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

                if (InventoryManager.Instance.IsToolWorking(type))
                    toChange.sprite = tool.buffedIcon;
            }
        }

        public void UpdateHover(int index) {
            for (int i = 0; i < toolButtons.Length; i++)
                SlotsImages[i].sprite = SlotsNormal[i];

            SlotsImages[index].sprite = SlotsPressed[index];
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
    }
}