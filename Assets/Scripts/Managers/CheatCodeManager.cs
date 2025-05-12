using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using Tables;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class CheatCodeManager {
        private CheatCodeConfigList _cheatCodesConfig;
        private List<CheatCodeConfig> CheatCodes => _cheatCodesConfig.CheatCodes;

        private SerializableDictionary<string, int> CheatCodesActivated =>
            SaveLoadManager.CurrentSave.CheatCodesActivated;

        public CheatCodeManager(CheatCodeConfigList cheatCodesConfig) {
            _cheatCodesConfig = cheatCodesConfig;
        }

        public bool CheatCodeAvailable(string code, out string errorMessage) {
            CheatCodeConfig cheatCode = CheatCodes.FirstOrDefault(c => c.Code == code);
            if (cheatCode == null) {
                errorMessage = "Такого кода не существует :P";
                return false;
            }

            if (CheatCodesActivated.ContainsKey(code)) {
                if (cheatCode.NumberOfUses <= CheatCodesActivated[code]) {
                    errorMessage = "Все активации кода использованы :3";
                    return false;
                }
            }

            errorMessage = "";
            return true;
        }

        public void ExecuteCheatCode(string code) {
         
                CheatCodeConfig cheatCodeConfig = CheatCodes.First(c => c.Code == code);
                TryAddEnergy(cheatCodeConfig);
                TryAddCoins(cheatCodeConfig);
                TryAddCrops(cheatCodeConfig);
                TryAddSeeds(cheatCodeConfig);
                TryAddTools(cheatCodeConfig);
                SaveCodeUsed(code);
                SaveLoadManager.SaveGame();
                
        }

        private void TryAddEnergy(CheatCodeConfig cheatCode) {
            if (cheatCode.RechargingClockEnergyAmount > 0) {
                if (Clock.Instance != null) {
                    Clock.Instance.AddEnergy(cheatCode.RechargingClockEnergyAmount);
                } else {
                    SaveLoadManager.CurrentSave.ClockEnergy += cheatCode.RechargingClockEnergyAmount;
                    if (SaveLoadManager.CurrentSave.ClockEnergy >= Clock.MAX_ENERGY) {
                        SaveLoadManager.CurrentSave.ClockEnergy = Clock.MAX_ENERGY;
                    }
                }
            }
        }

        private void TryAddCoins(CheatCodeConfig cheatCode) {
            if (cheatCode.CoinsAdded > 0) {
                if (InventoryManager.Instance != null) {
                    InventoryManager.Instance.AddCoins(cheatCode.CoinsAdded);
                } else {
                    SaveLoadManager.CurrentSave.Coins += cheatCode.CoinsAdded;
                }
            }
        }

        private void TryAddCrops(CheatCodeConfig cheatCode) {
            if (cheatCode.CropsCollectedAdded > 0) {
                SaveLoadManager.CurrentSave.CropPoints += cheatCode.CropsCollectedAdded;
            }
        }

        private void TryAddSeeds(CheatCodeConfig cheatCode) {
            if (cheatCode.SeedsAdded is {Count: > 0}) {
                foreach (Crop cropType in cheatCode.SeedsAdded.Keys) {
                    SaveLoadManager.CurrentSave.Seeds[cropType] += cheatCode.SeedsAdded[cropType];
                }
            }
        } 
        private void TryAddTools(CheatCodeConfig cheatCode) {
            if (cheatCode.ToolsAdded is {Count: > 0}) {
                foreach (ToolBuff toolType in cheatCode.ToolsAdded.Keys) {
                    InventoryManager.Instance.BuyTool(toolType,0,cheatCode.ToolsAdded[toolType]);
                }
            }
        }

        private void SaveCodeUsed(string code) {
            if (!CheatCodesActivated.ContainsKey(code)) {
                CheatCodesActivated.Add(code, 1);
            } else {
                CheatCodesActivated[code]++;
            }
        }
    }
}