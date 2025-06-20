using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BatteryView : MonoBehaviour {
        [SerializeField]
        private List<Image> _chargeImages, _goldenChargeImages;
      
        [SerializeField]
        private Sprite _green, _red;
        
        [SerializeField]
        private GameObject _normalBattery, _goldenBattery;
   

        [SerializeField]
        private Animation _animation;

        [SerializeField]
        private AnimationClip _zeroCharge, _wasteOne, _hasEnergy;
        private Coroutine _coroutine;

        private List<Image> CurrentChargeImages => _isGolden ? _goldenChargeImages : _chargeImages;
        private bool _isGolden;

        public void UpdateGoldenState() {
            _isGolden = SaveLoadManager.CurrentSave.RealShopData.HasGoldenBattery;
            _normalBattery.SetActive(!_isGolden);
            _goldenBattery.SetActive(_isGolden);
            UIHud.Instance.SetBattery(SaveLoadManager.CurrentSave.Energy);
        }
        
        public void NoEnergy() {
            EndCoroutine();
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(NoEnergyCoroutine());
        }

        private void EndCoroutine() {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            for (int i = 0; i < CurrentChargeImages.Count; i++) CurrentChargeImages[i].sprite = _green;
        }

        private IEnumerator NoEnergyCoroutine() {
            for (int i = 0; i < CurrentChargeImages.Count; i++) {
                CurrentChargeImages[i].enabled = true;
                CurrentChargeImages[i].sprite = _red;
            }
            _animation.Stop();
            _animation.Play(_zeroCharge.name);
            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < CurrentChargeImages.Count; i++)
                CurrentChargeImages[i].enabled = false;

            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < CurrentChargeImages.Count; i++)
                CurrentChargeImages[i].enabled = true;

            yield return new WaitForSeconds(0.30f);

            for (int i = 0; i < CurrentChargeImages.Count; i++) {
                CurrentChargeImages[i].enabled = false;
                CurrentChargeImages[i].sprite = _green;
            }
        }

        public void UpdateCharge(int amount) {
            _animation.Stop();
            _animation.Play(_zeroCharge.name);
            EndCoroutine();
            if (amount < 0) {
                UnityEngine.Debug.Log("энергии меньше нуля?!");
                amount = 0;
            }

            for (int i = 0; i < CurrentChargeImages.Count; i++)
                if (i < amount)
                    CurrentChargeImages[i].enabled = true;
                else
                    CurrentChargeImages[i].enabled = false;
        }

        public void ShowHasEnergyAnimation() {
            _animation.Stop();
            _animation.Play(_hasEnergy.name);
        }
    }
}