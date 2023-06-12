using System;
using DefaultNamespace;
using DefaultNamespace.Abstract;
using DefaultNamespace.Managers;

namespace Managers {
    public class GameSceneEntryPoint : SceneEntryPoint {
        protected override void Start() {
            Init();
            LoadGame();

            Clock.Instance.TryRefillForRealtimePassed();
        }

        private static void Init() {
            PlayerController.Instance.Init();
            PlayerController.Instance.GetComponent<FastPanelScript>().Init();
            InventoryManager.instance.Init();
            TilesTable.instance.CreateDictionary();
        }

        private static void LoadGame() {
            if (GameModeManager.Instance.DoNotSave)
                SaveLoadManager.ClearSave();

            SaveLoadManager.LoadGame();

            SetLoadedData();
        }

        private static void SetLoadedData() {
            GameSaveProfile save = SaveLoadManager.CurrentSave;
            UIHud.Instance.ShopsPanel.seedShopView.SetSeedShopWithData(save);
            UIHud.Instance.ShopsPanel.toolShopView.SetToolShopWithData(save);
            Energy.Instance.SetEnergy(save.Energy);
            Clock.Instance.SetEnergy(save.ClockEnergy);

            SmartTilemap.instance.GenerateTilesWithData(save.TilesData);

            InventoryManager.instance.SetInventoryWithData(save);
            UIHud.Instance.FastPanelScript.UpdateToolsImages();
            
            UIHud.Instance.ShopsPanel.BuildingShopView.InitializeWithData(save.buildingPrice);
            
            // В этом методе запускаете ежесекудный корутин, который подсчитывает кол-во прошедших дней.

            DateTime dateTime = DateTime.Parse(save.Date);
            Time.Instance.SetDaysWithData(save.Days, dateTime);
        }
    }
}