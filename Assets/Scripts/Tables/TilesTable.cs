using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class TilesTable : MonoBehaviour {
    public static TileData[] tileDatas;
    public static TilesTable instance;

    public Group[] groups;
    public Group[] BuildingGroups;
    public int DictionaryEntrancies;

    [Header("Tap to recreate Dictionary")]
    public bool RecreateDictionary;

    public void Awake() {
        if (instance == null)
            instance = this;
    }

    private void Update() {
        if (RecreateDictionary) {
            CreateDictionary();
            RecreateDictionary = false;
        }
    }

    public static TileData TileByType(TileType type) {
        for (int i = 0; i < tileDatas.Length; i++)
            if (tileDatas[i].type == type)
                return tileDatas[i];
        Debug.Log("Нет класса TileData под тип " + type);
        return new TileData();
    }

    public void CreateDictionary() {
        List<TileData> list = new();

        for (int i = 0; i < groups.Length; i++) {
            TileData[] td = groups[i].tiledatas;
            for (int j = 0; j < td.Length; j++) {
                td[j].IsBuilding = false;
                td[j].CropType = groups[i].CropType;
                td[j].BuildingType = BuildingType.None;
                list.Add(td[j]);
            }
        }

        for (int i = 0; i < BuildingGroups.Length; i++) {
            TileData[] td = BuildingGroups[i].tiledatas;
            for (int j = 0; j < td.Length; j++) {
                td[j].IsBuilding = true;
                td[j].BuildingType = groups[i].BuildingType;
                td[j].CropType = CropsType.None;
                list.Add(td[j]);
            }
        }

        tileDatas = list.ToArray();
        DictionaryEntrancies = tileDatas.Length;
    }
}

[Serializable]
public enum TileType {
    None,
    Sand,
    Soil,
    Weed,
    WateredSoil,
    Rocks,
    TomatoSeed,
    WateredTomatoSeed,
    Tomato_1,
    Tomato_2,
    Tomato_3,
    EggplantSeed,
    WateredEggplantSeed,
    Eggplant_1,
    Eggplant_2,
    Eggplant_3,
    CornSeed,
    WateredCornSeed,
    Corn_1,
    DandellionSeed,
    WateredDandelliomSeed,
    Dandellion_1,
    StrawberrySeed,
    WateredStrawberrySeed,
    Strawberry_1,
    CactusSeed,
    Cactus_1,
    FernSeed,
    WateredFernSeed,
    Fern_1,
    WateredFern_1,
    FernDead,
    Beautyflowerseed_1,
    Beautyflowerseed_2,
    Beautyflowerseed_3,
    Beautyflowerseed_4,
    Beautyflowerseed_5,
    Beautyflowerseed_6,
    Beautyflowerseed_7,
    WateredBeautyflowerseed_1,
    WateredBeautyflowerseed_2,
    WateredBeautyflowerseed_3,
    WateredBeautyflowerseed_4,
    WateredBeautyflowerseed_5,
    WateredBeautyflowerseed_6,
    WateredBeautyflowerseed_7,
    Beautyflower_1,
    BeautyflowerDead,
    BeautyflowerSibling,
    FlycatherSeed_1,
    FlycatherSeed_2,
    FlycatherSeed_3,
    Flycather_1,
    OnionSeed,
    OnionWatered,
    Onion_1,
    Biogen_empty,
    Biogen_full,
    Biogen_T1,
    Biogen_T2,
    Biogen_T3,
    Biogen_Construction,
    Freshener_empty,
    Freshener_1,
    Freshener_2,
    Freshener_3,
    Freshener_4,
    Freshener_5,
    Freshener_6,
    Freshener_full,
    Freshener_Construction,
    Freshener_T1,
    Freshener_T2,
    Freshener_T3,
    Sprinkler_empty,
    Sprinkler_1,
    Sprinkler_2,
    Sprinkler_3,
    Sprinkler_4,
    Sprinkler_5,
    Sprinkler_full,
    Sprinkler_target,
    Sprinkler_Construction,
    Sprinkler_T1,
    Sprinkler_T2,
    Sprinkler_T3,
    SeedDoubler_empty,
    SeedDoubler_full,
    SeedDoubler_Construction,
    SeedDoubler_T1,
    SeedDoubler_T2,
    SeedDoubler_T3,
    Tractor_1,
    Tractor_2,
    Tractor_Construction,
    Tractor_T1,
    Tractor_T2,
    Tractor_T3,
    Pumpkinseed_1,
    Pumpkinseed_2,
    Pumpkinseed_3,
    WateredPumpkinseed_1,
    WateredPumpkinseed_2,
    WateredPumpkinseed_3,
    GrownPumpkinseed_1,
    GrownPumpkinseed_2,
    GrownPumpkinseed_3,
    Pumpkin_1,
    Pumpkin_2,
    Pumpkin_3,
    RadishSeed,
    WateredRadishSeed,
    Radish_1,
    PeanutSeed,
    WateredPeanutSeed,
    Peanut_1,
    PeanutDead
}

