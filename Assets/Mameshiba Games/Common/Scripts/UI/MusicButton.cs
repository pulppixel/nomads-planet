using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Common.UI
{
    
    public class MusicButton : MonoBehaviour
    {
        private static MusicButton _musicButton;
        
        [SerializeField]
        private Button musicButton;

        [SerializeField]
        private Image musicIcon;
    
        [SerializeField]
        private AudioSource audioSource;
        
        [SerializeField]
        private AudioClip defaultSound;

        [SerializeField]
        [Range(0,1)]
        private float defaultVolume = 1;

        [SerializeField]
        private Sprite audioOnIcon;
    
        [SerializeField]
        private Sprite audioOffIcon;
        
        private bool _enabledAudio;
        
        private void Awake()
        {
            _musicButton = this;
            
            #if UNITY_EDITOR
            _enabledAudio = EditorPrefs.GetBool("MAMESHIBA_KEKOS_MUSIC_ENABLE", true);
            #endif
            
            HandleMusicChange();
        
            musicButton.onClick.AddListener(() =>
            {
                _enabledAudio = !_enabledAudio;
                HandleMusicChange();
            });
        }

        private void HandleMusicChange()
        {
            if (_enabledAudio)
            {
                musicIcon.sprite = audioOnIcon;
                audioSource.volume = 1;
            }
            else
            {
                musicIcon.sprite = audioOffIcon;
                audioSource.volume = 0;
            }
            
            #if UNITY_EDITOR
            EditorPrefs.SetBool("MAMESHIBA_KEKOS_MUSIC_ENABLE", _enabledAudio);
            #endif
        }

        public static void PlaySound(AudioClip sound, float volume = 0.3f)
        {
            _musicButton.InternalPlaySound(sound, volume);
        }

        private void InternalPlaySound(AudioClip sound, float volume = 0.3f)
        {
            if (_enabledAudio)
                audioSource.PlayOneShot(sound ? sound : defaultSound, sound ? volume : defaultVolume);
        }
    }
}