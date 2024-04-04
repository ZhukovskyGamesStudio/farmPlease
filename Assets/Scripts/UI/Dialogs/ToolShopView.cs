using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class ToolShopView : MonoBehaviour {
        public RectTransform[] slotPosition;
        public GameObject ToolOfferPrefab;

        public GameObject ChangeButton;
        public int ChangeToolCost;
        private Dictionary<ToolBuff, GameObject> _buttonD;

        private GameObject[] _toolButtons;
        [SerializeField]
        private TextMeshProUGUI _coinsCounter;
        
        [SerializeField]
        private GameObject _exitButton, _toolBox;

        [SerializeField]
        private Animation _landingPlatformAnimation;
        
        [SerializeField]
        private Animation _tabletAnimation, _bagAnimation;

        private bool _waitingToolMovedToBag;
        
        /**********/

        private void OnEnable() {
            UpdateCoinsCounter();
        }

        private void GenerateButtons() {
            _toolButtons = new GameObject[ToolsTable.Instance.ToolsSO.Length];
            _buttonD = new Dictionary<ToolBuff, GameObject>();
            for (int i = 0; i < _toolButtons.Length; i++) {
                _toolButtons[i] = Instantiate(ToolOfferPrefab, transform);

                ToolConfig tool = ToolsTable.Instance.ToolsSO[i];
                _toolButtons[i].name = ToolsTable.Instance.ToolsSO[i].ToString();
                ToolOffer toolOffer = _toolButtons[i].GetComponent<ToolOffer>();
                toolOffer.costText.text = tool.cost.ToString();
                toolOffer.explainText.text = tool.explainText;

                toolOffer.BuyButton.onClick.AddListener(() => BuyTool(tool, toolOffer.gameObject));
                toolOffer.BuyButton.onClick.AddListener(() => Audio.Instance.PlaySound(Sounds.Button));

                toolOffer.OfferImage.sprite = tool.gridIcon;
                _buttonD.Add(tool.buff, _toolButtons[i]);
                _toolButtons[i].SetActive(false);
            }
        }

       
        
        private void UpdateCoinsCounter(){
            _coinsCounter.text = SaveLoadManager.CurrentSave.Coins.ToString();
        }

        public void SetToolShopWithData( GameSaveProfile save) {
            bool[] buttons = save.ToolShopButtonsData;
            bool isChangeButtonActive = save.ToolShopChangeButton;
            GenerateButtons();
            int counter = 0;
            for (int i = 0; i < buttons.Length; i++) {
                _toolButtons[i].SetActive(buttons[i]);
                if (buttons[i]) {
                    if (counter < slotPosition.Length) {
                        SetSlotToPosition(_toolButtons[i], slotPosition[counter]);
                        counter++;
                    } else {
                        _toolButtons[i].SetActive(false);
                        UnityEngine.Debug.LogWarning("Лишняя активная кнопка");
                    }
                }
            }

            ChangeButton.SetActive(isChangeButtonActive);
        }

        private void SetSlotToPosition(GameObject offer, Transform slot) {
            offer.transform.SetParent(slot);
            offer.transform.localPosition = Vector3.zero;
            offer.transform.localScale = Vector3.one;
        }

        public bool[] GetButtons() {
            bool[] buttons = new bool[_toolButtons.Length];
            for (int i = 0; i < buttons.Length; i++)
                buttons[i] = _toolButtons[i].activeSelf;

            return buttons;
        }

        public void ChangeTools() {
            if (_toolButtons == null)
                GenerateButtons();
            List<GameObject> buttons = new();
            foreach (ToolBuff key in _buttonD.Keys)
                //Здесь должна быть двойная проверка: если всегда доступен ИЛИ уже куплен
                if (ToolsTable.ToolByType(key).isAlwaysAvailable || InventoryManager.Instance.IsToolsBoughtD[key]) {
                    GameObject button = _buttonD[key];
                    button.SetActive(false);
                    buttons.Add(button);
                }

            for (int i = 0; i < 2; i++) {
                int x = Random.Range(0, buttons.Count);
                GameObject button = buttons[x];

                buttons.Remove(button);

                button.SetActive(true);
                SetSlotToPosition(button, slotPosition[i]);
            }

            ChangeButton.SetActive(true);
            UpdateCoinsCounter();
        }

        public void ChangeToolsButton() {
            if (InventoryManager.Instance.EnoughMoney(ChangeToolCost)) {
                InventoryManager.Instance.AddCoins(-1 * ChangeToolCost);

                ChangeTools();
                ChangeButton.SetActive(false);
                SaveLoadManager.SaveGame();
            }
        }

        public void BuyTool(ToolConfig tool, GameObject button) {
            if (InventoryManager.Instance.EnoughMoney(tool.cost)) {
                InventoryManager.Instance.BuyTool(tool.buff, tool.cost, tool.buyAmount);
                
                
                button.SetActive(false);
                StartCoroutine(Buying());
                SaveLoadManager.SaveGame();
                UpdateCoinsCounter();
            }
        }

       

        public void OnMovedToBag() {
            _waitingToolMovedToBag = false;
            _toolBox.SetActive(false);
        }
        
        private IEnumerator Buying() {
            _toolBox.SetActive(true);
            _exitButton.SetActive(false);
            _waitingToolMovedToBag = true;
            _landingPlatformAnimation.Play("LandingPlatformPrepare");
            _tabletAnimation.Play("TabletHide");
            yield return new WaitWhile(() => _landingPlatformAnimation.isPlaying);
            _landingPlatformAnimation.Play("LandingPlatformLand");
            yield return new WaitWhile(() => _landingPlatformAnimation.isPlaying);
            _bagAnimation.Play("Show");
            yield return new WaitWhile(() => _waitingToolMovedToBag);
            _bagAnimation.Play("Hide");
            yield return new WaitWhile(() => _bagAnimation.isPlaying);
            _tabletAnimation.Play("TabletShow");
            _landingPlatformAnimation.Play("LandingPlatformIdle");
            _exitButton.SetActive(true);
        }
    }
}