[Serializable]
public class SmartTile : MonoBehaviour {
    public TileType type;
    public bool isActive;

    private Vector3Int position;
    private SmartTilemap tilemap;

    public void Init(SmartTilemap _tilemap, TileType _type, Vector3Int _pos) {
        tilemap = _tilemap;
        type = _type;
        position = _pos;
        isActive = true;
        tilemap.MainTilemap.SetColor(position, Color.red);
    }

    /**********/

    public bool CanBeHoed() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).CanBeHoed) return true;

        if (type == TileType.Strawberry_1) {
            isActive = false;
            SmartTile[] neighbors = tilemap.GetHexNeighbors(position);
            List<SmartTile> neighborsL = new();

            for (int j = 0; j < neighbors.Length; j++)
                if (neighbors[j].CanBeWatered())
                    neighborsL.Add(neighbors[j]);
            isActive = true;
            return neighborsL.Count > 0;
        }

        return false;
    }

    public bool CanBeSeeded() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).TIndex > 0)
            return tilemap.GetBuildingByPart(position).CanBeSeeded();
        if (TilesTable.TileByType(type).CanBeSeeded) return true;

        return false;
    }

    public bool CanBeWatered() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).TIndex > 0) return tilemap.GetBuildingByPart(position).CanBeWatered();

        if (TilesTable.TileByType(type).CanBeWatered) {
            return true;
        }

        if (type == TileType.Dandellion_1) {
            isActive = false;
            SmartTile[] neighbors = tilemap.GetHexNeighbors(position);
            List<SmartTile> neighborsL = new();

            for (int j = 0; j < neighbors.Length; j++)
                if (neighbors[j].CanBeHoed())
                    neighborsL.Add(neighbors[j]);

            isActive = true;
            return neighborsL.Count > 0;
        }

        return false;
    }

    public bool CanBeCollected() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).CanBeCollected) return true;

        return false;
    }

    public bool CanbeNewDayed() {
        if (!isActive)
            return false;
        if (TilesTable.TileByType(type).CanBeNewDayed) return true;

        if (type == TileType.Sprinkler_target) {
            TileType[] tocheck = new TileType[2] {TileType.Sprinkler_Construction, TileType.Sprinkler_empty};
            return !tilemap.HasTile(tocheck);
        }

        return false;
    }

    public bool CanBeErosioned() {
        if (!isActive)
            return false;
        if (TilesTable.TileByType(type).CanBeErosioned) return true;

        return false;
    }

    public bool CanBeClicked() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).TIndex > 0)
            return tilemap.GetBuildingByPart(position).CanBeClicked();
        if (TilesTable.TileByType(type).CanBeClicked) return true;

        return false;
    }

    public bool IsBuilding() {
        if (!isActive)
            return false;

        if (TilesTable.TileByType(type).IsBuilding) return true;

        return false;
    }

    /**********/

    public void BecomeActive() {
        isActive = true;
        tilemap.MainTilemap.SetColor(position, Color.white);
    }

    public void BecomeInactive() {
        isActive = false;
        tilemap.MainTilemap.SetColor(position, Color.grey);
    }

    /**********/

    public IEnumerator OnHoed(float animtime) {
        BecomeInactive();

        SmartTile[] neighborTiles = tilemap.GetHexNeighbors(position);

        switch (type) {
            case TileType.Sand:
            case TileType.FernDead:
            case TileType.BeautyflowerDead:
            case TileType.PeanutDead:
                SwitchType(TileType.Soil, AnimationType.Hoe);
                break;

            case TileType.Strawberry_1:
                SmartTile[] neighbors = tilemap.GetHexNeighbors(position);
                List<SmartTile> neighborsL = new();

                for (int i = 0; i < neighbors.Length; i++)
                    if (neighbors[i].CanBeWatered())
                        neighborsL.Add(neighbors[i]);

                for (int i = 0; i < 2; i++)
                    if (neighborsL.Count > 0) {
                        SmartTile tile = neighborsL[Random.Range(0, neighborsL.Count)];
                        neighborsL.Remove(tile);
                        yield return StartCoroutine(tile.OnWatered(animtime));
                    }

                break;
        }

        yield return new WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[3].position)[5].CanBeHoed())
            yield return tilemap.GetHexNeighbors(neighborTiles[3].position)[5].OnHoed(animtime);

        if (neighborTiles[4].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[4].position)[4].CanBeHoed())
            yield return tilemap.GetHexNeighbors(neighborTiles[4].position)[4].OnHoed(animtime);

        if (neighborTiles[5].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[5].position)[3].CanBeHoed())
            yield return tilemap.GetHexNeighbors(neighborTiles[5].position)[3].OnHoed(animtime);

        AudioManager.instance.PlaySound(Sounds.Hoed);
        BecomeActive();
    }

    public IEnumerator OnSeeded(CropsType seedtype, float animtime) {
        BecomeInactive();

        SmartTile[] neighborTiles = tilemap.GetHexNeighbors(position);

        TileData data = TilesTable.TileByType(type);
        if (data.TIndex == 1)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(1, -1, 0))
                .OnSeeded(seedtype, animtime));
        else if (data.TIndex == 2)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(0, -1, 0))
                .OnSeeded(seedtype, animtime));
        else if (data.TIndex == 3)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(-1, 0, 0))
                .OnSeeded(seedtype, animtime));
        else
            switch (type) {
                case TileType.SeedDoubler_empty:
                case TileType.SeedDoubler_full:
                    tilemap.seedShop.SetAmbarCrop(seedtype);
                    SwitchType(TileType.SeedDoubler_full);
                    break;

                case TileType.Soil:
                    switch (seedtype) {
                        case CropsType.Tomato:
                            SwitchType(TileType.TomatoSeed);
                            break;

                        case CropsType.Eggplant:
                            SwitchType(TileType.EggplantSeed);
                            break;

                        case CropsType.Corn:
                            SwitchType(TileType.CornSeed);
                            break;

                        case CropsType.Dandellion:
                            SwitchType(TileType.DandellionSeed);
                            break;

                        case CropsType.Strawberry:
                            SwitchType(TileType.StrawberrySeed);
                            break;

                        case CropsType.Cactus:
                            SwitchType(TileType.CactusSeed);
                            break;

                        case CropsType.Fern:
                            SwitchType(TileType.FernSeed);
                            break;

                        case CropsType.Beautyflower:
                            if (animtime == -1)
                                SwitchType(TileType.BeautyflowerSibling);
                            else
                                SwitchType(TileType.Beautyflowerseed_1);
                            break;

                        case CropsType.Flycatcher:
                            SwitchType(TileType.FlycatherSeed_1);
                            break;

                        case CropsType.Onion:
                            SwitchType(TileType.OnionSeed);
                            break;

                        case CropsType.Pumpkin:

                            SmartTile[] neighbors = tilemap.GetHexNeighbors(position);
                            List<SmartTile> pumpList = new();
                            for (int i = 0; i < neighbors.Length; i++) {
                                TileType tmpType = neighbors[i].type;
                                if (tmpType == TileType.Pumpkinseed_1) {
                                    neighbors[i].SwitchType(TileType.Pumpkinseed_2);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.Pumpkinseed_2) {
                                    neighbors[i].SwitchType(TileType.Pumpkinseed_3);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.Pumpkinseed_3)
                                    pumpList.Add(neighbors[i]);

                                if (tmpType == TileType.WateredPumpkinseed_1) {
                                    neighbors[i].SwitchType(TileType.WateredPumpkinseed_2);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.WateredPumpkinseed_2) {
                                    neighbors[i].SwitchType(TileType.WateredPumpkinseed_3);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.WateredPumpkinseed_3)
                                    pumpList.Add(neighbors[i]);

                                if (tmpType == TileType.GrownPumpkinseed_1) {
                                    neighbors[i].SwitchType(TileType.GrownPumpkinseed_2);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.GrownPumpkinseed_2) {
                                    neighbors[i].SwitchType(TileType.GrownPumpkinseed_3);
                                    pumpList.Add(neighbors[i]);
                                }

                                if (tmpType == TileType.GrownPumpkinseed_3)
                                    pumpList.Add(neighbors[i]);
                            }

                            if (pumpList.Count == 0)
                                SwitchType(TileType.Pumpkinseed_1);
                            if (pumpList.Count == 1)
                                SwitchType(TileType.Pumpkinseed_2);
                            if (pumpList.Count >= 2)
                                SwitchType(TileType.Pumpkinseed_3);

                            break;

                        case CropsType.Radish:
                            SwitchType(TileType.RadishSeed);
                            break;

                        case CropsType.Peanut:
                            SwitchType(TileType.PeanutSeed);
                            break;
                    }

                    break;
            }

        yield return new WaitForSeconds(animtime);

        if (InventoryManager.instance.seedsInventory[seedtype] > 0) {
            if (neighborTiles[3].type == TileType.Radish_1 &&
                tilemap.GetHexNeighbors(neighborTiles[3].position)[5].CanBeSeeded()) {
                yield return tilemap.GetHexNeighbors(neighborTiles[3].position)[5].OnSeeded(seedtype, animtime);
                InventoryManager.instance.LoseSeed(seedtype);
            }

            if (neighborTiles[4].type == TileType.Radish_1 &&
                tilemap.GetHexNeighbors(neighborTiles[4].position)[4].CanBeSeeded()) {
                yield return tilemap.GetHexNeighbors(neighborTiles[4].position)[4].OnSeeded(seedtype, animtime);
                InventoryManager.instance.LoseSeed(seedtype);
            }

            if (neighborTiles[5].type == TileType.Radish_1 &&
                tilemap.GetHexNeighbors(neighborTiles[5].position)[3].CanBeSeeded()) {
                yield return tilemap.GetHexNeighbors(neighborTiles[5].position)[3].OnSeeded(seedtype, animtime);
                InventoryManager.instance.LoseSeed(seedtype);
            }
        }

        AudioManager.instance.PlaySound(Sounds.Seeded);
        BecomeActive();
    }

    public IEnumerator OnWatered(float animtime) {
        BecomeInactive();

        SmartTile[] neighborTiles = tilemap.GetHexNeighbors(position);
        if (TilesTable.TileByType(type).CanBeWatered) {
            SwitchType(TilesTable.TileByType(type).WaterSwitch, AnimationType.Watercan);
        } else {
            TileData data = TilesTable.TileByType(type);
            if (data.TIndex == 1)
                yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(1, -1, 0)).OnWatered(animtime));
            else if (data.TIndex == 2)
                yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(0, -1, 0)).OnWatered(animtime));
            else if (data.TIndex == 3)
                yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(-1, 0, 0)).OnWatered(animtime));
            else
                switch (type) {
                    case TileType.Dandellion_1:

                        SmartTile[] neighbors = tilemap.GetHexNeighbors(position);
                        List<SmartTile> neighborsL = new();

                        for (int i = 0; i < neighbors.Length; i++)
                            if (neighbors[i].CanBeHoed())
                                neighborsL.Add(neighbors[i]);

                        for (int i = 0; i < 2; i++)
                            if (neighborsL.Count > 0) {
                                SmartTile tile = neighborsL[Random.Range(0, neighborsL.Count)];
                                neighborsL.Remove(tile);
                                yield return StartCoroutine(tile.OnHoed(animtime));
                            }

                        break;
                }
        }

        yield return new WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[3].position)[5].CanBeWatered())
            yield return tilemap.GetHexNeighbors(neighborTiles[3].position)[5].OnWatered(animtime);

        if (neighborTiles[4].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[4].position)[4].CanBeWatered())
            yield return tilemap.GetHexNeighbors(neighborTiles[4].position)[4].OnWatered(animtime);

        if (neighborTiles[5].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[5].position)[3].CanBeWatered())
            yield return tilemap.GetHexNeighbors(neighborTiles[5].position)[3].OnWatered(animtime);

        AudioManager.instance.PlaySound(Sounds.Watered);
        BecomeActive();
    }

    public IEnumerator OnCollected(bool isPlayerHaveGreenScythe, float animtime) {
        BecomeInactive();
        int multiplier = 1;
        SmartTile[] neighborTiles = tilemap.GetHexNeighbors(position);
        for (int i = 0; i < neighborTiles.Length; i++)
            if ((neighborTiles[i].type == TileType.Fern_1 && type != TileType.Onion_1) ||
                (neighborTiles[i].type == TileType.WateredFern_1 && type != TileType.Onion_1))
                multiplier = 2;
        if (TimeManager.instance.daysHappenings[TimeManager.instance.day] == HappeningType.Love)
            multiplier *= 2;

        if (TilesTable.TileByType(type).collectAmount > 0) {
            InventoryManager.instance.CollectCrop(TilesTable.TileByType(type).CropCollected,
                TilesTable.TileByType(type).collectAmount * multiplier);
            SwitchType(TileType.Soil, AnimationType.Hoe);
        } else {
            switch (type) {
                case TileType.Weed:
                    InventoryManager.instance.AddCoins(1 * multiplier);
                    if (isPlayerHaveGreenScythe)
                        InventoryManager.instance.CollectCrop(CropsType.Weed, 1 * multiplier);
                    SwitchType(TileType.Soil, AnimationType.Hoe);
                    break;

                case TileType.Onion_1:
                    //Собирает овощи с соседних клеток и за каждый увеличивает умножитель
                    neighborTiles = tilemap.GetHexNeighbors(position);
                    int counter = 1;
                    for (int i = 0; i < neighborTiles.Length; i++)
                        if (neighborTiles[i].CanBeCollected()) {
                            yield return neighborTiles[i].OnCollected(false, animtime);
                            counter++;
                        }

                    multiplier *= counter;

                    InventoryManager.instance.CollectCrop(CropsType.Onion, multiplier);
                    SwitchType(TileType.Soil, AnimationType.Hoe);
                    break;

                case TileType.Peanut_1:
                    InventoryManager.instance.CollectCrop(CropsType.Peanut, 1 * multiplier);
                    List<SmartTile> soilTiles = tilemap.GetNeighborsWithType(position, TileType.Peanut_1);

                    if (soilTiles.Count > 0)
                        if (soilTiles[0].CanBeCollected())
                            yield return soilTiles[0].OnCollected(false, animtime);

                    SwitchType(TileType.Soil, AnimationType.Hoe);
                    break;
            }
        }

        yield return new WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[3].position)[5].CanBeCollected())
            yield return tilemap.GetHexNeighbors(neighborTiles[3].position)[5]
                .OnCollected(isPlayerHaveGreenScythe, animtime);

        if (neighborTiles[4].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[4].position)[4].CanBeCollected())
            yield return tilemap.GetHexNeighbors(neighborTiles[4].position)[4]
                .OnCollected(isPlayerHaveGreenScythe, animtime);

        if (neighborTiles[5].type == TileType.Radish_1 &&
            tilemap.GetHexNeighbors(neighborTiles[5].position)[3].CanBeCollected())
            yield return tilemap.GetHexNeighbors(neighborTiles[5].position)[3]
                .OnCollected(isPlayerHaveGreenScythe, animtime);

        AudioManager.instance.PlaySound(Sounds.Collect);
        BecomeActive();
    }

    public IEnumerator OnNeyDayed(float animtime) {
        BecomeInactive();

        if (TilesTable.TileByType(type).NewDaySwitch != TileType.None)
            SwitchType(TilesTable.TileByType(type).NewDaySwitch);
        else
            switch (type) {
                case TileType.WateredSoil:
                    if (Random.Range(0, 2) == 1)
                        SwitchType(TileType.Weed);
                    break;

                case TileType.WateredCornSeed:
                    SwitchType(TileType.Corn_1);

                    SmartTile[] surroundingtiles = tilemap.GetHexNeighbors(position);
                    List<SmartTile> soilTiles = new();

                    for (int i = 0; i < surroundingtiles.Length; i++)
                        if (surroundingtiles[i].CanBeSeeded() &&
                            !TilesTable.TileByType(surroundingtiles[i].type).IsBuilding)
                            soilTiles.Add(surroundingtiles[i]);

                    if (soilTiles.Count > 0) {
                        int rnd = Random.Range(0, soilTiles.Count);
                        yield return StartCoroutine(soilTiles[rnd].OnSeeded(CropsType.Corn, animtime));
                    }

                    break;

                case TileType.WateredBeautyflowerseed_7:
                    SwitchType(TileType.Beautyflower_1);
                    SmartTile[] alltiles = tilemap.GetAllTiles();
                    for (int i = 0; i < alltiles.Length; i++)
                        if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                            yield return StartCoroutine(alltiles[i].OnSeeded(CropsType.Beautyflower, -1));
                    break;

                case TileType.FlycatherSeed_1:
                    SwitchType(TileType.FlycatherSeed_2);
                    alltiles = tilemap.GetAllTiles();
                    List<SmartTile> toSeedList = new();
                    for (int i = 0; i < alltiles.Length; i++)
                        if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                            toSeedList.Add(alltiles[i]);
                    for (int i = 0; i < 1; i++)
                        if (toSeedList.Count > 0) {
                            SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                            toSeedList.Remove(tile);
                            yield return StartCoroutine(tile.OnSeeded(CropsType.Flycatcher, animtime / 2));
                        }

                    break;

                case TileType.FlycatherSeed_2:
                    SwitchType(TileType.FlycatherSeed_3);
                    alltiles = tilemap.GetAllTiles();
                    toSeedList = new List<SmartTile>();
                    for (int i = 0; i < alltiles.Length; i++)
                        if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                            toSeedList.Add(alltiles[i]);
                    for (int i = 0; i < 2; i++)
                        if (toSeedList.Count > 0) {
                            SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                            toSeedList.Remove(tile);
                            yield return StartCoroutine(tile.OnSeeded(CropsType.Flycatcher, animtime / 2));
                        }

                    break;

                case TileType.FlycatherSeed_3:

                    alltiles = tilemap.GetAllTiles();
                    toSeedList = new List<SmartTile>();
                    for (int i = 0; i < alltiles.Length; i++)
                        if (alltiles[i].CanBeSeeded())
                            toSeedList.Add(alltiles[i]);
                    for (int i = 0; i < 3; i++)
                        if (toSeedList.Count > 0) {
                            SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                            toSeedList.Remove(tile);
                            yield return StartCoroutine(tile.OnSeeded(CropsType.Flycatcher, animtime / 2));
                        }

                    break;

                case TileType.Sprinkler_target:
                    if (tilemap.GetHexNeighbors(position)[4].CanBeWatered())
                        yield return StartCoroutine(tilemap.GetHexNeighbors(position)[4].OnWatered(animtime));

                    break;

                case TileType.Tractor_1:
                    SmartTile[] sandTiles = tilemap.GetAllTiles(TileType.Sand);
                    if (sandTiles.Length > 0) {
                        SwitchType(TileType.Tractor_2);
                        yield return sandTiles[Random.Range(0, sandTiles.Length)].OnHoed(animtime * 10);
                        SwitchType(TileType.Tractor_1);
                    }

                    break;

                case TileType.WateredPeanutSeed:
                    SwitchType(TileType.Peanut_1);

                    surroundingtiles = tilemap.GetHexNeighbors(position);
                    soilTiles = new List<SmartTile>();

                    for (int i = 0; i < surroundingtiles.Length; i++)
                        if (surroundingtiles[i].CanBeSeeded() &&
                            !TilesTable.TileByType(surroundingtiles[i].type).IsBuilding)
                            soilTiles.Add(surroundingtiles[i]);

                    if (soilTiles.Count > 0) {
                        int rnd = Random.Range(0, soilTiles.Count);
                        soilTiles[rnd].SwitchType(TileType.WateredPeanutSeed);
                        yield return new WaitForSeconds(animtime);

                        //Сложная система смерти всей цепочки при смерти последнего растения
                        surroundingtiles = tilemap.GetHexNeighbors(soilTiles[rnd].position);

                        for (int i = 0; i < surroundingtiles.Length; i++)
                            if (!TilesTable.TileByType(surroundingtiles[i].type).IsBuilding &&
                                TilesTable.TileByType(surroundingtiles[i].type).CropType != CropsType.Peanut &&
                                TilesTable.TileByType(surroundingtiles[i].type).CropType != CropsType.None) {
                                soilTiles[rnd].SwitchType(TileType.PeanutDead);

                                Vector3Int curPose = soilTiles[rnd].position;
                                soilTiles = tilemap.GetNeighborsWithType(curPose, TileType.Peanut_1);

                                int whileStopper = 0;
                                while (soilTiles.Count > 0 && whileStopper < 1000) {
                                    soilTiles[0].SwitchType(TileType.PeanutDead);
                                    yield return new WaitForSeconds(animtime);
                                    curPose = soilTiles[0].position;
                                    soilTiles = tilemap.GetNeighborsWithType(curPose, TileType.Peanut_1);
                                    whileStopper++;
                                }
                            }
                    }

                    break;

                case TileType.Radish_1:
                    InventoryManager.instance.AddCoins(-2);
                    break;
            }

        yield return new WaitForSeconds(animtime);
        BecomeActive();
    }

    public IEnumerator OnErosioned(float animtime) {
        BecomeInactive();
        switch (type) {
            case TileType.Freshener_full:
                SwitchType(TileType.Freshener_empty);
                break;

            case TileType.Soil:
                SwitchType(TileType.Sand);
                break;

            case TileType.WateredSoil:
                SwitchType(TileType.Weed);
                break;

            case TileType.CactusSeed:
                SwitchType(TileType.Cactus_1);
                break;
        }

        yield return new WaitForSeconds(animtime);
        BecomeActive();
    }

    public IEnumerator OnInsected(float animtime) {
        if (type == TileType.FlycatherSeed_3) {
            BecomeInactive();
            SwitchType(TileType.Flycather_1);
            yield break;
        }

        if (CanBeCollected()) {
            BecomeInactive();
            SwitchType(TileType.Soil);
        }

        yield return new WaitForSeconds(animtime);
        BecomeActive();
    }

    public IEnumerator OnClicked(float animtime) {
        BecomeInactive();

        TileData data = TilesTable.TileByType(type);
        if (data.TIndex == 1)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(1, -1, 0)).OnClicked(animtime));
        else if (data.TIndex == 2)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(0, -1, 0)).OnClicked(animtime));
        else if (data.TIndex == 3)
            yield return StartCoroutine(tilemap.GetTile(position + new Vector3Int(-1, 0, 0)).OnClicked(animtime));
        else
            switch (type) {
                case TileType.Biogen_empty:

                    List<SmartTile> weedArea = new() {
                        tilemap.GetTile(position + new Vector3Int(2, 0, 0)),
                        tilemap.GetTile(position + new Vector3Int(3, 0, 0)),
                        tilemap.GetTile(position + new Vector3Int(4, 0, 0)),
                        tilemap.GetTile(position + new Vector3Int(1, 1, 0)),
                        tilemap.GetTile(position + new Vector3Int(2, 1, 0)),
                        tilemap.GetTile(position + new Vector3Int(3, 1, 0))
                    };
                    while (weedArea.Count > 0) {
                        SmartTile tile = weedArea[Random.Range(0, weedArea.Count)];
                        if (tile.type == TileType.Weed) {
                            SwitchType(TileType.Biogen_full);
                            tile.SwitchType(TileType.Soil);

                            yield return new WaitForSeconds(1.5f);
                            PlayerController.instance.RestoreEnergy(1);
                            SwitchType(TileType.Biogen_empty);
                            break;
                        }

                        weedArea.Remove(tile);
                    }

                    break;
            }

        yield return new WaitForSeconds(0);
        BecomeActive();
    }

    /**********/

    public void SwitchType(TileType newType, AnimationType animationType = AnimationType.None) {
        type = newType;
        tilemap.PlaceTile(position, type);

        tilemap.toolsAnimTilemap.StartAnimationInCoord(position, animationType);
    }
}

[Serializable]
public struct Group {
    public string name;
    public bool isBuilding;
    public CropsType CropType;
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
    public CropsType CropCollected;
    public int collectAmount;

    [HideInInspector]
    public CropsType CropType;

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