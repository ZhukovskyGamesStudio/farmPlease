using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "FtueConfig", menuName = "ScriptableObjects/FtueConfig", order = 6)]
    public class FtueConfig : ScriptableObject {
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
        public SpotlightAnimConfig DoWaterAgainHint;
        
        public SpotlightAnimConfig ScytheHint;
        public SpotlightAnimConfig DoScytheHint;

        public SpotlightAnimConfig ScalesHint;
        public SpotlightAnimConfig DoSellHint;
        public SpotlightAnimConfig DoCloseScalesHint;

        public SpotlightAnimConfig SeedShopHint;
        public SpotlightAnimConfig DoBuyHint;
        public SpotlightAnimConfig DoCloseSeedsShopHint;

        public SpotlightAnimConfig BookHint;

        [Multiline] public string EndHint;
    }
}