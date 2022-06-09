using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public AudioClip[] songs, click, clickWrong, clickButton, nextPage, zeroEnergy;
    public AudioClip[] collect, hoed, watered, seeded;

    public AudioSource musicSource;
    public AudioSource[] effectsSource;
    private int curSource;
    private bool isInitialized;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (!musicSource.isPlaying && isInitialized) {
            musicSource.clip = songs[Random.Range(0, songs.Length)];
            musicSource.Play();
        }
    }

    public void Initialize() {
        isInitialized = true;
    }

    public void ChangeVolume(float master, float music, float effects) {
        musicSource.volume = master * music;
        foreach (AudioSource source in effectsSource) source.volume = master * effects;
    }

    private void NextSource() {
        curSource++;
        if (curSource > effectsSource.Length - 1)
            curSource = 0;
    }

    public void PlaySound(Sounds sound) // 0 - click, 1 - clickWrong, 2 - clickButton
    {
        NextSource();
        switch (sound) {
            case Sounds.ClickOnTile:
                effectsSource[curSource].clip = click[Random.Range(0, click.Length)];
                break;

            case Sounds.Button:
                effectsSource[curSource].clip = clickButton[Random.Range(0, clickButton.Length)];
                break;

            case Sounds.NextPage:
                effectsSource[curSource].clip = nextPage[Random.Range(0, nextPage.Length)];
                break;

            case Sounds.Collect:
                effectsSource[curSource].clip = collect[Random.Range(0, collect.Length)];
                break;

            case Sounds.ZeroEnergy:
                effectsSource[curSource].clip = zeroEnergy[Random.Range(0, zeroEnergy.Length)];
                break;

            case Sounds.Hoed:
                effectsSource[curSource].clip = hoed[Random.Range(0, hoed.Length)];
                break;

            case Sounds.Watered:
                effectsSource[curSource].clip = watered[Random.Range(0, watered.Length)];
                break;

            case Sounds.Seeded:
                effectsSource[curSource].clip = seeded[Random.Range(0, seeded.Length)];
                break;
        }

        effectsSource[curSource].Play();
    }
}

[Serializable]
public enum Sounds {
    Button = 0,
    NextPage = 1,
    ClickOnTile = 2,
    Collect = 3,
    ZeroEnergy = 4,
    Hoed = 5,
    Watered = 6,
    Seeded = 7
}

public class MonoBehaviourSoundStarter : MonoBehaviour {
    public void PlaySound(int soundIndex) {
        AudioManager.instance.PlaySound((Sounds) soundIndex);
    }
}