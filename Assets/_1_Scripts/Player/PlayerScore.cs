using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        [field: SerializeField] public int MaxScore { get; private set; } = 100;

        public NetworkVariable<int> CurrentScore = new();
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

            CurrentScore.Value = 0;
        }

        public void GetScore(int scoreValue)
        {
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

            hitVfx.SetActive(false);
            int newScore = CurrentScore.Value + value;
            CurrentScore.Value = Mathf.Clamp(newScore, 0, MaxScore);

            if (CurrentScore.Value == MaxScore)
            {
                OnWin?.Invoke(this);
                _isScoreMax = true;
            }

            hitVfx.SetActive(true);
        }
    }
}