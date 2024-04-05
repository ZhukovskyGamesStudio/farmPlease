using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = Managers.Time;

namespace UI {
    public class ScalesPanelView : MonoBehaviour {
        [SerializeField]
        private ScalesView scalesView;

        [SerializeField]
        private Animation _animation;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private SellTabletView _sellTablet;

        [SerializeField]
        private TextMeshProUGUI _coinsCounter;

        public SellTabletView SellTabletView => _sellTablet;
        
        public bool IsSellingAnimation { get; private set; }
        public bool IsRainingCropsAnimation { get; private set; }
        public Button CloseButton => _closeButton;

        private void OnEnable() {
            IsSellingAnimation = false;
            IsRainingCropsAnimation = true;
            scalesView.StartRainingCrops(SaveLoadManager.CurrentSave.CropsCollectedQueue, delegate { IsRainingCropsAnimation = false; });
            _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue);
            UpdateCoinsCounter();
        }

        public void Close() {
            if (IsSellingAnimation || IsRainingCropsAnimation) {
                return;
            }

            gameObject.SetActive(false);
        }

        public void SellSelected(List<Crop> crops) {
            if (IsSellingAnimation || IsRainingCropsAnimation) {
                return;
            }

            IsSellingAnimation = true;

            StartCoroutine(SellCoroutine(crops));
        }

        private void UpdateCoinsCounter() {
            _coinsCounter.text = SaveLoadManager.CurrentSave.Coins.ToString();
        }

        private IEnumerator SellCoroutine(List<Crop> crops) {
            _sellTablet.Close();
            _animation.Play("StartSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            yield return StartCoroutine(scalesView.SellCrops(crops));
            int cropsAmount = crops.Count;
            int coinsGain = cropsAmount * (Time.Instance.IsTodayLoveDay ? 2 : 1);
            SaveLoadManager.CurrentSave.Coins += coinsGain;
            UIHud.Instance.UpdateCounters();
            RemoveCropsFromCollected(crops);
            SaveLoadManager.SaveGame();
            IsSellingAnimation = false;
            _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue);
            UpdateCoinsCounter();
            yield return new WaitWhile(() => _animation.isPlaying);
            _animation.Play("ContinueSelling");
            yield return new WaitWhile(() => _animation.isPlaying);

            if (false) {
                _animation.Play("MarkMission");
                yield return new WaitWhile(() => _animation.isPlaying);
            }

            _animation.Play("EndSelling");
            //yield return new WaitWhile(() => _animation.isPlaying);
        }

        private void RemoveCropsFromCollected(List<Crop> crops) {
            foreach (Crop crop in crops) {
                SaveLoadManager.CurrentSave.CropsCollected.Remove(crop);
            }
        }
    }
}