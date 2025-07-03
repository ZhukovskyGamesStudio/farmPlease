using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrobotAnimTilemap : MonoBehaviour {
    [SerializeField]
    private Tilemap _animationsTilemap;

    [SerializeField]
    private TileBase _landAnimation, _noAnimation, _flyAnimation;

    [SerializeField]
    private Vector2Int _coord;

    public async void ShowLandAnimation() {
        _animationsTilemap.SetTile((Vector3Int)_coord, _landAnimation);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _animationsTilemap.SetTile((Vector3Int)_coord, _noAnimation);
    }

    public void ShowIdle() {
        _animationsTilemap.SetTile((Vector3Int)_coord, _noAnimation);
    }
    
    public async void ShowFlyAnimation() {
        _animationsTilemap.SetTile((Vector3Int)_coord, _flyAnimation);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _animationsTilemap.SetTile((Vector3Int)_coord, null);
    }
}

public enum BrobotAnimationType {
    Land,
    NoAnimation,
    Fly
}