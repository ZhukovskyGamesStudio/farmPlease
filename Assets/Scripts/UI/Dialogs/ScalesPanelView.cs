using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using Time = Managers.Time;

namespace UI {
    public class ScalesPanelView : MonoBehaviour {
        [SerializeField] private ScalesView scalesView;
        [SerializeField] private Animation _animation;
        [SerializeField] private Button _sellAllButton;
        [SerializeField] private Button _closeButton;
        public bool IsSellingAnimation { get; private set; }
        public Button SellAllButton => _sellAllButton;
        public Button CloseButton => _closeButton;

        private void OnEnable() {
            IsSellingAnimation = false;
            scalesView.StartRainingCrops(SaveLoadManager.CurrentSave.CropsCollectedQueue);
        }

        public void Close() {
            if (IsSellingAnimation) {
                return;
            }

            gameObject.SetActive(false);
        }

        public void SellAll() {
            if (IsSellingAnimation) {
                return;
            }

            IsSellingAnimation = true;
            StartCoroutine(SellCoroutine());
        }

        private IEnumerator SellCoroutine() {
            _animation.Play("StartSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            yield return StartCoroutine(scalesView.SellAllCrops());
            int cropsAmount = SaveLoadManager.CurrentSave.CropsCollected.Count;
            int coinsGain = cropsAmount * (Time.Instance.IsTodayLoveDay ? 2 : 1);
            SaveLoadManager.CurrentSave.Coins += coinsGain;
            UIHud.Instance.UpdateCounters();
            SaveLoadManager.CurrentSave.CropsCollected = new List<Crop>();
            SaveLoadManager.Instance.SaveGame();

            yield return new WaitWhile(() => _animation.isPlaying);
            _animation.Play("ContinueSelling");
            yield return new WaitWhile(() => _animation.isPlaying);

            if (false) {
                _animation.Play("MarkMission");
                yield return new WaitWhile(() => _animation.isPlaying);
            }

            _animation.Play("EndSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            IsSellingAnimation = false;
        }
    }
}