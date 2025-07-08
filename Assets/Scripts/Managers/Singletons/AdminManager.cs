using System;
using System.Collections.Generic;
using Managers;
using Tables;
using UI;
using UnityEngine;

public class AdminManager : MonoBehaviour {
    [SerializeField]
    private GameObject _adminPanelToggle, _adminPanel;

    public static AdminManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void Init(bool isAdmin) {
        _adminPanelToggle.gameObject.SetActive(isAdmin);
    }

    public void ToggleAdminPanel(bool isOn) {
        _adminPanel.SetActive(isOn);
    }

    public void AddCoins(int amount) {
        InventoryManager.Instance.AddCoins(amount);
    }

    public void AddXp(int amount) {
        InventoryManager.Instance.AddXp(amount);
    }
    public void AddXpToNextLevel() {
        int amount =    SaveLoadManager.CurrentSave.Xp;
        int amountToNextLevel = XpUtils.GetNextLevelByXp(SaveLoadManager.CurrentSave.Xp) - amount;
        if(amountToNextLevel > 10000) {
            amountToNextLevel = 10000; // Limit to prevent excessive XP addition
        }
     
        SaveLoadManager.CurrentSave.CurrentLevel++;
        UIHud.Instance.ProfileView.SetData(SaveLoadManager.CurrentSave);
        InventoryManager.Instance.AddXp(amountToNextLevel);
        SaveLoadManager.SaveGame();
    }

    public void AddCollectedCrops(int amount) {
        InventoryManager.Instance.AddCollectedCrop(Crop.Tomato, amount);
    }
    
    public void AddBattery(int amount) {
        InventoryManager.Instance.AddTool(ToolBuff.WeekBattery, amount);
    }

    public void SetGameSpeed(Single value) {
        GameModeManager.Instance.Config.GameSpeed = value;
    }

    public void NextDay() {
        if (!PlayerController.CanInteract) {
            return;
        }
        TimeManager.Instance.AddDay();
    }

    public void UnlockEverything() {
        var knowledges = Enum.GetValues(typeof(Knowledge));
        foreach (Knowledge knowledge in knowledges) {
            KnowledgeUtils.AddKnowledge(knowledge);
        }
        List<string>  unlockables = new List<string>(Enum.GetNames(typeof(Unlockable)));
        unlockables.AddRange(Enum.GetNames(typeof(Crop)));
        unlockables.AddRange(Enum.GetNames(typeof(ToolBuff)));
        unlockables.AddRange(Enum.GetNames(typeof(HappeningType)));
        foreach (string unlockable in unlockables) {
            UnlockableUtils.Unlock(unlockable);
        }
        
        
        UIHud.Instance.UpdateLockedUI();
    }

    public void AddSeeds(int amount) {
        var crops = Enum.GetValues(typeof(Crop));
        foreach (Crop crop in crops) {
            if (crop is Crop.None or Crop.Weed) {
                continue;
            }
            InventoryManager.Instance.AddSeed(crop, amount);
        }
    }

    public void AddMainQuestLine() {
        QuestsManager.Instance.ProgressMainQuestline();
    }

    public void ResetMainQuestline() {
        SaveLoadManager.CurrentSave.QuestsData.MainQuestProgressIndex = -1;
        QuestsManager.Instance.ProgressMainQuestline();
    }
}