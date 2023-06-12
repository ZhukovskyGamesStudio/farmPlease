using UnityEngine;
using UnityEngine.UI;

public class FastPanelScript : MonoBehaviour {
    public Button[] toolButtons;
    public Sprite HoeNormalSprite, WatercanNormalSprite, SeedNormalSprite, ScytheNormalSprite, CalendarNormalSprite;
    public Image HoeImage, WatercanImage, SeedImage, ScytheImage;
    public Text SeedText;
    public Image[] SlotsImages;
    public Sprite[] SlotsNormal, SlotsPressed;
    private Backpack Backpack;
    private Image CalendarImage;

    private CropsType curCropSeed;

    public void Init() {
        CalendarImage = UIHud.Instance.TimePanel.CalendarImage;
        Backpack = UIHud.Instance.Backpack;
    }

    //Переделать в единый метод для всех инструментов
    public void UpdateToolsImages() {
        HoeImage.sprite = HoeNormalSprite;
        WatercanImage.sprite = WatercanNormalSprite;

        ScytheImage.sprite = ScytheNormalSprite;
        CalendarImage.sprite = CalendarNormalSprite;

        foreach (ToolBuff type in InventoryManager.instance.toolsInventory.Keys) {
            ToolSO tool = ToolsTable.ToolByType(type);
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
                    toChange = CalendarImage;
                    break;
            }

            if (tool.toolUIType == ToolUIType.Seed) Backpack.SetBuffed(InventoryManager.instance.IsToolWorking(type));

            if (InventoryManager.instance.IsToolWorking(type))
                toChange.sprite = tool.buffedIcon;
        }
    }

    public void UpdateHover(int index) {
        for (int i = 0; i < toolButtons.Length; i++)
            SlotsImages[i].sprite = SlotsNormal[i];

        SlotsImages[index].sprite = SlotsPressed[index];
    }

    public void ChangeSeedFastPanel(CropsType crop, int amount) {
        SeedText.text = amount.ToString();
        SeedImage.sprite = CropsTable.CropByType(crop).SeedSprite;
        curCropSeed = crop;
    }

    public void UpdateSeedFastPanel(CropsType crop, int amount) {
        if (curCropSeed == crop)
            SeedText.text = amount.ToString();
    }
}