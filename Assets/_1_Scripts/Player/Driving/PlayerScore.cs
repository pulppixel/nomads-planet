using System;
using NomadsPlanet.Utils;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        private const int MaxScore = 1000;

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
            ModifyScoreClientRpc(scoreValue);
        }

        public void LostScore(int scoreValue)
        {
            ModifyScoreClientRpc(-scoreValue);
            int currentCoin = ES3.Load(PrefsKey.InGameCoinKey, 0);
            ES3.Save(PrefsKey.InGameCoinKey, Mathf.Max(currentCoin - scoreValue, 0));
        }

        [ClientRpc]
        private void ModifyScoreClientRpc(int value)
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