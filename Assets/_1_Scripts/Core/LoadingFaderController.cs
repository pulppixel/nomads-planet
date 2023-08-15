using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class LoadingFaderController : MonoBehaviour
    {
        [SerializeField] private Image backgroundImg;
        [SerializeField] private Image childImg;

        public float startAlpha = 0f;
        public float duration = .5f;

        private void Start()
        {
            backgroundImg.color = new Color(0f, 0f, 0f, startAlpha);
            childImg.color = new Color(1f, 1f, 1f, startAlpha);
            backgroundImg.gameObject.SetActive(startAlpha > 0.9f);
        }

        // 어두워짐
        public IEnumerator FadeIn()
        {
            backgroundImg.gameObject.SetActive(true);
            var tween = backgroundImg.DOFade(1f, duration);
            childImg.DOFade(1f, duration);

            yield return tween.WaitForCompletion();
        }

        // 밝아짐
        public IEnumerator FadeOut()
        {
            backgroundImg.gameObject.SetActive(true);
            var tween = backgroundImg.DOFade(0f, duration);
            childImg.DOFade(0f, duration);
            yield return tween.WaitForCompletion();

            backgroundImg.gameObject.SetActive(false);
        }
    }
}