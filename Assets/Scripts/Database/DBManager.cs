using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

public class DBManager : MonoBehaviour
{
    #region Singleton
    public static DBManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }                 
        else if (instance == this)
            Destroy(gameObject);
    }
    #endregion


    public Farm tmpFarm;
    public Player tmpPlayer;

    [SerializeField]
    
    private const string url = "http://farmplease.somee.com/api/";
    //private const string url = "https://localhost:44301/api/";

    #region Player
    public IEnumerator GetPlayer(Player curPlayer, UnityAction<Player> callback)
    {

        UnityWebRequest request = UnityWebRequest.Get(url + "players/" + curPlayer.Id);
        yield return request.SendWebRequest();

        //Костыль пиздец, надо разобраться с JSON сериализацией
        //string json = "{\"players\":" + request.downloadHandler.text + "}";
        Player fullPlayer = new Player();
        if (request.responseCode == 200)
        {
            fullPlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);            
        }
        tmpPlayer = fullPlayer;
        callback(fullPlayer);
    }

    public IEnumerator PostPlayer(Player player, UnityAction<Player> callback)
    {
        WWWForm form = new WWWForm();

        string json = JsonUtility.ToJson(player);
        Debug.LogWarning(json);

        UnityWebRequest request = UnityWebRequest.Post(url + "players", form);

        byte[] playerBytes = Encoding.UTF8.GetBytes(json);
        UploadHandler uploadHandler = new UploadHandlerRaw(playerBytes);

        request.uploadHandler = uploadHandler;

        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();

        Player responsePlayer = new Player();
        DebugManager.instance.Log("code: " + request.responseCode);
        if (request.responseCode == 201)
        {
            DebugManager.instance.Log("id: " + responsePlayer.Id);
            DebugManager.instance.Log("email: " + responsePlayer.Email);
            DebugManager.instance.Log("password: " + responsePlayer.Password);
            responsePlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);
        }
        callback(responsePlayer);
    }

    public IEnumerator PutPlayer(Player player, int id, UnityAction<Player> callback)
    {

        string json = JsonUtility.ToJson(player);

        UnityWebRequest request = UnityWebRequest.Put(url + "players" + "/" + id, json);
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();
        Player responsePlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);

        callback(responsePlayer);
    }

    public IEnumerator DeletePlayer(Player player, UnityAction<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Delete(url + "players/" + player.Id);
        yield return request.SendWebRequest();
        callback(request.responseCode.ToString());

    }

    #endregion

    #region Farm
    public IEnumerator GetFarm(int id,string password,Player player, UnityAction<Farm> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "farms/connect/" + id + "/"+player.Id +  "/" + password);
        yield return request.SendWebRequest();

        Farm farm = new Farm();
        if ( request.responseCode == 200)
        {
           
            farm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
        }
        else
            DebugManager.instance.Log("Getting farm code is " + request.responseCode);

        tmpFarm = farm;
        callback(farm);
    }
    
    public IEnumerator GetDates(int farmId, string farmPassword, UnityAction<Farm> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "farms/"+ farmId + "/" + farmPassword + "/date");
        yield return request.SendWebRequest();

        Farm datesHolder = new Farm();
        if (request.responseCode == 200)
        {
            datesHolder = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
        }
        else
            Debug.Log("request " + request.url + "\ncode error is " + request.responseCode);
        callback(datesHolder);
    }

    public IEnumerator PostFarm(Farm farm, Player player, UnityAction<Farm> callback)
    {
        WWWForm form = new WWWForm();

        string json = JsonUtility.ToJson(farm);

        UnityWebRequest request = UnityWebRequest.Post(url + "farms/" + player.Id +"/" + player.Password, form);

        byte[] farmBytes = Encoding.UTF8.GetBytes(json);
        UploadHandler uploadHandler = new UploadHandlerRaw(farmBytes);

        request.uploadHandler = uploadHandler;

        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();

       
        if (request.responseCode == 200)
        {
            Farm responseFarm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
            callback(responseFarm);
        }
        else
            Debug.Log("response errorCode " + request.responseCode);
    }

    public IEnumerator PutFarm(Farm farm, Player player, UnityAction<Farm> callback)
    {
        string json = JsonUtility.ToJson(farm);

        UnityWebRequest request = UnityWebRequest.Put(url + "farms/" + +farm.Id + "/" + farm.Password + "/" + player.Id, json);
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();

        Farm responseFarm = new Farm();

        if(request.responseCode == 200)
        {
            responseFarm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
        }
        else
            DebugManager.instance.Log("response errorCode " + request.responseCode);
        
        callback(responseFarm);

    }

    public IEnumerator DeleteFarm(Player player, UnityAction<string> callback)
    {         
        UnityWebRequest request = UnityWebRequest.Delete(url + "farms/" + player.FarmId + "/" + player.Id + "/" + player.Password);
        yield return request.SendWebRequest();
        callback(request.responseCode.ToString());
    }


    #endregion


    /*
    public IEnumerator PutFarmMessage(FarmStruct farm, PlayerStruct player, UnityAction<FarmStruct> callback)
    {
        string json = JsonUtility.ToJson(farm);
        string fullUrl = url + "farms/" + +farm.Id + "/" + farm.Password.TrimEnd() + "/" + player.Id;
        DebugManager.instance.Log(fullUrl);
        UnityWebRequest request = UnityWebRequest.Put(fullUrl, json);
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();

        FarmStruct responseFarm = new FarmStruct();

        if (request.responseCode == 200)
        {
            responseFarm = JsonUtility.FromJson<FarmStruct>(request.downloadHandler.text);
        }
        else
            DebugManager.instance.Log("response errorCode " + request.responseCode);

        callback(responseFarm);

    } */

}