using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class CoinWallet : NetworkBehaviour
    {
        public NetworkVariable<int> TotalCoins = new();

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Coin>(out Coin coin))
            {
                return;
            }

            int coinValue = coin.Collect();

            if (!IsServer)
            {
                return;
            }

            TotalCoins.Value += coinValue;
        }
    }
}