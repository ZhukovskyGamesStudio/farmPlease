﻿using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "CheatCode", menuName = "ScriptableObjects/CheatCode", order = 7)]
    public class CheatCodeConfig : ScriptableObject {
        public string Code;
        public int NumberOfUses;
        public int RechargingClockEnergyAmount;

        public int CoinsAdded;
        public int CropsCollectedAdded;
        public SerializableDictionary<Crop, int> SeedsAdded;
    }
}