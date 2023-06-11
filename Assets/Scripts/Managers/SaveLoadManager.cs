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
    private BuildingShopPanel BuildingShopPanel;
    private FastPanelScript FastPanelScript;

    private InventoryManager InventoryManager;
    private PlayerController PlayerController;
    private SeedShopScript SeedShop;
    private SmartTilemap SmartTilemap;

    private ToolShopPanel ToolShop;
    public static GameSaveProfile CurrentSave;

    private void Start() {
        PlayerController = PlayerController.Instance;
        PlayerController.Init();

        FastPanelScript = PlayerController.GetComponent<FastPanelScript>();
        FastPanelScript.Init();
        SmartTilemap = SmartTilemap.instance;

        InventoryManager = InventoryManager.instance;
        InventoryManager.Init();

        ToolShop = UIHud.Instance.ShopsPanel.ToolShopPanel;
        SeedShop = UIHud.Instance.ShopsPanel.seedShopScript;
        BuildingShopPanel = UIHud.Instance.ShopsPanel.BuildingShopPanel;

        TilesTable.instance.CreateDictionary();

        if (GameModeManager.Instance.GameMode == GameMode.Online)
            profile = 2;
        if (GameModeManager.Instance.GameMode == GameMode.RealTime)
            profile = 1;
        else if (GameModeManager.Instance.GameMode == GameMode.Training)
            profile = -1;
        else
            profile = -2;

        if (GameModeManager.Instance.DoNotSave)
            ClearSave();

        if (GameModeManager.Instance.GameMode != GameMode.Online)
            LoadGame();
        
        Clock.Instance.TryRefillForRealtimePassed();
    }

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
        GameSaveProfile gameSave = new();

        gameSave.profile = profile;
        gameSave.money = Instance.InventoryManager.coins;
        gameSave.energy = CurrentSave.energy;
        gameSave.clockEnergy = CurrentSave.clockEnergy;
        gameSave.allCrops = Instance.InventoryManager.AllCropsCollected;

        gameSave.currentDay = Time.Instance.day;
        gameSave.dayOfWeek = Time.Instance.DayOfWeek;
        gameSave.KnowledgeList = CurrentSave.KnowledgeList;

        if (GameModeManager.Instance.GameMode == GameMode.Training) {
            DateTime trainingDate = new(2018, 1, Time.Instance.day + 1);
            gameSave.Date = trainingDate.ToString();
        } else {
            gameSave.Date = DateTime.Now.ToString();
        }

        gameSave.crops = Instance.InventoryManager.GetCollectedCropsData();
        gameSave.seedsData = Instance.InventoryManager.GetSeedsData();
        gameSave.toolsData = Instance.InventoryManager.GetToolsData();

        gameSave.tilesData = Instance.SmartTilemap.GetTilesData();

        gameSave.daysData = Time.Instance.GetDaysData();

        gameSave.seedShopButtonData = Instance.SeedShop.GetButtonsData();
        gameSave.seedShopChangeButton = Instance.SeedShop.ChangeSeedsButton.activeSelf;
        gameSave.ambarCropType = Instance.SeedShop.GetAmbarSeedData();

        gameSave.toolShopButtonsData = Instance.ToolShop.GetButtons();
        gameSave.toolShopChangeButton = Instance.ToolShop.ChangeButton.activeSelf;

        if (GameModeManager.Instance.GameMode != GameMode.Training) {
            gameSave.cropBoughtData = Instance.InventoryManager.GetIsBoughtData(0);
            gameSave.toolBoughtData = Instance.InventoryManager.GetIsBoughtData(1);
            gameSave.buildingBoughtData = Instance.InventoryManager.GetIsBoughtData(2);

            gameSave.buildingPrice = Instance.BuildingShopPanel.GetBuildingPrice();
        }

        return JsonUtility.ToJson(gameSave, false);
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

    public static bool IsNoSaveExist() => !File.Exists(SavePath);

    public static void LoadSavedData() {
        CurrentSave = GameSaveProfile.LoadJson();
    }
    
    public static void LoadGame(string jsonString = null) {
        if (jsonString != null)
            CurrentSave = GameSaveProfile.LoadFromString(jsonString);
        else
            CurrentSave = GameSaveProfile.LoadJson();

        if (CurrentSave == null) {
            GenerateGame();
            Debug.Instance.Log("Generating finished. Saving started");
            Instance.SaveGame();
            Debug.Instance.Log("New profile is saved");
            return;
        }

        //DebugManager.instance.Log("Started loading of Existing profile");

        Instance.SmartTilemap.GenerateTilesWithData(CurrentSave.tilesData);

        Instance.InventoryManager.SetInventoryWithData(CurrentSave.seedsData, CurrentSave.crops, CurrentSave.toolsData,
            CurrentSave.money, CurrentSave.allCrops, CurrentSave.cropBoughtData, CurrentSave.toolBoughtData,
            CurrentSave.buildingBoughtData);
        Instance.FastPanelScript.UpdateToolsImages();
        Energy.Instance.SetEnergy(CurrentSave.energy);
        Clock.Instance.SetEnergy(CurrentSave.clockEnergy);

        Instance.SeedShop.SetSeedShopWithData(CurrentSave.seedShopButtonData, CurrentSave.seedShopChangeButton,
            CurrentSave.ambarCropType);
        Instance.ToolShop.SetToolShopWithData(CurrentSave.toolShopButtonsData, CurrentSave.toolShopChangeButton);

        if (GameModeManager.Instance.GameMode != GameMode.Training)
            Instance.BuildingShopPanel.InitializeWithData(CurrentSave.buildingPrice);

        // В этом методе запускаете ежесекудный корутин, который подсчитывает кол-во прошедших дней.

        DateTime dateTime = DateTime.Parse(CurrentSave.Date);
        Time.Instance.SetDaysWithData(CurrentSave.daysData, dateTime);
    }

    public static void GenerateGame() {
        CurrentSave = new GameSaveProfile() {
            money = 5,
            allCrops = 0,
            Date = DateTime.Now.ToString(),
            KnowledgeList = new List<Knowledge>()
        };

        if (GameModeManager.Instance.GameMode == GameMode.Training)
            CurrentSave.Date = new DateTime(2018, 1, Time.Instance.day + 1).ToString();

        Instance.SmartTilemap.GenerateTiles();
        Instance.InventoryManager.GenerateInventory();

        Energy.Instance.RefillEnergy();
        Clock.Instance.RefillToMaxEnergy();
        Instance.SeedShop.ChangeSeedsNewDay();
        Instance.ToolShop.ChangeTools();

        if (GameModeManager.Instance.GameMode != GameMode.Training)
            Instance.BuildingShopPanel.Initialize();
        Time.Instance.GenerateDays(GameModeManager.Instance.GameMode == GameMode.Training, true);
    }

    public static void FakeGenerateGame() {
        Debug.Instance.Log("Fake generating all except tiles");
        CurrentSave = new GameSaveProfile() {
            money = 5,
            allCrops = 0,
            Date = DateTime.Now.ToString()
        };

        Instance.InventoryManager.GenerateInventory();

        Energy.Instance.RefillEnergy();
        Clock.Instance.RefillToMaxEnergy();
        Instance.SeedShop.ChangeSeedsNewDay();
        Instance.ToolShop.ChangeTools();

        if (GameModeManager.Instance.GameMode != GameMode.Training)
            Instance.BuildingShopPanel.Initialize();
        Time.Instance.GenerateDays(GameModeManager.Instance.GameMode == GameMode.Training, true);
    }

    public void Reload() {
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClearSaveAndReload() {
        ClearSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClearAll() {
        int mustStay = PlayerPrefs.GetInt("SaveDropped 26.02", 0);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("SaveDropped 26.02", mustStay);

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

[Serializable]
public class GameSaveProfile {
    public int profile;
    public int money;
    public int energy;
    public int clockEnergy;
    public long lastClockRefilledTimestamp = Clock.NowTotalMilliseconds;
    public int allCrops;

    public string Date;

    public string[] crops;

    public string tilesData;
    public int currentDay;
    public int dayOfWeek;

    public string[] daysData;
    public string[] seedsData;
    public string[] toolsData;

    public bool[] seedShopButtonData;
    public bool seedShopChangeButton;
    public int ambarCropType;

    public bool[] toolShopButtonsData;
    public bool toolShopChangeButton;

    public bool[] cropBoughtData;
    public bool[] toolBoughtData;
    public bool[] buildingBoughtData;
    public int buildingPrice;

    public List<Knowledge> KnowledgeList;

    public static GameSaveProfile LoadFromString(string loadFrom) {
        try {
            GameSaveProfile res = (GameSaveProfile) JsonUtility.FromJson(loadFrom, typeof(GameSaveProfile));
            return res;
        } catch {
            UnityEngine.Debug.LogError("Wrong Format");
            return null;
        }
    }

    public static GameSaveProfile LoadJson() {
        string filepath = SaveLoadManager.SavePath;

        if (File.Exists(filepath)) {
            string save = File.ReadAllText(filepath);
            GameSaveProfile res = (GameSaveProfile) JsonUtility.FromJson(save, typeof(GameSaveProfile));
            return res;
        }

        UnityEngine.Debug.Log("File " + filepath + " Does Not Exists");
        return null;
    }
}