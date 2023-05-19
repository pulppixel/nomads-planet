using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Kekos.CharacterEditorScene.UI
{
    public class PlayAnim : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] frames;

        [SerializeField]
        private int framesPerSecond = 10;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            int index = (int)((Time.time * framesPerSecond) % frames.Length);
            _image.sprite = frames[index];
        }
    }
}