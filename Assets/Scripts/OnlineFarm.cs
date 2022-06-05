using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class OnlineFarm : MonoBehaviour
{
    #region Singleton
    public static OnlineFarm instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
            Destroy(gameObject);
    }
    #endregion



    public Text FarmIdText;

    public Text ChatText;
    public InputField ChatInput;
    public SmartTilemap smartTilemap;

    public Player curPlayer;
    public Farm curFarm;


    bool NeedToUpdateFarm;

    void Start()
    {
        curPlayer = DBManager.instance.tmpPlayer;
        curFarm = DBManager.instance.tmpFarm;

        if (curPlayer.Id == -1)
            Debug.Log("Player is not loaded");
        ChatText.text = "";

       

        FarmIdText.text = "Ферма номер #" + curFarm.Id;

        StartCoroutine(FarmUpdater());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && ChatInput.text != "")
        {
            ChatInput.text = "";
            ChatInput.DeactivateInputField();
        }
    }

    public IEnumerator FarmUpdater()
    {
        yield return StartCoroutine(DBManager.instance.GetFarm(curFarm.Id, curFarm.Password, curPlayer, UpdateLocalFarm));
        for (; ; )
        {
            yield return StartCoroutine(DBManager.instance.GetDates(curFarm.Id, curFarm.Password, IsNeedToUpdate));

            if (NeedToUpdateFarm && !isPutting)
                yield return StartCoroutine(DBManager.instance.GetFarm(curFarm.Id, curFarm.Password, curPlayer, UpdateLocalFarm));
            else
                yield return new WaitForEndOfFrame();
        }

    }

    void IsNeedToUpdate(Farm datesHolder)
    {
        NeedToUpdateFarm = (curFarm.FarmPostDate != datesHolder.FarmPostDate);
        curFarm.FarmPostDate = datesHolder.FarmPostDate;          
    }


    void UpdateLocalFarm(Farm newFarm)
    {
        NeedToUpdateFarm = false;
        curFarm = newFarm;

        //ChatText.text +="\n" + curFarm.LastMessage;

        if (newFarm.JsonString == null || newFarm.JsonString == "")
        {
            SaveLoadManager.GenerateGame();
            DebugManager.instance.Log("Posting generated online Farm");
            ChangeFarmAndPut(SaveLoadManager.GenerateJsonString());
        }
        else
        {
           
            SaveLoadManager.LoadGame(newFarm.JsonString);
            DebugManager.instance.Log("Synchronizing local Farm");
        }
    }

    bool isPutting;


    public void EndPutting(Farm farm)
    {
        isPutting = false;
        PlayerController.canInteract = true;
    }

    public void PutChatMessage()
    {
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


    public void ChangeFarmAndPut(string JsonString)
    {
        isPutting = true;
        curFarm.JsonString = JsonString;
        StartCoroutine(DBManager.instance.PutFarm(curFarm, curPlayer, EndPutting));
    }


    void DoNothing(Farm farm)
    {
        //DebugManager.instance.Log("nothing special");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
