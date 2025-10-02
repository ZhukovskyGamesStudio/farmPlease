using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;

// ReSharper disable once InconsistentNaming
public static class DOTweenExtensions {
    public static Task WithCancellation(this Tween tween, CancellationToken ct) {
        if (tween == null) throw new System.ArgumentNullException(nameof(tween));

        using var registration = ct.Register(() => {
            if (tween.IsActive() && tween.IsPlaying()) {
                tween.Kill();
            }
        });

        return tween.AsyncWaitForCompletion();
    }
}