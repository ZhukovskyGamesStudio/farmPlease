﻿using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Tilemaps {
    /// <summary>
    ///     Rule Override Tiles are Tiles which can override a subset of Rules for a given Rule Tile to provide specialised
    ///     behaviour while keeping most of the Rules originally set in the Rule Tile.
    /// </summary>
    [MovedFrom(true, "UnityEngine")]
    [Serializable]
    [HelpURL(
        "https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/RuleOverrideTile.html")]
    public class AdvancedRuleOverrideTile : RuleOverrideTile {
        /// <summary>
        ///     The Default Sprite set when creating a new Rule override.
        /// </summary>
        public Sprite m_DefaultSprite;

        /// <summary>
        ///     The Default GameObject set when creating a new Rule override.
        /// </summary>
        public GameObject m_DefaultGameObject;

        /// <summary>
        ///     The Default Collider Type set when creating a new Rule override.
        /// </summary>
        public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;

        /// <summary>
        ///     A list of TilingRule Overrides
        /// </summary>
        public List<RuleTile.TilingRuleOutput> m_OverrideTilingRules = new();

        /// <summary>
        ///     Gets the overriding TilingRuleOutput of a given TilingRule.
        /// </summary>
        /// <param name="originalRule">The original TilingRule that is overridden</param>
        public RuleTile.TilingRuleOutput this[RuleTile.TilingRule originalRule] {
            get {
                foreach (RuleTile.TilingRuleOutput overrideRule in m_OverrideTilingRules)
                    if (overrideRule.m_Id == originalRule.m_Id)
                        return overrideRule;

                return null;
            }
            set {
                for (int i = m_OverrideTilingRules.Count - 1; i >= 0; i--)
                    if (m_OverrideTilingRules[i].m_Id == originalRule.m_Id) {
                        m_OverrideTilingRules.RemoveAt(i);
                        break;
                    }

                if (value != null) {
                    string json = JsonUtility.ToJson(value);
                    RuleTile.TilingRuleOutput overrideRule = JsonUtility.FromJson<RuleTile.TilingRuleOutput>(json);
                    m_OverrideTilingRules.Add(overrideRule);
                }
            }
        }

        /// <summary>
        ///     Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void ApplyOverrides(IList<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> overrides) {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            for (int i = 0; i < overrides.Count; i++)
                this[overrides[i].Key] = overrides[i].Value;
        }

        /// <summary>
        ///     Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <param name="validCount">Returns the number of valid overrides for Rules</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        public void GetOverrides(List<KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>> overrides,
            ref int validCount) {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            overrides.Clear();

            if (m_Tile)
                foreach (RuleTile.TilingRule originalRule in m_Tile.m_TilingRules) {
                    RuleTile.TilingRuleOutput overrideRule = this[originalRule];
                    overrides.Add(
                        new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>(originalRule, overrideRule));
                }

            validCount = overrides.Count;

            foreach (RuleTile.TilingRuleOutput overrideRule in m_OverrideTilingRules)
                if (!overrides.Exists(o => o.Key.m_Id == overrideRule.m_Id)) {
                    RuleTile.TilingRule originalRule = new RuleTile.TilingRule {m_Id = overrideRule.m_Id};
                    overrides.Add(
                        new KeyValuePair<RuleTile.TilingRule, RuleTile.TilingRuleOutput>(originalRule, overrideRule));
                }
        }

        /// <summary>
        ///     Updates the Rules with the Overrides set for this AdvancedRuleOverrideTile
        /// </summary>
        public override void Override() {
            if (!m_Tile || !m_InstanceTile)
                return;

            PrepareOverride();

            RuleTile tile = m_InstanceTile;

            tile.m_DefaultSprite = m_DefaultSprite;
            tile.m_DefaultGameObject = m_DefaultGameObject;
            tile.m_DefaultColliderType = m_DefaultColliderType;

            foreach (RuleTile.TilingRule rule in tile.m_TilingRules) {
                RuleTile.TilingRuleOutput overrideRule = this[rule];
                if (overrideRule != null) JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(overrideRule), rule);
            }
        }
    }
}