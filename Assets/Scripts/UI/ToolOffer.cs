using System;
using Managers;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ToolOffer : MonoBehaviour {
        public TextMeshProUGUI costText;
        public TextMeshProUGUI explainText;
        public Button BuyButton;
        public Image OfferImage;

        public ToolBuff ToolBuff;

        public void Init(Action<ToolBuff> onButtonClick) {
            BuyButton.onClick.AddListener(() => {
                onButtonClick?.Invoke(ToolBuff);
            });
        }
        
        

        public void SetData(ToolConfig tool, bool isActive) {
            gameObject.SetActive(isActive);
            ToolBuff = tool.buff;
            costText.text = tool.cost.ToString();
            explainText.text = tool.explainText;

            OfferImage.sprite = tool.gridIcon;
        }
    }
}