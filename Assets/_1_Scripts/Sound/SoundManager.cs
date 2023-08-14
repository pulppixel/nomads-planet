using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

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

        private void Start()
        {
            bgmSource.clip = inGameBgm;
            bgmSource.volume = 0f;
            bgmSource.DOFade(1f, .5f);
        }

        public void PlayCoinGetSfx()
        {
            sfxSource.PlayOneShot(coinGetSfx);
        }

        public void HitSoundSfx()
        {
            sfxSource.PlayOneShot(hitCarSfx);
        }

        public void BoosterSfx()
        {
            sfxSource.PlayOneShot(boosterSfx);
        }

        public void ChangeBgm()
        {
            bgmSource.DOFade(0f, 1f);
            bgmSource.clip = resultBgm;
            bgmSource.DOFade(1f, 1f)
                .SetDelay(1f);
        }
    }
}