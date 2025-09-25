using System.Collections;
using UI;
using UnityEngine;

public class FlyingXpFx : FlyingItemFx {
    public void Init(Vector3 parentPosition) {
        transform.position = parentPosition;
        StartCoroutine(PlayOrbAnimAndDestroy(Target));
    }
    
    protected IEnumerator PlayOrbAnimAndDestroy(Transform target) {
        // случайное небольшое смещение при старте
        float rndX = Random.Range(-0.7f, 0.7f);
        float rndY = Random.Range(0.6f, 0.9f);
        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + new Vector3(rndX, rndY, 0f);

        float appearTime = Random.Range(0.2f, 0.4f); // время до «зависания»
        float elapsed = 0f;
        float maxScale = transform.localScale.x;

        // появление и подлет
        transform.localScale = Vector3.zero;
        while (elapsed < appearTime) {
            float t = elapsed / appearTime;
            transform.position = Vector3.Lerp(startPos, peakPos, t);
            float scale = Mathf.SmoothStep(0f, maxScale, t);
            transform.localScale = new Vector3(scale, scale, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = peakPos;
        transform.localScale = Vector3.one * maxScale;

        // маленькая пауза в воздухе
        yield return new WaitForSeconds(0.2f);

        // полёт к цели
        Vector3 endPos = Camera.main.ScreenToWorldPoint(target.position);
        float flyTime = 0.5f;
        elapsed = 0f;

        while (elapsed < flyTime) {
            float t = elapsed / flyTime;

            // линейная интерполяция + небольшая парабола (подъём/спуск)
            Vector3 linearPos = Vector3.Lerp(peakPos, endPos, t);
            float arc = 0.25f * Mathf.Sin(t * Mathf.PI); // небольшая арка
            linearPos.y += arc;

            transform.position = linearPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // можно добавить лёгкое исчезновение или анимацию цели
        Destroy(gameObject);
    }

    protected override RectTransform Target => UIHud.Instance.ProfileView.XpProgressBar.GetComponent<RectTransform>();
}