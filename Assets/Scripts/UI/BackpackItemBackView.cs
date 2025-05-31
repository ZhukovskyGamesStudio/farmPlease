using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

public class BackpackItemBackView : MonoBehaviour {
    [SerializeField]
    private Image _background;
    
    [SerializeField]
    private SerializableDictionary<ItemColorType, Color> _colorsDict = new SerializableDictionary<ItemColorType, Color>(); 

    public void InitColor( ItemColorType colorType) {
        _background.color = _colorsDict[colorType];
    }
    
}
