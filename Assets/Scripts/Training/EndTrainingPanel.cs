using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EndTrainingPanel : MonoBehaviour
{

    public Text CropsCollectedText;
    public Text SessionTimeText;
    public InputField NameInputField;
    public GameObject InputPanel;
    int curCropsCollected;
    string curName;
    int curSessionTime;

    public GameObject RecordPrefab;
    public Transform RecordsGrid;

    List<Record> records;
    public GameObject VegPrefab;
    public GameObject VegHolder;
    public RectTransform Lcorner, Rcorner;
    public float timeSpanBetweenCropdrop;

    public void ShowEndPanel(int cropsCollected, int sessionTime, Queue<CropsType> cropsQueue)
    {
        gameObject.SetActive(true);

        curSessionTime = sessionTime;
        curCropsCollected = cropsCollected;
        CropsCollectedText.text = "Вы вырастили " + cropsCollected.ToString() + " овощей";
        SessionTimeText.text = "Вы играли " + TimeToString(sessionTime);
        StartCoroutine(DropCrops(cropsQueue));

        NameInputField.Select();
        LoadRecords();
        UpdateRecordsGrid();
    }

    public IEnumerator DropCrops(Queue<CropsType> cropsQueue)
    {
        while (cropsQueue.Count > 0)
        {

            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * UnityEngine.Random.Range(0.05f, 0.95f);
            Quaternion quat = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.forward);

            Image a = Instantiate(VegPrefab, pose, quat, VegHolder.transform).GetComponent<Image>();
            a.gameObject.SetActive(true);

            CropsType crop = cropsQueue.Dequeue();
            a.sprite = CropsTable.CropByType(crop).VegSprite;

            yield return new WaitForSeconds(timeSpanBetweenCropdrop);
        }

        yield return false;
    }

    public void EndOfNameInput()
    {
        if (NameInputField.text.Length > 0)
        {
            curName = NameInputField.text;
            NameInputField.interactable = false;
            InputPanel.SetActive(false); // Анимация туть

            records.Add(new Record(curCropsCollected, curName, DateTime.Today.Day.ToString() + "." + DateTime.Today.Month.ToString() + "." + DateTime.Today.Year.ToString(), curSessionTime));

            UpdateRecordsGrid();
            SaveRecords();
            SaveLoadManager.instance.ClearSave(); // стирает текущее сохранение фермы        
        }

    }

    public void UpdateRecordsGrid()
    {
        foreach (Transform child in RecordsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        records = records.OrderByDescending(r => r.cropsCollected).ToList();
        for (int i = 0; i < records.Count; i++)
        {
            RecordPrefab record = Instantiate(RecordPrefab, RecordsGrid).GetComponent<RecordPrefab>();
            record.cropsText.text = "Овощи: " + records[i].cropsCollected.ToString();
            record.nameText.text = records[i].name;
            record.dateText.text = "Дата: " + records[i].dateTime + " " + TimeToString(records[i].sessionTime);
            record.number = i;
            record.deleteButton.onClick.AddListener(() => DeleteRecord(record.number));
        }
    }

    public void LoadRecords()
    {
        records = new List<Record>();
        int recordsAmount = PlayerPrefs.GetInt("recordsAmount", 0);
        for (int i = 0; i < recordsAmount; i++)
        {
            records.Add(Record.LoadRecord(i.ToString()));
        }

    }

    public void DeleteRecord(int index)
    {
        //Debug.Log(records.Count + "     index " + index);
        records.RemoveAt(index);
        SaveRecords();
        UpdateRecordsGrid();
    }

    public void SaveRecords()
    {
        PlayerPrefs.SetInt("recordsAmount", records.Count);
        for (int i = 0; i < records.Count; i++)
        {
            Record.SaveRecord(i.ToString(), records[i]);
        }
    }

    string TimeToString(int number)
    {
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

public class Record
{
    public string name;
    public int cropsCollected;
    public string dateTime;
    public int sessionTime;

    public Record(int _cropsCollected, string _Name, string _date, int _sessionTime)
    {
        cropsCollected = _cropsCollected;
        name = _Name;
        dateTime = _date;
        sessionTime = _sessionTime;
    }


    public static Record LoadRecord(string hash)
    {
        return new Record(
            PlayerPrefs.GetInt(hash + "cropsCollected"),
            PlayerPrefs.GetString(hash + "name"),
            PlayerPrefs.GetString(hash + "dateTime"),
                PlayerPrefs.GetInt(hash + "sessionTime")
            );
    }

    public static void SaveRecord(string hash, Record toSave)
    {

        PlayerPrefs.SetInt(hash + "cropsCollected", toSave.cropsCollected);
        PlayerPrefs.SetString(hash + "dateTime", toSave.dateTime);
        PlayerPrefs.SetString(hash + "name", toSave.name);
        PlayerPrefs.SetInt(hash + "sessionTime", toSave.sessionTime);
    }

}
