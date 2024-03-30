using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScalesView : MonoBehaviour{
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

    [SerializeField]
    private Transform _cropPipeTarget,_pipeVegsContainer;
    [SerializeField]
    private Transform _movingPlatform, _pipe;

    [SerializeField]
    private float movingPlatformVerticalLimit = 10;

    [SerializeField]
    private Transform _scaleArrow;

    [SerializeField]
    private float scalesArrowRotationLimit = 55;

    private List<DroppingVegView> _vegViews;
    private Vector3 _pipeDeltaFromPlatform;

    private void Awake() {
        if (_pipe != null) {
            _pipeDeltaFromPlatform = _pipe.transform.position - _movingPlatform.transform.position;
        }
    }

    public void StartRainingCrops(Queue<Crop> cropsQueue, Action onCropRainEnd){
        if (cropsQueue == null){
            cropsQueue = new Queue<Crop>();
        }

        _rainCoroutine = StartCoroutine(CropRain(cropsQueue,onCropRainEnd));
    }

    private IEnumerator CropRain(Queue<Crop> cropsQueue,Action onCropRainEnd){
        Queue<Crop> tmpShown = new Queue<Crop>(_shownCropsQueue);
        _cropCount = 0;
        int startingAmount = cropsQueue.Count;
        VegPanel.SetActive(true);
        while (cropsQueue.Count > 0){
            if (tmpShown.Count > 0 && tmpShown.Peek() == cropsQueue.Peek()){
                tmpShown.Dequeue();
                cropsQueue.Dequeue();
                continue;
            }

            if (_cropCount > cropMax){
                DestroyExcessVeg();
            }

            Crop crop = cropsQueue.Dequeue();
            _shownCropsQueue.Enqueue(crop);

            InstantiateFallingVeg(crop, delegate{ OnVegTouchScale(_shownCropsQueue.Count); });

            _cropCount++;
            yield return new WaitForSeconds(secondsWait * (cropsQueue.Count * 1f / startingAmount));
        }
        onCropRainEnd?.Invoke();
    }

    private void DestroyExcessVeg(){
        Destroy(VegsHolder.GetChild(0).gameObject);
    }

    private void InstantiateFallingVeg(Crop crop, Action onTouch = null){
        Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
        pose += Vector3.up * VerticalShift;
        Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

        DroppingVegView fallingVeg = Instantiate(VegPrefab, pose, quat, VegsHolder);
        fallingVeg.Init(crop, onTouch);
        AddRandomForce(fallingVeg);
    }

    private void OnVegTouchScale(int vegsAmount){
        float percent = Mathf.Clamp((vegsAmount * 1f) / maxCropsAmountForScaleLimit, 0, 1);
        float degree = Mathf.Lerp(scalesArrowRotationLimit, -scalesArrowRotationLimit, percent);
        _scaleArrow.rotation = Quaternion.Euler(0, 0, degree);

        float verticalMove = Mathf.Lerp(0, -movingPlatformVerticalLimit, percent);
        ChangePlatformPos(verticalMove);
    }

    private void ChangePlatformPos(   float verticalMove) {
        Vector3 tmp = _movingPlatform.transform.localPosition;
        tmp.y = verticalMove;
        _movingPlatform.transform.localPosition = tmp;
        _pipe.transform.position = _movingPlatform.transform.position + _pipeDeltaFromPlatform;
    }

    private void AddRandomForce(DroppingVegView fallingVeg){
        Vector2 rndForce = new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier;
        fallingVeg.gameObject.GetComponent<Rigidbody2D>().AddForce(rndForce, ForceMode2D.Impulse);
    }

    public IEnumerator SellCrops(List<Crop> crops){
        yield return StartCoroutine(SellCoroutine(crops));
        List<Crop> tmp = new List<Crop>(_shownCropsQueue);
        foreach (var crop in crops) {
            tmp.Remove(crop);
        }
        _shownCropsQueue = new Queue<Crop>(tmp);
    }

    private IEnumerator SellCoroutine(List<Crop> crops){
        List<DroppingVegView> vegs = VegsHolder.GetComponentsInChildren<DroppingVegView>().ToList();
        float deltaTime = 0.3f;
        for (int index = 0; index < crops.Count; index++) {
            Crop crop = crops[index];
            DroppingVegView veg = vegs.FirstOrDefault(v => v.CropType == crop);
            if (veg != null) {
                StartCoroutine(SellVeg(veg, deltaTime));
                vegs.Remove(veg);
                OnVegTouchScale(vegs.Count);
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
        Destroy(veg.gameObject,deltaTime + 0.4f );
        yield return StartCoroutine(veg.MoveTo(_cropPipeTarget.transform.position, deltaTime));
        veg.transform.SetParent(_pipeVegsContainer);
        veg.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        yield return new WaitForSeconds(0.4f);
    }
}