using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers;
using Tables;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class SmartTile {
    private bool _isSandAfterHarvest = true;
    public TileType type;
    public bool isActive;

    private Vector2Int _position;
    private SmartTilemap _tilemap;

    public void Init(SmartTilemap tilemap, TileType type, Vector2Int pos) {
        _tilemap = tilemap;
        this.type = type;
        _position = pos;
        isActive = true;
        SwitchType(this.type);
    }

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
            TileType[] tocheck = new TileType[2] { TileType.SprinklerConstruction, TileType.SprinklerEmpty };
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

    public void BecomeActive() {
        isActive = true;
        _tilemap.MainTilemap.SetColor((Vector3Int)_position, Color.white);
    }

    public void BecomeInactive() {
        isActive = false;
        _tilemap.MainTilemap.SetColor((Vector3Int)_position, Color.grey);
    }

    public async UniTask OnHoed(float animtime, bool isDandellion = false, bool isTractor = false) {
        BecomeInactive();

        SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
        var animationType = AnimationType.Hoe;
        if (isDandellion) {
            animationType = AnimationType.Dandellion;
            QuestsManager.TriggerQuest(nameof(QuestTypes.Collect) + SpecialTargetTypes.DandellionHoed, 1);
        }

        if (isTractor) {
            animationType = AnimationType.Tractor;
        }

        switch (type) {
            case TileType.Sand:
                SwitchType(TileType.Soil, animationType);
                break;
            case TileType.FernDead:
            case TileType.BeautyflowerDead:
            case TileType.PeanutDead:
                SwitchType(TileType.Soil, animationType);
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
                        await tile.OnWatered(animtime, isStrawberry: true);
                    }

                break;
        }

        await UniTask.WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeHoed())
            await _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnHoed(animtime, isDandellion, isTractor);

        if (neighborTiles[4].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeHoed())
            await _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnHoed(animtime, isDandellion, isTractor);

        if (neighborTiles[5].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeHoed())
            await _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnHoed(animtime, isDandellion, isTractor);

        BurstFx(_tilemap.HoeFx);
        Audio.Instance.PlaySound(Sounds.Hoed);
        BecomeActive();
    }

    public async UniTask OnSeeded(Crop seedtype, float animtime, bool isWind = false) {
        BecomeInactive();

        SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
        var animationType = AnimationType.None;
        if (isWind) {
            animationType = AnimationType.Wind;
        }

        TileData data = TilesTable.TileByType(type);
        if (data.TIndex == 1)
            await _tilemap.GetTile(_position + new Vector2Int(1, -1)).OnSeeded(seedtype, animtime);
        else if (data.TIndex == 2)
            await _tilemap.GetTile(_position + new Vector2Int(0, -1)).OnSeeded(seedtype, animtime);
        else if (data.TIndex == 3)
            await _tilemap.GetTile(_position + new Vector2Int(-1, 0)).OnSeeded(seedtype, animtime);
        else
            switch (type) {
                case TileType.SeedDoublerEmpty:
                case TileType.SeedDoublerFull:
                    if (isWind) {
                        break;
                    }

                    SaveLoadManager.CurrentSave.SeedShopData.AmbarCrop = seedtype;
                    SwitchType(TileType.SeedDoublerFull);
                    break;

                case TileType.Soil:
                    switch (seedtype) {
                        case Crop.Tomato:
                            SwitchType(TileType.TomatoSeed, animationType);
                            break;

                        case Crop.Eggplant:
                            SwitchType(TileType.EggplantSeed, animationType);
                            break;

                        case Crop.Corn:
                            SwitchType(TileType.CornSeed, animationType);
                            break;

                        case Crop.Dandellion:
                            SwitchType(TileType.DandellionSeed, animationType);
                            break;

                        case Crop.Strawberry:
                            SwitchType(TileType.StrawberrySeed, animationType);
                            break;

                        case Crop.Cactus:
                            SwitchType(TileType.CactusSeed, animationType);
                            break;

                        case Crop.Fern:
                            SwitchType(TileType.FernSeed, animationType);
                            break;

                        case Crop.Beautyflower:
                            if (animtime == -1)
                                SwitchType(TileType.BeautyflowerSibling, animationType);
                            else
                                SwitchType(TileType.Beautyflowerseed1, animationType);
                            break;

                        case Crop.Flycatcher:
                            SwitchType(TileType.FlycatherSeed1, animationType);
                            break;

                        case Crop.Onion:
                            SwitchType(TileType.OnionSeed, animationType);
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
                                SwitchType(TileType.Pumpkinseed1, animationType);
                            if (pumpList.Count == 1)
                                SwitchType(TileType.Pumpkinseed2, animationType);
                            if (pumpList.Count >= 2)
                                SwitchType(TileType.Pumpkinseed3, animationType);

                            break;

                        case Crop.Radish:
                            SwitchType(TileType.RadishSeed, animationType);
                            break;

                        case Crop.Peanut:
                            SwitchType(TileType.PeanutSeed, animationType);
                            break;
                    }

                    break;
            }

        await UniTask.WaitForSeconds(animtime);

        if (InventoryManager.SeedsInventory[seedtype] > 0) {
            if (neighborTiles[3].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeSeeded()) {
                await _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnSeeded(seedtype, animtime, isWind);
                InventoryManager.Instance.LoseSeed(seedtype);
            }

            if (neighborTiles[4].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeSeeded()) {
                await _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnSeeded(seedtype, animtime, isWind);
                InventoryManager.Instance.LoseSeed(seedtype);
            }

            if (neighborTiles[5].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeSeeded()) {
                await _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnSeeded(seedtype, animtime, isWind);
                InventoryManager.Instance.LoseSeed(seedtype);
            }
        }

        Audio.Instance.PlaySound(Sounds.Seeded);
        BecomeActive();
    }

    public async UniTask OnWatered(float animtime, bool isRain = false, bool isStrawberry = false) {
        BecomeInactive();

        SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
        TileData data = TilesTable.TileByType(type);
        if (data.CanBeWatered) {
            AnimationType animType = AnimationType.Watercan;
            if (isStrawberry) {
                animType = AnimationType.Strawberry;
                QuestsManager.TriggerQuest(nameof(QuestTypes.Collect) + SpecialTargetTypes.StrawberryWatered, 1);
            } else if (isRain) {
                animType = AnimationType.Rain;
            }

            SwitchType(TilesTable.TileByType(type).WaterSwitch, animType);
        } else {
            if (data.TIndex == 1)
                await _tilemap.GetTile(_position + new Vector2Int(1, -1)).OnWatered(animtime, isRain, isStrawberry);
            else if (data.TIndex == 2)
                await _tilemap.GetTile(_position + new Vector2Int(0, -1)).OnWatered(animtime, isRain, isStrawberry);
            else if (data.TIndex == 3)
                await _tilemap.GetTile(_position + new Vector2Int(-1, 0)).OnWatered(animtime, isRain, isStrawberry);
            else
                switch (type) { }
        }

        await UniTask.WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeWatered())
            await _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnWatered(animtime, isRain, isStrawberry);

        if (neighborTiles[4].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeWatered())
            await _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnWatered(animtime, isRain, isStrawberry);

        if (neighborTiles[5].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeWatered())
            await _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnWatered(animtime, isRain, isStrawberry);

        BurstFx(_tilemap.WaterFx);
        Audio.Instance.PlaySound(Sounds.Watered);
        BecomeActive();
    }

    public async UniTask OnCollected(bool hasGreenScythe, bool hasGoldenScythe, float animtime) {
        hasGreenScythe = hasGreenScythe && !hasGoldenScythe;
        BecomeInactive();
        int multiplier = 1;
        SmartTile[] neighborTiles = _tilemap.GetHexNeighbors(_position);
        for (int i = 0; i < neighborTiles.Length; i++)
            if ((neighborTiles[i].type == TileType.Fern1 && type != TileType.Onion1) ||
                (neighborTiles[i].type == TileType.WateredFern1 && type != TileType.Onion1))
                multiplier = 2;

        if (hasGoldenScythe)
            multiplier *= 3;

        if (TilesTable.TileByType(type).collectAmount > 0) {
            var crop = TilesTable.TileByType(type).cropCollected;
            var amount = TilesTable.TileByType(type).collectAmount * multiplier;
            HarvestCrop(crop, amount);
            if (type == TileType.Eggplant3) {
                QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + SpecialTargetTypes.GiantEggplant, 1);
            }

            var obj = _tilemap.MainTilemap.GetInstantiatedObject((Vector3Int)_position);
            if (obj != null) {
                obj.GetComponent<Tile3d>().Scythe(crop, amount).Forget();
            }

            SwitchType(_isSandAfterHarvest ? TileType.Sand : TileType.Soil, AnimationType.Scythe);
        } else {
            switch (type) {
                case TileType.Weed:
                    UnlockableUtils.Unlock(Crop.Weed);
                    DropCoin(1 * multiplier);
                    InventoryManager.Instance.AddCoins(1 * multiplier);
                    if (hasGreenScythe) {
                        HarvestCrop(Crop.Weed, 1 * multiplier);
                    }

                    SwitchType(_isSandAfterHarvest ? TileType.Sand : TileType.Soil, AnimationType.Scythe);
                    break;

                case TileType.Onion1:
                    //Собирает овощи с соседних клеток и за каждый увеличивает умножитель
                    neighborTiles = _tilemap.GetHexNeighbors(_position);
                    int counter = 1;
                    for (int i = 0; i < neighborTiles.Length; i++)
                        if (neighborTiles[i].CanBeCollected()) {
                            await neighborTiles[i].OnCollected(false, false, animtime);
                            counter++;
                        }

                    multiplier *= counter;

                    HarvestCrop(Crop.Onion, 1 * multiplier);
                    SwitchType(_isSandAfterHarvest ? TileType.Sand : TileType.Soil, AnimationType.Scythe);
                    break;

                case TileType.Peanut1:
                    HarvestCrop(Crop.Peanut, 1 * multiplier);
                    List<SmartTile> soilTiles = _tilemap.GetNeighborsWithType(_position, TileType.Peanut1);

                    if (soilTiles.Count > 0)
                        if (soilTiles[0].CanBeCollected())
                            await soilTiles[0].OnCollected(false, false, animtime);

                    SwitchType(_isSandAfterHarvest ? TileType.Sand : TileType.Soil, AnimationType.Scythe);
                    break;
            }
        }

        await UniTask.WaitForSeconds(animtime);

        if (neighborTiles[3].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].CanBeCollected())
            await _tilemap.GetHexNeighbors(neighborTiles[3]._position)[5].OnCollected(hasGreenScythe, hasGoldenScythe, animtime);

        if (neighborTiles[4].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].CanBeCollected())
            await _tilemap.GetHexNeighbors(neighborTiles[4]._position)[4].OnCollected(hasGreenScythe, hasGoldenScythe, animtime);

        if (neighborTiles[5].type == TileType.Radish1 && _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].CanBeCollected())
            await _tilemap.GetHexNeighbors(neighborTiles[5]._position)[3].OnCollected(hasGreenScythe, hasGoldenScythe, animtime);

        BurstFx(_tilemap.ScytheFx);
        Audio.Instance.PlaySound(Sounds.Collect);
        BecomeActive();
    }

    public async UniTask OnNeyDayed(float animtime) {
        BecomeInactive();

        if (TilesTable.TileByType(type).NewDaySwitch != TileType.None) {
            SwitchType(TilesTable.TileByType(type).NewDaySwitch);
            await OnAppear(animtime);
        } else
            switch (type) {
                case TileType.WateredSoil:
                    if (Random.Range(0, 5) == 1)
                        SwitchType(TileType.Weed);
                    break;

                case TileType.WateredCornSeed:
                    SwitchType(TileType.Corn1);

                    SmartTile[] surroundingtiles = _tilemap.GetHexNeighbors(_position);
                    List<SmartTile> soilTiles = new();

                    for (int i = 0; i < surroundingtiles.Length; i++)
                        if (surroundingtiles[i].CanBeSeeded() && !TilesTable.TileByType(surroundingtiles[i].type).IsBuilding)
                            soilTiles.Add(surroundingtiles[i]);

                    if (soilTiles.Count > 0) {
                        int rnd = Random.Range(0, soilTiles.Count);
                        await soilTiles[rnd].OnSeeded(Crop.Corn, animtime);
                    }

                    break;

                case TileType.WateredBeautyflowerseed7:
                    SwitchType(TileType.Beautyflower1);
                    SmartTile[] alltiles = _tilemap.GetAllTiles();
                    for (int i = 0; i < alltiles.Length; i++)
                        if (alltiles[i].CanBeSeeded() && !TilesTable.TileByType(alltiles[i].type).IsBuilding)
                            await alltiles[i].OnSeeded(Crop.Beautyflower, -1);
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
                            await tile.OnSeeded(Crop.Flycatcher, animtime / 2);
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
                            await tile.OnSeeded(Crop.Flycatcher, animtime / 2);
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
                            await tile.OnSeeded(Crop.Flycatcher, animtime / 2);
                        }

                    break;

                case TileType.SprinklerTarget:
                    if (_tilemap.GetHexNeighbors(_position)[4].CanBeWatered())
                        await _tilemap.GetHexNeighbors(_position)[4].OnWatered(animtime, true);

                    break;

                case TileType.Tractor1:
                    SmartTile[] sandTiles = _tilemap.GetAllTiles(TileType.Sand);
                    if (sandTiles.Length > 0) {
                        SwitchType(TileType.Tractor2);
                        await sandTiles[Random.Range(0, sandTiles.Length)].OnHoed(animtime * 10);
                        SwitchType(TileType.Tractor1);
                    }

                    break;

                case TileType.WateredPeanutSeed:
                    SwitchType(TileType.Peanut1);

                    surroundingtiles = _tilemap.GetHexNeighbors(_position);
                    soilTiles = new List<SmartTile>();

                    for (int i = 0; i < surroundingtiles.Length; i++)
                        if (surroundingtiles[i].CanBeSeeded() && !TilesTable.TileByType(surroundingtiles[i].type).IsBuilding)
                            soilTiles.Add(surroundingtiles[i]);

                    if (soilTiles.Count > 0) {
                        int rnd = Random.Range(0, soilTiles.Count);
                        soilTiles[rnd].SwitchType(TileType.WateredPeanutSeed);
                        await UniTask.WaitForSeconds(animtime);

                        //Сложная система смерти всей цепочки при смерти последнего растения
                        surroundingtiles = _tilemap.GetHexNeighbors(soilTiles[rnd]._position);

                        for (int i = 0; i < surroundingtiles.Length; i++) {
                            TileData tile = TilesTable.TileByType(surroundingtiles[i].type);
                            if (!tile.IsBuilding && tile.crop != Crop.Peanut && tile.crop != Crop.None) {
                                soilTiles[rnd].SwitchType(TileType.PeanutDead);

                                Vector2Int curPose = soilTiles[rnd]._position;
                                soilTiles = _tilemap.GetNeighborsWithType(curPose, TileType.Peanut1);

                                int whileStopper = 0;
                                while (soilTiles.Count > 0 && whileStopper < 1000) {
                                    soilTiles[0].SwitchType(TileType.PeanutDead);
                                    await UniTask.WaitForSeconds(animtime);
                                    curPose = soilTiles[0]._position;
                                    soilTiles = _tilemap.GetNeighborsWithType(curPose, TileType.Peanut1);
                                    whileStopper++;
                                }
                            }
                        }
                    }

                    break;

                case TileType.Radish1:
                    InventoryManager.Instance.AddCoins(-1);
                    break;
            }

        await UniTask.WaitForSeconds(animtime);
        BecomeActive();
    }

    public async UniTask OnAppear(float animtime) {
        switch (type) {
            case TileType.Dandellion1:
                SmartTile[] neighbors = _tilemap.GetHexNeighbors(_position);
                List<SmartTile> neighborsL = neighbors.Where(t => t.CanBeHoed()).ToList();

                for (int i = 0; i < 2; i++)
                    if (neighborsL.Count > 0) {
                        SmartTile tile = neighborsL[Random.Range(0, neighborsL.Count)];
                        neighborsL.Remove(tile);
                        await tile.OnHoed(animtime, isDandellion: true);
                    }

                break;
        }
    }

    public async UniTask OnErosioned(float animtime) {
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
                QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + SpecialTargetTypes.EroseNWeeds, 1);
                break;

            case TileType.CactusSeed:
                SwitchType(TileType.Cactus1);
                break;
        }

        await UniTask.WaitForSeconds(animtime);
        BecomeActive();
    }

    public async UniTask OnInsected(float animtime) {
        if (type == TileType.FlycatherSeed3) {
            BecomeInactive();
            SwitchType(TileType.Flycather1);
            return;
        }

        if (CanBeCollected()) {
            BecomeInactive();
            SwitchType(TileType.Soil);
        }

        await UniTask.WaitForSeconds(animtime);
        BecomeActive();
    }

    public async UniTask OnClicked(float animtime) {
        BecomeInactive();

        TileData data = TilesTable.TileByType(type);
        if (data.TIndex == 1)
            await _tilemap.GetTile(_position + new Vector2Int(1, -1)).OnClicked(animtime);
        else if (data.TIndex == 2)
            await _tilemap.GetTile(_position + new Vector2Int(0, -1)).OnClicked(animtime);
        else if (data.TIndex == 3)
            await _tilemap.GetTile(_position + new Vector2Int(-1, 0)).OnClicked(animtime);
        else
            switch (type) {
                case TileType.BiogenEmpty:

                    List<SmartTile> weedArea = new() {
                        _tilemap.GetTile(_position + new Vector2Int(2, 0)),
                        _tilemap.GetTile(_position + new Vector2Int(3, 0)),
                        _tilemap.GetTile(_position + new Vector2Int(4, 0)),
                        _tilemap.GetTile(_position + new Vector2Int(1, 1)),
                        _tilemap.GetTile(_position + new Vector2Int(2, 1)),
                        _tilemap.GetTile(_position + new Vector2Int(3, 1))
                    };
                    while (weedArea.Count > 0) {
                        SmartTile tile = weedArea[Random.Range(0, weedArea.Count)];
                        if (tile.type == TileType.Weed) {
                            SwitchType(TileType.BiogenFull);
                            tile.SwitchType(TileType.Soil);

                            await UniTask.WaitForSeconds(1.5f);
                            Energy.Instance.RestoreEnergy(1);
                            SwitchType(TileType.BiogenEmpty);
                            break;
                        }

                        weedArea.Remove(tile);
                    }

                    break;
                case TileType.QuestBoard1_new:
                case TileType.QuestBoard1_11:
                case TileType.QuestBoard1_10:
                case TileType.QuestBoard1_01:
                case TileType.QuestBoard1_00:
                case TileType.QuestBoard2:
                case TileType.QuestBoard3:
                case TileType.QuestBoard4:
                    QuestsManager.Instance.OpenQuestsDialog();
                    break;
            }

        await UniTask.WaitForSeconds(0);
        BecomeActive();
    }

    public void SwitchType(TileType newType, AnimationType animationType) {
        SwitchType(newType);
        if (animationType != AnimationType.None) {
            _tilemap.toolsAnimTilemap.StartAnimationInCoord(_position, animationType);
            /*if (animationType == AnimationType.Scythe) {
                _tilemap.scytheAnimTilemap.StartAnimationInCoord(_position, animationType);
            } else {
                _tilemap.toolsAnimTilemap.StartAnimationInCoord(_position, animationType);
            }*/
        }
    }

    public void SwitchType(TileType newType) {
        type = newType;
        _tilemap.PlaceTile(_position, type);
    }

    public void HarvestCrop(Crop croptype, int amount) {
        QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + SpecialTargetTypes.CollectNFromOneTile, amount, true);

        InventoryManager.Instance.AddCollectedCrop(croptype, amount);
        InventoryManager.Instance.AddXp(ConfigsManager.Instance.CostsConfig.XpForBaseAction);

        //DropXp(ConfigsManager.Instance.CostsConfig.XpForBaseAction);

        //DropCrop(croptype, amount);
    }

    private void DropCrop(Crop croptype, int amount) {
        Vector3 worldPosition = _tilemap.MainTilemap.CellToWorld((Vector3Int)_position);
        for (int i = 0; i < amount; i++) {
            var obj = Object.Instantiate(CropsTable.Instance.FlyingCropFxPrefab);
            obj.Init(CropsTable.CropByType(croptype).VegSprite, worldPosition);
        }
    }

    private void DropCoin(int amount) {
        Vector3 worldPosition = _tilemap.MainTilemap.CellToWorld((Vector3Int)_position);
        for (int i = 0; i < amount; i++) {
            var obj = Object.Instantiate(CropsTable.Instance.FlyingCoinFxPrefab);
            obj.Init(worldPosition);
        }
    }

    private void DropXp(int amount) {
        Vector3 worldPosition = _tilemap.MainTilemap.CellToWorld((Vector3Int)_position);
        for (int i = 0; i < amount; i++) {
            var obj = Object.Instantiate(CropsTable.Instance.FlyingXpFxPrefab);
            obj.Init(worldPosition);
        }
    }

    private void BurstFx(ParticleSystem prefab) {
        Vector3 worldPosition = _tilemap.MainTilemap.CellToWorld((Vector3Int)_position);
        var ps = Object.Instantiate(prefab, worldPosition, Quaternion.identity);
    }
}