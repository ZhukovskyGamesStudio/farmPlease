using UnityEngine;

public abstract class ConfigWithCroponomPage : ScriptableObject {
    [Header("CroponomPage")]
    public string header;

    public string firstText;
    public Sprite firstSprite;
    public string secondText;
    public Sprite secondSprite;
}