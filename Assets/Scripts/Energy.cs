using System.Collections;
using DefaultNamespace.Abstract;

namespace DefaultNamespace {
    public class Energy : Singleton<Energy> { 
        
        private const int MAX_ENERGY = 7;
        public int curEnergy;

        public void LoseOneEnergy() {
            curEnergy--;
            if (curEnergy < 0)
                curEnergy = 0;

            if (GameModeManager.Instance.InfiniteEnergy)
                curEnergy = MAX_ENERGY;

            UIHud.Instance.SetBattery(curEnergy);
        }

        public void RefillEnergy() {
            SetEnergy(MAX_ENERGY);
        }

        public void SetEnergy(int newEnergy) {
            curEnergy = newEnergy;
            UIHud.Instance.SetBattery(curEnergy);
        }

        public void RestoreEnergy() {
            curEnergy = MAX_ENERGY;
            UIHud.Instance.SetBattery(curEnergy);
        }

        public void RestoreEnergy(int amount) {
            curEnergy += amount;
            if (curEnergy > MAX_ENERGY)
                curEnergy = MAX_ENERGY;
            UIHud.Instance.SetBattery(curEnergy);
        }
        
        public bool HasEnergy() {
            if (curEnergy == 0) {
                UIHud.Instance.NoEnergy();
                Audio.Instance.PlaySound(Sounds.ZeroEnergy);
            }

            return curEnergy > 0;
        }

      
    }
}