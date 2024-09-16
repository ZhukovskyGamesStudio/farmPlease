using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Tables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI {
    public class ToolShopView : MonoBehaviour {

        public GameObject ChangeButton;

        [SerializeField]
        private ToolOffer _toolOffer1, _toolOffer2;

        [SerializeField]
        private GameObject _exitButton, _toolBox;

        [SerializeField]
        private Animation _landingPlatformAnimation;

        [SerializeField]
        private Animation _tabletAnimation, _bagAnimation;

        private bool _waitingToolMovedToBag;

        private ToolBuff _toolBuff1, _toolBuff2;

        private bool _fistActive, _secondActive;

        [SerializeField]
        private Transform _noToolsText;
        /**********/

        private void Awake() {
            InitButtons();
        }

        private void InitButtons() {
            _toolOffer1.Init(buff => {
                BuyTool(_toolOffer1, buff);
                Audio.Instance.PlaySound(Sounds.Button);
            });
            _toolOffer2.Init(buff => {
                BuyTool(_toolOffer2, buff);
                Audio.Instance.PlaySound(Sounds.Button);
            });
        }

        public void SetToolShopWithData(GameSaveProfile save) {
            _toolOffer1.SetData(ToolsTable.ToolByType(save.ToolFirstOffer), save.ToolFirstOfferActive);
            _toolBuff1 = save.ToolFirstOffer;
            _fistActive = save.ToolFirstOfferActive;

            _toolOffer2.SetData(ToolsTable.ToolByType(save.ToolSecondOffer), save.ToolSecondOfferActive);
            _toolBuff2 = save.ToolSecondOffer;
            _secondActive = save.ToolSecondOfferActive;

            ChangeButton.SetActive(save.ToolShopChangeButton);
            UpdateNoToolsMessage();
        }

        public void ChangeToolsNewDay() {
            ChangeTools();
            SaveLoadManager.CurrentSave.ToolShopChangeButton = true;
            ChangeButton.SetActive(true);
        }

        public void ChangeTools() {
            List<ToolBuff> possibleTools = ToolsTable.Tools
                .Where(key => ToolsTable.ToolByType(key).isAlwaysAvailable || InventoryManager.Instance.IsToolsBoughtD[key]).ToList();

            GameSaveProfile sv = SaveLoadManager.CurrentSave;
            sv.ToolFirstOffer = possibleTools[Random.Range(0, possibleTools.Count)];
            possibleTools.Remove(sv.ToolFirstOffer);
            sv.ToolSecondOffer = possibleTools[Random.Range(0, possibleTools.Count)];
            sv.ToolFirstOfferActive = true;
            sv.ToolSecondOfferActive = true;

            SetToolShopWithData(sv);
        }

        public void ChangeToolsButton() {
            if (InventoryManager.Instance.EnoughMoney(ConfigsManager.Instance.CostsConfig.ToolsShopChangeCost)) {
                InventoryManager.Instance.AddCoins(-1 * ConfigsManager.Instance.CostsConfig.ToolsShopChangeCost);
                ChangeTools();
                SaveLoadManager.CurrentSave.ToolShopChangeButton = false;
                ChangeButton.SetActive(false);
                SaveLoadManager.SaveGame();
            }
        }

        public void BuyTool(ToolOffer offer, ToolBuff tool) {
            var config = ToolsTable.ToolByType(tool);
            if (InventoryManager.Instance.EnoughMoney(config.cost)) {
                InventoryManager.Instance.BuyTool(config.buff, config.cost, config.buyAmount);
                if (_toolBuff1 == tool) {
                    _fistActive = false;
                    SaveLoadManager.CurrentSave.ToolFirstOfferActive = false;
                } else {
                    _secondActive = false;
                    SaveLoadManager.CurrentSave.ToolSecondOfferActive = false;
                }

                offer.gameObject.SetActive(false);
                StartCoroutine(Buying());
                SaveLoadManager.SaveGame();
                UpdateNoToolsMessage();
            }
        }

        private void UpdateNoToolsMessage() {
            _noToolsText.gameObject.SetActive(!_fistActive && !_secondActive);
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