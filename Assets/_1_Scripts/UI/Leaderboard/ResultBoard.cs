using DG.Tweening;
using NomadsPlanet.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class ResultBoard : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image board;
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private Button okButton;

        [SerializeField] private Leaderboard leaderboard;

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

            int coinValues = ES3.Load(PrefsKey.CoinKey, 0);
            ES3.Save(PrefsKey.CoinKey, coinValues + leaderboard.GetClientDisplay().Coins);

            coinText.DOText(
                    leaderboard.GetClientDisplay().Coins.ToString("N0"),
                    1f,
                    scrambleMode: ScrambleMode.Numerals
                )
                .SetDelay(.5f);

            okButton.image.rectTransform.DOScale(Vector3.one, .5f)
                .SetDelay(1f)
                .SetEase(Ease.OutBack);
        }
    }
}