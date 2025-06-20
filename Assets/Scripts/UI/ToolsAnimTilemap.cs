﻿using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI
{
    public class ToolsAnimTilemap : MonoBehaviour {
        public Tilemap AnimationsTilemap;

        public TileBase hoeAnimation;
        public TileBase watercanAnimation;
        [SerializeField]
        public TileBase _scytheAnimation,_rainAnimation;

        public float animationTime;

        public void StartAnimationInCoord(Vector2Int coord, AnimationType type) {
            StartCoroutine(PlaceAnimation(coord, type));
        }

        public IEnumerator PlaceAnimation(Vector2Int coord, AnimationType type) {
            var vec3 = (Vector3Int)coord;
            switch (type) {
                case AnimationType.Hoe:
                    AnimationsTilemap.SetTile(vec3, hoeAnimation);
                    break;
                case AnimationType.Watercan:
                    AnimationsTilemap.SetTile(vec3, watercanAnimation);
                    break;
                case AnimationType.Scythe:
                    AnimationsTilemap.SetTile(vec3, _scytheAnimation);
                    break;
                case AnimationType.Rain:
                    AnimationsTilemap.SetTile(vec3, _rainAnimation);
                    break;
            }

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
        Rain
    }
}