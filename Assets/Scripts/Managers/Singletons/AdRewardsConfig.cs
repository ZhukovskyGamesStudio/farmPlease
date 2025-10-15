using UnityEngine;

[CreateAssetMenu(fileName = "AdRewardsConfig", menuName = "Scriptable Objects/AdRewardsConfig", order = 0)]
public class AdRewardsConfig : ScriptableObject {
    public AYellowpaper.SerializedCollections.SerializedDictionary<AdRewards, Sprite> AdRewardIcons;
}