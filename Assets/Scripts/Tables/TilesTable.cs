using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Time = Managers.Time;

namespace Tables
{
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
    public enum TileType {
        None,
        Sand,
        Soil,
        Weed,
        WateredSoil,
        Rocks,
        TomatoSeed,
        WateredTomatoSeed,
        Tomato1,
        Tomato2,
        Tomato3,
        EggplantSeed,
        WateredEggplantSeed,
        Eggplant1,
        Eggplant2,
        Eggplant3,
        CornSeed,
        WateredCornSeed,
        Corn1,
        DandellionSeed,
        WateredDandelliomSeed,
        Dandellion1,
        StrawberrySeed,
        WateredStrawberrySeed,
        Strawberry1,
        CactusSeed,
        Cactus1,
        FernSeed,
        WateredFernSeed,
        Fern1,
        WateredFern1,
        FernDead,
        Beautyflowerseed1,
        Beautyflowerseed2,
        Beautyflowerseed3,
        Beautyflowerseed4,
        Beautyflowerseed5,
        Beautyflowerseed6,
        Beautyflowerseed7,
        WateredBeautyflowerseed1,
        WateredBeautyflowerseed2,
        WateredBeautyflowerseed3,
        WateredBeautyflowerseed4,
        WateredBeautyflowerseed5,
        WateredBeautyflowerseed6,
        WateredBeautyflowerseed7,
        Beautyflower1,
        BeautyflowerDead,
        BeautyflowerSibling,
        FlycatherSeed1,
        FlycatherSeed2,
        FlycatherSeed3,
        Flycather1,
        OnionSeed,
        OnionWatered,
        Onion1,
        BiogenEmpty,
        BiogenFull,
        BiogenT1,
        BiogenT2,
        BiogenT3,
        BiogenConstruction,
        FreshenerEmpty,
        Freshener1,
        Freshener2,
        Freshener3,
        Freshener4,
        Freshener5,
        Freshener6,
        FreshenerFull,
        FreshenerConstruction,
        FreshenerT1,
        FreshenerT2,
        FreshenerT3,
        SprinklerEmpty,
        Sprinkler1,
        Sprinkler2,
        Sprinkler3,
        Sprinkler4,
        Sprinkler5,
        SprinklerFull,
        SprinklerTarget,
        SprinklerConstruction,
        SprinklerT1,
        SprinklerT2,
        SprinklerT3,
        SeedDoublerEmpty,
        SeedDoublerFull,
        SeedDoublerConstruction,
        SeedDoublerT1,
        SeedDoublerT2,
        SeedDoublerT3,
        Tractor1,
        Tractor2,
        TractorConstruction,
        TractorT1,
        TractorT2,
        TractorT3,
        Pumpkinseed1,
        Pumpkinseed2,
        Pumpkinseed3,
        WateredPumpkinseed1,
        WateredPumpkinseed2,
        WateredPumpkinseed3,
        GrownPumpkinseed1,
        GrownPumpkinseed2,
        GrownPumpkinseed3,
        Pumpkin1,
        Pumpkin2,
        Pumpkin3,
        RadishSeed,
        WateredRadishSeed,
        Radish1,
        PeanutSeed,
        WateredPeanutSeed,
        Peanut1,
        PeanutDead
    }

    [Serializable]
    public class SmartTile : MonoBehaviour {
        public TileType type;
        public bool isActive;

        private Vector3Int _position;
        private SmartTilemap _tilemap;

        public void Init(SmartTilemap tilemap, TileType type, Vector3Int pos) {
            this._tilemap = tilemap;
            this.type = type;
            _position = pos;
            isActive = true;
            this._tilemap.MainTilemap.SetColor(_position, Color.red);
        }

        /**********/

