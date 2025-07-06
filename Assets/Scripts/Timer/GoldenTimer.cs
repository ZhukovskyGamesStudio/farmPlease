using System;
using TMPro;
using UnityEngine;

public class GoldenTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    public void SetTime(TimeSpan timer)
    {
        _timerText.text = timer.ToString(@"mm\:ss");
        
    }
}
