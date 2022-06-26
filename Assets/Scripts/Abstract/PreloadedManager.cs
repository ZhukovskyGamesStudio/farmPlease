using System;
using System.Collections;
using UnityEngine;

public abstract class PreloadedManager : MonoBehaviour {
  

    public virtual IEnumerator Init() {
      yield break;
    }
}