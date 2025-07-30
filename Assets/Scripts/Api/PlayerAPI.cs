using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerAPI : BaseApi {
    public static async UniTask<string> CreatePlayerAsync(GameSaveProfile data) {
        string json = JsonUtility.ToJson(data);
        using var req = new UnityWebRequest($"{BaseUrl}/player", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest().ToUniTask();

        if (req.result == UnityWebRequest.Result.Success) {
            AbstractMongoEntity res = JsonUtility.FromJson<AbstractMongoEntity>(req.downloadHandler.text);
            Debug.Log("Created player: " + JsonUtility.ToJson(res) +  "ID is: " + res._id);
            return res._id;
        } else {
            Debug.LogError("Error creating player: " + req.error);
            return null;
        }
    }

    public static async UniTask<GameSaveProfile> GetPlayerAsync(string playerId) {
        using var req = UnityWebRequest.Get($"{BaseUrl}/player/{playerId}");
        
        try {
            await req.SendWebRequest();

            var code = req.responseCode;
            var json = req.downloadHandler.text;

            if (code == 200) {
                GameSaveProfile res = JsonUtility.FromJson<GameSaveProfile>(req.downloadHandler.text);
                Debug.Log("Fetched player: " + req.downloadHandler.text);
                return res;
            } else {
                Debug.LogWarning($"Player not found or error ({code}): {json}");
                return null;
            }
        } catch (UnityWebRequestException e) {
            if (e.ResponseCode == 404) {
                Debug.LogWarning($"Player not found");
                return null;
            }
            return null;
        }
    }

    public static async UniTaskVoid UpdatePlayerAsync(string playerId, GameSaveProfile data) {
        string json = JsonUtility.ToJson(data);
        using var req = new UnityWebRequest($"{BaseUrl}/player/{playerId}", "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        await req.SendWebRequest().ToUniTask();

        if (req.result == UnityWebRequest.Result.Success) {
           // Debug.Log("Updated player: " + req.downloadHandler.text);
        } else {
            Debug.LogError("Error updating player: " + req.error);
        }
    }
}