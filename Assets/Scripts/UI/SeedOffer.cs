using System;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class SeedOffer : MonoBehaviour{
        [SerializeField]
        private TextMeshProUGUI _costText;

        [SerializeField]
        private TextMeshProUGUI _explainText;

        public GameObject RareEdge;
        public GameObject LegendaryEdge;
        public CanvasGroup CanvasGroup;
        public Image OfferImage;
        private Action _buyCallback;
        private Action _startDragCallback;

        [SerializeField]
        private GameObject _infoButton;

        [SerializeField]
        private GameObject _infoHint;

        public Crop CurrentCrop{ get; private set; }

        public void SetData(CropConfig cropConfig, Action buyCallback, Action startDragCallback){
            CurrentCrop = cropConfig.type;
            _costText.text = cropConfig.cost.ToString();
            _explainText.text = cropConfig.explainText;
            OfferImage.sprite = cropConfig.SeedSprite;
            _buyCallback = buyCallback;
            _startDragCallback = startDragCallback;
            SetRarity(cropConfig.Rarity);
        }

        public void StartDrag(){
            _startDragCallback?.Invoke();
            _infoButton.gameObject.SetActive(false);
        }

        public void EndDrag(){
            _infoButton.gameObject.SetActive(true);
        }

        public void TryBuy(){
            _buyCallback?.Invoke();
        }

        public void ShowHint(){
            _infoButton.gameObject.SetActive(false);
            _infoHint.gameObject.SetActive(true);
        }

        public void CloseHint(){
            _infoButton.gameObject.SetActive(true);
            _infoHint.gameObject.SetActive(false);
        }

        private void SetRarity(int rarity){
            switch (rarity){
                case 1:
                    RareEdge.SetActive(true);
                    break;
                case 2:
                    LegendaryEdge.SetActive(true);
                    break;
            }
        }
    }
}