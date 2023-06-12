using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinGamePanel : MonoBehaviour {
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
    private Farm _curFarm;

    private Player _curPlayer;

    private void LoadPlayer() {
        _curPlayer = SaveLoadManager.LoadPlayerStruct();

        if (_curPlayer.Id == 0) {
            RegisterPanel.SetActive(true);
        } else {
            Debug.Instance.Log("CurPlayerId: " + _curPlayer.Id);
            RegisterPanel.SetActive(false);
            StartCoroutine(DB.Instance.GetPlayer(_curPlayer, EndGetPlayer));
        }
    }

    public void PanelOpened() {
        gameObject.SetActive(true);

        LoadPlayer();
    }

    public void DeletePlayer() {
        StartCoroutine(DB.Instance.DeletePlayer(_curPlayer, EndDeletingPlayer));
    }

    public void EndDeletingPlayer(string statusCode) {
        Debug.Instance.Log("Deleting player is " + statusCode);
        PlayerPrefs.DeleteKey("player_id");
        _curPlayer.Id = -1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void EndGetPlayer(Player player) {
        if (player.Id == -1) {
            PlayerNameText.text = "wname";
            Debug.Instance.Log("player is null");
            return;
        }

        _curPlayer = player;
        PlayerNameText.text = _curPlayer.Name + "<color=Grey> #" + _curPlayer.Id + "</color>";
        if (player.FarmId != 0) {
            FarmIdText.text = "Farm id: " + player.FarmId;
            CreateFarmButton.GetComponent<Button>().interactable = false;
            DeleteFarmButton.GetComponent<Button>().interactable = true;
        } else {
            FarmIdText.text = "Farm id: ###";
            DeleteFarmButton.GetComponent<Button>().interactable = false;
        }

        if (!_curPlayer.IsConfirmed)
            ConfirmPanel.SetActive(true);
        else
            ConfirmPanel.SetActive(false);

        _curFarm = SaveLoadManager.LoadFarmStruct();
        if (_curFarm != null) {
            ConnectIdInput.text = _curFarm.Id.ToString();
            ConnectPasswordInput.text = _curFarm.Password;
        }
    }

    public void Confirm() {
        if (ConfirmInput.text == "true") {
            _curPlayer.IsConfirmed = true;
            StartCoroutine(DB.Instance.PutPlayer(_curPlayer, _curPlayer.Id, EndGetPlayer));
        }
    }

    public void CreateFarm() {
        string password = FarmPasswordInput.text;
        if (password != "") {
            Farm farm = new() {
                Password = password
            };

            StartCoroutine(DB.Instance.PostFarm(farm, _curPlayer, EndCreatingFarm));
        }
    }

    private void EndCreatingFarm(Farm newFarm) {
        FarmIdText.text = "Farm id: " + newFarm.Id;
        CreatePanel.SetActive(false);

        CreateFarmButton.GetComponent<Button>().interactable = false;
        DeleteFarmButton.GetComponent<Button>().interactable = true;

        Debug.Instance.Log("newFarm id: " + newFarm.Id);
        Debug.Instance.Log("newFarm password: " + newFarm.Password);

        _curFarm = newFarm;
        SaveLoadManager.SaveFarmStruct(_curFarm);
        if (_curFarm.Id != -1) {
            ConnectIdInput.text = _curFarm.Id.ToString();
            ConnectPasswordInput.text = _curFarm.Password;
        }
    }

    public void DeleteFarm() {
        StartCoroutine(DB.Instance.DeleteFarm(_curPlayer, EndDeleting));
    }

    public void EndDeleting(string statusCode) {
        CreateFarmButton.GetComponent<Button>().interactable = true;
        DeleteFarmButton.GetComponent<Button>().interactable = false;
        _curFarm.Id = -1;
        _curPlayer.FarmId = -1;
        FarmIdText.text = "Farm id: ###";
        SaveLoadManager.SaveFarmStruct(_curFarm);
        ConnectIdInput.text = "";
        ConnectPasswordInput.text = "";

        Debug.Instance.Log("Deleting farm is " + statusCode);
    }

    public void Connect() {
        int id = int.Parse(ConnectIdInput.text);
        string password = ConnectPasswordInput.text;

        StartCoroutine(DB.Instance.GetFarm(id, password, _curPlayer, EndConnecting));
    }

    public void EndConnecting(Farm farm) {
        if (farm.Id <= 0) {
            ConnectIdInput.text = "";
            ConnectPasswordInput.text = "";
            return;
        }

        SceneManager.LoadScene("OnlineFarm");
    }

    public void Register() {
        string password1 = PasswordInput.text;
        string password2 = ConfirmPasswordInput.text;
        if (password1 != password2) {
            ConfirmPasswordInput.text = "";
            return;
        }

        RegisterButton.interactable = false;

        _curPlayer = new Player {
            Name = NameInput.text,
            Email = EmailInput.text,
            Password = password1,
            IsConfirmed = false
        };
        if (Debug.Instance.IsDevelopmentBuild)
            _curPlayer.IsConfirmed = true;

        UnityEngine.Debug.LogWarning(_curPlayer.Email);
        StartCoroutine(DB.Instance.PostPlayer(_curPlayer, EndRegister));
    }

    public void EndRegister(Player player) {
        RegisterButton.interactable = true;
        SaveLoadManager.SavePlayerStruct(player);
        Debug.Instance.Log("Player is registered. Now Confirm your email");
        EndGetPlayer(player);
        RegisterPanel.SetActive(false);
    }
}