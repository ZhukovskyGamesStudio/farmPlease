using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Training
{
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
        private int _curCropsCollected;
        private string _curName;
        private int _curSessionTime;

        private List<Record> _records;

        public void ShowEndPanel(int cropsCollected, int sessionTime, Queue<Crop> cropsQueue) {
            gameObject.SetActive(true);

            _curSessionTime = sessionTime;
            _curCropsCollected = cropsCollected;
            CropsCollectedText.text = "Вы вырастили " + cropsCollected + " овощей";
            SessionTimeText.text = "Вы играли " + TimeToString(sessionTime);
            StartCoroutine(DropCrops(cropsQueue));

            NameInputField.Select();
            LoadRecords();
            UpdateRecordsGrid();
        }

        public IEnumerator DropCrops(Queue<Crop> cropsQueue) {
            while (cropsQueue.Count > 0) {
                Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
                Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

                Image a = Instantiate(VegPrefab, pose, quat, VegHolder.transform).GetComponent<Image>();
                a.gameObject.SetActive(true);

                Crop crop = cropsQueue.Dequeue();
                a.sprite = CropsTable.CropByType(crop).VegSprite;

                yield return new WaitForSeconds(timeSpanBetweenCropdrop);
            }

            yield return false;
        }

        public void EndOfNameInput() {
            if (NameInputField.text.Length > 0) {
                _curName = NameInputField.text;
                NameInputField.interactable = false;
                InputPanel.SetActive(false); // Анимация туть

                _records.Add(new Record(_curCropsCollected, _curName,
                    DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year, _curSessionTime));

                UpdateRecordsGrid();
                SaveRecords();
                SaveLoadManager.ClearSave(); // стирает текущее сохранение фермы        
            }
        }

        public void UpdateRecordsGrid() {
            foreach (Transform child in RecordsGrid.transform) Destroy(child.gameObject);

            _records = _records.OrderByDescending(r => r.CropsCollected).ToList();
            for (int i = 0; i < _records.Count; i++) {
                RecordPrefab record = Instantiate(RecordPrefab, RecordsGrid).GetComponent<RecordPrefab>();
                record.cropsText.text = "Овощи: " + _records[i].CropsCollected;
                record.nameText.text = _records[i].Name;
                record.dateText.text = "Дата: " + _records[i].DateTime + " " + TimeToString(_records[i].SessionTime);
                record.number = i;
                record.deleteButton.onClick.AddListener(() => DeleteRecord(record.number));
            }
        }

        public void LoadRecords() {
            _records = new List<Record>();
            int recordsAmount = PlayerPrefs.GetInt("recordsAmount", 0);
            for (int i = 0; i < recordsAmount; i++) _records.Add(Record.LoadRecord(i.ToString()));
        }

        public void DeleteRecord(int index) {
            //Debug.Log(records.Count + "     index " + index);
            _records.RemoveAt(index);
            SaveRecords();
            UpdateRecordsGrid();
        }

        public void SaveRecords() {
            PlayerPrefs.SetInt("recordsAmount", _records.Count);
            for (int i = 0; i < _records.Count; i++) Record.SaveRecord(i.ToString(), _records[i]);
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
        public int CropsCollected;
        public string DateTime;
        public string Name;
        public int SessionTime;

        public Record(int cropsCollected, string name, string date, int sessionTime) {
            CropsCollected = cropsCollected;
            Name = name;
            DateTime = date;
            SessionTime = sessionTime;
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
            PlayerPrefs.SetInt(hash + "cropsCollected", toSave.CropsCollected);
            PlayerPrefs.SetString(hash + "dateTime", toSave.DateTime);
            PlayerPrefs.SetString(hash + "name", toSave.Name);
            PlayerPrefs.SetInt(hash + "sessionTime", toSave.SessionTime);
        }
    }
}