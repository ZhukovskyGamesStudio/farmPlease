using System;
using Localization;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ToolOffer : MonoBehaviour {
        public TextMeshProUGUI costText;
        public TextMeshProUGUI explainText;
        public Image OfferImage;

        private ToolBuff _toolBuff;
        private Action<ToolBuff> _onButtonClick;

        public void OnClick() {
            _onButtonClick?.Invoke(_toolBuff);
        }

        public void SetData(ToolConfig tool, bool isActive, Action<ToolBuff> onButtonClick) {
            _onButtonClick = onButtonClick;
            gameObject.SetActive(isActive);
            _toolBuff = tool.buff;
            costText.text = tool.cost.ToString();
            explainText.text = LocalizationUtils.L(tool.explainTextLoc);

            OfferImage.sprite = tool.gridIcon;
        }
    }
}