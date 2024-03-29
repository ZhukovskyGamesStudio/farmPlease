using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI{
    public class SellTabletView : CustomMonoBehaviour{
        [SerializeField]
        private SellTableLine _linePrefab, _emptyLinePrefab;

        [SerializeField]
        private Transform _linesHolder;

        [SerializeField]
        private Animation _openCloseAnimation;

        [SerializeField]
        private DraggableSellTablet _draggable;

        [SerializeField]
        private ScalesPanelView _scalesPanel;

        private Dictionary<Crop, int> _cropsAmount;
        private Dictionary<Crop, SellTableLine> _cropLineMap = new Dictionary<Crop, SellTableLine>();
        private List<SellTableLine> _lines = new List<SellTableLine>();

        [SerializeField]
        private Vector3 _openedPos, _closedPos;

        [SerializeField]
        private Button _sellAllButton, _selectAllButton;

        private bool _isOpened = true;

        private void Awake(){
            _draggable.SetIsActiveFunc(IsDraggableActive);
            _draggable.SetClampValues(true, new Vector2(_closedPos.y, _openedPos.y));
        }

        private bool IsDraggableActive() => !_scalesPanel.IsSellingAnimation;

        public void SetData(Queue<Crop> cropsCollected){
            Dictionary<Crop, int> crops = new Dictionary<Crop, int>();
            foreach (var crop in cropsCollected){
                if (crops.ContainsKey(crop)){
                    crops[crop]++;
                } else{
                    crops.Add(crop, 1);
                }
            }

            SetData(crops);
            Close();
        }

        public void SetData(Dictionary<Crop, int> crops){
            _cropsAmount = crops;
            InitLines(_cropsAmount);
            InitButtons(_cropsAmount);
        }

        private void InitLines(Dictionary<Crop, int> crops){
            DestroyChildren(_linesHolder);
            
            _cropLineMap = new Dictionary<Crop, SellTableLine>();
            if (crops.Count == 0){
                SellTableLine line = Instantiate(_emptyLinePrefab, _linesHolder);
                _cropLineMap.Add(Crop.None, line);
            } else{
                foreach (var cropIntPair in crops){
                    SellTableLine line = Instantiate(_linePrefab, _linesHolder);
                    line.SetData(cropIntPair.Key, cropIntPair.Value);
                    _cropLineMap.Add(cropIntPair.Key, line);
                }
            }
        }

        private void InitButtons(Dictionary<Crop, int> crops){
            _sellAllButton.interactable = crops.Count > 0;
            _selectAllButton.interactable = crops.Count > 0;
        }

        public void Open(){
            if (_isOpened){
                return;
            }

            _isOpened = true;
            _draggable.ChangeStartingPlace(_openedPos);
            _draggable.ChangeRects(true);
            _draggable.GoBackToPlaceSmooth();
        }

        public void Close(){
            if (!_isOpened){
                return;
            }

            _isOpened = false;
            _draggable.ChangeStartingPlace(_closedPos);
            _draggable.ChangeRects(false);
            _draggable.GoBackToPlaceSmooth();
        }

        public void SellSelectedCrops(){
            List<Crop> cropsToSell = new List<Crop>();
            foreach (var cropLine in _cropLineMap){
                for (int i = 0; i < cropLine.Value.SelectedAmount; i++){
                    cropsToSell.Add(cropLine.Key);
                }
            }

            _scalesPanel.SellSelected(cropsToSell);
        }

        public void SelectAll(){
            foreach (SellTableLine line in _cropLineMap.Values){
                line.SelectAll();
            }
        }
    }
}