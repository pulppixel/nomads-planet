using DG.Tweening;
using NomadsPlanet.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class ResultBoard : MonoBehaviour
    {
        public Image background;
        public Image board;
        public TMP_Text coinText;

        public Button okButton;

        private void Start()
        {
            background.color = Color.clear;
            board.rectTransform.localScale = Vector3.zero;
            okButton.image.rectTransform.localScale = Vector3.zero;
            coinText.text = 0.ToString();
        }

        public void Entrance()
        {
            background.DOColor(Color.white, 1f);
            board.rectTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

            int resultCoin = ES3.Load(PrefsKey.LocalCoinKey, 0);
            coinText.DOText(resultCoin.ToString("N0"), 1f)
                .SetDelay(1f);

            okButton.image.rectTransform.DOScale(Vector3.one, 1f)
                .SetDelay(1.5f);

            int coinValues = ES3.Load(PrefsKey.PlayerCoinKey, 0);
            ES3.Save(PrefsKey.PlayerCoinKey, coinValues + resultCoin);
        }
    }
}