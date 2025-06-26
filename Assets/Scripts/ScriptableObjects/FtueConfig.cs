using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "FtueConfig", menuName = "Scriptable Objects/FtueConfig", order = 6)]
    public class FtueConfig : ScriptableObject {
        [LocalizationKey("Ftue")]
        public string StartHintLoc;
        [LocalizationKey("Ftue")]
        public string StartHint2Loc;
        [Multiline] public string StartHint;
        [Multiline] public string StartHint2;
        public SpotlightAnimConfig HoeHint;
        public SpotlightAnimConfig DoHoeHint;
        public SpotlightAnimConfig EnergyHint;

        public SpotlightAnimConfig BackpackHint;
        public SpotlightAnimConfig SeedSelectHint;
        public SpotlightAnimConfig DoSeedHint;

        public SpotlightAnimConfig WaterHint;
        public SpotlightAnimConfig DoWaterHint;

        public SpotlightAnimConfig ClockHint;
        public SpotlightAnimConfig ClockLostEnergyHint;
        public SpotlightAnimConfig DoWaterAgainHint;
        
        public SpotlightAnimConfig ScytheHint;
        public SpotlightAnimConfig DoScytheHint;

        public SpotlightAnimConfig ScalesHint;
        public SpotlightAnimConfig SelectAllHint;
        public SpotlightAnimConfig SellHint;
        public SpotlightAnimConfig CloseScalesHint;

        public SpotlightAnimConfig SeedShopHint;
        public SpotlightAnimConfig BuyTomatoHint;
        public SpotlightAnimConfig BuyEggplantHint;
        public SpotlightAnimConfig CloseSeedsShopHint;

        public SpotlightAnimConfig BookHint;
        public SpotlightAnimConfig ProfileHint;

        [Multiline] public string EndHint;
        [LocalizationKey("Ftue")]
        public string EndHintLoc;
    }
}