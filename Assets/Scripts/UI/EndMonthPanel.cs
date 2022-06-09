using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMonthPanel : MonoBehaviour {
    public Text MonthCropsCollectedText;
    public Text AllCropsCollectedText;

    public GameObject VegPrefab;
    public GameObject Walls;
    public RectTransform Lcorner, Rcorner;
    public CropsTable CropsTable;
    public float timeSpanBetweenCropdrop;

    public void ShowEndMonthPanel(Queue<CropsType> cropsQueue, int allCrops) {
        gameObject.SetActive(true);
        int cropsAmount = 0;
        if (cropsQueue != null)
            cropsAmount = cropsQueue.Count;

        MonthCropsCollectedText.text = "За этот месяц вы вырастили " + cropsAmount + " овощей";
        AllCropsCollectedText.text = "Всего  " + allCrops + " овощей";
        StartCoroutine(DropCrops(cropsQueue));
    }

    public void ContinueButton() {
        gameObject.SetActive(false);
    }

    public IEnumerator DropCrops(Queue<CropsType> cropsQueue) {
        Queue<CropsType> tmpQueue = new(cropsQueue);
        while (tmpQueue.Count > 0) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            Image a = Instantiate(VegPrefab, pose, quat, Walls.transform).GetComponent<Image>();

            CropsType crop = tmpQueue.Dequeue();
            a.sprite = CropsTable.CropByType(crop).VegSprite;

            yield return new WaitForSeconds(timeSpanBetweenCropdrop);
        }

        yield return false;
    }
}