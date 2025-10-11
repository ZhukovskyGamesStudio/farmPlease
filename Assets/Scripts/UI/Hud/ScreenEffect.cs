using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace UI {
    public class ScreenEffect : MonoBehaviour {
        [SerializeField]
        private SerializableDictionary<HappeningType, Animation> _happeningAnimations;

        [SerializeField]
        private Animation _nightAnimation, _nightStarsAnimation;

        private const string BEFORE_DAY_APPEAR = "BeforeDayAppear";
        private const string BEFORE_DAY_IDLE = "BeforeDayIdle";
        private const string DAY_APPEAR = "DayAppear";
        private const string DAY_IDLE = "DayIdle";
        private const string DAY_DISAPPEAR = "DayDisappear";

        private const string NIGHT_APPEAR = "NightAppear";
        private const string NIGHT_DISAPPEAR = "NightDisappear";

        private HappeningType _curHappeningType;

        [SerializeField]
        private AnimationClip _starsStart, _starsEnd;

        public async UniTask SetEffectCoroutine(HappeningType type, bool isTomorrow) {
            if (NoneHappenings.Contains(type)) {
                type = HappeningType.NormalSunnyDay;
            }

            _curHappeningType = type;

            await TryPlayCurrentAnimation(type, isTomorrow);
        }

        public async UniTask PlayOverNightAnimation() {
            _nightAnimation.Play(NIGHT_APPEAR);
            _nightAnimation.PlayQueued(NIGHT_DISAPPEAR);
            _nightStarsAnimation.Play(_starsStart.name);
            _nightStarsAnimation.PlayQueued(_starsEnd.name);
            await UniTask.WaitWhile(() => _nightAnimation.isPlaying);
        }

        public async UniTask ChangeEffectCoroutine(HappeningType type, bool isTomorrow) {
            if (NoneHappenings.Contains(type)) {
                type = HappeningType.NormalSunnyDay;
            }

            if (_curHappeningType == type) {
                return;
            }

            await TryPlayDisappearAnimation(_curHappeningType);
            await SetEffectCoroutine(type, isTomorrow);
        }

        private async UniTask TryPlayCurrentAnimation(HappeningType type, bool isTomorrow) {
            if (!_happeningAnimations.ContainsKey(type)) {
                return;
            }

            if (isTomorrow) {
                _happeningAnimations[type].Play(BEFORE_DAY_APPEAR);
                await UniTask.WaitWhile(() => _happeningAnimations[type].isPlaying);
                _happeningAnimations[type].PlayQueued(BEFORE_DAY_IDLE);
            } else {
                _happeningAnimations[type].Play(DAY_APPEAR);
                await UniTask.WaitWhile(() => _happeningAnimations[type].isPlaying);
                _happeningAnimations[type].PlayQueued(DAY_IDLE);
            }
        }

        private async UniTask TryPlayDisappearAnimation(HappeningType type) {
            if (!_happeningAnimations.ContainsKey(type)) {
                return;
            }

            _happeningAnimations[type].Play(DAY_DISAPPEAR);
            await UniTask.WaitWhile(() => _happeningAnimations[type].isPlaying);
        }

        private List<HappeningType> NoneHappenings => new List<HappeningType>() {
            HappeningType.FoodMarket, HappeningType.Love
        };
    }
}