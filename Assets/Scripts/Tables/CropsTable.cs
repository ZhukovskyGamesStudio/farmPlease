using System;
using ScriptableObjects;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Tables
{
    public class CropsTable : Singleton<CropsTable> {
        public static CropsTable Instance;

        public CropConfig[] Crops;

        public void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static CropConfig CropByType(Crop type) {
            for (int i = 0; i < Instance.Crops.Length; i++)
                if (Instance.Crops[i].type == type)
                    return Instance.Crops[i];
            UnityEngine.Debug.Log("Нет класса Crop под тип " + type);
            return null;
        }

        public static bool ContainCrop(Crop type) {
            for (int i = 0; i < Instance.Crops.Length; i++)
                if (Instance.Crops[i].type == type)
                    return true;
            return false;
        }
    }

    [Serializable]
    public enum Crop {
        None = -1,
        Tomato,
        Eggplant,
        Corn,
        Dandellion,
        Strawberry,
        Fern,
        Cactus,
        Beautyflower,
        Flycatcher,
        Onion,
        Weed,
        Pumpkin,
        Radish,
        Peanut
    }
}