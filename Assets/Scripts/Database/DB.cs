using System.Collections;
using System.Text;
using Abstract;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Debug = Managers.Debug;

namespace Database
{
    public class DB : PreloadableSingleton<DB> {
    
        private const string DB_URL = "http://farmplease.somee.com/api/";
        //private const string url = "https://localhost:44301/api/";

        public Farm tmpFarm;
        public Player tmpPlayer;



        #region Player

        public IEnumerator GetPlayer(Player curPlayer, UnityAction<Player> callback) {
            UnityWebRequest request = UnityWebRequest.Get(DB_URL + "players/" + curPlayer.Id);
            yield return request.SendWebRequest();

            //Костыль пиздец, надо разобраться с JSON сериализацией
            //string json = "{\"players\":" + request.downloadHandler.text + "}";
            Player fullPlayer = new();
            if (request.responseCode == 200) fullPlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);
            tmpPlayer = fullPlayer;
            callback(fullPlayer);
        }

        public IEnumerator PostPlayer(Player player, UnityAction<Player> callback) {
            WWWForm form = new();

            string json = JsonUtility.ToJson(player);
            UnityEngine.Debug.LogWarning(json);

            UnityWebRequest request = UnityWebRequest.Post(DB_URL + "players", form);

            byte[] playerBytes = Encoding.UTF8.GetBytes(json);
            UploadHandler uploadHandler = new UploadHandlerRaw(playerBytes);

            request.uploadHandler = uploadHandler;

            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            Player responsePlayer = new();
            Debug.Instance.Log("code: " + request.responseCode);
            if (request.responseCode == 201) {
                Debug.Instance.Log("id: " + responsePlayer.Id);
                Debug.Instance.Log("email: " + responsePlayer.Email);
                Debug.Instance.Log("password: " + responsePlayer.Password);
                responsePlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);
            }

            callback(responsePlayer);
        }

        public IEnumerator PutPlayer(Player player, int id, UnityAction<Player> callback) {
            string json = JsonUtility.ToJson(player);

            UnityWebRequest request = UnityWebRequest.Put(DB_URL + "players" + "/" + id, json);
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();
            Player responsePlayer = JsonUtility.FromJson<Player>(request.downloadHandler.text);

            callback(responsePlayer);
        }

        public IEnumerator DeletePlayer(Player player, UnityAction<string> callback) {
            UnityWebRequest request = UnityWebRequest.Delete(DB_URL + "players/" + player.Id);
            yield return request.SendWebRequest();
            callback(request.responseCode.ToString());
        }

        #endregion

        #region Farm

        public IEnumerator GetFarm(int id, string password, Player player, UnityAction<Farm> callback) {
            UnityWebRequest request = UnityWebRequest.Get(DB_URL + "farms/connect/" + id + "/" + player.Id + "/" + password);
            yield return request.SendWebRequest();

            Farm farm = new();
            if (request.responseCode == 200)
                farm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
            else
                Debug.Instance.Log("Getting farm code is " + request.responseCode);

            tmpFarm = farm;
            callback(farm);
        }

        public IEnumerator GetDates(int farmId, string farmPassword, UnityAction<Farm> callback) {
            UnityWebRequest request = UnityWebRequest.Get(DB_URL + "farms/" + farmId + "/" + farmPassword + "/date");
            yield return request.SendWebRequest();

            Farm datesHolder = new();
            if (request.responseCode == 200)
                datesHolder = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
            else
                UnityEngine.Debug.Log("request " + request.url + "\ncode error is " + request.responseCode);
            callback(datesHolder);
        }

        public IEnumerator PostFarm(Farm farm, Player player, UnityAction<Farm> callback) {
            WWWForm form = new();

            string json = JsonUtility.ToJson(farm);

            UnityWebRequest request = UnityWebRequest.Post(DB_URL + "farms/" + player.Id + "/" + player.Password, form);

            byte[] farmBytes = Encoding.UTF8.GetBytes(json);
            UploadHandler uploadHandler = new UploadHandlerRaw(farmBytes);

            request.uploadHandler = uploadHandler;

            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            if (request.responseCode == 200) {
                Farm responseFarm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
                callback(responseFarm);
            } else {
                UnityEngine.Debug.Log("response errorCode " + request.responseCode);
            }
        }

        public IEnumerator PutFarm(Farm farm, Player player, UnityAction<Farm> callback) {
            string json = JsonUtility.ToJson(farm);

            UnityWebRequest request =
                UnityWebRequest.Put(DB_URL + "farms/" + +farm.Id + "/" + farm.Password + "/" + player.Id, json);
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            Farm responseFarm = new();

            if (request.responseCode == 200)
                responseFarm = JsonUtility.FromJson<Farm>(request.downloadHandler.text);
            else
                Debug.Instance.Log("response errorCode " + request.responseCode);

            callback(responseFarm);
        }

        public IEnumerator DeleteFarm(Player player, UnityAction<string> callback) {
            UnityWebRequest request =
                UnityWebRequest.Delete(DB_URL + "farms/" + player.FarmId + "/" + player.Id + "/" + player.Password);
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
}