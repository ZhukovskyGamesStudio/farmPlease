using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour, ISoundStarter {
    public CroponomManager CroponomButton;
    public GameObject DiscordButton;
    public GameObject ExitButton;

    public Toggle LogoToggle;
    public Transform Lcorner, Rcorner, VegsHolder;
    public GameObject VegPrefab, VegPanel;
    public GameObject FakeGameButton;
    public float impulseMultiplier;
    public float secondsWait;
    public int cropMax = 150;
    private int cropCount;

    private void Start() {
        cropCount = 0;
#if UNITY_ANDROID
        DiscordButton.SetActive(false);
        ExitButton.SetActive(false);
#else
        DiscordButton.SetActive(true);
        ExitButton.SetActive(true);
#endif

        if (DebugManager.instance.IsDevelopmentBuild)
            FakeGameButton.SetActive(true);
        else
            FakeGameButton.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void OnApplicationQuit() {
        //  GPSManager.ExitFromGPS();
    }

    public IEnumerator CropRain() {
        cropCount = 0;
        VegPanel.SetActive(true);
        while (true) {
            Vector3 pose = Lcorner.position + (Rcorner.position - Lcorner.position) * Random.Range(0.05f, 0.95f);
            Quaternion quat = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            if (cropCount > cropMax) Destroy(VegsHolder.GetChild(0).gameObject);
            Image a = Instantiate(VegPrefab, pose, quat, VegsHolder).GetComponent<Image>();
            a.gameObject.SetActive(true);
            a.gameObject.GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(Random.Range(-0.3f, 0.3f), -1) * impulseMultiplier, ForceMode2D.Impulse);

            CropsType crop = CropsTable.instance.Crops[Random.Range(0, CropsTable.instance.Crops.Length)].type;
            a.sprite = CropsTable.CropByType(crop).VegSprite;

            cropCount++;
            yield return new WaitForSeconds(secondsWait);
        }
    }

    public void EndCropRain() {
        VegPanel.SetActive(false);
        GameObject[] objs = new GameObject[VegsHolder.childCount];
        for (int i = 0; i < objs.Length; i++) Destroy(VegsHolder.GetChild(i).gameObject, 8);
    }

    public void ToggledCropRain() {
        if (LogoToggle.isOn) {
            StartCoroutine(CropRain());
        } else {
            EndCropRain();
            StopAllCoroutines();
        }
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Discord() {
        Application.OpenURL("https://discord.gg/XmskYmCWdj");
    }

    public void OpenSettings() {
        SettingsManager.instance.SettingsPanel.gameObject.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}