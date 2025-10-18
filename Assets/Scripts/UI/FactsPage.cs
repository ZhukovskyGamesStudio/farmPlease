using System.Collections.Generic;
using Managers;
using ZG_Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class FactsPage : MonoBehaviour {
        [Header("FactsPage")]
        public TextMeshProUGUI FactsHeader;

        [SerializeField]
        private List<GameObject> _iconsContainers;

        [SerializeField]
        private List<Image> _icons;

        [SerializeField]
        private TextMeshProUGUI _factsText;

        public ConfigWithCroponomPage Config { get; private set; }
        
        public void UpdatePage(ConfigWithCroponomPage pageData) {
            Config = pageData;
            FactsHeader.text = LocalizationUtils.L(pageData.HeaderLoc);
            _factsText.text = LocalizationUtils.L(pageData.FirstTextLoc) + "\n" + LocalizationUtils.L(pageData.SecondTextLoc);

            foreach (var iconContainer in _iconsContainers) {
                iconContainer.SetActive(false);
            }

            for (int index = 0; index < pageData.FactsSprites.Count; index++) {
                _iconsContainers[index].SetActive(true);
                _icons[index].sprite = pageData.FactsSprites[index];
                _icons[index].SetNativeSize();
            }

            SaveLoadManager.CurrentSave.LastCroponomPage = pageData.GetUnlockable();
            SaveLoadManager.SaveGame();
        }
    }
}