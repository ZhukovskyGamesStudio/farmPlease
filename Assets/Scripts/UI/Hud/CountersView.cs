using Managers;
using UnityEngine;

namespace UI {
    public class CountersView : MonoBehaviour {
        [SerializeField] public CoinsCounter coinsCounter, cropsCounter;

        public void UpdateCounters() {
            coinsCounter.UpdateCoins(SaveLoadManager.CurrentSave.Coins);
            cropsCounter.UpdateCoins(SaveLoadManager.CurrentSave.CropPoints);
        }
    }
}