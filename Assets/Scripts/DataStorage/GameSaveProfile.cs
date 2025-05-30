using System;
using System.Collections.Generic;
using Managers;
using Tables;
using ZhukovskyGamesPlugin;

[Serializable]
public class GameSaveProfile {
    public int Coins;
    public int Xp;
    public int CurrentLevel;
    public int Energy;
    public int ClockEnergy;
    public long LastClockRefilledTimestamp = Clock.NowTotalMilliseconds;
    public int CropPoints;
    public string Date;
    public string SavedDate;
    public TilesData TilesData = new TilesData();
    public int CurrentDay;
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

    public ToolBuff ToolFirstOffer = ToolBuff.Doublehoe, ToolSecondOffer = ToolBuff.Unlimitedwatercan;
    public bool ToolFirstOfferActive = true, ToolSecondOfferActive = true;
    public bool ToolShopChangeButton;
    
    public int BuildingPrice;

    public List<Knowledge> KnowledgeList = new List<Knowledge>();
    public List<string> Unlocked = new List<string>();
    public SettingsData SettingsData = new SettingsData();
}