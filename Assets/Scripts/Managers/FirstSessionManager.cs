using System.Collections;
using DefaultNamespace.Abstract;
using DefaultNamespace.Tables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace.UI {
    public class FirstSessionManager : Singleton<FirstSessionManager>, IPreloadable {
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

            DisableUiParts();

            PlayerController.Instance.ChangeTool(Tool.Collect);
            ShowSpeakingBot(_ftueConfig.StartHint);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            
            ShowScytheSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => false); // TODO add waiting for seeds collected

            yield return new WaitForSeconds(0.3f);
            ShowHoeSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 4);
            
            ShowSeedsSelectSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() =>  PlayerController.Instance.seedBagCrop != CropsType.Tomato); 
            
            yield return new WaitForSeconds(0.3f);
            ShowSeedsUsedSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 2); 
            
            ShowWaterSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 0); 

            yield return new WaitForSeconds(0.3f);
            ShowClockSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy < 7); 

            yield return new WaitForSeconds(0.6f);
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
        }

        private void EnableUiParts() {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
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

        private void ShowScytheSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.FastPanelScript.toolButtons[3].transform, _ftueConfig.ScytheHint, StepEnded);
        }

        private void ShowCollectGarbageSpotlight() {
            
            //TODO Implement garbage  gainSeedHint
        }

        private void ShowHoeSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.FastPanelScript.toolButtons[0].transform, _ftueConfig.HoeHint, StepEnded);
        }

        private void ShowSeedsSelectSpotlight() {
            _isWatingForStepEnd = true;
            PlayerController.Instance.seedBagCrop = CropsType.Weed;
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(false);
            InventoryManager.instance.BuySeed(CropsType.Eggplant,0,1);
            InventoryManager.instance.BuySeed(CropsType.Tomato,0,1);
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.Backpack.transform, _ftueConfig.SeedSelectHint, StepEnded);
        }
        
        private void ShowSeedsUsedSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight( UIHud.Instance.FastPanelScript.toolButtons[2].transform, _ftueConfig.SeedHint, StepEnded);
        }

        private void ShowWaterSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.FastPanelScript.toolButtons[1].transform, _ftueConfig.WaterHint, StepEnded);
        }

        private void ShowClockSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(false);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ClockView.transform, _ftueConfig.ClockHint, StepEnded);
        }

        public void Init() {
            TryStartFtue();
        }
    }
}