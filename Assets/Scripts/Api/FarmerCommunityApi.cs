using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FarmerCommunityApi : BaseApi {
    public async UniTask<GameSaveProfile> GetRandomPlayerAsync(List<string> excludeIds, CancellationToken cancellationToken) {
        string json = JsonUtility.ToJson(new ExcludeIdsRequest { excludeIds = excludeIds });

        using var req = new UnityWebRequest($"{BaseUrl}/farmercommunity/random", "GET");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

        if (req.result == UnityWebRequest.Result.Success) {
            GameSaveProfile farmData = JsonUtility.FromJson<GameSaveProfile>(req.downloadHandler.text);
            Debug.Log("Random player fetched: " + JsonUtility.ToJson(farmData));
            return farmData;
        } else {
            Debug.LogWarning("Failed to fetch random player: " + req.error);
            return null;
        }
    }

    [Serializable]
    public class ExcludeIdsRequest {
        public List<string> excludeIds;
    }
}