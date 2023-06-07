﻿using System;
using DefaultNamespace.Abstract;
using UnityEngine;

public class CropsTable : Singleton<CropsTable> {
    public static CropsTable instance;

    public CropSO[] Crops;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static CropSO CropByType(CropsType type) {
        for (int i = 0; i < instance.Crops.Length; i++)
            if (instance.Crops[i].type == type)
                return instance.Crops[i];
        UnityEngine.Debug.Log("Нет класса Crop под тип " + type);
        return null;
    }

    public static bool ContainCrop(CropsType type) {
        for (int i = 0; i < instance.Crops.Length; i++)
            if (instance.Crops[i].type == type)
                return true;
        return false;
    }
}

[Serializable]
public enum CropsType {
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

[Serializable]
public class Crop {
    [Header("Crop")]
    public string name;

    public CropsType type;
    public Sprite VegSprite;

    [Header("SeedShopProperties")]
    public bool CanBeBought = true;

    public Sprite SeedSprite;
    public int cost;
    public string explainText;
    public int buyAmount;
    public int Rarity;

    [Header("CroponomPage")]
    public string header;

    public string firstText;
    public Sprite firstSprite;
    public string secondText;
    public Sprite secondSprite;
}