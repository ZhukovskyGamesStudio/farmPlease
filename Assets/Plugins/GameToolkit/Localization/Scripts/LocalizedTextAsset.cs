// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace GameToolkit.Localization {
    /// <summary>
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizedTextAsset", menuName = "GameToolkit/Localization/Text Asset")]
    public class LocalizedTextAsset : LocalizedAsset<TextAsset> {
        [SerializeField]
        private TextAssetLocaleItem[] m_LocaleItems = new TextAssetLocaleItem[1];

        public override LocaleItemBase[] LocaleItems => m_LocaleItems;

        [Serializable]
        private class TextAssetLocaleItem : LocaleItem<TextAsset> {
        }
    }
}