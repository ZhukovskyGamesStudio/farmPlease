using Localization;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FactsPage : MonoBehaviour {
        [Header("FactsPage")]
        public TextMeshProUGUI FactsHeader;

        public TextMeshProUGUI FactsFirstText;
        public TextMeshProUGUI FactsSecondText;
        public Image FactsFirstImage;
        public Image FactsSecondImage;

        public void UpdatePage(ConfigWithCroponomPage pageData) {
            FactsHeader.text = pageData.header;
            FactsFirstText.text = LocalizationUtils.L(pageData.FirstTextLoc);
            FactsSecondText.text = LocalizationUtils.L(pageData.SecondTextLoc);

            if (pageData.firstSprite != null) {
                FactsFirstImage.enabled = true;
                FactsFirstImage.sprite = pageData.firstSprite;
            } else {
                FactsFirstImage.enabled = false;
            }

            if (pageData.secondSprite != null) {
                FactsSecondImage.enabled = true;
                FactsSecondImage.sprite = pageData.secondSprite;
            } else {
                FactsSecondImage.enabled = false;
            }
        }
    }
}