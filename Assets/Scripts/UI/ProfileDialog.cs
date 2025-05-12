using UnityEngine;

public class ProfileDialog : MonoBehaviour {

    public void Show() {
        gameObject.SetActive(true);
    }
    
    public void Close() {
        gameObject.SetActive(false);
    }
}