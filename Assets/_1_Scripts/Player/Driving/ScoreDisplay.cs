using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class ScoreDisplay : NetworkBehaviour
    {
        public PlayerScore score;
        public Slider scoreSlider;
        public TMP_Text scoreText;

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
            {
                return;
            }

            score.currentScore.OnValueChanged += HandleScoreChanged;
            HandleScoreChanged(0, score.currentScore.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient)
            {
                return;
            }

            score.currentScore.OnValueChanged -= HandleScoreChanged;
        }

        private void HandleScoreChanged(int oldScore, int newScore)
        {
            scoreText.text = $"{newScore:N0} <#b3bedb>/{score.MaxScore:N0}";
            scoreSlider.value = (float)newScore / score.MaxScore;
        }
    }
}