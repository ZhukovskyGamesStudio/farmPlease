using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimatedFarmBackground : MonoBehaviour {
    [SerializeField]
    private List<Transform> _decorsToWiggle;

    [SerializeField]
    private float _wiggleAngle = 15f; // угол отклонения в градусах

    [SerializeField]
    private float _wiggleDuration = 0.2f; // время одного wiggle в секундах

    private void Start() {
        WiggleDecors(this.GetCancellationTokenOnDestroy()).Forget();
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
}