using System;
using NomadsPlanet.Utils;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        public int maxScore = 1000;

        public PlayerWeapon playerWeapon;
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
            int currentCoin = ES3.Load(PrefsKey.InGameCoinKey, 0);
            ES3.Save(PrefsKey.InGameCoinKey, Mathf.Max(currentCoin - scoreValue, 0));
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
                OnWin?.Invoke(this);
                _isScoreMax = true;
            }
        }
    }
}