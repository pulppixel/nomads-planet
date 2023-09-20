using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace NomadsPlanet
{
    public class MenuBgmManager : MonoBehaviour
    {
        private const float MaxVolume = .8f;

        [SerializeField] private AudioClip[] audioClips; // 0: Main, 1: Chat, 2: Bumper

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void ChangeBgm(int idx)
        {
            StartCoroutine(Fader());

            IEnumerator Fader()
            {
                Tween co = _audioSource.DOFade(0f, .5f);
                yield return co.WaitForCompletion();
                _audioSource.clip = audioClips[idx];
                _audioSource.Play();
                _audioSource.DOFade(MaxVolume, .5f);
            }
        }
    }
}