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

    public void AddCollectedCrops(int amount) {
        InventoryManager.Instance.AddCollectedCrop(Crop.Tomato, amount);
    }
    
    public void AddBattery(int amount) {
        InventoryManager.Instance.AddTool(ToolBuff.WeekBattery, amount);
    }

    public void SetGameSpeed(Single value) {
        GameModeManager.Instance.Config.GameSpeed = value;
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
}