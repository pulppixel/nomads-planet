using System.Collections;
using MameshibaGames.Common.UI;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.UI
{
    public class ButtonsEffect : MonoBehaviour
    {
        [SerializeField]
        private Transform markerImageTransform;

        [SerializeField]
        private MenuButton[] menuButtons;

        [SerializeField]
        private float duration = 0.5f;
        
        [SerializeField]
        private AnimationCurve scaleCurve;

        private MenuButton _currentButton;

        private void Awake()
        {
            _currentButton = menuButtons[0];

            for (int i = 0; i < menuButtons.Length; i++)
            {
                MenuButton menuButton = menuButtons[i];
                menuButton.button.onClick.AddListener(() => ChangeToButton(menuButton));
                
                if (i > 0)
                    menuButton.DisableButton(0);
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            _currentButton.button.onClick.Invoke();
        }

        private void ChangeToButton(MenuButton menuButton)
        {
            if (_currentButton == menuButton) return;

            _currentButton.DisableButton(duration);
            _currentButton = menuButton;
            _currentButton.EnableButton(duration, scaleCurve);
            
            StopAllCoroutines();
            StartCoroutine(markerImageTransform.LerpPosition(menuButton.transform.position, duration));
        }
    }
}
