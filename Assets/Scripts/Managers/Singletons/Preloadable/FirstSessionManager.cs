using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Localization;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Managers {
    public class FirstSessionManager {
        private FtueConfig FtueConfig => ConfigsManager.Instance.FtueConfig;
        
        private int _curFtueStep;
        private bool _isWaitingForStepEnd;
        private CancellationTokenSource _endFtueCts = new CancellationTokenSource();

        private void Update() {
            if (Input.GetKeyDown(KeyCode.RightShift)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void TryStartFtue() {
            bool isFirstSession = !KnowledgeUtils.HasKnowledge(Knowledge.Training);
            if (!isFirstSession) {
                return;
            }

            _endFtueCts = new CancellationTokenSource();
            FtueCoroutine().Forget();
            UpdateCheck(_endFtueCts.Token).Forget();
        }

        private async UniTask UpdateCheck(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                if (Input.GetKeyDown(KeyCode.RightShift)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

                await UniTask.WaitForEndOfFrame(token);
            }
        }

        private async UniTask FtueCoroutine() {
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);

            if (GameModeManager.Instance.IsSkipTraining) {
                EndFtue();
                return;
            }

            DisableUiParts();

            PlayerController.Instance.ChangeTool(Tool.Collect);
            await ShowSpeakingBot(LocalizationUtils.L(FtueConfig.StartHintLoc));
            await ChangeSpeakingBot(LocalizationUtils.L(FtueConfig.StartHint2Loc), true);

            await ShowHoeSpotlight();
            await ShowDoHoeSpotlight();
            await ShowEnergySpotlight();
            await ShowClockSpotlight(FtueConfig.ClockHint);
            
            ShowBackpackSpotlight();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.Delay(100); //waiting for backpack to open
            ShowSeedsSelectSpotlight();

            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            UIHud.Instance.BackpackAttention.Hide();
            await ShowDoSeedSpotlight();
            await ShowClockSpotlight(FtueConfig.ClockHint2);
            ShowWaterSpotlight();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await ShowDoWaterSpotlight();

            //await ShowClockLostEnergySpotlight();
            await ShowDoWaterAgainSpotlight();

            ShowScytheSpotlight();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);

            await ShowDoScytheSpotlight();

            await ShowScalesSpotlight();
            await ShowSelectAllSpotlight();
            await ShowSellSpotlight();
            await ShowCloseScalesSpotlight();

            await ShowSeedShopSpotlight();
            await ShowBuyTomatoSpotlight();
            //await (ShowBuyEggplantSpotlight());
            await ShowCloseSeedShopSpotlight();

            //await (ShowCroponomSpotlight());

           
            await ShowSpeakingBot(LocalizationUtils.L(FtueConfig.EndHintLoc), true);
            
#if UNITY_EDITOR
            SaveLoadManager.CurrentSave.IsEditor = true;
#endif
            EndFtue();
            await ShowGetNextLevelSpotlight();
            _endFtueCts.Cancel();
            _endFtueCts.Dispose();
        }
        
        private void EndFtue() {
            QuestsManager.Instance.GenerateNextMainQuest();
            EnableUiParts();
            KnowledgeUtils.AddKnowledge(Knowledge.Training);
            SaveLoadManager.TryCreateFirstSave();
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
        
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(false);
            UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(false);
            UIHud.Instance.CroponomButton.gameObject.SetActive(false);
            UIHud.Instance.SettingsButton.gameObject.SetActive(false);
            UIHud.Instance.OpenRealShopButton.gameObject.SetActive(false);
            
            UIHud.Instance.QuestsInvisibleButton.gameObject.SetActive(false);
            UIHud.Instance.ProfileView.IsLockedByFtue = true;
            AnimatedFarmBackground.Instance.SetUpgradeState(0);
        }

        private void EnableUiParts() {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.BatteryView.gameObject.SetActive(true);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(true);

            UIHud.Instance.ClockView.GetComponent<Button>().interactable = true;
            UIHud.Instance.Backpack.IsLockOpenCloseByFtue = false;
            UIHud.Instance.Backpack.OpenButton.interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[2].interactable = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = true;
            UIHud.Instance.ClockView.IsLockedByFtue = false;
            UIHud.Instance.CroponomButton.gameObject.SetActive(true);
            UIHud.Instance.SettingsButton.gameObject.SetActive(true);
            UIHud.Instance.OpenRealShopButton.gameObject.SetActive(true);
            UIHud.Instance.QuestsInvisibleButton.gameObject.SetActive(true);

            UIHud.Instance.ProfileView.IsLockedByFtue = false;
            UnlockTiles();
            AddQuestboard();
            UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.Hoe);
            UIHud.Instance.UpdateLockedUI();
            AnimatedFarmBackground.Instance.SetUpgradeState(1);
        }

        private static void UnlockTiles() {
            TileUtils.UnlockTiles(SmartTilemap.GenerateInitialCircleTiles());
            SmartTilemap.Instance.GenerateTilesWithData(SaveLoadManager.CurrentSave.TilesData);
        }
        
        private static void AddQuestboard() {
            QuestsUtils.PlaceQuestBoard();
            SmartTilemap.Instance.GenerateTilesWithData(SaveLoadManager.CurrentSave.TilesData);
            SaveLoadManager.CurrentSave.QuestsData.IsUnseenUpdate = true;
            QuestsUtils.ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);
        }

        private void StepEnded() {
            _isWaitingForStepEnd = false;
        }

        private async UniTask ShowSpeakingBot(string hintText, bool isHidingAfter = false) {
            _isWaitingForStepEnd = true;
            UIHud.Instance.KnowledgeCanSpeak.ShowSpeak(hintText, StepEnded, isHidingAfter);
            var delay = UniTask.Delay(TimeSpan.FromSeconds(FtueConfig.AutoSkipAfterSeconds));
            var waitForTap = UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.WhenAny(delay, waitForTap);
            if (_isWaitingForStepEnd) {
                UIHud.Instance.KnowledgeCanSpeak.HideSpeak();
            }
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }
        
        private async UniTask ChangeSpeakingBot(string hintText, bool isHidingAfter = false) {
            _isWaitingForStepEnd = true;
            UIHud.Instance.KnowledgeCanSpeak.ChangeSpeak(hintText, StepEnded, isHidingAfter);
            var delay = UniTask.Delay(TimeSpan.FromSeconds(FtueConfig.AutoSkipAfterSeconds));
            var waitForTap = UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.WhenAny(delay, waitForTap);
            if (_isWaitingForStepEnd) {
                UIHud.Instance.KnowledgeCanSpeak.HideSpeak();
            }
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private async UniTask ShowEnergySpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.BatteryView.gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.BatteryView.transform, FtueConfig.EnergyHint, StepEnded);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private void ShowBackpackSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            PlayerController.Instance.ChangeTool(Tool.SeedBag);
            PlayerController.Instance.seedBagCrop = Crop.Weed;
            InventoryManager.Instance.TryBuySeed(Crop.Tomato, 0, 7);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.OpenButton, FtueConfig.BackpackHint, delegate {
                UIHud.Instance.Backpack.IsLockOpenCloseByFtue = true;
                StepEnded();
            });
        }

        private void ShowSeedsSelectSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = false;
            UIHud.Instance.BackpackAttention.ShowAttention();
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.TomatoButton, FtueConfig.SeedSelectHint, StepEnded);
        }

        private async UniTask ShowDoSeedSpotlight() {
            _isWaitingForStepEnd = true;

            UIHud.Instance.FastPanelScript.toolButtons[0].interactable = false;

            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(GetFarmCenterSpotlight(), FtueConfig.DoSeedHint,
                StepEnded, false);

            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 0);
            UIHud.Instance.SpotlightWithText.Hide();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.Delay(300);
        }

        private void ShowScytheSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[3].interactable = true;

            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[3], FtueConfig.ScytheHint,
                StepEnded);
        }

        private async UniTask ShowDoScytheSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.gameObject.SetActive(true);
            UIHud.Instance.ShopsPanel.ScalesButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(GetFarmCenterSpotlight(), FtueConfig.DoScytheHint,
                StepEnded, false);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.CropPoints < 7+7);
            UIHud.Instance.SpotlightWithText.Hide();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.Delay(300);
        }

        private static Transform GetFarmCenterSpotlight() {
            return UIHud.Instance.CenterFarmTransform;
        }
        private static Transform GetQuestboardSpotlight() {
            return UIHud.Instance.QuestboardTransform;
        }

        private async UniTask ShowHoeSpotlight() {
            _isWaitingForStepEnd = true;
            
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[0], FtueConfig.HoeHint,
                StepEnded);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.WaitWhile(() => PlayerController.Instance.CurTool != Tool.Hoe);
        }

        private async UniTask ShowDoHoeSpotlight() {
            _isWaitingForStepEnd = true;
            
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(GetFarmCenterSpotlight(), FtueConfig.DoHoeHint,
                StepEnded, false);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 0);
            UIHud.Instance.SpotlightWithText.Hide();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private void ShowWaterSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.Backpack.OpenButton.interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[2].interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[1].interactable = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[1], FtueConfig.WaterHint,
                StepEnded);
        }

        private async UniTask ShowDoWaterSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(GetFarmCenterSpotlight(), FtueConfig.DoWaterHint,
                StepEnded, false);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy >0);
            StepEnded();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.Delay(300);
        }

        private async UniTask ShowClockSpotlight(SpotlightAnimConfig hint) {
            _isWaitingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].interactable = false;
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.ClockView.IsLockedByFtue = false;
            bool isWaitingForClock = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ClockView.GetComponent<Button>(), hint,
                delegate {
                    isWaitingForClock = false;
                }, true);
            await UniTask.WaitWhile(() => isWaitingForClock);
            await UniTask.Delay(2200);
            UIHud.Instance.ClockView.IsLockedByFtue = true;
            StepEnded();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy < 7);
        }

        private async UniTask ShowDoWaterAgainSpotlight() {
            _isWaitingForStepEnd = true;
            
            UIHud.Instance.SpotlightWithText.ShowSpotlight(GetFarmCenterSpotlight(),
                FtueConfig.DoWaterAgainHint, StepEnded, false);
            UIHud.Instance.ClockView.IsLockedByFtue = false;
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 0);
            UIHud.Instance.ClockView.IsLockedByFtue = true;
            UIHud.Instance.ClockView.GetComponent<Button>().interactable = false;
            await UniTask.Delay(100);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Energy >0);
            StepEnded();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private async UniTask ShowScalesSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ScalesButton, FtueConfig.ScalesHint, StepEnded,
                true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private async UniTask ShowSelectAllSpotlight() {
            _isWaitingForStepEnd = true;
            await UniTask.Delay(300);
            var scaledDialog = Object.FindAnyObjectByType<ScalesDialog>();
            scaledDialog.CloseButton.gameObject.SetActive(false);
            scaledDialog.SellTabletView.SelectAllButton.gameObject.SetActive(false);
            scaledDialog.SellTabletView.SellButton.gameObject.SetActive(false);
            scaledDialog.SellTabletView.SelectAllButton.gameObject.SetActive(true);
            scaledDialog.SellTabletView.ScrollCanvasGroup.blocksRaycasts = false;
            scaledDialog.SellTabletView.IsFixedByTraining = true;
            
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(scaledDialog.SellTabletView.SelectAllButton,
                FtueConfig.SelectAllHint, StepEnded);
            scaledDialog.SellTabletView.SelectAllButton.gameObject.SetActive(true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private async UniTask ShowSellSpotlight() {
            _isWaitingForStepEnd = true;
            var scaledDialog = Object.FindAnyObjectByType<ScalesDialog>();
            scaledDialog.SellTabletView.SellButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(scaledDialog.SellTabletView.SellButton,
                FtueConfig.SellHint, StepEnded, true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.WaitWhile(() => scaledDialog.IsSellingAnimation);
        }

        private async UniTask ShowCloseScalesSpotlight() {
            await UniTask.Delay(2000);
            _isWaitingForStepEnd = true;
            var scaledDialog = Object.FindAnyObjectByType<ScalesDialog>();
            scaledDialog.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(scaledDialog.CloseButton, FtueConfig.CloseScalesHint,
                StepEnded);

            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            await UniTask.Delay(300);
        }

        private async UniTask ShowSeedShopSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.ShopsPanel.SeedShopButton.gameObject.SetActive(true);
            
            SaveLoadManager.CurrentSave.SeedShopData.FirstOffer = Crop.Tomato;
            SaveLoadManager.CurrentSave.SeedShopData.SecondOffer = Crop.Tomato;
            SaveLoadManager.CurrentSave.SeedShopData.ChangeButtonActive = false;
            
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.SeedShopButton, FtueConfig.SeedShopHint, StepEnded,
                true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
            var seedShopDialog = Object.FindAnyObjectByType<SeedShopDialog>();
            seedShopDialog.CloseButton.gameObject.SetActive(false);
        }

        private async UniTask ShowBuyTomatoSpotlight() {
            _isWaitingForStepEnd = true;
            await UniTask.Delay(1500);
            var seedShopDialog = Object.FindAnyObjectByType<SeedShopDialog>();
            UIHud.Instance.SpotlightWithText.ShowSpotlight(seedShopDialog.FirstBagCanvas.transform,
                FtueConfig.BuyTomatoHint, StepEnded, false);
            await UniTask.WaitWhile(() => SaveLoadManager.CurrentSave.Seeds[Crop.Tomato] < 12);
            UIHud.Instance.SpotlightWithText.Hide();
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }

        private async UniTask ShowCloseSeedShopSpotlight() {
            _isWaitingForStepEnd = true;
            var seedShopDialog = Object.FindAnyObjectByType<SeedShopDialog>();
            seedShopDialog.CloseButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.JumpSpotlightFromVeryBigOnButton(seedShopDialog.CloseButton,
                FtueConfig.CloseSeedsShopHint, StepEnded, true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }
        
        private async UniTask ShowGetNextLevelSpotlight() {
            _isWaitingForStepEnd = true;
            UIHud.Instance.ProfileView.IsLockedByFtue = false;
         
            var buttonObj = GetQuestboardSpotlight();
            buttonObj.gameObject.SetActive(true);
            buttonObj.gameObject.AddComponent(typeof(Button));
            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
                QuestsManager.Instance.OpenQuestsDialog();
                buttonObj.gameObject.SetActive(false);
            });
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(buttonObj.GetComponent<Button>(), FtueConfig.ProfileHint, StepEnded, true);
            await UniTask.WaitWhile(() => _isWaitingForStepEnd);
        }
    }
}