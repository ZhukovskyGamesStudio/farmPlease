using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using Time = Managers.Time;

namespace UI{
    public class ScalesPanelView : MonoBehaviour{
        [SerializeField]
        private ScalesView scalesView;

        [SerializeField]
        private Animation _animation;
        

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private SellTabletView _sellTablet;

        public bool IsSellingAnimation{ get; private set; }
        public Button CloseButton => _closeButton;
        
        private void OnEnable(){
            IsSellingAnimation = false;
            scalesView.StartRainingCrops(SaveLoadManager.CurrentSave.CropsCollectedQueue);
            _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue);
        }

        public void Close(){
            if (IsSellingAnimation){
                return;
            }

            gameObject.SetActive(false);
        }

        public void SellAll(){
            SellSelected(new List<Crop>(SaveLoadManager.CurrentSave.CropsCollected));
        }

        public void SellSelected(List<Crop> crops){
            if (IsSellingAnimation){
                return;
            }

            IsSellingAnimation = true;

            StartCoroutine(SellCoroutine(crops));
        }

        private IEnumerator SellCoroutine(List<Crop> crops){
            _sellTablet.Close();
            _animation.Play("StartSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            yield return StartCoroutine(scalesView.SellCrops(crops));
            int cropsAmount = SaveLoadManager.CurrentSave.CropsCollected.Count;
            int coinsGain = cropsAmount * (Time.Instance.IsTodayLoveDay ? 2 : 1);
            SaveLoadManager.CurrentSave.Coins += coinsGain;
            UIHud.Instance.UpdateCounters();
            SaveLoadManager.CurrentSave.CropsCollected = new List<Crop>();
            SaveLoadManager.SaveGame();

            yield return new WaitWhile(() => _animation.isPlaying);
            _animation.Play("ContinueSelling");
            yield return new WaitWhile(() => _animation.isPlaying);

            if (false){
                _animation.Play("MarkMission");
                yield return new WaitWhile(() => _animation.isPlaying);
            }

            _animation.Play("EndSelling");
            yield return new WaitWhile(() => _animation.isPlaying);
            IsSellingAnimation = false;
            _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue);
        }
    }
}