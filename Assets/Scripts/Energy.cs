using System.Collections;
using DefaultNamespace.Abstract;

namespace DefaultNamespace {
    public class Energy : Singleton<Energy> { 
        
        private const int MAX_ENERGY = 7;
        public int CurEnergy => SaveLoadManager.CurrentSave.energy;

        public void LoseOneEnergy() {
            SaveLoadManager.CurrentSave.energy--;
            if (CurEnergy < 0)
                SaveLoadManager.CurrentSave.energy = 0;

            if (GameModeManager.Instance.InfiniteEnergy)
                SaveLoadManager.CurrentSave.energy = MAX_ENERGY;

            UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.energy);
            SaveLoadManager.Instance.SaveGame();
        }

        public void RefillEnergy() {
            SetEnergy(MAX_ENERGY);
        }

        public void SetEnergy(int newEnergy) {
            SaveLoadManager.CurrentSave.energy = newEnergy;
            UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.energy);
        }

        public void RestoreEnergy() {
            SaveLoadManager.CurrentSave.energy = MAX_ENERGY;
            UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.energy);
        }

        public void RestoreEnergy(int amount) {
            SaveLoadManager.CurrentSave.energy += amount;
            if (CurEnergy > MAX_ENERGY)
                SaveLoadManager.CurrentSave.energy = MAX_ENERGY;
            UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.energy);
        }
        
        public bool HasEnergy() {
            if (CurEnergy == 0) {
                UIHud.Instance.NoEnergy();
                Audio.Instance.PlaySound(Sounds.ZeroEnergy);
            }

            return CurEnergy > 0;
        }

      
    }
}