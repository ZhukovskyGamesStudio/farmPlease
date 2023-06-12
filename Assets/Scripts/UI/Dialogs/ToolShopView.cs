using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;

namespace UI
{
    public class ToolShopView : MonoBehaviour {
        public RectTransform[] slotPosition;
        public GameObject ToolOfferPrefab;

        public GameObject ChangeButton;
        public int ChangeToolCost;
        private Dictionary<ToolBuff, GameObject> _buttonD;

        private GameObject[] _toolButtons;

        /**********/

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

                toolOffer.OfferImage.sprite = tool.firstSprite;
                _buttonD.Add(tool.buff, _toolButtons[i]);
                _toolButtons[i].SetActive(false);
            }
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
                        _toolButtons[i].transform.position = slotPosition[counter].position;
                        counter++;
                    } else {
                        _toolButtons[i].SetActive(false);
                        UnityEngine.Debug.LogWarning("Лишняя активная кнопка");
                    }
                }
            }

            ChangeButton.SetActive(isChangeButtonActive);
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
                button.transform.position = slotPosition[i].position;
            }

            ChangeButton.SetActive(true);
        }

        public void ChangeToolsButton() {
            if (InventoryManager.Instance.EnoughMoney(ChangeToolCost)) {
                InventoryManager.Instance.AddCoins(-1 * ChangeToolCost);

                ChangeTools();
                ChangeButton.SetActive(false);
                SaveLoadManager.Instance.SaveGame();
            }
        }

        public void BuyTool(ToolConfig tool, GameObject button) {
            if (InventoryManager.Instance.EnoughMoney(tool.cost)) {
                InventoryManager.Instance.BuyTool(tool.buff, tool.cost, tool.buyAmount);

                button.SetActive(false);
                SaveLoadManager.Instance.SaveGame();
            }
        }
    }
}