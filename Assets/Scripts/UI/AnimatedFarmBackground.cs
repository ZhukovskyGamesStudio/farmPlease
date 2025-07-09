using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

public class AnimatedFarmBackground : Singleton<AnimatedFarmBackground> {
    [SerializeField]
    private List<Transform> _decorsToWiggle;

    [SerializeField]
    private GameObject _tent, _solar, _bridge, _bridgeShadow, _bucket;

    [SerializeField]
    private float _wiggleAngle = 15f; // угол отклонения в градусах

    [SerializeField]
    private float _wiggleDuration = 0.2f; // время одного wiggle в секундах

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private List<Animation> _flyArounAnimations;

    private string _animType = "Type";

    protected override bool IsDontDestroyOnLoad => false;

    private void Start() {
        WiggleDecors(this.GetCancellationTokenOnDestroy()).Forget();
        FlyAround(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public void SetUpgradeState(int level) {
        _bucket.SetActive(level >= 1);
        _tent.SetActive(level >= 2);
        _solar.SetActive(level >= 2);
        _bridge.SetActive(level >= 3);
        _bridgeShadow.SetActive(level >= 3);
    }
    

    private async UniTaskVoid WiggleDecors(CancellationToken cancellationToken) {
        while (true) {
            foreach (var decor in _decorsToWiggle) {
                WiggleDecor(decor, cancellationToken).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(_wiggleDuration), cancellationToken: cancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }
    }

    private async UniTaskVoid FlyAround(CancellationToken cancellationToken) {
        while (true) {
            await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
            _flyArounAnimations = _flyArounAnimations.OrderBy(_ => Random.Range(0, 1f)).ToList();
            foreach (var decor in _flyArounAnimations) {
                decor.Stop();
                decor.Play();
                await UniTask.WaitWhile(() => decor.isPlaying, cancellationToken: cancellationToken);
                await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
            }
        }
    }

    private async UniTask WiggleDecor(Transform decor, CancellationToken cancellationToken) {
        if (decor == null) return;

        Quaternion originalRotation = decor.localRotation;
        Quaternion leftRotation = Quaternion.Euler(0, 0, _wiggleAngle);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -_wiggleAngle);

        // Влево
        await RotateTo(decor, leftRotation, _wiggleDuration / 2f, cancellationToken);
        // Вправо
        await RotateTo(decor, rightRotation, _wiggleDuration, cancellationToken);
        // Назад к исходному
        RotateTo(decor, originalRotation, _wiggleDuration / 2f, cancellationToken).Forget();
    }

    private async UniTask RotateTo(Transform target, Quaternion targetRotation, float duration, CancellationToken cancellationToken) {
        float elapsed = 0f;
        Quaternion startRotation = target.localRotation;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            target.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            await UniTask.Yield(cancellationToken);
        }

        target.localRotation = targetRotation;
    }

    public void SetState(HappeningType happening) {
        int type = 0;
        switch (happening) {
            case HappeningType.Rain:
                type = (int)DayAnimationType.Rain;
                break;
            case HappeningType.Wind:
                type = (int)DayAnimationType.Wind;
                break;
            case HappeningType.Erosion:
                type = (int)DayAnimationType.Erosion;
                break;
        }

        _animator.SetInteger(_animType, type);
    }

    public enum DayAnimationType {
        Day = 0,
        Rain,
        Wind,
        Erosion,
    }
}