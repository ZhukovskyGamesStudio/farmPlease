using UnityEngine;

[CreateAssetMenu(fileName = "SongsConfig", menuName = "Scriptable Objects/SongsConfig", order = 0)]
public class SongsConfig : ScriptableObject {
    public AudioClip[] songs;
}