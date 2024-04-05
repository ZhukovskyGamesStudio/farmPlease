using System.Collections;
using Abstract;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers {
    public class FirstSessionManager : PreloadableSingleton<FirstSessionManager> {
        [SerializeField]
        private FtueConfig _ftueConfig;

        public bool IsFirstSession => !KnowledgeManager.HasKnowledge(Knowledge.Training);

        private int _curFtueStep;
        private bool _isWatingForStepEnd;

        private void TryStartFtue() {
            SaveLoadManager.LoadSavedData();
            if (IsFirstSession) {
                StartCoroutine(FtueCoroutine());
            }
        }

        private IEnumerator FtueCoroutine() {
            SaveLoadManager.ClearSave();
            WaitForLoadingEnd();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            if (GameModeManager.Instance.IsSkipTraining) {
                KnowledgeManager.AddKnowledge(Knowledge.Training);
                yield break;
            }

            DisableUiParts();

            PlayerController.Instance.ChangeTool(Tool.Collect);
            ShowSpeakingBot(_ftueConfig.StartHint);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            ShowSpeakingBot(_ftueConfig.StartHint2);
            yield return new WaitWhile(() => _isWatingForStepEnd);

            yield return StartCoroutine(ShowHoeSpotlight());
            yield return StartCoroutine(ShowDoHoeSpotlight());
            yield return StartCoroutine(ShowEnergySpotlight());

            ShowBackpackSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            ShowSeedsSelectSpotlight();

            yield return new WaitWhile(() => _isWatingForStepEnd);

            yield return StartCoroutine(ShowDoSeedSpotlight());

            ShowWaterSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return StartCoroutine(ShowDoWaterSpotlight());

            ShowClockSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy < 7);

            ShowClockLostEnergySpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            yield return StartCoroutine(ShowDoWaterAgainSpotlight());

            ShowScytheSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            yield return StartCoroutine(ShowDoScytheSpotlight());

            yield return StartCoroutine(ShowScalesSpotlight());
            yield return StartCoroutine(ShowSelectAllSpotlight());
            yield return StartCoroutine(ShowSellSpotlight());
            yield return StartCoroutine(ShowCloseScalesSpotlight());

            yield return StartCoroutine(ShowSeedShopSpotlight());
            yield return StartCoroutine(ShowBuyTomatoSpotlight());
            yield return StartCoroutine(ShowBuyEggplantSpotlight());
            yield return StartCoroutine(ShowCloseSeedShopSpotlight());

            yield return StartCoroutine(ShowCroponomSpotlight());

            EnableUiParts();
            ShowSpeakingBot(_ftueConfig.EndHint);
            yield return new WaitWhile(() => _isWatingForStepEnd);

            KnowledgeManager.AddKnowledge(Knowledge.Training);
        }

        private void DisableUiParts() {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(false);
            UIHud.Instance.Backpack.gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(false);
            UIHud.Instance.ClockView.gameObject.SetActive(false);
            UIHud.Instance.BatteryView.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.ScalesView.CloseButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(false);
            UIHud.Instance.CroponomButton.gameObject.SetActive(false);

            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SelectAllButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SellButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SelectAllButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.seedShopView.CloseButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.blocksRaycasts = false;
            UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.blocksRaycasts = false;
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.ScrollCanvasGroup.blocksRaycasts = false;
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.IsFixedByTraining = true;
        }

        private void EnableUiParts() {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.BatteryView.gameObject.SetActive(true);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ScalesView.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.interactable = true;
            UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.interactable = true;
            UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.blocksRaycasts = true;
            UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.blocksRaycasts = true;

            UIHud.Instance.ClockView.GetComponent<Button>().interactable = true;
            UIHud.Instance.Backpack.IsLockOpenCloseByFtue = false;
            UIHud.Instance.Backpack.OpenButton.interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[2].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = true;
            UIHud.Instance.ClockView.IsLockedByFtue = false;
            UIHud.Instance.CroponomButton.gameObject.SetActive(true);

            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SelectAllButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SellButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.seedShopView.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.ScrollCanvasGroup.blocksRaycasts = true;
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.IsFixedByTraining = false;
        }

        private void WaitForLoadingEnd() {
            _isWatingForStepEnd = true;
            SceneManager.sceneLoaded += delegate { StepEnded(); };
        }

        private void StepEnded() {
            _isWatingForStepEnd = false;
        }

        private void ShowSpeakingBot(string hintText) {
            _isWatingForStepEnd = true;
            UIHud.Instance.KnowledgeCanSpeak.ShowSpeak(hintText, StepEnded);
        }

        private IEnumerator ShowEnergySpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.BatteryView.gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.BatteryView.transform, _ftueConfig.EnergyHint, StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private void ShowBackpackSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            PlayerController.Instance.ChangeTool(Tool.SeedBag);
            PlayerController.Instance.seedBagCrop = Crop.Weed;
            InventoryManager.Instance.BuySeed(Crop.Tomato, 0, 1);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.OpenButton, _ftueConfig.BackpackHint, delegate {
                UIHud.Instance.Backpack.IsLockOpenCloseByFtue = true;
                StepEnded();
            });
        }

        private void ShowSeedsSelectSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = false;

            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.TomatoButton, _ftueConfig.SeedSelectHint, StepEnded);
        }

        private IEnumerator ShowDoSeedSpotlight() {
            _isWatingForStepEnd = true;

            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = false;

            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform, _ftueConfig.DoSeedHint,
                StepEnded, false);

            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 6);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitForSeconds(0.3f);
        }

        private void ShowScytheSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = true;

            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[3], _ftueConfig.ScytheHint,
                StepEnded);
        }

        private IEnumerator ShowDoScytheSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform, _ftueConfig.DoScytheHint,
                StepEnded, false);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.CropPoints == 0);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitForSeconds(0.3f);
        }

        private IEnumerator ShowHoeSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[0], _ftueConfig.HoeHint,
                StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => PlayerController.Instance.CurTool != Tool.Hoe);
        }

        private IEnumerator ShowDoHoeSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform, _ftueConfig.DoHoeHint,
                StepEnded, false);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 7);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private void ShowWaterSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.OpenButton.interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[2].interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[1], _ftueConfig.WaterHint,
                StepEnded);
        }

        private IEnumerator ShowDoWaterSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform, _ftueConfig.DoWaterHint,
                StepEnded, false);

            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 5);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitForSeconds(0.3f);
        }

        private void ShowClockSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].interactable = false;
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ClockView.GetComponent<Button>(), _ftueConfig.ClockHint,
                delegate {
                    StepEnded();
                    UIHud.Instance.ClockView.IsLockedByFtue = true;
                });
        }

        private void ShowClockLostEnergySpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ClockView.transform, _ftueConfig.ClockLostEnergyHint, StepEnded);
        }

        private IEnumerator ShowDoWaterAgainSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.ClockView.GetComponent<Button>().interactable = false;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoWaterAgainHint, StepEnded, false);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 5);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowScalesSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.ShopsPanel.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ScalesButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ScalesButton, _ftueConfig.ScalesHint, StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowSelectAllSpotlight() {
            _isWatingForStepEnd = true;
            yield return new WaitForSeconds(1.5f);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SelectAllButton,
                _ftueConfig.SelectAllHint, StepEnded);
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SelectAllButton.gameObject.SetActive(true);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowSellSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SellButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ScalesView.SellTabletView.SellButton,
                _ftueConfig.SellHint, StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => UIHud.Instance.ShopsPanel.ScalesView.IsSellingAnimation);
        }

        private IEnumerator ShowCloseScalesSpotlight() {
            yield return new WaitForSeconds(2f);
            _isWatingForStepEnd = true;
            UIHud.Instance.ShopsPanel.ScalesView.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ScalesView.CloseButton,
                _ftueConfig.CloseScalesHint, StepEnded);

            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitForSeconds(0.3f);
        }

        private IEnumerator ShowSeedShopSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.seedShopView.SetSeedsShop(Crop.Tomato, Crop.Eggplant);
            UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.gameObject.SetActive(false);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.SeedShopButton, _ftueConfig.SeedShopHint,
                StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowBuyTomatoSpotlight() {
            _isWatingForStepEnd = true;
            yield return new WaitForSeconds(2.5f);
            UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.blocksRaycasts = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.transform,
                _ftueConfig.BuyTomatoHint, delegate {
                    UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.interactable = false;
                    UIHud.Instance.ShopsPanel.seedShopView.FirstBagCanvas.blocksRaycasts = false;
                    StepEnded();
                });
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Seeds[Crop.Tomato] < 3);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowBuyEggplantSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.blocksRaycasts = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.transform,
                _ftueConfig.BuyEggplantHint, delegate {
                    UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.interactable = false;
                    UIHud.Instance.ShopsPanel.seedShopView.SecondBagCanvas.blocksRaycasts = false;
                    StepEnded();
                });
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Seeds[Crop.Eggplant] < 3);
            UIHud.Instance.SpotlightWithText.Hide();
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowCloseSeedShopSpotlight() {
            _isWatingForStepEnd = true;

            UIHud.Instance.ShopsPanel.seedShopView.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.seedShopView.CloseButton,
                _ftueConfig.CloseSeedsShopHint, StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        private IEnumerator ShowCroponomSpotlight() {
            _isWatingForStepEnd = true;

            UIHud.Instance.CroponomButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.CroponomButton.transform, _ftueConfig.BookHint, StepEnded);
            yield return new WaitWhile(() => _isWatingForStepEnd);
        }

        protected override void OnFirstInit() {
            TryStartFtue();
        }
    }
}