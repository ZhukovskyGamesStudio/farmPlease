using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ToolShopView : MonoBehaviour {
    public RectTransform[] slotPosition;
    public GameObject ToolOfferPrefab;

    public GameObject ChangeButton;
    public int ChangeToolCost;
    private Dictionary<ToolBuff, GameObject> buttonD;

    private GameObject[] ToolButtons;

    /**********/

    private void GenerateButtons() {
        ToolButtons = new GameObject[ToolsTable.instance.ToolsSO.Length];
        buttonD = new Dictionary<ToolBuff, GameObject>();
        for (int i = 0; i < ToolButtons.Length; i++) {
            ToolButtons[i] = Instantiate(ToolOfferPrefab, transform);

            ToolSO tool = ToolsTable.instance.ToolsSO[i];
            ToolButtons[i].name = ToolsTable.instance.ToolsSO[i].ToString();
            ToolOffer toolOffer = ToolButtons[i].GetComponent<ToolOffer>();
            toolOffer.costText.text = tool.cost.ToString();
            toolOffer.explainText.text = tool.explainText;

            toolOffer.BuyButton.onClick.AddListener(() => BuyTool(tool, toolOffer.gameObject));
            toolOffer.BuyButton.onClick.AddListener(() => Audio.Instance.PlaySound(Sounds.Button));

            toolOffer.OfferImage.sprite = tool.firstSprite;
            buttonD.Add(tool.buff, ToolButtons[i]);
            ToolButtons[i].SetActive(false);
        }
    }

    public void SetToolShopWithData( GameSaveProfile save) {
        bool[] buttons = save.toolShopButtonsData;
        bool isChangeButtonActive = save.toolShopChangeButton;
        GenerateButtons();
        int counter = 0;
        for (int i = 0; i < buttons.Length; i++) {
            ToolButtons[i].SetActive(buttons[i]);
            if (buttons[i]) {
                if (counter < slotPosition.Length) {
                    ToolButtons[i].transform.position = slotPosition[counter].position;
                    counter++;
                } else {
                    ToolButtons[i].SetActive(false);
                    UnityEngine.Debug.LogWarning("Лишняя активная кнопка");
                }
            }
        }

        ChangeButton.SetActive(isChangeButtonActive);
    }

    public bool[] GetButtons() {
        bool[] buttons = new bool[ToolButtons.Length];
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = ToolButtons[i].activeSelf;

        return buttons;
    }

    public void ChangeTools() {
        if (ToolButtons == null)
            GenerateButtons();
        List<GameObject> buttons = new();
        foreach (ToolBuff key in buttonD.Keys)
            //Здесь должна быть двойная проверка: если всегда доступен ИЛИ уже куплен
            if (ToolsTable.ToolByType(key).isAlwaysAvailable || InventoryManager.instance.isToolsBoughtD[key]) {
                GameObject button = buttonD[key];
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
        if (InventoryManager.instance.EnoughMoney(ChangeToolCost)) {
            InventoryManager.instance.AddCoins(-1 * ChangeToolCost);

            ChangeTools();
            ChangeButton.SetActive(false);
            SaveLoadManager.Instance.SaveGame();
        }
    }

    public void BuyTool(ToolSO tool, GameObject button) {
        if (InventoryManager.instance.EnoughMoney(tool.cost)) {
            InventoryManager.instance.BuyTool(tool.buff, tool.cost, tool.buyAmount);

            button.SetActive(false);
            SaveLoadManager.Instance.SaveGame();
        }
    }
}