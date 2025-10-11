using Tables;
using UnityEngine;

[CreateAssetMenu(fileName = "TileGroupConfig", menuName = "Scriptable Objects/TileGroupConfig", order = 0)]
public class TileGroupConfig : ScriptableObject {
    public Group[] Groups;
    public Group[] BuildingGroups;
}