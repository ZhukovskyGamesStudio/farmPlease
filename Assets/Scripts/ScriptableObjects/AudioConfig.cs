
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Scriptable Objects/AudioConfig", order = 0)]
public class AudioConfig : ScriptableObject {
    public AudioClip[] click, clickWrong, clickButton, nextPage, zeroEnergy;
    public AudioClip[] collect, hoed, watered, seeded;
}