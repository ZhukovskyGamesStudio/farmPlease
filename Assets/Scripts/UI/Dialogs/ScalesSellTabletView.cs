using System;
using System.Collections.Generic;
using System.Linq;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI {
    public class ScalesSellTabletView : CustomMonoBehaviour {
        [SerializeField]
        private SellTableLine _linePrefab, _emptyLinePrefab;

        [SerializeField]
        private Transform _linesHolder;

        [SerializeField]
        private Animation _openCloseAnimation;

        [SerializeField]
        private AnimationClip _openAnimationClip, _closeAnimationClip;

        [SerializeField]
        private CanvasGroup _scrollCanvasGroup;

        public CanvasGroup ScrollCanvasGroup => _scrollCanvasGroup;

        [SerializeField]
        private ScalesDialog _scalesPanel;

        private Dictionary<Crop, int> _cropsAmount;
        private Dictionary<Crop, SellTableLine> _cropLineMap = new Dictionary<Crop, SellTableLine>();
        private List<SellTableLine> _lines = new List<SellTableLine>();

        [SerializeField]
        private Vector3 _openedPos, _closedPos;

        [SerializeField]
        private Button _sellAllButton, _selectAllButton;

        [SerializeField]
        private TextMeshProUGUI _sellAmountText, _sellForAmountText;

        private bool _isOpened = true;

        public Button SelectAllButton => _selectAllButton;

        public Button SellButton => _sellAllButton;

        public bool IsFixedByTraining = false;
        private Action<Crop, int> _onSelectedAmountChange;

        private void Awake() {
            RecalculateSellButton();
            Open();
        }

        private bool IsDraggableActive() =>
            false && !_scalesPanel.IsSellingAnimation && !IsFixedByTraining;

        public void SetData(Queue<Crop> cropsCollected, Action<Crop, int> onSelectedAmountChange) {
            _onSelectedAmountChange = onSelectedAmountChange;
            Dictionary<Crop, int> crops = new Dictionary<Crop, int>();
            foreach (var crop in cropsCollected) {
                if (crops.ContainsKey(crop)) {
                    crops[crop]++;
                } else {
                    crops.Add(crop, 1);
                }
            }

            SetData(crops);
            Close();
            RecalculateSellButton();
        }

        public void SetData(Dictionary<Crop, int> crops) {
            _cropsAmount = crops;
            InitLines(_cropsAmount);
            InitButtons(_cropsAmount);
        }

        private void InitLines(Dictionary<Crop, int> crops) {
            DestroyChildren(_linesHolder);

            _cropLineMap = new Dictionary<Crop, SellTableLine>();
            if (crops.Count == 0) {
                SellTableLine line = Instantiate(_emptyLinePrefab, _linesHolder);
                _cropLineMap.Add(Crop.None, line);
            } else {
                foreach (var cropIntPair in crops) {
                    SellTableLine line = Instantiate(_linePrefab, _linesHolder);
                    line.SetData(cropIntPair.Key, cropIntPair.Value, OnSelectedAmountChange);
                    _cropLineMap.Add(cropIntPair.Key, line);
                }
            }
        }

        private void OnSelectedAmountChange(Crop crop, int diff) {
            _onSelectedAmountChange?.Invoke(crop, diff);
            RecalculateSellButton();
        }

        private void RecalculateSellButton() {
            int selectedAmount = CountSelectedCrops();
            int reward = selectedAmount * (TimeManager.Instance.IsTodayLoveDay ? 2 : 1);
            _sellAmountText.text = selectedAmount.ToString();
            _sellForAmountText.text = reward.ToString();
            _sellAllButton.interactable = selectedAmount > 0;
        }

        private void InitButtons(Dictionary<Crop, int> crops) {
            _sellAllButton.interactable = crops.Count > 0;
            _selectAllButton.interactable = crops.Count > 0;
        }

        public void Open() {
            if (_isOpened) {
                return;
            }

            _isOpened = true;
            _openCloseAnimation.Play(_openAnimationClip.name);
        }

        public void Close() {
            if (!_isOpened) {
                return;
            }

            _isOpened = false;
            _openCloseAnimation.Play(_closeAnimationClip.name);
        }

        public void SellSelectedCrops() {
            List<Crop> cropsToSell = new List<Crop>();
            foreach (var cropLine in _cropLineMap) {
                for (int i = 0; i < cropLine.Value.SelectedAmount; i++) {
                    cropsToSell.Add(cropLine.Key);
                }
            }

            _scalesPanel.SellSelected(cropsToSell);
        }

        private int CountSelectedCrops() {
            return _cropLineMap.Values.Sum(s => s.SelectedAmount);
        }

        public void SelectAll() {
            foreach (SellTableLine line in _cropLineMap.Values) {
                line.SelectAll();
            }
        }
    }
}