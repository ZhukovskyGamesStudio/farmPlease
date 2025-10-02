using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tables;
using UnityEngine;
using Random = System.Random;

public class Tile3d : MonoBehaviour {
    [Header("Grow")]
    public bool IsGrowOnStart;

    [Header("GrowFromOtherState")]
    [SerializeField]
    private Tile3d _previousState;

    [SerializeField]
    private Tile3dAnimConfig _animConfig;
    
    [SerializeField]
    private float _endPixelCropScale = 2.5f;

    [Header("Wind Sway")]
    [SerializeField]
    private List<SpriteRenderer> _crops = new();

    private void Start() {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();

        if (IsGrowOnStart) {
            if (_previousState != null) {
                GrowFromPrevious().ContinueWith(StartWind);
            } else {
                Grow(ct: ct).ContinueWith(StartWind);
            }
        } else {
            StartWind();
        }
    }

    private void StartWind() {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in children) {
            if (t == transform) continue;
            WindSway(t, ct).Forget();
        }
    }

    private async UniTask GrowFromPrevious() {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();

        List<Transform> children = new();
        foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
            if (t != transform) {
                children.Add(t);
                t.gameObject.SetActive(false);
            }
        }

        Tile3d prev = Instantiate(_previousState, transform.position, transform.rotation, transform);

        await prev.SquashTo(_animConfig.SquashScale, _animConfig.SquashDuration, ct);

        Destroy(prev.gameObject);

        foreach (Transform t in children) {
            t.gameObject.SetActive(true);
            t.localScale = new Vector3(_animConfig.SquashScale, _animConfig.SquashScale, 1f);
            t.localRotation = Quaternion.identity;
        }

        await Grow(fromScale: _animConfig.SquashScale, overshoot: _animConfig.OvershootScale, ct: ct);
    }

    private async UniTask Grow(float fromScale = 0f, float overshoot = 1f, CancellationToken ct = default) {
        Transform[] objs = transform.GetComponentsInChildren<Transform>();
        List<UniTask> tasks = new();
        Random rnd = new();

        foreach (Transform t in objs) {
            if (t == transform) {
                continue;
            }

            if (fromScale == 0f) {
                t.localScale = Vector3.zero;
            }

            t.localRotation = Quaternion.identity;

            float delay = (float)rnd.NextDouble() * _animConfig.Delay;
            float duration = _animConfig.Duration + (float)rnd.NextDouble() * _animConfig.AddedDuration;

            tasks.Add(GrowOne(t, duration, delay, fromScale, overshoot, ct));
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask GrowOne(Transform t, float duration, float startDelay, float fromScale, float overshoot,
        CancellationToken cancellationToken) {
        if (startDelay > 0f) {
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: cancellationToken);
        }

        float elapsed = 0f;
        float wiggleFreq = 8f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / duration);

            float baseScale = Mathf.Lerp(fromScale, 1f, Mathf.SmoothStep(0f, 1f, p));

            float scaleWithOvershoot = Mathf.Lerp(baseScale, overshoot, p * (1f - p) * 2f);

            float angle = Mathf.Sin(p * Mathf.PI * wiggleFreq) * _animConfig.WiggleAmplitudeDegrees * (1f - p);

            t.localScale = new Vector3(scaleWithOvershoot, scaleWithOvershoot, 1f);
            t.localRotation = Quaternion.Euler(0f, 0f, angle);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
    }

    public async UniTask Scythe(Crop cropCollected, int collectedAmount) {
        List<FlyingCropFx> fcs = new();
        _crops = _crops.OrderBy(t => UnityEngine.Random.Range(0, 1f)).ToList();
        Queue<SpriteRenderer> queue = new(_crops);
        Vector3 startPos = transform.position;
        int min = Math.Min(collectedAmount, _crops.Count);
        for (int index = 0; index < min; index++) {
            SpriteRenderer crop = queue.Dequeue();
            queue.Enqueue(crop);
            crop.transform.SetParent(null);
            crop.sortingOrder = 3;
            FlyingCropFx fc = crop.gameObject.AddComponent<FlyingCropFx>();
            fc.CacheFinalSprite(CropsTable.CropByType(cropCollected).VegSprite);
            if (index >= collectedAmount) {
                fc.SetEarlyDisappear();
            }

            fcs.Add(fc);
        }

        for (int i = 0; i < collectedAmount - _crops.Count; i++) {
            FlyingCropFx rndc = fcs[UnityEngine.Random.Range(0, fcs.Count)];
            FlyingCropFx clone = Instantiate(rndc);
            fcs.Add(clone);
        }

        Transform[] objs = transform.GetComponentsInChildren<Transform>();
        List<UniTask> tasks = new();
        Random rnd = new();

        foreach (Transform t in objs) {
            if (t == transform) {
                continue;
            }

            float delay = (float)rnd.NextDouble() * _animConfig.ScytheDelay;
            float duration = _animConfig.ScytheDuration + (float)rnd.NextDouble() * _animConfig.ScytheAddedDuration;

            tasks.Add(ScytheOne(t, duration, delay + 0.25f));
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
        StartFlyingAll(fcs, startPos);
        await UniTask.WhenAll(tasks);
    }

    private void StartFlyingAll(List<FlyingCropFx> crops, Vector3 fanCenter, float fanAngle = 135f) {
        int total = crops.Count;
        crops = crops.OrderByDescending(c => c.transform.position.x).ToList();
        for (int i = 0; i < total; i++) {
            crops[i].PlayAnimAndDestroyFan(_endPixelCropScale,i, total, fanCenter, fanAngle).Forget();
        }
    }

    private async UniTask ScytheOne(Transform t, float duration, float startDelay) {
        Transform copy = Instantiate(t, t.position, t.rotation);
        t.gameObject.SetActive(false);
        SpriteRenderer sr = copy.GetComponent<SpriteRenderer>();

        if (startDelay > 0f) {
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay));
        }

        Vector3 startPos = copy.localPosition;
        Quaternion startRot = copy.localRotation;
        Color startColor = sr != null ? sr.color : Color.white;

        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / duration);

            // поворот
            float angle = Mathf.Lerp(0f, _animConfig.ScytheAngle, p);
            copy.localRotation = Quaternion.Euler(0f, 0f, angle);

            // падение вниз
            copy.localPosition = startPos + Vector3.down * _animConfig.ScytheFallDistance * p;

            // затухание
            if (sr != null) {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, p);
                sr.color = c;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        if (sr != null) {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        Destroy(copy.gameObject);
    }

    private async UniTaskVoid WindSway(Transform t, CancellationToken ct) {
        Random rnd = new();
        float waitTime = UnityEngine.Random.Range(0, _animConfig.WindSwayIntervalMin);
        while (!ct.IsCancellationRequested) {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: ct);

            float elapsed = 0f;
            float duration = 2f; // длительность одного покачивания
            Quaternion startRot = t.localRotation;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float angle = Mathf.Sin(elapsed * Mathf.PI * _animConfig.WindSwayFrequency) * _animConfig.WindSwayAmplitude;
                t.localRotation = startRot * Quaternion.Euler(0f, 0f, angle);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            t.localRotation = startRot; // вернуть исходную ориентацию
            waitTime = _animConfig.WindSwayIntervalMin +
                       (float)rnd.NextDouble() * (_animConfig.WindSwayIntervalMax - _animConfig.WindSwayIntervalMin);
        }
    }
}
