using Managers;
using UI;
using ZhukovskyGamesPlugin;

public class Energy : Singleton<Energy> {
    public const int MAX_ENERGY = 7;
    public const int MAX_GOLDEN_ENERGY = 14;
    public int CurEnergy => SaveLoadManager.CurrentSave.Energy;

    private static int MaxEnergy => SaveLoadManager.CurrentSave.RealShopData.HasGoldenBattery ? MAX_GOLDEN_ENERGY : MAX_ENERGY;

    public static void GenerateEnergy() {
        SaveLoadManager.CurrentSave.Energy = MaxEnergy;
    }

    public void LoseOneEnergy() {
        SaveLoadManager.CurrentSave.Energy--;
        if (CurEnergy < 0) {
            SaveLoadManager.CurrentSave.Energy = 0;
        }

        if (RealShopUtils.IsGoldenClockActive(SaveLoadManager.CurrentSave.RealShopData)) {
            SaveLoadManager.CurrentSave.Energy = MaxEnergy;
        }

        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);

        if (CurEnergy == 0 && SaveLoadManager.CurrentSave.ClockEnergy == 0) {
            KnowledgeHintsFactory.Instance.TryShowNoEnergyHint();
        }

        SaveLoadManager.SaveGame();
    }

    public void RefillEnergy() {
        SetEnergy(MaxEnergy);
    }

    public void SetEnergy(int newEnergy) {
        SaveLoadManager.CurrentSave.Energy = newEnergy;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }

    public void RestoreEnergy() {
        SaveLoadManager.CurrentSave.Energy = MaxEnergy;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }

    public void RestoreEnergy(int amount) {
        SaveLoadManager.CurrentSave.Energy += amount;
        if (CurEnergy > MaxEnergy)
            SaveLoadManager.CurrentSave.Energy = MaxEnergy;
        UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
    }

    public bool HasEnergy(bool isShowNoEnergyAnimation = true) {
        if (CurEnergy == 0 && isShowNoEnergyAnimation) {
            UIHud.Instance.NoEnergy();
            Audio.Instance.PlaySound(Sounds.ZeroEnergy);
        }

        return CurEnergy > 0;
    }
}