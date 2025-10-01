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

    [SerializeField]
    private float _duration = 0.45f;

    [SerializeField]
    private float _addedDuration = 0.35f;

    [SerializeField]
    private float _delay = 0.18f;

    [SerializeField]
    private float _wiggleAmplitudeDegrees = 12f;

    [Header("GrowFromOtherState")]
    [SerializeField]
    private Tile3d _previousState;

    [SerializeField]
    private float _squashScale = 0.75f;

    [SerializeField]
    private float _squashDuration = 0.25f;

    [SerializeField]
    private float _overshootScale = 1.1f;

    [Header("Scythe")]
    [SerializeField]
    private float _scytheAngle = 30f;

    [SerializeField]
    private float _scytheFallDistance = 0.6f;

    [SerializeField]
    private float _scytheDuration = 0.5f;

    [SerializeField]
    private float _scytheAddedDuration = 0.2f;

    [SerializeField]
    private float _scytheDelay = 0.05f;

    [Header("Wind Sway")]
    [SerializeField]
    private float _windSwayAmplitude = 3f; // градусы

    [SerializeField]
    private float _windSwayFrequency = 1f; // сколько колебаний в секунду

    [SerializeField]
    private float _windSwayIntervalMin = 5f; // минимальное время между покачиваниями

    [SerializeField]
    private float _windSwayIntervalMax = 15f; // максимальное время

    [SerializeField]
    private List<SpriteRenderer> _crops = new List<SpriteRenderer>();

    private void Start() {
        var ct = this.GetCancellationTokenOnDestroy();

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
        var ct = this.GetCancellationTokenOnDestroy();
        var children = transform.GetComponentsInChildren<Transform>();
        foreach (var t in children) {
            if (t == transform) continue;
            WindSway(t, ct).Forget();
        }
    }

    private async UniTask GrowFromPrevious() {
        var ct = this.GetCancellationTokenOnDestroy();

        var children = new List<Transform>();
        foreach (var t in transform.GetComponentsInChildren<Transform>()) {
            if (t != transform) {
                children.Add(t);
                t.gameObject.SetActive(false);
            }
        }

        var prev = Instantiate(_previousState, transform.position, transform.rotation, transform);

        await prev.SquashTo(_squashScale, _squashDuration, ct);

        Destroy(prev.gameObject);

        foreach (var t in children) {
            t.gameObject.SetActive(true);
            t.localScale = new Vector3(_squashScale, _squashScale, 1f);
            t.localRotation = Quaternion.identity;
        }

        await Grow(fromScale: _squashScale, overshoot: _overshootScale, ct: ct);
    }

    private async UniTask Grow(float fromScale = 0f, float overshoot = 1f, CancellationToken ct = default) {
        var objs = transform.GetComponentsInChildren<Transform>();
        var tasks = new List<UniTask>();
        var rnd = new System.Random();

        foreach (var t in objs) {
            if (t == transform) {
                continue;
            }

            if (fromScale == 0f) {
                t.localScale = Vector3.zero;
            }

            t.localRotation = Quaternion.identity;

            float delay = (float)rnd.NextDouble() * _delay;
            float duration = _duration + (float)rnd.NextDouble() * _addedDuration;

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

            float angle = Mathf.Sin(p * Mathf.PI * wiggleFreq) * _wiggleAmplitudeDegrees * (1f - p);

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
        var startPos = transform.position;
        int min = Math.Min(collectedAmount, _crops.Count);
        for (int index = 0; index < min; index++) {
            SpriteRenderer crop = queue.Dequeue();
            queue.Enqueue(crop);
            crop.transform.SetParent(null);
            crop.sortingOrder = 3;
            var fc = crop.gameObject.AddComponent<FlyingCropFx>();
            fc.CacheFinalSprite(CropsTable.CropByType(cropCollected).VegSprite);
            if (index >= collectedAmount) {
                fc.SetEarlyDisappear();
            }

            fcs.Add(fc);
        }

        for (int i = 0; i < collectedAmount - _crops.Count; i++) {
            var rndc = fcs[UnityEngine.Random.Range(0, fcs.Count)];
            var clone = Instantiate(rndc);
            fcs.Add(clone);
        }
      
       

        var objs = transform.GetComponentsInChildren<Transform>();
        var tasks = new List<UniTask>();
        var rnd = new System.Random();

      
        foreach (var t in objs) {
            if (t == transform) {
                continue;
            }

            float delay = (float)rnd.NextDouble() * _scytheDelay;
            float duration = _scytheDuration + (float)rnd.NextDouble() * _scytheAddedDuration;

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
            crops[i].PlayAnimAndDestroyFan(i, total, fanCenter, fanAngle).Forget();
        }
    }

    private async UniTask ScytheOne(Transform t, float duration, float startDelay) {
        var copy = Instantiate(t, t.position, t.rotation);
        t.gameObject.SetActive(false);
        var sr = copy.GetComponent<SpriteRenderer>();

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
            float angle = Mathf.Lerp(0f, _scytheAngle, p);
            copy.localRotation = Quaternion.Euler(0f, 0f, angle);

            // падение вниз
            copy.localPosition = startPos + Vector3.down * _scytheFallDistance * p;

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
        var rnd = new Random();
        float waitTime = UnityEngine.Random.Range(0, _windSwayIntervalMin);
        while (!ct.IsCancellationRequested) {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: ct);

            float elapsed = 0f;
            float duration = 2f; // длительность одного покачивания
            Quaternion startRot = t.localRotation;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float angle = Mathf.Sin(elapsed * Mathf.PI * _windSwayFrequency) * _windSwayAmplitude;
                t.localRotation = startRot * Quaternion.Euler(0f, 0f, angle);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            t.localRotation = startRot; // вернуть исходную ориентацию
            waitTime = _windSwayIntervalMin + (float)rnd.NextDouble() * (_windSwayIntervalMax - _windSwayIntervalMin);
        }
    }
}

public static class Tile3dExtensions {
    public static async UniTask SquashTo(this Tile3d tile, float targetScale, float duration, CancellationToken ct) {
        tile.IsGrowOnStart = false;
        var objs = tile.transform.GetComponentsInChildren<Transform>();
        var tasks = new List<UniTask>();

        foreach (var t in objs) {
            if (t == tile.transform) {
                continue;
            }

            tasks.Add(SquashOne(t, targetScale, duration, ct));
        }

        await UniTask.WhenAll(tasks);
    }

    private static async UniTask SquashOne(Transform t, float targetScale, float duration, CancellationToken ct) {
        float startScale = t.localScale.x;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / duration);
            float s = Mathf.Lerp(startScale, targetScale, p);
            t.localScale = new Vector3(s, s, 1f);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        t.localScale = new Vector3(targetScale, targetScale, 1f);
    }
}