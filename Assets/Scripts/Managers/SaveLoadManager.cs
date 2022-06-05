using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    #region Singleton
    public static SaveLoadManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);
    }
    #endregion

    public static int profile = 0;

    InventoryManager InventoryManager;
    SmartTilemap SmartTilemap;
    TimeManager TimeManager;
    PlayerController PlayerController;
    FastPanelScript FastPanelScript;
    ToolShopPanel ToolShop;
    SeedShopScript SeedShop;
    BuildingShopPanel BuildingShopPanel;

    //bool isInitilisationDone;
    /**********/
    public static string path, filename, filepath;

    

    private void Start()
    {
        PlayerController = PlayerController.instance;
        PlayerController.Init();

        FastPanelScript = PlayerController.GetComponent<FastPanelScript>();
        FastPanelScript.Init();

        TimeManager = TimeManager.instance;
        TimeManager.Init();
        SmartTilemap = SmartTilemap.instance;

        InventoryManager = InventoryManager.instance;
        InventoryManager.Init();

        ToolShop = UIScript.instance.ShopsPanel.ToolShopPanel;
        SeedShop = UIScript.instance.ShopsPanel.seedShopScript;
        BuildingShopPanel = UIScript.instance.ShopsPanel.BuildingShopPanel;



        TilesTable.instance.CreateDictionary();

        if (GameModeManager.instance.GameMode == GameMode.Online)
            profile = 2;
        if (GameModeManager.instance.GameMode == GameMode.RealTime)
            profile = 1;
        else if (GameModeManager.instance.GameMode == GameMode.Training)
            profile = -1;
        else
            profile = -2;

        path = Application.persistentDataPath + "/saves";
        filename = "save" + profile.ToString() + ".txt";
        filepath = path + "/" + filename;



        if (GameModeManager.instance.DoNotSave)
            ClearSave();

        if (GameModeManager.instance.GameMode != GameMode.Online)
            LoadGame();
    }

    private void OnApplicationQuit()
    {
       // GPSManager.ExitFromGPS();
    }

    public static string GetPath()
    {
        if (path == null)
        {
            path = Application.persistentDataPath + "/saves";
            filename = "save" + profile.ToString() + ".txt";
            filepath = path + "/" + filename;
        }
        return path;
    }
    /**********/


    // Пока в игре происходят какие-то действия, игрок не может ничего сделать
    // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке
    public void Sequence(bool isStart)
    {
        if (isStart)
            PlayerController.canInteract = false;
        else
        {
            if (GameModeManager.instance.GameMode == GameMode.Online)
            {
                OnlineFarm.instance.ChangeFarmAndPut(GenerateJsonString());
            }
            else
            {
                PlayerController.canInteract = true;
                SaveGame();
            }
        }
    }

    public static string GenerateJsonString()
    {

        GameSaveProfile gameSave = new GameSaveProfile();

        gameSave.profile = profile;
        gameSave.money = instance.InventoryManager.coins;
        gameSave.energy = instance.PlayerController.curEnergy;
        gameSave.allCrops = instance.InventoryManager.AllCropsCollected;

        gameSave.currentDay = instance.TimeManager.day;
        gameSave.dayOfWeek = instance.TimeManager.DayOfWeek;

        if (GameModeManager.instance.GameMode == GameMode.Training)
        {
            DateTime trainingDate = new DateTime(2018, 1, instance.TimeManager.day + 1);
            gameSave.Date = trainingDate.ToString();
        }
        else
            gameSave.Date = System.DateTime.Now.ToString();



        gameSave.crops = instance.InventoryManager.GetCollectedCropsData();
        gameSave.seedsData = instance.InventoryManager.GetSeedsData();
        gameSave.toolsData = instance.InventoryManager.GetToolsData();

        gameSave.tilesData = instance.SmartTilemap.GetTilesData();


        gameSave.daysData = instance.TimeManager.GetDaysData();


        gameSave.seedShopButtonData = instance.SeedShop.GetButtonsData();
        gameSave.seedShopChangeButton = instance.SeedShop.ChangeSeedsButton.activeSelf;
        gameSave.ambarCropType = instance.SeedShop.GetAmbarSeedData();

        gameSave.toolShopButtonsData = instance.ToolShop.GetButtons();
        gameSave.toolShopChangeButton = instance.ToolShop.ChangeButton.activeSelf;

        if (GameModeManager.instance.GameMode != GameMode.Training)
        {
            gameSave.cropBoughtData = instance.InventoryManager.GetIsBoughtData(0);
            gameSave.toolBoughtData = instance.InventoryManager.GetIsBoughtData(1);
            gameSave.buildingBoughtData = instance.InventoryManager.GetIsBoughtData(2);

            gameSave.buildingPrice = instance.BuildingShopPanel.GetBuildingPrice();

        }
        return JsonUtility.ToJson(gameSave, prettyPrint: false);
    }

    public void SaveGame()
    {
        if (GameModeManager.instance.DoNotSave || GameModeManager.instance.GameMode == GameMode.Online)
            return;

        string toSave = GenerateJsonString();

        string filePath = path + "/save" + profile.ToString() + ".txt";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(filePath))
            File.Create(filePath).Close();

        File.WriteAllText(filePath, toSave);
    }

    public static void LoadGame(string jsonString = null)
    {
       
        GameSaveProfile currentSave;
        if (jsonString != null)
        {
            currentSave = GameSaveProfile.LoadFromString(jsonString);
        }
        else
            currentSave = GameSaveProfile.LoadJson();


        if (currentSave == null)
        {
            GenerateGame();
            DebugManager.instance.Log("Generating finished. Saving started");
            instance.SaveGame();
            DebugManager.instance.Log("New profile is saved");
            return;
        }

        //DebugManager.instance.Log("Started loading of Existing profile");

        instance.SmartTilemap.GenerateTilesWithData(currentSave.tilesData);

        instance.InventoryManager.SetInventoryWithData(currentSave.seedsData, currentSave.crops, currentSave.toolsData, currentSave.money, currentSave.allCrops, currentSave.cropBoughtData, currentSave.toolBoughtData, currentSave.buildingBoughtData);
        instance.FastPanelScript.UpdateToolsImages();
        instance.PlayerController.SetEnergy(currentSave.energy);

        instance.SeedShop.SetSeedShopWithData(currentSave.seedShopButtonData, currentSave.seedShopChangeButton, currentSave.ambarCropType);
        instance.ToolShop.SetToolShopWithData(currentSave.toolShopButtonsData, currentSave.toolShopChangeButton);

        if (GameModeManager.instance.GameMode != GameMode.Training)
            instance.BuildingShopPanel.InitializeWithData(currentSave.buildingPrice);


        // В этом методе запускаете ежесекудный корутин, который подсчитывает кол-во прошедших дней.

        DateTime dateTime = DateTime.Parse(currentSave.Date);
        instance.TimeManager.SetDaysWithData(currentSave.daysData, dateTime);

        //DebugManager.instance.Log("Loading finished.");
    }

    public static void GenerateGame()
    {
        GameSaveProfile currentSave = new GameSaveProfile();

        currentSave.money = 5;
        currentSave.allCrops = 0;
        currentSave.Date = DateTime.Now.ToString();
        if (GameModeManager.instance.GameMode == GameMode.Training)
            currentSave.Date = (new DateTime(2018, 1, instance.TimeManager.day + 1)).ToString();

        instance.SmartTilemap.GenerateTiles();
        instance.InventoryManager.GenerateInventory();


        instance.PlayerController.SetEnergy(0, true);
        instance.SeedShop.ChangeSeedsNewDay();
        instance.ToolShop.ChangeTools();

        if (GameModeManager.instance.GameMode != GameMode.Training)
            instance.BuildingShopPanel.Initialize();
        instance.TimeManager.GenerateDays(GameModeManager.instance.GameMode == GameMode.Training, true);
    }

    public static void FakeGenerateGame()
    {
        DebugManager.instance.Log("Fake generating all except tiles");
        GameSaveProfile currentSave = new GameSaveProfile();

        currentSave.money = 5;
        currentSave.allCrops = 0;
        currentSave.Date = DateTime.Now.ToString();

        instance.InventoryManager.GenerateInventory();

        instance.PlayerController.SetEnergy(7, true);
        instance.SeedShop.ChangeSeedsNewDay();
        instance.ToolShop.ChangeTools();

        if (GameModeManager.instance.GameMode != GameMode.Training)
            instance.BuildingShopPanel.Initialize();
        instance.TimeManager.GenerateDays(GameModeManager.instance.GameMode == GameMode.Training, true);
    }

    public void Reload()
    {
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClearSaveAndReload()
    {
        ClearSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClearAll()
    {
        int mustStay = PlayerPrefs.GetInt("SaveDropped 26.02", 0);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("SaveDropped 26.02", mustStay);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClearSave()
    {

        if (File.Exists(filepath))
            File.Delete(filepath);
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

    public static void SaveStringArray(string[] tosave, string name)
    {
        PlayerPrefs.SetInt(name + "Amount_" + profile, tosave.Length);
        for (int i = 0; i < tosave.Length; i++)
            PlayerPrefs.SetString(name + i + "_" + profile, tosave[i]);
    }

    public static void SaveBoolArray(bool[] tosave, string name)
    {
        PlayerPrefs.SetInt(name + "Amount_" + profile, tosave.Length);
        for (int i = 0; i < tosave.Length; i++)
            PlayerPrefs.SetInt(name + i + "_" + profile, tosave[i] ? 1 : 0);
    }

    public static string[] LoadStringArray(string name)
    {
        int amount = PlayerPrefs.GetInt(name + "Amount_" + profile, 0);
        if (amount == 0)
            return null;

        string[] res = new string[amount];
        for (int i = 0; i < res.Length; i++)
            res[i] = PlayerPrefs.GetString(name + i + "_" + profile);
        return res;
    }

    public static bool[] LoadBoolArray(string name, int desiredLength)
    {
        int amount = desiredLength;
        if (PlayerPrefs.HasKey(name + "Amount_" + profile))
        {
            bool[] res = new bool[PlayerPrefs.GetInt(name + "Amount_" + profile)];
            for (int i = 0; i < res.Length; i++)
                res[i] = PlayerPrefs.GetInt(name + i + "_" + profile) == 1;

            return res;
        }
        else
        {
            bool[] res = new bool[amount];
            for (int i = 0; i < res.Length; i++)
                res[i] = false;

            return res;
        }
    }

    #endregion

    #region PlayerStruct
    public static Player LoadPlayerStruct()
    {
        Player player = new Player();
        if (!PlayerPrefs.HasKey("player_id"))
        {
            return new Player { Id = 0 };
        }

        player.Id = PlayerPrefs.GetInt("player_id");
        player.Email = PlayerPrefs.GetString("player_email");
        player.Password = PlayerPrefs.GetString("player_password");
        player.IsConfirmed = PlayerPrefs.GetInt("player_confirmed") == 1;

        player.Name = PlayerPrefs.GetString("player_name");
        player.FarmId = PlayerPrefs.GetInt("player_farm_id");

        return player;
    }
    public static void SavePlayerStruct(Player tosave)
    {
        PlayerPrefs.SetInt("player_id", tosave.Id);
        PlayerPrefs.SetString("player_email", tosave.Email);
        PlayerPrefs.SetString("player_password", tosave.Password);
        PlayerPrefs.SetInt("player_confirmed", tosave.IsConfirmed ? 1 : 0);
        PlayerPrefs.SetString("player_name", tosave.Name);
        PlayerPrefs.SetInt("player_farm_Id", tosave.FarmId);
    }

    #endregion 

    #region FarmStruct
    /*public static Farm LoadFarmStruct()
    {
        if (!PlayerPrefs.HasKey("farm_id"))
        {
            return new Farm { Id = 0 };
        }

        return new Farm
        {
            Id = PlayerPrefs.GetInt("farm_id"),
            Password = PlayerPrefs.GetString("farm_password")
        };
    }

    public static void SaveFarmStruct(Farm tosave)
    {
        PlayerPrefs.SetInt("farm_id", tosave.Id);
        PlayerPrefs.SetString("farm_password", tosave.Password);
    }  
    */

    public static void SaveFarmStruct(Farm tosave)
    {
        string path = SaveLoadManager.GetPath();
        string filepath = path + "/online.txt";

        string toSave = JsonUtility.ToJson(tosave);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(filepath))
            File.Create(filepath).Close();

        File.WriteAllText(filepath, toSave);
    }
    public static Farm LoadFarmStruct()
    {
        string path = SaveLoadManager.GetPath();
        string filepath = path + "/online.txt";

        if (File.Exists(filepath))
        {
            string save = File.ReadAllText(filepath);
            Farm res = (Farm)JsonUtility.FromJson(save, typeof(Farm));
            return res;
        }
        Debug.LogWarning("File " + filepath + " Does Not Exists");
        return null;
    }

    #endregion
}

[System.Serializable]
public class GameSaveProfile
{
    public int profile;
    public int money;
    public int energy;
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

    public string SaveJson()
    {

        string path = SaveLoadManager.path;
        string filePath = path + "/save" + profile.ToString() + ".txt";

        string toSave = JsonUtility.ToJson(this, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(filePath))
            File.Create(filePath).Close();

        File.WriteAllText(filePath, toSave);
        return toSave;
    }

    public static GameSaveProfile LoadFromString(string loadFrom)
    {
        try
        {
            GameSaveProfile res = (GameSaveProfile)JsonUtility.FromJson(loadFrom, typeof(GameSaveProfile));
            return res;
        }
        catch
        {
            Debug.LogError("Wrong Format");
            return null;
        }

    }

    public static GameSaveProfile LoadJson()
    {
        string filepath = SaveLoadManager.filepath;

        if (File.Exists(filepath))
        {
            string save = File.ReadAllText(filepath);
            GameSaveProfile res = (GameSaveProfile)JsonUtility.FromJson(save, typeof(GameSaveProfile));
            return res;
        }
        Debug.Log("File " + filepath + " Does Not Exists");
        return null;
    }
}
