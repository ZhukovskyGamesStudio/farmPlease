// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor.IMGUI.Controls;

namespace GameToolkit.Localization.Editor {
    public class LocaleTreeViewItem : TreeViewItem {
        public LocaleTreeViewItem(int id, int depth, LocaleItemBase localeItem, AssetTreeViewItem parent) : base(id,
            depth, "") {
            LocaleItem = localeItem;
            Parent = parent;
        }

        public LocaleItemBase LocaleItem { get; }
        public AssetTreeViewItem Parent { get; }
    }
}