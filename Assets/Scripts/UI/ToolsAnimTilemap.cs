using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI
{
    public class ToolsAnimTilemap : MonoBehaviour {
        public Tilemap AnimationsTilemap;

        public TileBase hoeAnimation;
        public TileBase watercanAnimation;

        public float animationTime;

        public void StartAnimationInCoord(Vector3Int coord, AnimationType type) {
            StartCoroutine(PlaceAnimation(coord, type));
        }

        public IEnumerator PlaceAnimation(Vector3Int coord, AnimationType type) {
            if (type == AnimationType.Hoe)
                AnimationsTilemap.SetTile(coord, hoeAnimation);
            if (type == AnimationType.Watercan)
                AnimationsTilemap.SetTile(coord, watercanAnimation);

            yield return new WaitForSeconds(animationTime);

            AnimationsTilemap.SetTile(coord, null);
            yield return false;
        }
    }

    public enum AnimationType {
        None,
        Hoe,
        Watercan
    }
}