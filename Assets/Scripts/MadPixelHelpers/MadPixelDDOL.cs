using System;
using System.Collections.Generic;
using Abstract;
using UnityEngine;

public class MadPixelDDOL : PreloadableSingleton<MadPixelDDOL> {
    [SerializeField]
    private List<GameObject> _managers;

    public override int InitPriority => -100;

    protected override void OnFirstInit() {
        base.OnFirstInit();
        ActivateManagers();
    }

    private void ActivateManagers() {
        foreach (GameObject manager in _managers) {
            manager.SetActive(true);
        }
    }
}
