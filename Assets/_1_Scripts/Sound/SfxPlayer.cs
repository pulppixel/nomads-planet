using NomadsPlanet.Utils;
using UnityEngine;

namespace NomadsPlanet
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        [SerializeField] private AudioClip[] audioClips;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySfx(int idx)
        {
            _audioSource.PlayOneShot(audioClips[idx]);
        }
    }
}