using System;
using System.Collections;
using NomadsPlanet.Utils;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerScore : NetworkBehaviour
    {
        [field: SerializeField] public int MaxScore { get; private set; } = 100;

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
            StartCoroutine(AttackedLogic());
        }

        private IEnumerator AttackedLogic()
        {
            playerWeapon.enabled = false;
            yield return new WaitForSeconds(3f);
            playerWeapon.enabled = true;
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