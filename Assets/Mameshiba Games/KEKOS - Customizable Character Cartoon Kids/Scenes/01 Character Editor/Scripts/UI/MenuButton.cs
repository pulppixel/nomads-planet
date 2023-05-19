using MameshibaGames.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using static MameshibaGames.Kekos.CharacterEditorScene.Data.CustomColors;

namespace MameshibaGames.Kekos.CharacterEditorScene.UI
{
    public class MenuButton : MonoBehaviour
    {
        public Button button;

        [SerializeField]
        private Image icon;
        
        public void EnableButton(float duration, AnimationCurve animationCurve)
        {
            StopAllCoroutines();
            
            StartCoroutine(transform.LerpScale(Vector3.one, duration, animationCurve));
            StartCoroutine(button.image.LerpColor(enabledButtonColor, duration));
            StartCoroutine(icon.LerpColor(enabledIconColor, duration));
        }

        public void DisableButton(float duration)
        {
            StopAllCoroutines();
            
            StartCoroutine(transform.LerpScale(Vector3.one * 0.9f, duration));
            StartCoroutine(button.image.LerpColor(disabledButtonColor, duration));
            StartCoroutine(icon.LerpColor(disabledIconColor, duration));
        }
    }
}
