using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tables {
    [ExecuteAlways]
    public class TilesTable : MonoBehaviour {
        public static TileData[] TileDatas;
        public static TilesTable Instance;

        public Group[] groups;
        public Group[] BuildingGroups;
        public int DictionaryEntrancies;

        [Header("Tap to recreate Dictionary")]
        public bool RecreateDictionary;

        public void Awake() {
            if (Instance == null)
                Instance = this;
        }

        private void Update() {
            if (RecreateDictionary) {
                CreateDictionary();
                RecreateDictionary = false;
            }
        }

        public static TileData TileByType(TileType type) {
            for (int i = 0; i < TileDatas.Length; i++)
                if (TileDatas[i].type == type)
                    return TileDatas[i];
            UnityEngine.Debug.Log("Нет класса TileData под тип " + type);
            return new TileData();
        }

        public void CreateDictionary() {
            List<TileData> list = new();

            for (int i = 0; i < groups.Length; i++) {
                TileData[] td = groups[i].tiledatas;
                for (int j = 0; j < td.Length; j++) {
                    td[j].IsBuilding = false;
                    td[j].crop = groups[i].crop;
                    td[j].BuildingType = BuildingType.None;
                    list.Add(td[j]);
                }
            }

            for (int i = 0; i < BuildingGroups.Length; i++) {
                TileData[] td = BuildingGroups[i].tiledatas;
                for (int j = 0; j < td.Length; j++) {
                    td[j].IsBuilding = true;
                    td[j].BuildingType = groups[i].BuildingType;
                    td[j].crop = Crop.None;
                    list.Add(td[j]);
                }
            }

            TileDatas = list.ToArray();
            DictionaryEntrancies = TileDatas.Length;
        }
    }

   

    [Serializable]
    public struct Group {
        public string name;
        public bool isBuilding;
        public Crop crop;
        public BuildingType BuildingType;

        public TileData[] tiledatas;
    }

    [Serializable]
    public struct TileData {
        public TileType type;
        public TileBase TileBase;

        [Space(10)]
        public bool CanBeHoed;

        public bool CanBeSeeded;
        public bool CanBeWatered;
        public TileType WaterSwitch;
        public bool CanBeCollected;
        public Crop cropCollected;
        public int collectAmount;

        [HideInInspector]
        public Crop crop;

        [Space(10)]
        public bool CanBeNewDayed;

        public TileType NewDaySwitch;
        public bool CanBeErosioned;
        public bool CanBeClicked;

        [Space(10)]
        public int TIndex;

        [HideInInspector]
        public bool IsBuilding;

        [HideInInspector]
        public BuildingType BuildingType;
    }
}