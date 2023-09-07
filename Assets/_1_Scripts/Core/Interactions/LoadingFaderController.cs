using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace NomadsPlanet
{
    public class LoadingFaderController : MonoBehaviour
    {
        private Image _fadeImg;
        private Image _childImg;

        public float startAlpha = 0f;
        public float duration = .5f;

        private void Awake()
        {
            _fadeImg = GetComponent<Image>();
            _childImg = transform.GetChild(0).GetComponent<Image>();
        }

        private void Start()
        {
            _fadeImg.color = new Color(0f, 0f, 0f, startAlpha);
            _childImg.color = new Color(1f, 1f, 1f, startAlpha);
            _fadeImg.gameObject.SetActive(startAlpha > 0.9f);
        }

        // 어두워짐
        public IEnumerator FadeIn()
        {
            _fadeImg.gameObject.SetActive(true);
            var tween = _fadeImg.DOFade(1f, duration);
            _childImg.DOFade(1f, duration);

            yield return tween.WaitForCompletion();
        }

        // 밝아짐
        public IEnumerator FadeOut()
        {
            _fadeImg.gameObject.SetActive(true);
            var tween = _fadeImg.DOFade(0f, duration);
            _childImg.DOFade(0f, duration);

            yield return tween.WaitForCompletion();
            _fadeImg.gameObject.SetActive(false);
        }
    }
}