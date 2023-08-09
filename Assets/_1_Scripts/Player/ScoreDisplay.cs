using System;
using NomadsPlanet.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class ScoreDisplay : NetworkBehaviour
    {
        private PlayerScore _score;
        private Image _scoreImg;
        private TMP_Text _scoreText;

        private void Awake()
        {
            _score = transform.parent.GetComponent<PlayerScore>();
            _scoreImg = transform.GetChildFromName<Image>("ScoreBar");
            _scoreText = transform.GetChildFromName<TMP_Text>("ScoreText");
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
            {
                return;
            }

            _score.CurrentScore.OnValueChanged += HandleScoreChanged;
            HandleScoreChanged(0, _score.CurrentScore.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient)
            {
                return;
            }
            
            _score.CurrentScore.OnValueChanged -= HandleScoreChanged;
        }

        private void HandleScoreChanged(int oldScore, int newScore)
        {
            _scoreText.text = newScore.ToString();
            _scoreImg.fillAmount = (float)newScore / _score.MaxScore;
        }
    }
}