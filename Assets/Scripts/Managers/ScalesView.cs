using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Tables;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScalesView : MonoBehaviour {
    public Transform Lcorner, Rcorner, VegsHolder;
    public DroppingVegView VegPrefab;
    public GameObject VegPanel;
    public float impulseMultiplier;
    public float secondsWait;
    public float VerticalShift = 3f, VerticalShiftPerCrop = 0.05f;
    public int cropMax = 150;
    private int _cropCount;
    private Queue<Crop> _shownCropsQueue = new Queue<Crop>();

    [SerializeField]
    private int maxCropsAmountForScaleLimit = 150;

    [SerializeField]
    private Transform _cropPipeTarget, _pipeVegsContainer;

    [SerializeField]
    private Transform _movingPlatform, _pipe;

    [SerializeField]
    private float  _movingPlatformMin = -3.5f, _movingPlatformMax = -13.5f;

    [SerializeField]
    private Transform _scaleArrow;

    [SerializeField]
    private float scalesArrowRotationLimit = 55;

    private List<DroppingVegView> _vegViews = new List<DroppingVegView>();
    private Vector3 _pipeDeltaFromPlatform;

    public void Init() {
        _pipeDeltaFromPlatform = _pipe.transform.position - _movingPlatform.transform.position;
    }

    public void OnSelectedAmountChange(Crop crop, int diff) {
        if (diff < 0) {
            for (int i = 0; i < Mathf.Abs(diff); i++) {
                var obj = _vegViews.First(v => v.CropType == crop);
                Destroy(obj.gameObject);
                _vegViews.Remove(obj);
                OnVegTouchScale(_vegViews.Count);
            }
        } else {
            for (int i = 0; i < diff; i++) {
                InstantiateFallingVeg(crop, delegate { OnVegTouchScale(_vegViews.Count); });
            }
        }
    }

    private async UniTask CropRain(Queue<Crop> cropsQueue) {
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
            await UniTask.Delay(TimeSpan.FromSeconds(secondsWait * (cropsQueue.Count * 1f / startingAmount)));
        }
    }

    private void DestroyExcessVeg() {
        Destroy(VegsHolder.GetChild(0).gameObject);
    }

    private void InstantiateFallingVeg(Crop crop, Action onTouch = null) {
        Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
        pose += Vector3.up * (VerticalShift + VerticalShiftPerCrop * _vegViews.Count);
        pose.z = 0; // Ensure the z position is zero for 2D
        Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

        DroppingVegView fallingVeg = Instantiate(VegPrefab, pose, quat, VegsHolder);
        fallingVeg.Init(crop, onTouch);
        _vegViews.Add(fallingVeg);
        AddRandomForce(fallingVeg);
    }

    private void OnVegTouchScale(int vegsAmount) {
        float percent = Mathf.Clamp((vegsAmount * 1f) / maxCropsAmountForScaleLimit, 0, 1);
        ChangeArrowPos(percent);
        ChangePlatformPos(percent);
    }

    private void ChangeArrowPos(float percent) {
        float degree = Mathf.Lerp(scalesArrowRotationLimit, -scalesArrowRotationLimit, percent);
        _scaleArrow.rotation = Quaternion.Euler(0, 0, degree);
    }

    private void ChangePlatformPos(float percent) {
        float verticalMove = Mathf.Lerp(_movingPlatformMin, _movingPlatformMax, percent);
        Vector3 tmp = _movingPlatform.transform.localPosition;
        tmp.y = verticalMove;
        _movingPlatform.transform.localPosition = tmp;
        _pipe.transform.position = _movingPlatform.transform.position + _pipeDeltaFromPlatform;
    }

    private void AddRandomForce(DroppingVegView fallingVeg) {
        Vector2 rndForce = new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier;
        fallingVeg.gameObject.GetComponent<Rigidbody2D>().AddForce(rndForce, ForceMode2D.Impulse);
    }

    public IEnumerator SellCrops(List<Crop> crops) {
        yield return StartCoroutine(SellCoroutine(crops));
        List<Crop> tmp = new List<Crop>(_shownCropsQueue);
        foreach (var crop in crops) {
            tmp.Remove(crop);
        }

        _shownCropsQueue = new Queue<Crop>(tmp);
    }

    private IEnumerator SellCoroutine(List<Crop> crops) {
        float deltaTime = 0.3f;
        for (int index = 0; index < crops.Count; index++) {
            Crop crop = crops[index];
            DroppingVegView veg = _vegViews.Where(c => c.CropType == crop)
                .OrderBy(v => Vector3.SqrMagnitude(_cropPipeTarget.position - v.transform.position)).FirstOrDefault();
            if (veg != null) {
                StartCoroutine(SellVeg(veg, deltaTime));
                _vegViews.Remove(veg);
                OnVegTouchScale(_vegViews.Count);
            }

            yield return new WaitForSeconds(deltaTime);
            deltaTime *= 0.75f;
            if (deltaTime < 0.03f) {
                deltaTime = 0.03f;
            }
        }

        yield return new WaitForSeconds(0.4f);
    }

    private IEnumerator SellVeg(DroppingVegView veg, float deltaTime) {
        Destroy(veg.gameObject, deltaTime + 0.4f);
        yield return StartCoroutine(veg.MoveTo(_cropPipeTarget.transform.position, deltaTime));
        veg.transform.SetParent(_pipeVegsContainer);
        veg.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        yield return new WaitForSeconds(0.4f);
    }
}