using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class CoinWallet : NetworkBehaviour
    {
        // 플레이어가 갖고 있는 전체 동전의 수
        public PlayerScore playerScore;

        public NetworkVariable<int> totalCoins = new();
        public GameObject vfx;

        private void Start()
        {
            vfx.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Coin coin))
            {
                return;
            }

            vfx.SetActive(false);
            vfx.SetActive(true);
            int coinValue = coin.Collect();

            if (!IsServer)
            {
                return;
            }

            totalCoins.Value += coinValue;
            playerScore.GetScore(coinValue);
        }
    }
}