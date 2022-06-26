using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : PreloadedManager {
    public static DebugManager instance;
    public Text ErrorText;
    public GameObject DebugPanel, LogButton;

    public bool IsDevelopmentBuild = true;
    public bool IsPrintingInConsole = true;

    public override IEnumerator Init() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (IsDevelopmentBuild) {
                LogButton.SetActive(true);
                instance.Clear();
            } else {
                LogButton.SetActive(false);
            }
        } else {
            Destroy(gameObject);
        }

        yield break;
    }

    public void Log(string textToShow) {
        ErrorText.text += "\n- " + textToShow;
        if (IsPrintingInConsole)
            Debug.Log(textToShow);
    }

    public void Clear() {
        ErrorText.text = "";
    }

    public void OnOffDebugPanel() {
        DebugPanel.SetActive(!DebugPanel.activeSelf);
    }
}