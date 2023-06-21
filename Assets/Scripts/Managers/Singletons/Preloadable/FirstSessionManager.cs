using System.Collections;
using Abstract;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class FirstSessionManager : PreloadableSingleton<FirstSessionManager>
    {
        [SerializeField] private FtueConfig _ftueConfig;

        public bool IsFirstSession => !KnowledgeManager.HasKnowledge(Knowledge.Training);

        private int _curFtueStep;
        private bool _isWatingForStepEnd;

        private void TryStartFtue()
        {
            SaveLoadManager.LoadSavedData();
            if (IsFirstSession)
            {
                StartCoroutine(FtueCoroutine());
            }
        }

        private IEnumerator FtueCoroutine()
        {
            SaveLoadManager.ClearSave();
            WaitForLoadingEnd();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            DisableUiParts();

            PlayerController.Instance.ChangeTool(Tool.Collect);
            ShowSpeakingBot(_ftueConfig.StartHint);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            ShowSpeakingBot(_ftueConfig.StartHint2);
            yield return new WaitWhile(() => _isWatingForStepEnd);
            
            ShowHoeSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => PlayerController.Instance.CurTool != Tool.Hoe);
            
            ShowDoHoeSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 7);
            
            ShowEnergySpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            
            ShowBackpackSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitForSeconds(0.3f);
            ShowSeedsSelectSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => PlayerController.Instance.seedBagCrop != Crop.Tomato);
            ShowDoSeedSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 6);
            yield return new WaitForSeconds(0.3f);

            ShowWaterSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            ShowDoWaterSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy == 5);
            yield return new WaitForSeconds(0.3f);
            
            ShowClockSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy < 7);
            
            ShowDoWaterAgainSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.Energy > 5);

            ShowScytheSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);

            ShowDoScytheSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            yield return new WaitWhile(() => SaveLoadManager.CurrentSave.CropPoints == 0);
            yield return new WaitForSeconds(0.3f);
            
            yield return new WaitForSeconds(0.6f);
            EnableUiParts();
            ShowSpeakingBot(_ftueConfig.EndHint);
            yield return new WaitWhile(() => _isWatingForStepEnd);

            KnowledgeManager.AddKnowledge(Knowledge.Training);
        }

        private void DisableUiParts()
        {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(false);
            UIHud.Instance.Backpack.gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(false);
            UIHud.Instance.ClockView.gameObject.SetActive(false);
            UIHud.Instance.BatteryView.gameObject.SetActive(false);
        }

        private void EnableUiParts()
        {
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(true);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.BatteryView.gameObject.SetActive(true);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            
            UIHud.Instance.ClockView.GetComponent<Button>().interactable = true;
            UIHud.Instance.Backpack.OpenButton.interactable = true;
        }

        private void WaitForLoadingEnd()
        {
            _isWatingForStepEnd = true;
            SceneManager.sceneLoaded += delegate { StepEnded(); };
        }

        private void StepEnded()
        {
            _isWatingForStepEnd = false;
        }

        private void ShowSpeakingBot(string hintText)
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.KnowledgeCanSpeak.ShowSpeak(hintText, StepEnded);
        }
        private void ShowEnergySpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.BatteryView.gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(  UIHud.Instance.BatteryView.transform,
                _ftueConfig.EnergyHint, StepEnded);
        }
        
        private void ShowBackpackSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            PlayerController.Instance.seedBagCrop = Crop.Weed;
            InventoryManager.Instance.BuySeed(Crop.Tomato, 0, 2);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.OpenButton,
                _ftueConfig.BackpackHint, StepEnded);
        }
        
        private void ShowSeedsSelectSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.OpenButton.interactable = false;
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(false);
         
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.Backpack.TomatoButton,
                _ftueConfig.SeedSelectHint, StepEnded);
        }
        
        private void ShowDoSeedSpotlight()
        {
            _isWatingForStepEnd = true;
        
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(false);
         
            UIHud.Instance.Backpack.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoSeedHint, StepEnded);
        }
        
        private void ShowScytheSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[3].GetComponent<Button>(),
                _ftueConfig.ScytheHint, StepEnded);
        }
        
        private void ShowDoScytheSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(true);

            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoScytheHint, StepEnded);
        }

        private void ShowHoeSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[0].GetComponent<Button>(),
                _ftueConfig.HoeHint, StepEnded);
        }
        
        private void ShowDoHoeSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[3].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[0].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoHoeHint, StepEnded);
        }

        private void ShowWaterSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.Backpack.gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[2].gameObject.SetActive(false);
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.FastPanelScript.toolButtons[1].GetComponent<Button>(),
                _ftueConfig.WaterHint, StepEnded);
        }
        
        private void ShowDoWaterSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoWaterHint, StepEnded);
        }

        private void ShowClockSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.FastPanelScript.toolButtons[1].gameObject.SetActive(false);
            UIHud.Instance.ClockView.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ClockView.GetComponent<Button>(), _ftueConfig.ClockHint,
                StepEnded);
        }
        
        private void ShowDoWaterAgainSpotlight()
        {
            _isWatingForStepEnd = true;
            UIHud.Instance.ClockView.GetComponent<Button>().interactable = false;
            UIHud.Instance.SpotlightWithText.ShowSpotlight(SmartTilemap.Instance.GetTile(Vector3Int.zero).transform,
                _ftueConfig.DoWaterAgainHint, StepEnded);
        }

        protected override void OnFirstInit()
        {
            TryStartFtue();
        }
    }
}