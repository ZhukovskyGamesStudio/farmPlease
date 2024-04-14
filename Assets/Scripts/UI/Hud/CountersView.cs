using Managers;
using UnityEngine;

namespace UI {
    public class CountersView : MonoBehaviour {
        [SerializeField]
        private AnimatableCounter _cropsCounter, _coinsCounter;

        public AnimatableCounter CropsCounter => _cropsCounter;
        public AnimatableCounter CoinsCounter => _coinsCounter;

        public void SetCounters() {
            _coinsCounter.SetAmount(SaveLoadManager.CurrentSave.Coins);
            _cropsCounter.SetAmount(SaveLoadManager.CurrentSave.CropPoints);
        }
    }
}