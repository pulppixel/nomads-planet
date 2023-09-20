using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace NomadsPlanet
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [SerializeField] private AudioClip inGameBgm;
        [SerializeField] private AudioClip resultBgm;
        [SerializeField] private AudioClip coinGetSfx;
        [SerializeField] private AudioClip hitCarSfx;
        [SerializeField] private AudioClip boosterSfx;

        private static SoundManager instance;
        public static SoundManager Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        private IEnumerator Start()
        {
            bgmSource.clip = inGameBgm;
            bgmSource.volume = 0f;
            yield return new WaitForSeconds(1f);
            bgmSource.Play();
            bgmSource.DOFade(.75f, .5f);
        }

        public void PlayCoinGetSfx()
        {
            sfxSource.PlayOneShot(coinGetSfx);
        }

        public void PlayHitSoundSfx()
        {
            sfxSource.PlayOneShot(hitCarSfx);
        }

        public void PlayBoosterSfx()
        {
            sfxSource.PlayOneShot(boosterSfx);
        }

        public void PlayChangeBgm()
        {
            bgmSource.DOFade(0f, .5f);
            bgmSource.Stop();
            bgmSource.clip = resultBgm;
            bgmSource.Play();
            bgmSource.DOFade(1f, .5f)
                .SetDelay(1f);
        }
    }
}