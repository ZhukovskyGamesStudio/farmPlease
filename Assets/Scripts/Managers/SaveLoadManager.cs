using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using DefaultNamespace.Abstract;
using DefaultNamespace.Managers;
using DefaultNamespace.Tables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : Singleton<SaveLoadManager> {
    public static int profile = -2;

    //bool isInitilisationDone;
    /**********/
    private BuildingShopView _buildingShopView;
    private FastPanelScript FastPanelScript;

    private InventoryManager InventoryManager;
    private PlayerController PlayerController;
    private SeedShopView SeedShop;
    private SmartTilemap SmartTilemap;

    private ToolShopView ToolShop;
    public static GameSaveProfile CurrentSave;

    public static string SaveDirectory => $"{Application.persistentDataPath}/saves";

    public static string SavePath => SaveDirectory + $"/save{profile}.txt";
    /**********/

    // Пока в игре происходят какие-то действия, игрок не может ничего сделать
    // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке
    public void Sequence(bool isStart) {
        if (isStart) {
            PlayerController.canInteract = false;
        } else {
            if (GameModeManager.Instance.GameMode == GameMode.Online) {
                OnlineFarm.instance.ChangeFarmAndPut(GenerateJsonString());
            } else {
                PlayerController.canInteract = true;
                SaveGame();
            }
        }
    }

    public static string GenerateJsonString() {
        CurrentSave.Date = DateTime.Now.ToString();

        CurrentSave.TilesData = SmartTilemap.instance.GetTilesData();

        CurrentSave.seedShopButtonData = UIHud.Instance.ShopsPanel.seedShopView.GetButtonsData();
        CurrentSave.seedShopChangeButton = UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.activeSelf;
        CurrentSave.ambarCropType = UIHud.Instance.ShopsPanel.seedShopView.GetAmbarSeedData();

        CurrentSave.toolShopButtonsData = UIHud.Instance.ShopsPanel.toolShopView.GetButtons();
        CurrentSave.toolShopChangeButton = UIHud.Instance.ShopsPanel.toolShopView.ChangeButton.activeSelf;

        if (GameModeManager.Instance.GameMode != GameMode.Training) {
            CurrentSave.cropBoughtData = InventoryManager.instance.GetIsBoughtData(0);
            CurrentSave.toolBoughtData = InventoryManager.instance.GetIsBoughtData(1);
            CurrentSave.buildingBoughtData = InventoryManager.instance.GetIsBoughtData(2);

            CurrentSave.buildingPrice = UIHud.Instance.ShopsPanel.BuildingShopView.GetBuildingPrice();
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
            Date = DateTime.Now.ToString()
        };

        SmartTilemap.instance.GenerateTiles();
        InventoryManager.instance.GenerateInventory();

        Energy.Instance.RefillEnergy();
        Clock.Instance.RefillToMaxEnergy();
        UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsNewDay();
        UIHud.Instance.ShopsPanel.toolShopView.ChangeTools();

        if (GameModeManager.Instance.GameMode != GameMode.Training)
            UIHud.Instance.ShopsPanel.BuildingShopView.Initialize();
        Time.Instance.GenerateDays(GameModeManager.Instance.GameMode == GameMode.Training, true);
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
        PlayerPrefs.SetInt(name + "Amount_" + profile, tosave.Length);
        for (int i = 0; i < tosave.Length; i++)
            PlayerPrefs.SetString(name + i + "_" + profile, tosave[i]);
    }

    public static void SaveBoolArray(bool[] tosave, string name) {
        PlayerPrefs.SetInt(name + "Amount_" + profile, tosave.Length);
        for (int i = 0; i < tosave.Length; i++)
            PlayerPrefs.SetInt(name + i + "_" + profile, tosave[i] ? 1 : 0);
    }

    public static string[] LoadStringArray(string name) {
        int amount = PlayerPrefs.GetInt(name + "Amount_" + profile, 0);
        if (amount == 0)
            return null;

        string[] res = new string[amount];
        for (int i = 0; i < res.Length; i++)
            res[i] = PlayerPrefs.GetString(name + i + "_" + profile);
        return res;
    }

    public static bool[] LoadBoolArray(string name, int desiredLength) {
        int amount = desiredLength;
        if (PlayerPrefs.HasKey(name + "Amount_" + profile)) {
            bool[] res = new bool[PlayerPrefs.GetInt(name + "Amount_" + profile)];
            for (int i = 0; i < res.Length; i++)
                res[i] = PlayerPrefs.GetInt(name + i + "_" + profile) == 1;

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
