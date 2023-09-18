using UnityEngine;
using UnityEngine.Localization;

namespace NomadsPlanet
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        [SerializeField] private LocalizedAudioClip[] audioClips;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySfx(int idx)
        {
            if (audioClips == null || audioClips.Length <= idx)
            {
                return;
            }

            _audioSource.PlayOneShot(audioClips[idx].LoadAsset());
        }
    }
}