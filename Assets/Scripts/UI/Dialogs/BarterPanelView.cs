using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using Time = Managers.Time;

namespace UI {
    public class BarterPanelView : MonoBehaviour {
        [SerializeField] private ScalesView scalesView;
        [SerializeField] private Animation _animation;

        private bool isSelling;

        private void OnEnable() {
            isSelling = false;
            scalesView.StartRainingCrops(SaveLoadManager.CurrentSave.CropsCollectedQueue);
        }

        public void CloseButton() {
            if (isSelling) {
                return;
            }

            gameObject.SetActive(false);
        }

        public void SellAllButton() {
            if (isSelling) {
                return;
            }

            isSelling = true;
            StartCoroutine(SellCoroutine());
        }

        private IEnumerator SellCoroutine() {
            _animation.Play("StartSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            yield return StartCoroutine(scalesView.SellAllCrops());
            int cropsAmount = SaveLoadManager.CurrentSave.CropsCollected.Count;
            SaveLoadManager.CurrentSave.CropPoints += cropsAmount;
            int coinsGain = cropsAmount * (Time.Instance.IsTodayLoveDay ? 2 : 1);
            SaveLoadManager.CurrentSave.Coins += coinsGain;
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
            isSelling = false;
        }
    }
}