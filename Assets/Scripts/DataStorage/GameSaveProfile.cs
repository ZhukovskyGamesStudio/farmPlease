using System;
using System.Collections.Generic;
using System.Globalization;
using Managers;
using Tables;
using ZhukovskyGamesPlugin;

[Serializable]
public class GameSaveProfile {
    public bool IsAdmin;
    
    public string Nickname = "Player";
    public int Coins;
    public int Xp;
    public int CurrentLevel;
    public int Energy;
    public int ClockEnergy;
    public long LastClockRefilledTimestamp = Clock.NowTotalMilliseconds;
    public int CropPoints => CropsCollected.Count;
    public string Date;
    public string SavedDate;
    public TilesData TilesData = new TilesData();
    public int CurrentDayInMonth;
    public int CurrentMonth;
    public int DayOfWeek;

    public List<Crop> CropsCollected = new List<Crop>();
    public Queue<Crop> CropsCollectedQueue => new Queue<Crop>(CropsCollected);
    public List<HappeningType> Days = new List<HappeningType>();
    public SerializableDictionary<Crop, int> Seeds = new SerializableDictionary<Crop, int>();
    public SerializableDictionary<ToolBuff, int> ToolBuffs = new SerializableDictionary<ToolBuff, int>();
    public SerializableDictionary<ToolBuff, int> ToolBuffsStored = new SerializableDictionary<ToolBuff, int>();
    public SerializableDictionary<string, int> CheatCodesActivated = new SerializableDictionary<string, int>();
    
    public SerializableDictionary<Crop, bool> IsCropsBoughtD = new SerializableDictionary<Crop, bool>();
    public SerializableDictionary<ToolBuff, bool> IsToolsBoughtD = new SerializableDictionary<ToolBuff, bool>();
    public SerializableDictionary<BuildingType, bool> IsBuildingsBoughtD = new SerializableDictionary<BuildingType, bool>();
    
    public Crop ShopFirstOffer, ShopSecondOffer;
    public bool SeedShopChangeButton;
    public Crop AmbarCrop;
    
    public int BuildingPrice;

    public List<Knowledge> KnowledgeList = new List<Knowledge>();
    public List<string> Unlocked = new List<string>();
    public SettingsData SettingsData = new SettingsData();
    
    public SeedShopData SeedShopData = new SeedShopData();
    public ToolShopData ToolShopData = new ToolShopData();
    public List<string> UnseenCroponomPages = new List<string>();
 
    
    public DateTime ParsedDate => DateTime.Parse(Date, CultureInfo.InvariantCulture);
    
}


[Serializable]
public class ToolShopData {
    public ToolBuff FirstOffer = ToolBuff.Unlimitedwatercan;
    public ToolBuff SecondOffer= ToolBuff.Unlimitedwatercan;
    public bool FirstOfferActive = true;
    public bool SecondOfferActive = true;
    public bool ChangeButtonActive = true;
}

[Serializable]
public class SeedShopData {
    public bool NeedShowChange = true;
    public Crop FirstOffer = Crop.Tomato;
    public Crop SecondOffer = Crop.Tomato;
    public Crop AmbarCrop = Crop.None;
    public bool ChangeButtonActive = true;
}