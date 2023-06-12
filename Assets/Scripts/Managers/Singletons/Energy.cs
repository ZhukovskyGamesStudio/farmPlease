using System.Collections;
using Managers;
using UI;
using ZhukovskyGamesPlugin;

public class Energy : Singleton<Energy> { 
        
    private const int MAX_ENERGY = 7;
    public int CurEnergy => SaveLoadManager.CurrentSave.Energy;

    public void LoseOneEnergy() {
        SaveLoadManager.CurrentSave.Energy--;
        if (CurEnergy < 0)
            SaveLoadManager.CurrentSave.Energy = 0;

        if (GameModeManager.Instance.InfiniteEnergy)
            SaveLoadManager.CurrentSave.Energy = MAX_ENERGY;

        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
        SaveLoadManager.Instance.SaveGame();
    }

    public void RefillEnergy() {
        SetEnergy(MAX_ENERGY);
    }

    public void SetEnergy(int newEnergy) {
        SaveLoadManager.CurrentSave.Energy = newEnergy;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }

    public void RestoreEnergy() {
        SaveLoadManager.CurrentSave.Energy = MAX_ENERGY;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }

    public void RestoreEnergy(int amount) {
        SaveLoadManager.CurrentSave.Energy += amount;
        if (CurEnergy > MAX_ENERGY)
            SaveLoadManager.CurrentSave.Energy = MAX_ENERGY;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }
        
    public bool HasEnergy() {
        if (CurEnergy == 0) {
            UIHud.Instance.NoEnergy();
            Audio.Instance.PlaySound(Sounds.ZeroEnergy);
        }

        return CurEnergy > 0;
    }

      
}