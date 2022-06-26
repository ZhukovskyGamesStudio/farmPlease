using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CropRainManager : MonoBehaviour {
    public Toggle LogoToggle;
    public Transform Lcorner, Rcorner, VegsHolder;
    public GameObject VegPrefab, VegPanel;
    public float impulseMultiplier;
    public float secondsWait;
    public int cropMax = 150;
    private int cropCount;

    private void OnEnable() {
        StartCoroutine(CropRain());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    /*
    public void ToggledCropRain(bool isOn) {
        if (isOn) {
            StartCoroutine(CropRain());
        } else {
            EndCropRain();
            StopAllCoroutines();
        }
    }*/

    private IEnumerator CropRain() {
        cropCount = 0;
        VegPanel.SetActive(true);
        while (true) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            if (cropCount > cropMax) Destroy(VegsHolder.GetChild(0).gameObject);
            Image a = Instantiate(VegPrefab, pose, quat, VegsHolder).GetComponent<Image>();
            a.gameObject.SetActive(true);
            a.gameObject.GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier, ForceMode2D.Impulse);

            CropsType crop = CropsTable.instance.Crops[Random.Range(0, CropsTable.instance.Crops.Length)].type;
            a.sprite = CropsTable.CropByType(crop).VegSprite;

            cropCount++;
            yield return new WaitForSeconds(secondsWait);
        }
    }

    private void EndCropRain() {
        VegPanel.SetActive(false);
        GameObject[] objs = new GameObject[VegsHolder.childCount];
        for (int i = 0; i < objs.Length; i++) Destroy(VegsHolder.GetChild(i).gameObject, 8);
    }
}