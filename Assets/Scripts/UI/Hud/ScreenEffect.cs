using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace UI {
    public class ScreenEffect : MonoBehaviour {
        [SerializeField] private SerializableDictionary<HappeningType, Animation> _happeningAnimations;

        [SerializeField] private Animation _nightAnimation;

        private const string BEFORE_DAY_APPEAR = "BeforeDayAppear";
        private const string BEFORE_DAY_IDLE = "BeforeDayIdle";
        private const string DAY_APPEAR = "DayAppear";
        private const string DAY_IDLE = "DayIdle";
        private const string DAY_DISAPPEAR = "DayDisappear";

        private const string NIGHT_APPEAR = "NightAppear";
        private const string NIGHT_DISAPPEAR = "NightDisappear";

        private HappeningType _curHappeningType;

        public IEnumerator SetEffectCoroutine(HappeningType type, bool isTomorrow) {
            if (NoneHappenings.Contains(type)) {
                type = HappeningType.None;
            }

            _curHappeningType = type;

            yield return StartCoroutine(TryPlayCurrentAnimation(type, isTomorrow));
        }

        public IEnumerator PlayOverNightAnimation() {
            _nightAnimation.Play(NIGHT_APPEAR);
            _nightAnimation.PlayQueued(NIGHT_DISAPPEAR);
            yield return new WaitWhile(() => _nightAnimation.isPlaying);
        }

        public IEnumerator ChangeEffectCoroutine(HappeningType type, bool isTomorrow) {
            if (NoneHappenings.Contains(type)) {
                type = HappeningType.None;
            }

            if (_curHappeningType == type) {
                yield break;
            }

            yield return StartCoroutine(TryPlayDisappearAnimation(_curHappeningType));
            yield return StartCoroutine(SetEffectCoroutine(type, isTomorrow));
        }

        private IEnumerator TryPlayCurrentAnimation(HappeningType type, bool isTomorrow) {
            if (!_happeningAnimations.ContainsKey(type)) {
                yield break;
            }

            if (isTomorrow) {
                _happeningAnimations[type].Play(BEFORE_DAY_APPEAR);
                yield return new WaitWhile(() => _happeningAnimations[type].isPlaying);
                _happeningAnimations[type].PlayQueued(BEFORE_DAY_IDLE);
            } else {
                _happeningAnimations[type].Play(DAY_APPEAR);
                yield return new WaitWhile(() => _happeningAnimations[type].isPlaying);
                _happeningAnimations[type].PlayQueued(DAY_IDLE);
            }
        }

        private IEnumerator TryPlayDisappearAnimation(HappeningType type) {
            if (!_happeningAnimations.ContainsKey(type)) {
                yield break;
            }


            _happeningAnimations[type].Play(DAY_DISAPPEAR);
            yield return new WaitWhile(() => _happeningAnimations[type].isPlaying);
        }

        private List<HappeningType> NoneHappenings => new List<HappeningType>() {
            HappeningType.FoodMarket, HappeningType.Love
        };
    }
}