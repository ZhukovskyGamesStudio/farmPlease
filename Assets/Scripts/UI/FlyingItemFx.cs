using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;

public abstract class FlyingItemFx : MonoBehaviour {
    [Header("Jump Settings")]
    [SerializeField]
    private float _jumpTimeMin = 0.4f;

    [SerializeField]
    private float _jumpTimeMax = 1.6f;

    [SerializeField]
    private float _overshootScale = 1.2f;

    protected virtual float _upShift { get; } = 0;

    protected abstract RectTransform Target { get; }

    protected async UniTask PlayAnimAndDestroy() {
        float rndSizeForce = Random.Range(0.1f, 1.5f);
        if (Random.Range(0, 2) == 0) {
            rndSizeForce *= -1;
        }

        float jumpTime = Random.Range(_jumpTimeMin, _jumpTimeMax);
        Vector3 startPos = transform.position + Vector3.up * Random.Range(-0.1f, 0.1f);
        Vector3 endPos = startPos + Vector3.right * rndSizeForce + Vector3.up * _upShift;

        float maxScale = transform.localScale.x;
        transform.localScale = Vector3.zero;

        Sequence jumpSeq = DOTween.Sequence();
        jumpSeq.Join(transform.DOMoveX(endPos.x, jumpTime).SetEase(Ease.InOutSine));
        jumpSeq.Join(transform.DOMoveY(endPos.y, jumpTime).SetEase(Ease.InOutSine));
        jumpSeq.Join(transform.DOScale(maxScale, jumpTime).SetEase(Ease.OutBack, _overshootScale));

        await jumpSeq.AsyncWaitForCompletion();

        transform.localScale = Vector3.one * maxScale;

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f));

        Vector3 flyStart = transform.position;
        Vector3 flyEnd = Camera.main.ScreenToWorldPoint(Target.position);
        await FlyParabola(flyStart, flyEnd, 1f, 0.5f);

        UIHud.Instance.ShopsPanel.ScalesOpenButton.PlayAddedAnimation();
        Destroy(gameObject);
    }

    public async UniTask PlayAnimAndDestroyFan(int index, int total, Vector3 fanCenter, float fanAngle = 60f) {
        float jumpTime = 0.3f + index * 0.1f;
        float startScale = 1f;
        float maxScale = 2.5f;

        float halfAngle = fanAngle * 0.5f;
        float angleStep = fanAngle / Mathf.Max(total - 1, 1);
        float angle = -halfAngle + index * angleStep;
        if (total == 1) {
            angle = 0;
        }

        gameObject.SetActive(true);
        Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

        Vector3 startPos = transform.position + Vector3.up * Random.Range(-0.1f, 0.1f);
        Vector3 endPos = fanCenter + dir * _upShift;
        transform.localScale = Vector3.one * startScale;

        Sequence jumpSeq = DOTween.Sequence();
        jumpSeq.Join(transform.DOMove(endPos, jumpTime).SetEase(Ease.InOutSine));
        jumpSeq.Join(transform.DOScale(maxScale, jumpTime).SetEase(Ease.OutBack, _overshootScale));

        await jumpSeq.AsyncWaitForCompletion();

        transform.localScale = Vector3.one * maxScale;

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.15f));
        if (this is FlyingCropFx crop) {
            Sequence changeSeq = DOTween.Sequence();
            changeSeq.Join(transform.DORotate(Vector3.up * 90, 0.2f/2));
            changeSeq.AppendCallback(() => {
                transform.localScale = Vector3.one;
                crop.SetFinalSpriteFromCache();
            });
            changeSeq.Append(transform.DORotate(Vector3.zero, 0.2f/2));
            await changeSeq.AsyncWaitForCompletion();
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.15f));
        Vector3 flyStart = transform.position;
        Vector3 flyEnd = Camera.main.ScreenToWorldPoint(Target.position);
        await FlyParabola(flyStart, flyEnd, 1f, 1f);

        UIHud.Instance.ShopsPanel.ScalesOpenButton.PlayAddedAnimation();
        Destroy(gameObject);
    }

    private async UniTask FlyParabola(Vector3 startPos, Vector3 endPos, float height, float flyTime) {
        float time = 0f;

        await DOTween.To(() => time, x => {
            time = x;
            float t = time / flyTime;

            Vector3 linearPos = Vector3.Lerp(startPos, endPos, t);
            float arc = 4 * height * t * (1 - t);
            linearPos.y += arc;

            transform.position = linearPos;
        }, flyTime, flyTime).AsyncWaitForCompletion();

        transform.position = endPos;
    }
}