using System;
using Abstract;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Managers
{
    public class Audio : PreloadableSingleton<Audio> {
  
        public AudioClip[] songs, click, clickWrong, clickButton, nextPage, zeroEnergy;
        public AudioClip[] collect, hoed, watered, seeded;

        public AudioSource musicSource;
        public AudioSource[] effectsSource;
        private int _curSource;
        private bool _isInitialized;

        [SerializeField]
        private AudioMixerGroup _master, _music, _sfx;

        private float _curVolume;
        
        private void Update() {
            if (!musicSource.isPlaying && _isInitialized) {
                musicSource.clip = songs[Random.Range(0, songs.Length)];
                musicSource.Play();
            }
        }

        protected override void OnFirstInit() {
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
                    effectsSource[_curSource].clip = click[Random.Range(0, click.Length)];
                    break;

                case Sounds.Button:
                    effectsSource[_curSource].clip = clickButton[Random.Range(0, clickButton.Length)];
                    break;

                case Sounds.NextPage:
                    effectsSource[_curSource].clip = nextPage[Random.Range(0, nextPage.Length)];
                    break;

                case Sounds.Collect:
                    effectsSource[_curSource].clip = collect[Random.Range(0, collect.Length)];
                    break;

                case Sounds.ZeroEnergy:
                    effectsSource[_curSource].clip = zeroEnergy[Random.Range(0, zeroEnergy.Length)];
                    break;

                case Sounds.Hoed:
                    effectsSource[_curSource].clip = hoed[Random.Range(0, hoed.Length)];
                    break;

                case Sounds.Watered:
                    effectsSource[_curSource].clip = watered[Random.Range(0, watered.Length)];
                    break;

                case Sounds.Seeded:
                    effectsSource[_curSource].clip = seeded[Random.Range(0, seeded.Length)];
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
}