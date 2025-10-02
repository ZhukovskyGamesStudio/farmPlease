using UnityEngine;

[CreateAssetMenu(fileName = "Tile3dAnimConfig", menuName = "Scriptable Objects/Tile3dAnimConfig", order = 0)]
public class Tile3dAnimConfig : ScriptableObject {
    [Header("Grow")]
    public bool IsGrowOnStart;

    [SerializeField]
    public float Duration = 0.45f;

    [SerializeField]
    public float AddedDuration = 0.35f;

    [SerializeField]
    public float Delay = 0.18f;

    [SerializeField]
    public float WiggleAmplitudeDegrees = 12f;

    [Header("GrowFromOtherState")]
    [SerializeField]
    public float SquashScale = 0.75f;

    [SerializeField]
    public float SquashDuration = 0.25f;

    [SerializeField]
    public float OvershootScale = 1.1f;

    [Header("Scythe")]
    [SerializeField]
    public float ScytheAngle = 30f;

    [SerializeField]
    public float ScytheFallDistance = 0.6f;

    [SerializeField]
    public float ScytheDuration = 0.5f;

    [SerializeField]
    public float ScytheAddedDuration = 0.2f;

    [SerializeField]
    public float ScytheDelay = 0.05f;

    [Header("Wind Sway")]
    [SerializeField]
    public float WindSwayAmplitude = 3f;

    [SerializeField]
    public float WindSwayFrequency = 1f;

    [SerializeField]
    public float WindSwayIntervalMin = 5f;

    [SerializeField]
    public float WindSwayIntervalMax = 15f;
}