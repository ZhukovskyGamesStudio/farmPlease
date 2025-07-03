using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolsAnimTilemap : MonoBehaviour {
    public Tilemap AnimationsTilemap;

    public TileBase hoeAnimation;
    public TileBase watercanAnimation;

    [SerializeField]
    public TileBase _scytheAnimation, _rainAnimation, _strawberryAnimation, _dandellionAnimation, _tractorAnimation, _windAnimation;

    public float animationTime;

    private Dictionary<AnimationType, TileBase> AnimationsMap;

    private void Awake() {
        CreateAnimationsMap();
    }

    private void CreateAnimationsMap() {
        AnimationsMap = new Dictionary<AnimationType, TileBase>() {
            { AnimationType.Hoe, hoeAnimation },
            { AnimationType.Watercan, watercanAnimation },
            { AnimationType.Scythe, _scytheAnimation },
            { AnimationType.Rain, _rainAnimation },
            { AnimationType.Strawberry, _strawberryAnimation },
            { AnimationType.Dandellion, _dandellionAnimation },
            { AnimationType.Tractor, _tractorAnimation },
            { AnimationType.Wind, _windAnimation },
        };
    }

    public void StartAnimationInCoord(Vector2Int coord, AnimationType type) {
        StartCoroutine(PlaceAnimation(coord, type));
    }

    public IEnumerator PlaceAnimation(Vector2Int coord, AnimationType type) {
        var vec3 = (Vector3Int)coord;
        AnimationsTilemap.SetTile(vec3, AnimationsMap[type]);
        yield return new WaitForSeconds(animationTime);

        AnimationsTilemap.SetTile(vec3, null);
        yield return false;
    }
}

public enum AnimationType {
    None,
    Hoe,
    Watercan,
    Scythe,
    Rain,
    Strawberry,
    Dandellion,
    Tractor,
    Wind
}