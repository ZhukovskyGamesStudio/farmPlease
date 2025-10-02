using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class Tile3dExtensions {
    public static async UniTask SquashTo(this Tile3d tile, float targetScale, float duration, CancellationToken ct) {
        tile.IsGrowOnStart = false;
        Transform[] objs = tile.transform.GetComponentsInChildren<Transform>();
        List<UniTask> tasks = new();

        foreach (Transform t in objs) {
            if (t == tile.transform) {
                continue;
            }

            tasks.Add(SquashOne(t, targetScale, duration, ct));
        }

        await UniTask.WhenAll(tasks);
    }

    private static async UniTask SquashOne(Transform t, float targetScale, float duration, CancellationToken ct) {
        await t.DOScale(new Vector3(targetScale, targetScale, 1f), duration).WithCancellation(ct);
    }
}