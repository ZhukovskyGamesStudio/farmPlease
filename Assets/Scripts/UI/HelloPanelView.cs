using System;
using UnityEngine;
using UnityEngine.UI;

public class HelloPanelView : MonoBehaviour {
    [SerializeField]
    private Text _hintText;

    public void Show(string text, Transform target = null, bool isFlipped = false) {
        gameObject.SetActive(true);
        _hintText.text = text;
        if (target != null) {
            MoveHintToTarget(target);
        }

        SetFlipped(isFlipped);
    }

    private void MoveHintToTarget(Transform target) {
        throw new NotImplementedException();
    }

    private void SetFlipped(bool isFlipped) {
        Debug.Instance.Log("Hint SetFlipped not implemented");
    }
}