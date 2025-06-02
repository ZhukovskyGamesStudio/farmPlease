using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using ScriptableObjects;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Tables {
    public class CropsTable : PreloadableSingleton<CropsTable> {
        public CropConfig[] Crops;

        public FlyingCropFx FlyingCropFxPrefab;

        public static CropConfig CropByType(Crop type) {
            foreach (CropConfig t in Instance.Crops)
                if (t.type == type)
                    return t;

            Debug.Log("Нет класса Crop под тип " + type);
            return null;
        }

        public static bool ContainCrop(Crop type) {
            for (int i = 0; i < Instance.Crops.Length; i++)
                if (Instance.Crops[i].type == type)
                    return true;
            return false;
        }
        public static List<Crop> CropsTypes=> Instance.Crops.Select(t => t.type).ToList();
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