using System.Collections;
using UnityEngine;

public abstract class IPreloaded : MonoBehaviour {
    public virtual IEnumerator Init() {
        yield break;
    }
}