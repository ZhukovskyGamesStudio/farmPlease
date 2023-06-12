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
    public string TilesData;
    public int CurrentDay;
    public int DayOfWeek;

    public Queue<CropsType> CropsCollected = new Queue<CropsType>();
    public List<HappeningType> Days = new List<HappeningType>();
    public SerializableDictionary<CropsType, int> Seeds = new SerializableDictionary<CropsType, int>();
    public SerializableDictionary<ToolBuff, int> ToolBuffs = new SerializableDictionary<ToolBuff, int>();

    public bool[] seedShopButtonData;
    public bool seedShopChangeButton;
    public int ambarCropType;

    public bool[] toolShopButtonsData;
    public bool toolShopChangeButton;

    public bool[] cropBoughtData;
    public bool[] toolBoughtData;
    public bool[] buildingBoughtData;
    public int buildingPrice;

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