using System;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers {
    public class ScalesView : MonoBehaviour {
        public Transform Lcorner, Rcorner, VegsHolder;
        public DroppingVegView VegPrefab;
        public GameObject VegPanel;
        public float impulseMultiplier;
        public float secondsWait;
        public float VerticalShift = 3f;
        public int cropMax = 150;
        private int _cropCount;
        private Coroutine _rainCoroutine;
        private Queue<Crop> _shownCropsQueue = new Queue<Crop>();
        [SerializeField]
        private int maxCropsAmountForScaleLimit = 150;
        [SerializeField] private Text _sellForText;
        [SerializeField] private Button _sellButton;
        
        [SerializeField] private Transform _movingPlatform;
        [SerializeField] private float movingPlatformVerticalLimit = 10;
        [SerializeField] private Transform _scaleArrow;
        [SerializeField] private float scalesArrowRotationLimit = 55;
        public void StartRainingCrops(Queue<Crop> cropsQueue) {
            cropsQueue ??= new Queue<Crop>();
            _rainCoroutine = StartCoroutine(CropRain(cropsQueue));
        }

        private IEnumerator CropRain(Queue<Crop> cropsQueue) {
            UpdateButtonInteractable();
            Queue<Crop> tmpShown = new Queue<Crop>(_shownCropsQueue);
            _cropCount = 0;
            int startingAmount = cropsQueue.Count;
            VegPanel.SetActive(true);
            while (cropsQueue.Count > 0) {
                if (tmpShown.Count > 0 && tmpShown.Peek() == cropsQueue.Peek()) {
                    tmpShown.Dequeue();
                    cropsQueue.Dequeue();
                    continue;
                }

                if (_cropCount > cropMax) {
                    DestroyExcessVeg();
                }

                Crop crop = cropsQueue.Dequeue();
                _shownCropsQueue.Enqueue(crop);

                InstantiateFallingVeg(crop, delegate { OnVegTouchScale(_shownCropsQueue.Count); });

                _cropCount++;
                yield return new WaitForSeconds(secondsWait * (cropsQueue.Count * 1f / startingAmount));
            }

            UpdateButtonInteractable();
        }

        private void DestroyExcessVeg() {
            Destroy(VegsHolder.GetChild(0).gameObject);
        }

        private void InstantiateFallingVeg(Crop crop, Action onTouch = null) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            pose += Vector3.up * VerticalShift;
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            DroppingVegView fallingVeg = Instantiate(VegPrefab, pose, quat, VegsHolder);
            fallingVeg.Init(crop, onTouch);
            AddRandomForce(fallingVeg);
        }

        private void OnVegTouchScale(int vegsAmount) {
            float percent = Mathf.Clamp((vegsAmount * 1f) / maxCropsAmountForScaleLimit,0,1);
            float degree = Mathf.Lerp(scalesArrowRotationLimit, -scalesArrowRotationLimit, percent);
            _scaleArrow.rotation = Quaternion.Euler(0,0,degree);
            
            float verticalMove = Mathf.Lerp(0, -movingPlatformVerticalLimit, percent);
            Vector3 tmp = _movingPlatform.transform.localPosition;
            tmp.y = verticalMove;
            _movingPlatform.transform.localPosition = tmp;
            UpdateButtonText(vegsAmount);
        }

        private void UpdateButtonText(int vegsAmount) {
            if (vegsAmount == 0) {
                _sellForText.text = "Нечего продавать";
            } else {
                _sellForText.text = "Продать за " + vegsAmount;
            }
        }

        private void UpdateButtonInteractable() {
            _sellButton.interactable = _shownCropsQueue.Count>0;
        }

        private void AddRandomForce(DroppingVegView fallingVeg) {
            Vector2 rndForce = new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier;
            fallingVeg.gameObject.GetComponent<Rigidbody2D>().AddForce(rndForce, ForceMode2D.Impulse);
        }

        public IEnumerator SellAllCrops() {
            if (_rainCoroutine != null) {
                StopCoroutine(_rainCoroutine);
            }

            yield return StartCoroutine(SellCoroutine());

            _shownCropsQueue = new Queue<Crop>();
            UpdateButtonText(0);
            UpdateButtonInteractable();
        }

        private IEnumerator SellCoroutine() {
            foreach (var veg in VegsHolder.GetComponentsInChildren<DroppingVegView>()) {
                veg.ExplodeInRndTime();
            }
            yield return new WaitForSeconds(1);
        }
    }
}