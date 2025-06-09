using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FarmerCommunityApi : BaseApi {
    public async UniTask<GameSaveProfile> GetRandomPlayerAsync(string myId) {
        using var req = UnityWebRequest.Get($"{BaseUrl}/player/random/{myId}");
        await req.SendWebRequest().ToUniTask();

        if (req.result == UnityWebRequest.Result.Success) {
            var json = req.downloadHandler.text;
            var data = JsonUtility.FromJson<PlayerAPI.FarmData>(json);
            Debug.Log($"🎯 Random Player: {data.farmData.Nickname}");
            return data.farmData;
        } else {
            Debug.LogError("❌ Error fetching random player: " + req.error);
            return null;
        }
    }
}