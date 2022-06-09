// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace GameToolkit.Localization {
    [AddComponentMenu(ComponentMenuRoot + "Localized Renderer Texture")]
    [RequireComponent(typeof(Renderer))]
    public class LocalizedRendererBehaviour : LocalizedAssetBehaviour {
        public int MaterialIndex;
        public string PropertyName = "_MainTex";
        public LocalizedTexture LocalizedTexture;

        private void OnValidate() {
            MaterialIndex = Mathf.Max(0, MaterialIndex);
        }

        protected override void UpdateComponentValue() {
            if (LocalizedTexture) {
                Material[] materials = GetMaterials();
                if (MaterialIndex < materials.Length)
                    materials[MaterialIndex].SetTexture(PropertyName, GetValueOrDefault(LocalizedTexture));
                else
                    Debug.LogWarning(name + ": Material index out of bounds!");
            }
        }

        private Material[] GetMaterials() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return GetComponent<Renderer>().sharedMaterials;
#endif
            return GetComponent<Renderer>().materials;
        }
    }
}