        public bool CanBeHoed() {
            if (!isActive)
                return false;

            if (TilesTable.TileByType(type).CanBeHoed) return true;

            if (type == TileType.Strawberry1) {
                isActive = false;
                SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
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
                return _tilemap.GetBuildingByPart(_position).CanBeSeeded();
            if (TilesTable.TileByType(type).CanBeSeeded) return true;

            return false;
        }

        public bool CanBeWatered() {
            if (!isActive)
                return false;

            if (TilesTable.TileByType(type).TIndex > 0) return _tilemap.GetBuildingByPart(_position).CanBeWatered();

            if (TilesTable.TileByType(type).CanBeWatered) {
                return true;
            }

            if (type == TileType.Dandellion1) {
                isActive = false;
                SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
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

            if (type == TileType.SprinklerTarget) {
                TileType[] tocheck = new TileType[2] {TileType.SprinklerConstruction, TileType.SprinklerEmpty};
                return !_tilemap.HasTile(tocheck);
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
                return _tilemap.GetBuildingByPart(_position).CanBeClicked();
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
            _tilemap.MainTilemap.SetColor(_position, Color.white);
        }

        public void BecomeInactive() {
            isActive = false;
            _tilemap.MainTilemap.SetColor(_position, Color.grey);
        }

        /**********/

        public IEnumerator OnHoed(float animtime) {
            BecomeInactive();

            SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);

            switch (type) {
                case TileType.Sand:
                case TileType.FernDead:
                case TileType.BeautyflowerDead:
                case TileType.PeanutDead:
                    SwitchType(TileType.Soil, AnimationType.Hoe);
                    break;

                case TileType.Strawberry1:
                    SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
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

            if (neighborTiles[3].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeHoed())
                yield return _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnHoed(animtime);

            if (neighborTiles[4].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeHoed())
                yield return _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnHoed(animtime);

            if (neighborTiles[5].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeHoed())
                yield return _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnHoed(animtime);

            Audio.Instance.PlaySound(Sounds.Hoed);
            BecomeActive();
        }

        public IEnumerator OnSeeded(Crop seedtype, float animtime) {
            BecomeInactive();

            SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);

            TileData data = TilesTable.TileByType(type);
            if (data.TIndex == 1)
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(1, -1, 0))
                    .OnSeeded(seedtype, animtime));
            else if (data.TIndex == 2)
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(0, -1, 0))
                    .OnSeeded(seedtype, animtime));
            else if (data.TIndex == 3)
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(-1, 0, 0))
                    .OnSeeded(seedtype, animtime));
            else
                switch (type) {
                    case TileType.SeedDoublerEmpty:
                    case TileType.SeedDoublerFull:
                        _tilemap.seedShop.SetAmbarCrop(seedtype);
                        SwitchType(TileType.SeedDoublerFull);
                        break;

                    case TileType.Soil:
                        switch (seedtype) {
                            case Crop.Tomato:
                                SwitchType(TileType.TomatoSeed);
                                break;

                            case Crop.Eggplant:
                                SwitchType(TileType.EggplantSeed);
                                break;

                            case Crop.Corn:
                                SwitchType(TileType.CornSeed);
                                break;

                            case Crop.Dandellion:
                                SwitchType(TileType.DandellionSeed);
                                break;

                            case Crop.Strawberry:
                                SwitchType(TileType.StrawberrySeed);
                                break;

                            case Crop.Cactus:
                                SwitchType(TileType.CactusSeed);
                                break;

                            case Crop.Fern:
                                SwitchType(TileType.FernSeed);
                                break;

                            case Crop.Beautyflower:
                                if (animtime == -1)
                                    SwitchType(TileType.BeautyflowerSibling);
                                else
                                    SwitchType(TileType.Beautyflowerseed1);
                                break;

                            case Crop.Flycatcher:
                                SwitchType(TileType.FlycatherSeed1);
                                break;

                            case Crop.Onion:
                                SwitchType(TileType.OnionSeed);
                                break;

                            case Crop.Pumpkin:

                                SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
                                List<SmartTile> pumpList = new();
                                for (int i = 0; i < neighbors.Length; i++) {
                                    TileType tmpType = neighbors[i].type;
                                    if (tmpType == TileType.Pumpkinseed1) {
                                        neighbors[i].SwitchType(TileType.Pumpkinseed2);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.Pumpkinseed2) {
                                        neighbors[i].SwitchType(TileType.Pumpkinseed3);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.Pumpkinseed3)
                                        pumpList.Add(neighbors[i]);

                                    if (tmpType == TileType.WateredPumpkinseed1) {
                                        neighbors[i].SwitchType(TileType.WateredPumpkinseed2);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.WateredPumpkinseed2) {
                                        neighbors[i].SwitchType(TileType.WateredPumpkinseed3);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.WateredPumpkinseed3)
                                        pumpList.Add(neighbors[i]);

                                    if (tmpType == TileType.GrownPumpkinseed1) {
                                        neighbors[i].SwitchType(TileType.GrownPumpkinseed2);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.GrownPumpkinseed2) {
                                        neighbors[i].SwitchType(TileType.GrownPumpkinseed3);
                                        pumpList.Add(neighbors[i]);
                                    }

                                    if (tmpType == TileType.GrownPumpkinseed3)
                                        pumpList.Add(neighbors[i]);
                                }

                                if (pumpList.Count == 0)
                                    SwitchType(TileType.Pumpkinseed1);
                                if (pumpList.Count == 1)
                                    SwitchType(TileType.Pumpkinseed2);
                                if (pumpList.Count >= 2)
                                    SwitchType(TileType.Pumpkinseed3);

                                break;

                            case Crop.Radish:
                                SwitchType(TileType.RadishSeed);
                                break;

                            case Crop.Peanut:
                                SwitchType(TileType.PeanutSeed);
                                break;
                        }

                        break;
                }

            yield return new WaitForSeconds(animtime);

            if (InventoryManager.Instance.SeedsInventory[seedtype] > 0) {
                if (neighborTiles[3].type == TileType.Radish1 &&
                    _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeSeeded()) {
                    yield return _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnSeeded(seedtype, animtime);
                    InventoryManager.Instance.LoseSeed(seedtype);
                }

                if (neighborTiles[4].type == TileType.Radish1 &&
                    _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeSeeded()) {
                    yield return _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnSeeded(seedtype, animtime);
                    InventoryManager.Instance.LoseSeed(seedtype);
                }

                if (neighborTiles[5].type == TileType.Radish1 &&
                    _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeSeeded()) {
                    yield return _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnSeeded(seedtype, animtime);
                    InventoryManager.Instance.LoseSeed(seedtype);
                }
            }

            Audio.Instance.PlaySound(Sounds.Seeded);
            BecomeActive();
        }

        public IEnumerator OnWatered(float animtime) {
            BecomeInactive();

            SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
            if (TilesTable.TileByType(type).CanBeWatered) {
                SwitchType(TilesTable.TileByType(type).WaterSwitch, AnimationType.Watercan);
            } else {
                TileData data = TilesTable.TileByType(type);
                if (data.TIndex == 1)
                    yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(1, -1, 0)).OnWatered(animtime));
                else if (data.TIndex == 2)
                    yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(0, -1, 0)).OnWatered(animtime));
                else if (data.TIndex == 3)
                    yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(-1, 0, 0)).OnWatered(animtime));
                else
                    switch (type) {
                        case TileType.Dandellion1:

                            SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
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

            if (neighborTiles[3].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeWatered())
                yield return _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnWatered(animtime);

            if (neighborTiles[4].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeWatered())
                yield return _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnWatered(animtime);

            if (neighborTiles[5].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeWatered())
                yield return _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnWatered(animtime);

            Audio.Instance.PlaySound(Sounds.Watered);
            BecomeActive();
        }

        public IEnumerator OnCollected(bool isPlayerHaveGreenScythe, float animtime) {
            BecomeInactive();
            int multiplier = 1;
            SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
            for (int i = 0; i < neighborTiles.Length; i++)
                if ((neighborTiles[i].type == TileType.Fern1 && type != TileType.Onion1) ||
                    (neighborTiles[i].type == TileType.WateredFern1 && type != TileType.Onion1))
                    multiplier = 2;
            if (Time.Instance.IsTodayLoveDay)
                multiplier *= 2;

            if (TilesTable.TileByType(type).collectAmount > 0) {
                InventoryManager.Instance.AddCollectedCrop(TilesTable.TileByType(type).cropCollected,
                    TilesTable.TileByType(type).collectAmount * multiplier);
                SwitchType(TileType.Soil, AnimationType.Hoe);
            } else {
                switch (type) {
                    case TileType.Weed:
                        InventoryManager.Instance.AddCoins(1 * multiplier);
                        if (isPlayerHaveGreenScythe)
                            InventoryManager.Instance.AddCollectedCrop(Crop.Weed, 1 * multiplier);
                        SwitchType(TileType.Soil, AnimationType.Hoe);
                        break;

                    case TileType.Onion1:
                        //Собирает овощи с соседних клеток и за каждый увеличивает умножитель
                        neighborTiles = _tilemap.GetHexNeighbors(_position);
                        int counter = 1;
                        for (int i = 0; i < neighborTiles.Length; i++)
                            if (neighborTiles[i].CanBeCollected()) {
                                yield return neighborTiles[i].OnCollected(false, animtime);
                                counter++;
                            }

                        multiplier *= counter;

                        InventoryManager.Instance.AddCollectedCrop(Crop.Onion, multiplier);
                        SwitchType(TileType.Soil, AnimationType.Hoe);
                        break;

                    case TileType.Peanut1:
                        InventoryManager.Instance.AddCollectedCrop(Crop.Peanut, 1 * multiplier);
                        List<SmartTile> soilTiles = _tilemap.GetNeighborsWithType(_position, TileType.Peanut1);

                        if (soilTiles.Count > 0)
                            if (soilTiles[0].CanBeCollected())
                                yield return soilTiles[0].OnCollected(false, animtime);

                        SwitchType(TileType.Soil, AnimationType.Hoe);
                        break;
                }
            }

            yield return new WaitForSeconds(animtime);

            if (neighborTiles[3].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeCollected())
                yield return _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5]
                    .OnCollected(isPlayerHaveGreenScythe, animtime);

            if (neighborTiles[4].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeCollected())
                yield return _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4]
                    .OnCollected(isPlayerHaveGreenScythe, animtime);

            if (neighborTiles[5].type == TileType.Radish1 &&
                _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeCollected())
                yield return _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3]
                    .OnCollected(isPlayerHaveGreenScythe, animtime);

            Audio.Instance.PlaySound(Sounds.Collect);
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
                        SwitchType(TileType.Corn1);

                        SmartTile[] surroundingtiles = _tilemap.GetHexNeighbors(_position);
                        List<SmartTile> soilTiles = new();

                        for (int i = 0; i < surroundingtiles.Length; i++)
                            if (surroundingtiles[i].CanBeSeeded() &&
                                !TilesTable.TileByType(surroundingtiles[i].type).IsBuilding)
                                soilTiles.Add(surroundingtiles[i]);

                        if (soilTiles.Count > 0) {
                            int rnd = Random.Range(0, soilTiles.Count);
                            yield return StartCoroutine(soilTiles[rnd].OnSeeded(Crop.Corn, animtime));
                        }

                        break;

                    case TileType.WateredBeautyflowerseed7:
                        SwitchType(TileType.Beautyflower1);
                        SmartTile[] alltiles = _tilemap.GetAllTiles();
                        for (int i = 0; i < alltiles.Length; i++)
                            if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                                yield return StartCoroutine(alltiles[i].OnSeeded(Crop.Beautyflower, -1));
                        break;

                    case TileType.FlycatherSeed1:
                        SwitchType(TileType.FlycatherSeed2);
                        alltiles = _tilemap.GetAllTiles();
                        List<SmartTile> toSeedList = new();
                        for (int i = 0; i < alltiles.Length; i++)
                            if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                                toSeedList.Add(alltiles[i]);
                        for (int i = 0; i < 1; i++)
                            if (toSeedList.Count > 0) {
                                SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                                toSeedList.Remove(tile);
                                yield return StartCoroutine(tile.OnSeeded(Crop.Flycatcher, animtime / 2));
                            }

                        break;

                    case TileType.FlycatherSeed2:
                        SwitchType(TileType.FlycatherSeed3);
                        alltiles = _tilemap.GetAllTiles();
                        toSeedList = new List<SmartTile>();
                        for (int i = 0; i < alltiles.Length; i++)
                            if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                                toSeedList.Add(alltiles[i]);
                        for (int i = 0; i < 2; i++)
                            if (toSeedList.Count > 0) {
                                SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                                toSeedList.Remove(tile);
                                yield return StartCoroutine(tile.OnSeeded(Crop.Flycatcher, animtime / 2));
                            }

                        break;

                    case TileType.FlycatherSeed3:

                        alltiles = _tilemap.GetAllTiles();
                        toSeedList = new List<SmartTile>();
                        for (int i = 0; i < alltiles.Length; i++)
                            if (alltiles[i].CanBeSeeded())
                                toSeedList.Add(alltiles[i]);
                        for (int i = 0; i < 3; i++)
                            if (toSeedList.Count > 0) {
                                SmartTile tile = toSeedList[Random.Range(0, toSeedList.Count)];
                                toSeedList.Remove(tile);
                                yield return StartCoroutine(tile.OnSeeded(Crop.Flycatcher, animtime / 2));
                            }

                        break;

                    case TileType.SprinklerTarget:
                        if (_tilemap.GetHexNeighbors(_position)[4].CanBeWatered())
                            yield return StartCoroutine(_tilemap.GetHexNeighbors(_position)[4].OnWatered(animtime));

                        break;

                    case TileType.Tractor1:
                        SmartTile[] sandTiles = _tilemap.GetAllTiles(TileType.Sand);
                        if (sandTiles.Length > 0) {
                            SwitchType(TileType.Tractor2);
                            yield return sandTiles[Random.Range(0, sandTiles.Length)].OnHoed(animtime * 10);
                            SwitchType(TileType.Tractor1);
                        }

                        break;

                    case TileType.WateredPeanutSeed:
                        SwitchType(TileType.Peanut1);

                        surroundingtiles = _tilemap.GetHexNeighbors(_position);
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
                            surroundingtiles = _tilemap.GetHexNeighbors(soilTiles[rnd]._position);

                            for (int i = 0; i < surroundingtiles.Length; i++)
                            {
                                TileData tile = TilesTable.TileByType(surroundingtiles[i].type);
                                if (!tile.IsBuilding && tile.crop != Crop.Peanut && tile.crop != Crop.None) {
                                    soilTiles[rnd].SwitchType(TileType.PeanutDead);

                                    Vector3Int curPose = soilTiles[rnd]._position;
                                    soilTiles = _tilemap.GetNeighborsWithType(curPose, TileType.Peanut1);

                                    int whileStopper = 0;
                                    while (soilTiles.Count > 0 && whileStopper < 1000) {
                                        soilTiles[0].SwitchType(TileType.PeanutDead);
                                        yield return new WaitForSeconds(animtime);
                                        curPose = soilTiles[0]._position;
                                        soilTiles = _tilemap.GetNeighborsWithType(curPose, TileType.Peanut1);
                                        whileStopper++;
                                    }
                                }  
                            }
                              
                        }

                        break;

                    case TileType.Radish1:
                        InventoryManager.Instance.AddCoins(-2);
                        break;
                }

            yield return new WaitForSeconds(animtime);
            BecomeActive();
        }

        public IEnumerator OnErosioned(float animtime) {
            BecomeInactive();
            switch (type) {
                case TileType.FreshenerFull:
                    SwitchType(TileType.FreshenerEmpty);
                    break;

                case TileType.Soil:
                    SwitchType(TileType.Sand);
                    break;

                case TileType.WateredSoil:
                    SwitchType(TileType.Weed);
                    break;

                case TileType.CactusSeed:
                    SwitchType(TileType.Cactus1);
                    break;
            }

            yield return new WaitForSeconds(animtime);
            BecomeActive();
        }

        public IEnumerator OnInsected(float animtime) {
            if (type == TileType.FlycatherSeed3) {
                BecomeInactive();
                SwitchType(TileType.Flycather1);
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
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(1, -1, 0)).OnClicked(animtime));
            else if (data.TIndex == 2)
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(0, -1, 0)).OnClicked(animtime));
            else if (data.TIndex == 3)
                yield return StartCoroutine(_tilemap.GetTile(_position + new Vector3Int(-1, 0, 0)).OnClicked(animtime));
            else
                switch (type) {
                    case TileType.BiogenEmpty:

                        List<SmartTile> weedArea = new() {
                            _tilemap.GetTile(_position + new Vector3Int(2, 0, 0)),
                            _tilemap.GetTile(_position + new Vector3Int(3, 0, 0)),
                            _tilemap.GetTile(_position + new Vector3Int(4, 0, 0)),
                            _tilemap.GetTile(_position + new Vector3Int(1, 1, 0)),
                            _tilemap.GetTile(_position + new Vector3Int(2, 1, 0)),
                            _tilemap.GetTile(_position + new Vector3Int(3, 1, 0))
                        };
                        while (weedArea.Count > 0) {
                            SmartTile tile = weedArea[Random.Range(0, weedArea.Count)];
                            if (tile.type == TileType.Weed) {
                                SwitchType(TileType.BiogenFull);
                                tile.SwitchType(TileType.Soil);

                                yield return new WaitForSeconds(1.5f);
                                Energy.Instance.RestoreEnergy(1);
                                SwitchType(TileType.BiogenEmpty);
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
            _tilemap.PlaceTile(_position, type);

            _tilemap.toolsAnimTilemap.StartAnimationInCoord(_position, animationType);
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