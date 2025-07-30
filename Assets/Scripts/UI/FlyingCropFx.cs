using System.Collections;
using UnityEngine;

namespace UI {
    public class FlyingCropFx : MonoBehaviour {
        [SerializeField]
        private Animation _fxAnimation;

        [SerializeField]
        private string _animationName;

        [SerializeField]
        private SpriteRenderer _image;

        public void Init(Sprite icon, Vector3 parentPosition) {
            _image.sprite = icon;
            transform.position = parentPosition;
            StartCoroutine(PlayAnimAndDestroy());
        }

        private IEnumerator PlayAnimAndDestroy() {
            float rndSizeForce = Random.Range(0.1f, 1.5f);
            if (Random.Range(0, 2) == 0) {
                rndSizeForce *= -1;
            }

            float time = 0;
            float jumpTime = 1f;
            Vector3 startPos = transform.position + Vector3.up * Random.Range(-0.1f, 0.1f);
            Vector3 endPos = startPos + Vector3.right * rndSizeForce + Vector3.up * Mathf.Abs(rndSizeForce);
            transform.localScale = Vector3.zero;
            while (time < jumpTime) {
                Vector3 nextPos = Vector3.zero;
                nextPos.x = Mathf.Lerp(startPos.x, endPos.x, (1 + Mathf.Sin(-Mathf.PI / 2 + Mathf.PI * time / jumpTime)) / 2);
                nextPos.y = Mathf.Lerp(startPos.y, endPos.y, Mathf.Sin(Mathf.PI * time / jumpTime));
                transform.position = nextPos;
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time * 2 / jumpTime);
                time += Time.deltaTime * 3;
                yield return new WaitForEndOfFrame();
            }

            transform.localScale = Vector3.one;
            //transform.position = endPos;

            yield return new WaitForSeconds(0.5f);
            startPos = transform.position;
            endPos = Camera.main.ScreenToWorldPoint(UIHud.Instance.ShopsPanel.ScalesButton.GetComponent<RectTransform>().position);
            yield return StartCoroutine(FlyParabola(startPos, endPos, 1f, 0.5f));
            UIHud.Instance.ShopsPanel.ScalesOpenButton.PlayAddedAnimation();
            Destroy(gameObject);
        }
        
        IEnumerator FlyParabola(Vector3 startPos, Vector3 endPos, float height, float flyTime) {
            float time = 0f;

            while (time < flyTime) {
                float t = time / flyTime;

                // Горизонтальная интерполяция
                Vector3 linearPos = Vector3.Lerp(startPos, endPos, t);

                // Добавка "навеса" по оси Y
                float arc = 4 * height * t * (1 - t); // Парабола: 4h * t * (1 - t)

                // Вносим "навес" только в высоту
                linearPos.y += arc;

                transform.position = linearPos;

                time += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
        }
    }
}