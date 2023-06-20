using System;
using System.Globalization;
using System.IO;
using Database;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZhukovskyGamesPlugin;

namespace Managers
{
    public class SaveLoadManager : Singleton<SaveLoadManager> {
        public static int Profile = -2;

        private BuildingShopView _buildingShopView;
        private FastPanelScript _fastPanelScript;

        private InventoryManager _inventoryManager;
        private PlayerController _playerController;
        private SeedShopView _seedShop;
        private SmartTilemap _smartTilemap;

        private ToolShopView _toolShop;
        public static GameSaveProfile CurrentSave;

        public static string SaveDirectory => $"{Application.persistentDataPath}/saves";

        public static string SavePath => SaveDirectory + $"/save{Profile}.txt";

        // Пока в игре происходят какие-то действия, игрок не может ничего сделать
        // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке
        public void Sequence(bool isStart) {
            if (isStart) {
                PlayerController.CanInteract = false;
            } else {
                if (GameModeManager.Instance.GameMode == GameMode.Online) {
                    OnlineFarm.Instance.ChangeFarmAndPut(GenerateJsonString());
                } else {
                    PlayerController.CanInteract = true;
                    SaveGame();
                }
            }
        }

        public static string GenerateJsonString() {
            CurrentSave.SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);
        
            CurrentSave.TilesData = SmartTilemap.Instance.GetTilesData();

            CurrentSave.SeedShopButtonData = UIHud.Instance.ShopsPanel.seedShopView.GetButtonsData();
            CurrentSave.SeedShopChangeButton = UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.activeSelf;

            CurrentSave.ToolShopButtonsData = UIHud.Instance.ShopsPanel.toolShopView.GetButtons();
            CurrentSave.ToolShopChangeButton = UIHud.Instance.ShopsPanel.toolShopView.ChangeButton.activeSelf;

            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                CurrentSave.CropBoughtData = InventoryManager.Instance.GetIsBoughtData(0);
                CurrentSave.ToolBoughtData = InventoryManager.Instance.GetIsBoughtData(1);
                CurrentSave.BuildingBoughtData = InventoryManager.Instance.GetIsBoughtData(2);

                CurrentSave.BuildingPrice = UIHud.Instance.ShopsPanel.BuildingShopView.GetBuildingPrice();
            }

