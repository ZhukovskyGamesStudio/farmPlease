using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

public class CanvasScale : Singleton<CanvasScale> {
    [SerializeField]
    private CanvasScaler _canvasScaler;

    [SerializeField]
    private Canvas _canvas;

    private static RectTransform _canvasRect;
    public static float ScaleFactor => Instance._canvasScaler.scaleFactor;

    public static Rect CanvasSize => _canvasRect.rect;

    protected override void OnFirstInit() {
        _canvasRect = _canvas.GetComponent<RectTransform>();
    }
}