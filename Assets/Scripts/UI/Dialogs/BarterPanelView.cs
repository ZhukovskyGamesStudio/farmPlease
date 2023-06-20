using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using Time = Managers.Time;

namespace UI {
    public class BarterPanelView : MonoBehaviour {
        [SerializeField] private CropRainManager _cropRainManager;
        [SerializeField] private Text _sellForText;
        private void OnEnable() {
            _cropRainManager.StartRainingCrops(SaveLoadManager.CurrentSave.CropsCollectedQueue);
            _sellForText.text = "Продать за " + SaveLoadManager.CurrentSave.CropsCollected.Count;
        }

        public void SellAllCrops() {
            _cropRainManager.EndCropRain();
            int cropsAmount = SaveLoadManager.CurrentSave.CropsCollected.Count;
            SaveLoadManager.CurrentSave.CropPoints += cropsAmount;
            int coinsGain = cropsAmount * (Time.Instance.IsTodayLoveDay ? 2 : 1);
            SaveLoadManager.CurrentSave.Coins += coinsGain;
            SaveLoadManager.CurrentSave.CropsCollected = new List<Crop>();
            SaveLoadManager.Instance.SaveGame();
        }
    }
}