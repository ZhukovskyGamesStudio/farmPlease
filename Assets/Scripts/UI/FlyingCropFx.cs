using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
            time = 0;
            startPos = transform.position;
            endPos = UIHud.Instance.Backpack.transform.position;
            float flyTime = 1;
            while (time < flyTime) {
                transform.position = Vector3.Lerp(startPos, endPos, time / flyTime);
                time += Time.deltaTime * 3;
                yield return new WaitForEndOfFrame();
            }

            transform.position = endPos;
            UIHud.Instance.Backpack.ShowAddedAnimation();
            Destroy(gameObject);
        }
    }
}