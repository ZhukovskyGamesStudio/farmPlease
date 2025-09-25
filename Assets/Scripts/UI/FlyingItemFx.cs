using System.Collections;
using UI;
using UnityEngine;

public abstract class FlyingItemFx : MonoBehaviour {
    [Header("Jump Settings")]
    [SerializeField]
    private float _jumpTimeMin = 0.8f;

    [SerializeField]
    private float _jumpTimeMax = 1.2f;

    [SerializeField]
    private float _overshootScale = 1.2f;

    protected abstract RectTransform Target { get; }

    protected IEnumerator PlayAnimAndDestroy() {
        float rndSizeForce = Random.Range(0.1f, 1.5f);
        if (Random.Range(0, 2) == 0) {
            rndSizeForce *= -1;
        }

        float time = 0;
        float jumpTime = Random.Range(_jumpTimeMin, _jumpTimeMax);
        Vector3 startPos = transform.position + Vector3.up * Random.Range(-0.1f, 0.1f);
        Vector3 endPos = startPos + Vector3.right * rndSizeForce + Vector3.up * Mathf.Abs(rndSizeForce);

        float maxScale = transform.localScale.x;
        
        
        transform.localScale = Vector3.zero;
        while (time < jumpTime) {
            float t = time / jumpTime;

            Vector3 nextPos = Vector3.zero;
            nextPos.x = Mathf.Lerp(startPos.x, endPos.x, (1 + Mathf.Sin(-Mathf.PI / 2 + Mathf.PI * t)) / 2);
            nextPos.y = Mathf.Lerp(startPos.y, endPos.y, Mathf.Sin(Mathf.PI * t));
            transform.position = nextPos;

            float scaleT = Mathf.SmoothStep(0f, maxScale, t);
            float overshoot = Mathf.Sin(t * Mathf.PI) * (_overshootScale - 1f);
            float scale = Mathf.Clamp01(scaleT + overshoot);
            transform.localScale = new Vector3(scale , scale, 1);

            time += Time.deltaTime * 3;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one * maxScale;

        yield return new WaitForSeconds(0.5f);
        startPos = transform.position;
        endPos = Camera.main.ScreenToWorldPoint(Target.position);
        yield return StartCoroutine(FlyParabola(startPos, endPos, 1f, 0.5f));
        UIHud.Instance.ShopsPanel.ScalesOpenButton.PlayAddedAnimation();
        Destroy(gameObject);
    }

    protected IEnumerator FlyParabola(Vector3 startPos, Vector3 endPos, float height, float flyTime) {
        float time = 0f;

        while (time < flyTime) {
            float t = time / flyTime;

            Vector3 linearPos = Vector3.Lerp(startPos, endPos, t);
            float arc = 4 * height * t * (1 - t);
            linearPos.y += arc;

            transform.position = linearPos;

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}