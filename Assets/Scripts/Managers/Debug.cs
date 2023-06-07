using System.Collections;
using DefaultNamespace.Abstract;
using UnityEngine;
using UnityEngine.UI;

public class Debug : PreloadableSingleton<Debug> {
    public Text ErrorText;
    public GameObject DebugPanel, LogButton;

    public bool IsDevelopmentBuild = true;
    public bool IsPrintingInConsole = true;
    

    protected override void OnFirstInit() {
        if (IsDevelopmentBuild) {
            LogButton.SetActive(true);
            Instance.Clear();
        } else {
            LogButton.SetActive(false);
        }
    }

    public void Log(string textToShow) {
        ErrorText.text += "\n- " + textToShow;
        if (IsPrintingInConsole)
            UnityEngine.Debug.Log(textToShow);
    }

    public void Clear() {
        ErrorText.text = "";
    }

    public void OnOffDebugPanel() {
        DebugPanel.SetActive(!DebugPanel.activeSelf);
    }
}