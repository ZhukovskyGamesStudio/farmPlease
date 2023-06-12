using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = Managers.Debug;

namespace Database
{
    public class OnlineFarm : MonoBehaviour {
        public Text FarmIdText;

        public Text ChatText;
        public InputField ChatInput;
        public SmartTilemap smartTilemap;

        public Player curPlayer;
        public Farm curFarm;

        private bool _isPutting;

        private bool _needToUpdateFarm;

        private void Start() {
            curPlayer = DB.Instance.tmpPlayer;
            curFarm = DB.Instance.tmpFarm;

            if (curPlayer.Id == -1)
                UnityEngine.Debug.Log("Player is not loaded");
            ChatText.text = "";

            FarmIdText.text = "Ферма номер #" + curFarm.Id;

            StartCoroutine(FarmUpdater());
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && ChatInput.text != "") {
                ChatInput.text = "";
                ChatInput.DeactivateInputField();
            }
        }

        public IEnumerator FarmUpdater() {
            yield return StartCoroutine(
                DB.Instance.GetFarm(curFarm.Id, curFarm.Password, curPlayer, UpdateLocalFarm));
            for (;;) {
                yield return StartCoroutine(DB.Instance.GetDates(curFarm.Id, curFarm.Password, IsNeedToUpdate));

                if (_needToUpdateFarm && !_isPutting)
                    yield return StartCoroutine(DB.Instance.GetFarm(curFarm.Id, curFarm.Password, curPlayer,
                        UpdateLocalFarm));
                else
                    yield return new WaitForEndOfFrame();
            }
        }

        private void IsNeedToUpdate(Farm datesHolder) {
            _needToUpdateFarm = curFarm.FarmPostDate != datesHolder.FarmPostDate;
            curFarm.FarmPostDate = datesHolder.FarmPostDate;
        }

        private void UpdateLocalFarm(Farm newFarm) {
            _needToUpdateFarm = false;
            curFarm = newFarm;

            //ChatText.text +="\n" + curFarm.LastMessage;

            if (newFarm.JsonString == null || newFarm.JsonString == "") {
                SaveLoadManager.GenerateGame();
                Debug.Instance.Log("Posting generated online Farm");
                ChangeFarmAndPut(SaveLoadManager.GenerateJsonString());
            } else {
                SaveLoadManager.LoadGame(newFarm.JsonString);
                Debug.Instance.Log("Synchronizing local Farm");
            }
        }

        public void EndPutting(Farm farm) {
            _isPutting = false;
            PlayerController.CanInteract = true;
        }

        public void PutChatMessage() {
            /* 
         * if (ChatInput.text == "")
            return;
        string message = "[" + curPlayer.Name.TrimEnd() + "]: " + ChatInput.text;
        curFarm.LastMessage = message;
        StartCoroutine(DBManager.instance.PutFarm(curFarm, curPlayer, DoNothing));
        ChatInput.text = "";
        ChatInput.ActivateInputField();
        */
        }

        public void ChangeFarmAndPut(string jsonString) {
            _isPutting = true;
            curFarm.JsonString = jsonString;
            StartCoroutine(DB.Instance.PutFarm(curFarm, curPlayer, EndPutting));
        }

        private void DoNothing(Farm farm) {
            //DebugManager.instance.Log("nothing special");
        }

        public void ToMenu() {
            SceneManager.LoadScene(0);
        }

        #region Singleton

        public static OnlineFarm Instance;

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance == this)
                Destroy(gameObject);
        }

        #endregion
    }
}