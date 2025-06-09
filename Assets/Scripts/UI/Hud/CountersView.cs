using Managers;
using UnityEngine;

namespace UI {
    public class CountersView : MonoBehaviour {
        [SerializeField]
        private AnimatableCounter _cropsCounter, _coinsCounter;

        public AnimatableCounter CropsCounter => _cropsCounter;
        public AnimatableCounter CoinsCounter => _coinsCounter;

        public void SetData(GameSaveProfile saveProfile) {
            _coinsCounter.SetAmount(saveProfile.Coins);
            _cropsCounter.SetAmount(saveProfile.CropPoints);
        }
    }
}