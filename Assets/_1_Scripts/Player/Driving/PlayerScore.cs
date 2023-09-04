using System;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        public int maxScore = 1000;

        public PlayerWeapon playerWeapon;
        public NetworkVariable<int> currentScore = new();
        public GameObject hitVfx;

        private bool _isScoreMax;

        // 싱글 플레이 종료 이벤트
        public Action<PlayerScore> onWin;

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
            currentScore.Value = Mathf.Clamp(newScore, 0, maxScore);

            if (currentScore.Value == maxScore)
            {
                onWin?.Invoke(this);
                _isScoreMax = true;
            }
        }
    }
}