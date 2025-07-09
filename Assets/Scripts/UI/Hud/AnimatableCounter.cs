﻿using TMPro;
using UnityEngine;

namespace UI {
    public class AnimatableCounter : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _amountText;

        [SerializeField]
        private Sprite _icon;

        [SerializeField]
        private CounterChangeFx _changeUpFx, _changeDownFx;

        private int _amount = 0;
        
        public void SetAmount(int amount) {
            _amount = amount;
            if (amount > 99999) {
                _amountText.text = "много";
            } else {
                _amountText.text = amount.ToString();
            }
          
        }

        public void ChangeAmount(int changeAmount) {
            if (changeAmount == 0) {
                return;
            }
            _amount += changeAmount;
            if (_amount < 0) {
                _amount = 0;
            }
            SetAmount(_amount);
            if (gameObject.activeInHierarchy) {
                CounterChangeFx fx = changeAmount > 0 ? _changeUpFx : _changeDownFx;
                CounterChangeFx fxObj = Instantiate(fx, transform.position, Quaternion.identity, transform);
                fxObj.Init(_icon, changeAmount);
            }
        }
    }
}