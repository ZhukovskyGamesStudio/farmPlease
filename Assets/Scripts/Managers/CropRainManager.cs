using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers {
    public class CropRainManager : MonoBehaviour {
        public Transform Lcorner, Rcorner, VegsHolder;
        public GameObject VegPrefab, VegPanel;
        public float impulseMultiplier;
        public float secondsWait;
        public float VerticalShift = 3f;
        public int cropMax = 150;
        private int _cropCount;
        private Coroutine _rainCoroutine;
        private Queue<Crop> _shownCropsQueue = new Queue<Crop>();

        public void StartRainingCrops(Queue<Crop> cropsQueue) {
            cropsQueue ??= new Queue<Crop>();
            _rainCoroutine = StartCoroutine(CropRain(cropsQueue));
        }

        private IEnumerator CropRain(Queue<Crop> cropsQueue) {
            Queue<Crop> tmpShown = new Queue<Crop>(_shownCropsQueue);
            _cropCount = 0;
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

                InstantiateFallingVeg(crop);

                _cropCount++;
                yield return new WaitForSeconds(secondsWait);
            }
        }

        private void DestroyExcessVeg() {
            Destroy(VegsHolder.GetChild(0).gameObject);
        }

        private void InstantiateFallingVeg(Crop crop) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            pose += Vector3.up * VerticalShift;
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            Image fallingVeg = Instantiate(VegPrefab, pose, quat, VegsHolder).GetComponent<Image>();
            fallingVeg.gameObject.SetActive(true);

            AddRandomForce(fallingVeg);

            fallingVeg.sprite = CropsTable.CropByType(crop).VegSprite;
        }

        private void AddRandomForce(Image fallingVeg) {
            Vector2 rndForce = new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier;
            fallingVeg.gameObject.GetComponent<Rigidbody2D>().AddForce(rndForce, ForceMode2D.Impulse);
        }

        public void EndCropRain() {
            if (_rainCoroutine != null) {
                StopCoroutine(_rainCoroutine);
            }

            GameObject[] objs = new GameObject[VegsHolder.childCount];
            for (int i = 0; i < objs.Length; i++) {
                Destroy(VegsHolder.GetChild(i).gameObject);
            }

            _shownCropsQueue = new Queue<Crop>();
        }
    }
}