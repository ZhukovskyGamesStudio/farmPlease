using System;
using Abstract;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;

public class Audio : PreloadableSingleton<Audio> {
    public AudioConfig AudioConfig;
    public SongsConfig SongsConfig;

    public AudioSource musicSource;
    public AudioSource[] effectsSource;
    private int _curSource;
    private bool _isInitialized;

    [SerializeField]
    private AudioMixerGroup _master, _music, _sfx;

    private float _curVolume;

    private void Update() {
        if (!musicSource.isPlaying && _isInitialized) {
            musicSource.clip = SongsConfig.songs[Random.Range(0, SongsConfig.songs.Length)];
            musicSource.Play();
        }
    }

    public async UniTask LoadAudioConfigAsync() {
        AudioConfig = await Resources.LoadAsync<AudioConfig>("Configs/AudioConfig") as AudioConfig;
        SongsConfig = await Resources.LoadAsync<SongsConfig>("Configs/SongsConfig") as SongsConfig;
        await UniTask.Yield();
        _isInitialized = true;
    }

    public void ChangeVolume(float master, float music, float effects) {
        musicSource.volume = master * music;
        foreach (AudioSource source in effectsSource) source.volume = master * effects;
    }

    public void MuteForAd() {
        _master.audioMixer.SetFloat("Volume", 0);
    }

    public void UnmuteAfterAd() {
        _master.audioMixer.SetFloat("Volume", 1f);
    }

    private void NextSource() {
        _curSource++;
        if (_curSource > effectsSource.Length - 1)
            _curSource = 0;
    }

    public void PlaySound(Sounds sound) // 0 - click, 1 - clickWrong, 2 - clickButton
    {
        NextSource();
        switch (sound) {
            case Sounds.ClickOnTile:
                effectsSource[_curSource].clip = AudioConfig.click[Random.Range(0, AudioConfig.click.Length)];
                break;

            case Sounds.Button:
                effectsSource[_curSource].clip = AudioConfig.clickButton[Random.Range(0, AudioConfig.clickButton.Length)];
                break;

            case Sounds.NextPage:
                effectsSource[_curSource].clip = AudioConfig.nextPage[Random.Range(0, AudioConfig.nextPage.Length)];
                break;

            case Sounds.Collect:
                effectsSource[_curSource].clip = AudioConfig.collect[Random.Range(0, AudioConfig.collect.Length)];
                break;

            case Sounds.ZeroEnergy:
                effectsSource[_curSource].clip = AudioConfig.zeroEnergy[Random.Range(0, AudioConfig.zeroEnergy.Length)];
                break;

            case Sounds.Hoed:
                effectsSource[_curSource].clip = AudioConfig.hoed[Random.Range(0, AudioConfig.hoed.Length)];
                break;

            case Sounds.Watered:
                effectsSource[_curSource].clip = AudioConfig.watered[Random.Range(0, AudioConfig.watered.Length)];
                break;

            case Sounds.Seeded:
                effectsSource[_curSource].clip = AudioConfig.seeded[Random.Range(0, AudioConfig.seeded.Length)];
                break;
        }

        effectsSource[_curSource].Play();
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