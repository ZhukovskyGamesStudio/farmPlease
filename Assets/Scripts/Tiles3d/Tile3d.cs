using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Tile3d : MonoBehaviour
{
   private void Start() {
     
      
      
      
   }

   private async UniTask Grow() {
      var objs = transform.GetComponentsInChildren<Transform>();
      //tween all from 0 to 1 with little wiggle animation like growing sprouts
      //without dotween
   }
}
