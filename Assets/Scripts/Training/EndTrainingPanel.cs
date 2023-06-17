using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EndTrainingPanel : MonoBehaviour {
    public Text CropsCollectedText;
    public Text SessionTimeText;
    public InputField NameInputField;
    public GameObject InputPanel;

    public GameObject RecordPrefab;
    public Transform RecordsGrid;
    public GameObject VegPrefab;
    public GameObject VegHolder;
    public RectTransform Lcorner, Rcorner;
    public float timeSpanBetweenCropdrop;
    private int curCropsCollected;
    private string curName;
    private int curSessionTime;

    private List<Record> records;

    public void ShowEndPanel(int cropsCollected, int sessionTime, Queue<CropsType> cropsQueue) {
        gameObject.SetActive(true);

        curSessionTime = sessionTime;
        curCropsCollected = cropsCollected;
        CropsCollectedText.text = "Вы вырастили " + cropsCollected + " овощей";
        SessionTimeText.text = "Вы играли " + TimeToString(sessionTime);
        StartCoroutine(DropCrops(cropsQueue));

        NameInputField.Select();
        LoadRecords();
        UpdateRecordsGrid();
    }

    public IEnumerator DropCrops(Queue<CropsType> cropsQueue) {
        while (cropsQueue.Count > 0) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            Image a = Instantiate(VegPrefab, pose, quat, VegHolder.transform).GetComponent<Image>();
            a.gameObject.SetActive(true);

            CropsType crop = cropsQueue.Dequeue();
            a.sprite = CropsTable.CropByType(crop).VegSprite;

            yield return new WaitForSeconds(timeSpanBetweenCropdrop);
        }

        yield return false;
    }

    public void EndOfNameInput() {
        if (NameInputField.text.Length > 0) {
            curName = NameInputField.text;
            NameInputField.interactable = false;
            InputPanel.SetActive(false); // Анимация туть

            records.Add(new Record(curCropsCollected, curName,
                DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year, curSessionTime));

            UpdateRecordsGrid();
            SaveRecords();
            SaveLoadManager.ClearSave(); // стирает текущее сохранение фермы        
        }
    }

    public void UpdateRecordsGrid() {
        foreach (Transform child in RecordsGrid.transform) Destroy(child.gameObject);

        records = records.OrderByDescending(r => r.cropsCollected).ToList();
        for (int i = 0; i < records.Count; i++) {
            RecordPrefab record = Instantiate(RecordPrefab, RecordsGrid).GetComponent<RecordPrefab>();
            record.cropsText.text = "Овощи: " + records[i].cropsCollected;
            record.nameText.text = records[i].name;
            record.dateText.text = "Дата: " + records[i].dateTime + " " + TimeToString(records[i].sessionTime);
            record.number = i;
            record.deleteButton.onClick.AddListener(() => DeleteRecord(record.number));
        }
    }

    public void LoadRecords() {
        records = new List<Record>();
        int recordsAmount = PlayerPrefs.GetInt("recordsAmount", 0);
        for (int i = 0; i < recordsAmount; i++) records.Add(Record.LoadRecord(i.ToString()));
    }

    public void DeleteRecord(int index) {
        //Debug.Log(records.Count + "     index " + index);
        records.RemoveAt(index);
        SaveRecords();
        UpdateRecordsGrid();
    }

    public void SaveRecords() {
        PlayerPrefs.SetInt("recordsAmount", records.Count);
        for (int i = 0; i < records.Count; i++) Record.SaveRecord(i.ToString(), records[i]);
    }

    private string TimeToString(int number) {
        string res = "";
        if (number / 60 < 10)
            res += "0";
        res += number / 60 + ":";
        if (number % 60 < 10)
            res += "0";
        res += number % 60;
        return res;
    }
}

public class Record {
    public int cropsCollected;
    public string dateTime;
    public string name;
    public int sessionTime;

    public Record(int _cropsCollected, string _Name, string _date, int _sessionTime) {
        cropsCollected = _cropsCollected;
        name = _Name;
        dateTime = _date;
        sessionTime = _sessionTime;
    }

    public static Record LoadRecord(string hash) {
        return new Record(
            PlayerPrefs.GetInt(hash + "cropsCollected"),
            PlayerPrefs.GetString(hash + "name"),
            PlayerPrefs.GetString(hash + "dateTime"),
            PlayerPrefs.GetInt(hash + "sessionTime")
        );
    }

    public static void SaveRecord(string hash, Record toSave) {
        PlayerPrefs.SetInt(hash + "cropsCollected", toSave.cropsCollected);
        PlayerPrefs.SetString(hash + "dateTime", toSave.dateTime);
        PlayerPrefs.SetString(hash + "name", toSave.name);
        PlayerPrefs.SetInt(hash + "sessionTime", toSave.sessionTime);
    }
}