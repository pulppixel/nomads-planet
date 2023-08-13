using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        [field: SerializeField] public int MaxScore { get; private set; } = 100;

        public NetworkVariable<int> currentScore = new();
        public GameObject hitVfx;

        private bool _isScoreMax;

        public Action<PlayerScore> OnWin;

        private void Start()
        {
            hitVfx.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }

            currentScore.Value = 0;
        }

        public void GetScore(int scoreValue)
        {
            hitVfx.SetActive(false);
            hitVfx.SetActive(true);
            ModifyScoreServerRpc(scoreValue);
        }

        public void LostScore(int scoreValue)
        {
            ModifyScoreServerRpc(-scoreValue);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ModifyScoreServerRpc(int value)
        {
            if (_isScoreMax)
            {
                return;
            }

            int newScore = currentScore.Value + value;
            currentScore.Value = Mathf.Clamp(newScore, 0, MaxScore);

            if (currentScore.Value == MaxScore)
            {
                OnWin?.Invoke(this);
                _isScoreMax = true;
            }
        }
    }
}