using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinGamePanel : MonoBehaviour
{
    public GameObject MyFarmButton, CreateFarmButton, DeleteFarmButton;

    [Header("Register")]
    public GameObject RegisterPanel, ConfirmPanel;
    public Button RegisterButton;
    public InputField NameInput, EmailInput, PasswordInput, ConfirmPasswordInput;

    [Header("Confirm")]
    public InputField ConfirmInput;
    [Header("Info")]
    public Text PlayerNameText;
    public Text FarmIdText;

    [Header("CreateFarm")]
    public InputField FarmPasswordInput;
    public GameObject CreatePanel;

    [Header("ConnectFarm")]
    public InputField ConnectIdInput;
    public InputField ConnectPasswordInput;


    Player curPlayer;
    Farm curFarm;

    void LoadPlayer()
    {
        curPlayer = SaveLoadManager.LoadPlayerStruct();

        if (curPlayer.Id == 0)
            RegisterPanel.SetActive(true);
        else
        {
            DebugManager.instance.Log("CurPlayerId: " + curPlayer.Id);
            RegisterPanel.SetActive(false);
            StartCoroutine(DBManager.instance.GetPlayer(curPlayer, EndGetPlayer));
        }
    }
    public void PanelOpened()
    {
        gameObject.SetActive(true);


        LoadPlayer();

    }

    public void DeletePlayer()
    {
        StartCoroutine(DBManager.instance.DeletePlayer(curPlayer, EndDeletingPlayer));
    }

    public void EndDeletingPlayer(string statusCode)
    {
        DebugManager.instance.Log("Deleting player is " + statusCode);
        PlayerPrefs.DeleteKey("player_id");
        curPlayer.Id = -1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void EndGetPlayer(Player player)
    {
        if (player.Id == -1)
        {
            PlayerNameText.text = "wname";
            DebugManager.instance.Log("player is null");
            return;
        }

        curPlayer = player;
        PlayerNameText.text = curPlayer.Name + "<color=Grey> #" + curPlayer.Id + "</color>";
        if (player.FarmId != 0)
        {
            FarmIdText.text = "Farm id: " + player.FarmId;
            CreateFarmButton.GetComponent<Button>().interactable = false;
            DeleteFarmButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            FarmIdText.text = "Farm id: ###";
            DeleteFarmButton.GetComponent<Button>().interactable = false;
        }


        if (!curPlayer.IsConfirmed)
            ConfirmPanel.SetActive(true);
        else
        {
            ConfirmPanel.SetActive(false);
        }

        curFarm = SaveLoadManager.LoadFarmStruct();
        if (curFarm != null)
        {
            ConnectIdInput.text = curFarm.Id.ToString();
            ConnectPasswordInput.text = curFarm.Password;
        }

    }


    public void Confirm()
    {
        if (ConfirmInput.text == "true")
        {
            curPlayer.IsConfirmed = true;
            StartCoroutine(DBManager.instance.PutPlayer(curPlayer, curPlayer.Id, EndGetPlayer));
        }

    }

    public void CreateFarm()
    {
        string password = FarmPasswordInput.text;
        if (password != "")
        {
            Farm farm = new Farm()
            {
                Password = password
            };

            StartCoroutine(DBManager.instance.PostFarm(farm, curPlayer, EndCreatingFarm));

        }
    }

    void EndCreatingFarm(Farm newFarm)
    {
        FarmIdText.text = "Farm id: " + newFarm.Id;
        CreatePanel.SetActive(false);

        CreateFarmButton.GetComponent<Button>().interactable = false;
        DeleteFarmButton.GetComponent<Button>().interactable = true;

        DebugManager.instance.Log("newFarm id: " + newFarm.Id);
        DebugManager.instance.Log("newFarm password: " + newFarm.Password);

        curFarm = newFarm;
        SaveLoadManager.SaveFarmStruct(curFarm);
        if (curFarm.Id != -1)
        {
            ConnectIdInput.text = curFarm.Id.ToString();
            ConnectPasswordInput.text = curFarm.Password;
        }
    }

    public void DeleteFarm()
    {
        StartCoroutine(DBManager.instance.DeleteFarm(curPlayer, EndDeleting));
    }

    public void EndDeleting(string statusCode)
    {
        CreateFarmButton.GetComponent<Button>().interactable = true;
        DeleteFarmButton.GetComponent<Button>().interactable = false;
        curFarm.Id = -1;
        curPlayer.FarmId = -1;
        FarmIdText.text = "Farm id: ###";
        SaveLoadManager.SaveFarmStruct(curFarm);
        ConnectIdInput.text = "";
        ConnectPasswordInput.text = "";

        DebugManager.instance.Log("Deleting farm is " + statusCode);
    }

    public void Connect()
    {
        int id = int.Parse(ConnectIdInput.text);
        string password = ConnectPasswordInput.text;

        StartCoroutine(DBManager.instance.GetFarm(id, password, curPlayer, EndConnecting));
    }

    public void EndConnecting(Farm farm)
    {
        if (farm.Id <= 0)
        {
            ConnectIdInput.text = "";
            ConnectPasswordInput.text = "";
            return;
        }

        SceneManager.LoadScene("OnlineFarm");
    }


    public void Register()
    {
        string password1 = PasswordInput.text;
        string password2 = ConfirmPasswordInput.text;
        if (password1 != password2)
        {
            ConfirmPasswordInput.text = "";
            return;
        }
        RegisterButton.interactable = false;

        curPlayer = new Player
        {
            Name = NameInput.text,
            Email = EmailInput.text,
            Password = password1,
            IsConfirmed = false
        };
        if (DebugManager.instance.IsDevelopmentBuild)
            curPlayer.IsConfirmed = true;

        Debug.LogWarning(curPlayer.Email);
        StartCoroutine(DBManager.instance.PostPlayer(curPlayer, EndRegister));
    }

    public void EndRegister(Player player)
    {
        RegisterButton.interactable = true;
        SaveLoadManager.SavePlayerStruct(player);
        DebugManager.instance.Log("Player is registered. Now Confirm your email");
        EndGetPlayer(player);
        RegisterPanel.SetActive(false);
    }
}