            return JsonUtility.ToJson(CurrentSave, false);
        }

        public void SaveGame() {
            GameModeManager gameModeManager = GameModeManager.Instance;
            if (gameModeManager.DoNotSave)
                return;

            string toSave = GenerateJsonString();

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            if (!File.Exists(SavePath))
                File.Create(SavePath).Close();

            File.WriteAllText(SavePath, toSave);
        }

        public static void LoadSavedData() {
            CurrentSave = GameSaveProfile.LoadFromFile(SavePath);
        }

        public static void LoadGame(string jsonString = null) {
            if (jsonString != null)
                CurrentSave = GameSaveProfile.LoadFromString(jsonString);
            else
                CurrentSave = GameSaveProfile.LoadFromFile(SavePath);

            if (CurrentSave == null) {
                GenerateGame();
                Debug.Instance.Log("Generating finished. Saving started");
                Instance.SaveGame();
                Debug.Instance.Log("New profile is saved");
            }
        }

        public static void GenerateGame() {
            CurrentSave = new GameSaveProfile() {
                Coins = 5,
                CropPoints = 0,
                SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture),
                Date = Time.FirstDayOfGame.ToString(CultureInfo.InvariantCulture),
                AmbarCrop =  Crop.None
            };

            SmartTilemap.Instance.GenerateTiles();
            InventoryManager.Instance.GenerateInventory();

            Energy.Instance.RefillEnergy();
            Clock.Instance.RefillToMaxEnergy();
            UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsNewDay();
            UIHud.Instance.ShopsPanel.toolShopView.ChangeTools();
            Time.Instance.GenerateDays(0);
        }

        public void ClearSaveAndReload() {
            ClearSave();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void ClearSave() {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }

        #region MySaveMthods

        /*
   void DeleteArray(string name)
   {
       int amount = PlayerPrefs.GetInt(name + "Amount_" + profile);
       PlayerPrefs.DeleteKey(name + "Amount_" + profile);
       for (int i = 0; i < amount; i++)
           PlayerPrefs.DeleteKey(name + i + "_" + profile);
   } */

        public static void SaveStringArray(string[] tosave, string name) {
            PlayerPrefs.SetInt(name + "Amount_" + Profile, tosave.Length);
            for (int i = 0; i < tosave.Length; i++)
                PlayerPrefs.SetString(name + i + "_" + Profile, tosave[i]);
        }

        public static void SaveBoolArray(bool[] tosave, string name) {
            PlayerPrefs.SetInt(name + "Amount_" + Profile, tosave.Length);
            for (int i = 0; i < tosave.Length; i++)
                PlayerPrefs.SetInt(name + i + "_" + Profile, tosave[i] ? 1 : 0);
        }

        public static string[] LoadStringArray(string name) {
            int amount = PlayerPrefs.GetInt(name + "Amount_" + Profile, 0);
            if (amount == 0)
                return null;

            string[] res = new string[amount];
            for (int i = 0; i < res.Length; i++)
                res[i] = PlayerPrefs.GetString(name + i + "_" + Profile);
            return res;
        }

        public static bool[] LoadBoolArray(string name, int desiredLength) {
            int amount = desiredLength;
            if (PlayerPrefs.HasKey(name + "Amount_" + Profile)) {
                bool[] res = new bool[PlayerPrefs.GetInt(name + "Amount_" + Profile)];
                for (int i = 0; i < res.Length; i++)
                    res[i] = PlayerPrefs.GetInt(name + i + "_" + Profile) == 1;

                return res;
            } else {
                bool[] res = new bool[amount];
                for (int i = 0; i < res.Length; i++)
                    res[i] = false;

                return res;
            }
        }

        #endregion

        #region PlayerStruct

        public static Player LoadPlayerStruct() {
            Player player = new();
            if (!PlayerPrefs.HasKey("player_id")) return new Player {Id = 0};

            player.Id = PlayerPrefs.GetInt("player_id");
            player.Email = PlayerPrefs.GetString("player_email");
            player.Password = PlayerPrefs.GetString("player_password");
            player.IsConfirmed = PlayerPrefs.GetInt("player_confirmed") == 1;

            player.Name = PlayerPrefs.GetString("player_name");
            player.FarmId = PlayerPrefs.GetInt("player_farm_id");

            return player;
        }

        public static void SavePlayerStruct(Player tosave) {
            PlayerPrefs.SetInt("player_id", tosave.Id);
            PlayerPrefs.SetString("player_email", tosave.Email);
            PlayerPrefs.SetString("player_password", tosave.Password);
            PlayerPrefs.SetInt("player_confirmed", tosave.IsConfirmed ? 1 : 0);
            PlayerPrefs.SetString("player_name", tosave.Name);
            PlayerPrefs.SetInt("player_farm_Id", tosave.FarmId);
        }

        #endregion

        #region FarmStruct

        public static Farm LoadFarmStruct() {
            if (!PlayerPrefs.HasKey("farm_id")) {
                return new Farm {Id = 0};
            }

            return new Farm {
                Id = PlayerPrefs.GetInt("farm_id"),
                Password = PlayerPrefs.GetString("farm_password")
            };
        }

        public static void SaveFarmStruct(Farm tosave) {
            PlayerPrefs.SetInt("farm_id", tosave.Id);
            PlayerPrefs.SetString("farm_password", tosave.Password);
        }

/*
    public static void SaveFarmStruct(Farm tosave) {
        string path = GetPath();
        string filepath = path + "/online.txt";

        string toSave = JsonUtility.ToJson(tosave);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(filepath))
            File.Create(filepath).Close();

        File.WriteAllText(filepath, toSave);
    }

    public static Farm LoadFarmStruct() {
        string path = GetPath();
        string filepath = path + "/online.txt";

        if (File.Exists(filepath)) {
            string save = File.ReadAllText(filepath);
            Farm res = (Farm) JsonUtility.FromJson(save, typeof(Farm));
            return res;
        }

        UnityEngine.Debug.LogWarning("File " + filepath + " Does Not Exists");
        return null;
    }
*/

        #endregion
    }
}