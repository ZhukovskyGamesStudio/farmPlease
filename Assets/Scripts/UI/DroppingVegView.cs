using System;
using System.Collections;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class DroppingVegView : MonoBehaviour {
        public float blowImpulse;
        [SerializeField]private Rigidbody2D _rb;
        [SerializeField]
        private Image _image;
        private Action _onTouch;
        public Crop CropType{ get; private set; }
        public void Init(Crop crop, Action onTouch) {
            _onTouch = onTouch;
            gameObject.SetActive(true);
            CropType = crop;
            _image.sprite = CropsTable.CropByType(crop).VegSprite;
        }
        
        public void OnTapped() {
            return;
            StartCoroutine(KaBoom());
            GetComponent<Button>().interactable = false;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            _onTouch?.Invoke();
            _onTouch = null;
        }

        public void ExplodeInRndTime() {
            StartCoroutine(KaBoom(Random.Range(-0.2f,0.2f)));
        }
        

        private IEnumerator KaBoom(float rndShift = 0) {
            float scale = 1;
            while (scale < 1.4f + rndShift) {
                scale *= 1.05f;

                gameObject.transform.localScale = Vector3.one * scale;
                yield return new WaitForFixedUpdate();
            }

            gameObject.GetComponent<Image>().enabled = false;
            gameObject.transform.localScale = Vector3.one * scale * 5;
            _rb.AddForce(Vector2.up * blowImpulse * _rb.mass, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
        }

        private void OnDestroy() {
            _onTouch = null;
        }
    }
}