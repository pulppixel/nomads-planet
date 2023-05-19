using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MameshibaGames.Common.UI
{
    
    [RequireComponent(typeof(Button))]
    public class PlaySoundButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private AudioClip sound;

        [SerializeField]
        private float volume = 1;

        [Flags]
        public enum SoundMoment
        {
            None = 0,
            OnClick = 1<<1,
            OnPointerDown = 1<<2,
            OnPointerUp = 1<<3
        }

        public SoundMoment soundMoment = SoundMoment.OnClick;
        
        private void Awake()
        {
            if (soundMoment.HasFlag(SoundMoment.OnClick))
                GetComponent<Button>().onClick.AddListener(PlaySound);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (soundMoment.HasFlag(SoundMoment.OnPointerDown))
                PlaySound();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (soundMoment.HasFlag(SoundMoment.OnPointerUp))
                PlaySound();
        }

        private void PlaySound()
        {
            MusicButton.PlaySound(sound, volume);
        }
    }
}
