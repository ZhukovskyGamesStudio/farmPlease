using System.Collections;
using UnityEngine;

public class TabletJitterEffects : MonoBehaviour
{
    
    [SerializeField]
    private Animation[] _jitterAnimations;
    private void OnEnable() {
        StartCoroutine(StartJitterAnims());
    }

    private IEnumerator StartJitterAnims() {
        foreach (var jitterAnim in _jitterAnimations) {
            jitterAnim.Play("TabletJitterIdle");
            yield return new WaitForSeconds(0.025f);
        }
    }
}
