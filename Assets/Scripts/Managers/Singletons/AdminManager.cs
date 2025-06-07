using System;
using Managers;
using Tables;
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

    public void SetGameSpeed(Single value) {
        GameModeManager.Instance.Config.GameSpeed = value;
    }
}