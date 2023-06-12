using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace.Abstract;
using DefaultNamespace.Managers;
using DefaultNamespace.Tables;
using UnityEngine;

namespace DefaultNamespace {
   
[Serializable]
public class GameSaveProfile {
    public int Coins;
    public int Energy;
    public int ClockEnergy;
    public long LastClockRefilledTimestamp = Clock.NowTotalMilliseconds;
    public int CropPoints;
    public string Date;
    public string SavedDate;
    public string TilesData;
    public int CurrentDay;
    public int DayOfWeek;

    public Queue<Crop> CropsCollected = new Queue<Crop>();
    public List<HappeningType> Days = new List<HappeningType>();
    public SerializableDictionary<Crop, int> Seeds = new SerializableDictionary<Crop, int>();
    public SerializableDictionary<ToolBuff, int> ToolBuffs = new SerializableDictionary<ToolBuff, int>();

    public bool[] SeedShopButtonData;
    public bool SeedShopChangeButton;
    public Crop AmbarCrop;

    public bool[] ToolShopButtonsData;
    public bool ToolShopChangeButton;

    public bool[] CropBoughtData;
    public bool[] ToolBoughtData;
    public bool[] BuildingBoughtData;
    public int BuildingPrice;

    public List<Knowledge> KnowledgeList = new List<Knowledge>();

    public static GameSaveProfile LoadFromString(string loadFrom) {
        try {
            GameSaveProfile res = (GameSaveProfile) JsonUtility.FromJson(loadFrom, typeof(GameSaveProfile));
            return res;
        } catch {
            UnityEngine.Debug.LogError("Wrong Format");
            return null;
        }
    }

    public static GameSaveProfile LoadFromFile(string filepath) {

        if (File.Exists(filepath)) {
            string save = File.ReadAllText(filepath);
            GameSaveProfile res = (GameSaveProfile) JsonUtility.FromJson(save, typeof(GameSaveProfile));
            return res;
        }

        UnityEngine.Debug.Log("File " + filepath + " Does Not Exists");
        return null;
    }
}
}